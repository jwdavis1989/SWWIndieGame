using Palmmedia.ReportGenerator.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
[Serializable]
public class TinkerComponent 
{
    [Header("The TinkerComponent is used for upgrading weapons\n")]
    [Header("Type - must be set")]
    public TinkerComponentType componentType = 0;
    [Header("UI Fields")]
    public int count = 0;
    public string itemName = "Default TinkerComponent Name";
    [Header("Stats")]
    public float attack = 0;
    public ElementalStats elementalStats = new ElementalStats();
    [Header("Components made from recycled weapons behave differently")]
    public bool isWeapon = false;

}
public enum TinkerComponentType
{
    Micro,
    Bolt,
    Plating,
    Stabilizer,
    FirePrism,
    IcePrism,
    LightningPrism,
    WindPrism,
    EarthPrism,
    LightPrism,
    Razor,
    Hook,
    Drillbit,
    RareTinkeredComponents,
    Amethyst,
    Aquamarine,
    Diamond,
    Emerald,
    Garnet,
    Opal,
    Pearl,
    Peridot,
    Ruby,
    Sapphire,
    Topaz,
    Turquoise,
    //breakdown weapon comonent
    Weapon
}
