﻿using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

public class PlayerEquip : MonoBehaviour
{
    //EquipmentDisplay equipmentDisplay;

    Character character;

    public List<Equipment> toEquip = new List<Equipment>();

    public Dictionary<EquipSlot, Equipment> equipList = new Dictionary<EquipSlot, Equipment>();

    bool autoEquip = false;

    public void Start()
    {
        //equipmentDisplay = Resources.FindObjectsOfTypeAll<EquipmentDisplay>()[0].GetComponent<EquipmentDisplay>();
        character = GetComponent<Character>();

        foreach (EquipSlot slot in Enum.GetValues(typeof(EquipSlot)))
        {
            if (slot != EquipSlot.none) {
                equipList.Add(slot, null);
            }
        }
    }

    public void Equip(Equipment equipment)
    {
        if (CheckSlot(equipment.slot) == null)
        {
            if (equipment.slot == EquipSlot.none)
            {
                Debug.Log("Trying to equip null item!");
                return;
            }

            if (equipment.slot == EquipSlot.offHand)
            {
                if (equipList[EquipSlot.hand] == null ? false : equipList[EquipSlot.hand].twoHanded)
                    Unequip(EquipSlot.hand);
            }

            if (equipment.slot == EquipSlot.hand)
            {
                if (equipList[EquipSlot.offHand] == null ? false : equipment.twoHanded) Unequip(EquipSlot.offHand);
            }

            ApplyEquip(equipment);
        }
        else
        {
            Unequip(equipment.slot);

            if (equipment.slot == EquipSlot.hand)
            {
                if (equipList[EquipSlot.offHand] == null ? false : equipment.twoHanded) Unequip(EquipSlot.offHand);
            }

            ApplyEquip(equipment);
        }
    }

    void ApplyEquip(Equipment equipment)
    {
        equipList[equipment.slot] = equipment;
        
        if (equipment.newTrigger.Count > 0)
        {
            equipment.StatusTriggerList.Add(new object[equipment.newTrigger.Count]);
            for (int i = 0; i < equipment.newTrigger.Count; i++)
            {
                equipment.StatusTriggerList[0][i] = equipment.newTrigger[i];
            }
        }

        equipment.OnEquip(character);
    }

    Equipment CheckSlot (EquipSlot slot)
    {
        return equipList[slot];
    }

    public void Unequip(EquipSlot slot)
    {
        equipList[slot].OnUnequip(character);
        equipList[slot].AddItem(1);
        equipList[slot] = null;
    }

    public void ListEquipment()
    {
        foreach (Equipment equip in equipList.Values)
        {
            if (equip != null) Debug.Log(equip.name + " equipped in slot " + equip.slot);
        }
    }

    public void LateUpdate()
    {
        if (!autoEquip)
        {
            
            autoEquip = true;
            foreach (Equipment equip in toEquip)
            {
                Equipment newEquip = Instantiate(equip);

                Equip(newEquip);
            }

            //equipmentDisplay.UpdateEquipment(true);
        }
    }
}