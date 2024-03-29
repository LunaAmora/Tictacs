﻿using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class Trigger
{
    public static void CreateTrigger(this Character caller, string keyTriggerList, object[] trigger)
    {
        if (caller.triggerList.ContainsKey(keyTriggerList))
        {
            caller.triggerList[keyTriggerList].Add(trigger);
        }
        else
        {
            caller.triggerList.Add(keyTriggerList, new List<object[]> { trigger });
        }
    }

    public static void CallCombatEvents(this Character caller, string eventName, bool reactive = true)
    {
        caller.EventTrigger((caller.newDamage.actor == caller ?
            caller.newDamage.actorTriggerList : caller.newDamage.targetTriggerList), (eventName), true);
    }

    public static void EventTrigger(this Character target, Dictionary<string, List<object[]>> eventList, string eventName, bool reactive)
    {
        

        if (eventList.ContainsKey(eventName))
        {
            foreach (object[] trigger in eventList[eventName])
            {
                object[] args = new object[trigger.Length - 1];
                Type[] types = new Type[trigger.Length - 1];

                for (int i = 1; i < trigger.Length; i++)
                {
                    args[i - 1] = (trigger[i].ToString().Substring(0, 1) == "p" ? target.VarParser(trigger[i]) : trigger[i]);
                    types[i - 1] = args[i - 1].GetType();
                }

                if (reactive && trigger[0].ToString().Contains("oth_"))
                {
                    target = (target.newDamage.actor == target ? target.newDamage.target : target);
                    trigger[0] = trigger[0].ToString().Remove(0, 4);
                }
                
                if (target.GetType().GetMethod((string)trigger[0], types) != null)
                {
                    if (reactive)
                    {
                        (target.newDamage.actor == target ? target.newDamage.target : target).CallCombatEvents("res_" + eventName, false);
                    }
                    target.InvokeStringMethod((string)trigger[0], args, types);
                }
                else if (trigger[0].ToString().Substring(0, 4) != "res_")
                {
                    target.EventTrigger(eventList, (string)trigger[0], reactive);
                }
            }
        }
    }

    public static void InvokeStringMethod(this Character target, string methodName, object[] args, Type[] types)
    {
        MethodInfo methodInfo = target.GetType().GetMethod(methodName, types);
        methodInfo.Invoke(target, args);
    }

    public static object VarParser(this Character target, object variable)
    {
        switch (variable.ToString())
        {
            case "#Target": return target.newDamage.target;
            default: return variable;
        }
    }

    public static void ApplyTrigger(this List<object[]> triggerList, Character owner)
    {
        foreach (object[] toApply in triggerList)
        {
            object[] args = new object[toApply.Length - 1];

            for (int i = 1; i < toApply.Length; i++)
            {
                args[i - 1] = toApply[i];
            }

            owner.CreateTrigger((string)toApply[0], args);
        }
    }

    public static void RemoveTrigger(this List<object[]> triggerList, Character owner)
    {
        foreach (object[] toRemove in triggerList)
        {
            if (owner.triggerList.ContainsKey((string)toRemove[0]))
            {
                object[] args = new object[toRemove.Length - 1];

                for (int i = 1; i < toRemove.Length; i++)
                {
                    args[i - 1] = toRemove[i];
                }

                for (int a = 0; a < owner.triggerList[(string)toRemove[0]].Count; a++)
                {
                    int max = Mathf.Max(args.Length, owner.triggerList[(string)toRemove[0]][a].Length);

                    for (int i = 0; i < max; i++)
                    {
                        if (owner.triggerList[(string)toRemove[0]][a][0] != args[0]) return;
                        if (i == max - 1)
                        {
                            owner.triggerList[(string)toRemove[0]].Remove(owner.triggerList[(string)toRemove[0]][a]);
                            return;
                        }
                    }
                }
            }
        }
    }
}
