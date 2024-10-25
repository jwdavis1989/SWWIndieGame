using Palmmedia.ReportGenerator.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
/**
 * MonoBehaviour version that can be added to an object
 */

public class TinkerComponent : MonoBehaviour
{
    [Header("The TinkerComponent is used for upgrading weapons\n")]
    public TinkerComponentStats stats = new TinkerComponentStats();
}
/**
 * Serializable version that can read/write as json
 */

[Serializable]
public class TinkerComponentStats
{
    [Header("Type - must be set")]
    public TinkerComponentType componentType = 0;
    [Header("UI Fields")]
    public int count = 0;
    public string itemName = "Default TinkerComponent Name";
    [Header("Stats")]
    public float attack = 0;
    public float durability = 0;
    public float block = 0;
    public float stability = 0;
    public ElementalStats elementalStats = new ElementalStats();
    [Header("Components made from recycled weapons behave differently")]
    public bool isWeapon = false;
}
/**
 * List of all component types
 */
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
    //Uniqe - breakdown weapon component
    Weapon
}
