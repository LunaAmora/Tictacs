﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage
{
    public Dictionary<string, List<object[]>> triggerList = new Dictionary<string, List<object[]>>();

    public Character actor;
    public Character target;

    public int damage;
    public int critModifier;
    public int armourPierce;
    public bool canBeDodged;
    public bool canBeBlocked;

	public bool hasCrited = false;
}