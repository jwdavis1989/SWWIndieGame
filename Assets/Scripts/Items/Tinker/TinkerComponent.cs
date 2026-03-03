using Palmmedia.ReportGenerator.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
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
    //RareTinkeredComponents
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

    Weapon //Unique - breakdown weapon component, should always be last in this list
}
/**
 * MonoBehaviour TinkerComponent that one can be added to a game object
 */

public class TinkerComponent : PickupableItem
{
    //[Header("Image used for menu icon")]
    //public Sprite spr = null;//TODO replace usage with InventoryItem icon

    public override void HandlePickup(GameObject player)
    {
        base.HandlePickup(player);
        //Debug.Log("TinkerComponent HandlePickup");
        //TODO: Play Pick Up Sound here
    }
}
/**
 * Serializable object that can read/write as json
 */

[Serializable]
public class TinkerComponentStats
{
    [Header("Stats")]
    public float attack = 0;
    public float durability = 0;
    public float block = 0;
    public float stability = 0;
    public ElementalStats elementalStats = new ElementalStats();
    [Header("Components made from recycled weapons behave differently")]
    public bool isWeapon = false;
    public bool isSpecialWpn = false;
    public Dictionary<string, float> GetStats()
    {
        Dictionary<string, float> rv = new Dictionary<string, float>();
        rv.Add("Fire", elementalStats.firePower);
        rv.Add("Earth", elementalStats.earthPower);
        rv.Add("Ice", elementalStats.icePower);
        rv.Add("Light", elementalStats.lightPower);
        rv.Add("Lightning", elementalStats.lightningPower);
        rv.Add("Beast", elementalStats.beastPower);
        rv.Add("Wind", elementalStats.windPower);
        rv.Add("Scales", elementalStats.scalesPower);
        rv.Add("Tech", elementalStats.techPower);
        rv.Add("Attack", attack);
        rv.Add("Block", block);
        rv.Add("Durability", durability);
        rv.Add("Stability", stability);
        //filter out zeroes and return
        return rv.Where(kvp => kvp.Value != 0).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }
}

[Serializable]
public class WeaponSalvageComponent
{
    public string itemId;
    public string itemName = "Default";
    public TinkerComponentStats stats = new TinkerComponentStats();
}

