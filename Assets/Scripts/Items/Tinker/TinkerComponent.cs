using Palmmedia.ReportGenerator.Core;
using System;
using System.Collections;
using System.Collections.Generic;
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
/**
 * Serializable version that can read/write as json
 */

[Serializable]
public class TinkerComponentStats
{
    [Header("Type - must be set on prefab - Other values set from json in WeaponUpgradeManager")]
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
 * MonoBehaviour version that one can add to a game object
 */

public class TinkerComponent : MonoBehaviour
{
    [Header("The TinkerComponent is used for upgrading weapons\n")]
    public TinkerComponentStats stats = new TinkerComponentStats();

    void OnTriggerEnter(Collider other)
    {
        bool debug = WeaponUpgradeManager.instance.debugMode;
        if (debug) Debug.Log("TinkerComponent.OnTriggerEnter");
        if (other.CompareTag("Player"))
        {
            if(debug) Debug.Log("TinkerComponent encountered player");
            //TODO: Play Sound
            if (stats.isWeapon)
            {
                if (debug) Debug.Log("component is weapon");
                //Broken down Weapon Components probably shouldn't be on the ground but handle for them anyways
                WeaponUpgradeManager.instance.weaponComponents.Add(gameObject);
                gameObject.SetActive(false);
            }
            else
            {
                
                //regular component
                if (stats.count <= 0) stats.count = 1; // Allow use of positive count for multiple drop in 1 item, otherwise act as a single drop
                WeaponUpgradeManager.instance.AddBaseComponentToPlayer(stats.componentType, stats.count);
                if (debug) Debug.Log("component is type " + stats.componentType + " count = " + WeaponUpgradeManager.instance.baseComponents[(int)stats.componentType].GetComponent<TinkerComponent>().stats.count);
                Destroy(gameObject);
            }
        }
    }
}
