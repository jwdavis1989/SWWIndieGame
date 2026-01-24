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

public class TinkerComponent : InventoryItem
{
    [Header("The TinkerComponent is used for upgrading weapons\n")]
    [Header("These values are saved when saving game")]
    public TinkerComponentStats stats = new();
    [Header("Image used for menu icon")]
    public Sprite spr = null;//TODO replace usage with InventoryItem icon

    public override void HandlePickup(GameObject player)
    {
        Debug.Log("TinkerComponent HandlePickup");
        //TODO: Play Pick Up Sound here
        if (stats.isWeapon)
        {
            //Broken down Weapon Components probably shouldn't be on the ground but handle for them anyways
            TinkerComponentManager.instance.weaponComponents.Add(gameObject);
            Destroy(gameObject);
        }
        else
        {

            //regular component
            if (stats.count <= 0) stats.count = 1; // Allow use of positive count for multiple drop in 1 item, otherwise act as a single drop
            TinkerComponentManager.instance.AddBaseComponentToPlayer(stats.componentType, stats.count);
            Destroy(gameObject);
        }
    }
    public Dictionary<string, float> GetStats()
    {
        Dictionary<string, float> rv = new Dictionary<string, float>();
        rv.Add("Fire", stats.elementalStats.firePower);
        rv.Add("Earth", stats.elementalStats.earthPower);
        rv.Add("Ice", stats.elementalStats.icePower);
        rv.Add("Light", stats.elementalStats.lightPower);
        rv.Add("Lightning", stats.elementalStats.lightningPower);
        rv.Add("Beast", stats.elementalStats.beastPower);
        rv.Add("Wind", stats.elementalStats.windPower);
        rv.Add("Scales", stats.elementalStats.scalesPower);
        rv.Add("Tech", stats.elementalStats.techPower);
        rv.Add("Attack", stats.attack);
        rv.Add("Block", stats.block);
        rv.Add("Durability", stats.durability);
        rv.Add("Stability", stats.stability);
        //filter out zeroes and return
        return rv.Where(kvp => kvp.Value != 0).ToDictionary(kvp => kvp.Key, kvp =>kvp.Value);
    }
}
/**
 * Serializable object that can read/write as json
 */

[Serializable]
public class TinkerComponentStats
{
    [Header("Type")]
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
    public bool isSpecialWpn = false;
    public int price = 1;
}
