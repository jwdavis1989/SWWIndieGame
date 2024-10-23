using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TinkerComponent : MonoBehaviour
{
    [Header("The TinkerComponent is used for upgrading weapons\n")]
    [Header("UI Fields")]
    public int count = 0;
    public string itemName = "Default TinkerComponent Name";
    [Header("Stats")]
    public float attack = 0;
    public ElementalStats elementalStats = new ElementalStats();
    [Header("Components made from recycled weapons behave differently")]
    public bool isWeapon = false;

}
