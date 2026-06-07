using Palmmedia.ReportGenerator.Core.Common;
using Palmmedia.ReportGenerator.Core.Parser.Analysis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;


/**
 * Used to instantiate weapons with base stats into the world or to compare weapon types
 * Note: Use CharacterWeaponManager.AddWeaponToCurrentWeapons to add a weapon directly to a character
 *       Use PlayerWeaponMangaer.instance.AddWeaponToCurrentWeapons to add a weapon directly to the player
 */
public class WeaponsController : MonoBehaviour
{
    public static WeaponsController instance;
    [Header("The WeaponsContoller contains all base weapon prefabs and stats.\n" +
    "   Use this if adding a weapon to the game world\n" +
    "   Use PlayerWeaponManager to add directly to the player\n" +
    "   Use CharacterWeaponManager to add to another character")]
    [Header("List of all weapons. Will use prefab added in Editor.")]
    public GameObject[] baseWeapons; // list of all weapons, load with prefabs in Unity Editor. Initilized in Start()
    public bool debugMode = false; // Debug Text, adds to current weapons on Start

    /**
    * Creates and returns a weapon of any type at any location
    */
    public GameObject CreateWeapon(WeaponType type, Transform location)
    {
        return Instantiate(baseWeapons[(int)type], location);
    }
    public GameObject CreateWeaponById(string itemId, Transform location) // TODO
    {
        foreach (var weapon in baseWeapons)
            if (weapon.GetComponent<WeaponScript>() != null)
                if(weapon.GetComponent<WeaponScript>().stats.weaponName == itemId)
                    return Instantiate(weapon,location);
        return null;
    }
    void Start()
    {
        // Avoids destroying this object when changing scenes
        DontDestroyOnLoad(gameObject);
        SortWeaponsByType();
        // If debug mode is on run some basic tests
        RunTests();
    }

    public void Awake() {
        if (instance == null) {
            instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }
    public void SortWeaponsByType()
    {
        //add prefabs to initilizer array sorted by weapon type
        GameObject[] weaponsInitilizer = new GameObject[(int)WeaponType.UNKNOWN];//Enum.GetValues(typeof(WeaponType)).Cast<int>().Max()];
        foreach (var weapon in baseWeapons)
        {
            weaponsInitilizer[(int)weapon.GetComponent<WeaponScript>().stats.weaponType] = weapon;
        }
        //Set weapons here
        baseWeapons = weaponsInitilizer;
    }
    //Some simple tests to demonstrate functionality
    void RunTests()
    {
        if (debugMode)//astest
        {
            Debug.Log("============== LIST OF ALL WEAPONS =====================" + baseWeapons.Length + " :" + baseWeapons.ToString());
            int i = 0;
            foreach (GameObject weaponObj in baseWeapons)
            {
                WeaponScript weapon = weaponObj.GetComponent<WeaponScript>();
                if (weapon.stats.weaponType == WeaponType.UNKNOWN)
                    break;
                Debug.Log("Weapon " + i + ":" + weapon.stats.weaponName + " Atk: " + weapon.stats.attack
                    + " Fire:" + weapon.stats.elemental.firePower
                    + " Type:" + weapon.stats.weaponType);
            }
        }
    }
    /** Replace old weapon with it's evolution
     * oldWpn - weapon to be olved
     * newWeaponType - weapon type to evolve to
     * character - owner of the weapon to be evolved
     * @returns - a reference to the new weapon
     */
    public GameObject EvolveWeapon(GameObject oldWpn, string newWeaponType, CharacterWeaponManager character)
    {
        WeaponScript oldWpnScrpt = oldWpn.GetComponent<WeaponScript>();
        WeaponStats oldStats = oldWpnScrpt.stats;
        bool isSpecial = oldWpnScrpt.isSpecialWeapon;
        ItemDetails newWeaponDetails = ItemDropManager.GetDB().GetItem(newWeaponType);
        GameObject newWpn = Instantiate(newWeaponDetails.worldPrefab, oldWpn.transform.parent);
        WeaponStats newStatsRef = newWpn.GetComponent<WeaponScript>().stats;
        newStatsRef.attack = oldStats.attack;
        newStatsRef.durability = oldStats.durability;
        newStatsRef.block = oldStats.block;
        newStatsRef.stability = oldStats.stability;
        newStatsRef.elemental = oldStats.elemental;
        newStatsRef.currentTinkerPoints = oldStats.currentTinkerPoints;
        if (isSpecial)
        {
            int oldWpnIndex = character.ownedSpecialWeapons.IndexOf(oldWpn);
            if(oldWpnIndex == -1) return null;
            character.ownedSpecialWeapons[oldWpnIndex] = newWpn;
        }
        else
        {
            int oldWpnIndex = character.ownedWeapons.IndexOf(oldWpn);
            if (oldWpnIndex == -1) return null;
            character.ownedWeapons[oldWpnIndex] = newWpn;
        }
        Destroy(oldWpn);
        return newWpn;
    }
    public WeaponData GetWeaponData(string weaponId)
    {
        return ItemDropManager.GetDB().GetWeaponData(weaponId);
    }
    public List<string> GetAvailableEvolves(WeaponScript curWpn)
    {
        WeaponData curWpnData = GetWeaponData(curWpn.stats.weaponId);
        List<string> availableEvolves = new List<string>();
        foreach (string evolve in curWpnData.evolveWeaponIds)
        {
            //WeaponScript newWpn = baseWeapons[(int)evolve].GetComponent<WeaponScript>();
            WeaponData newWpnData = GetWeaponData(evolve);
            //check diff between req stats and current stats
            ElementalStats diff = newWpnData.baseElemental.Subract(curWpn.stats.elemental);
            //Debug.Log("Diff for " + newWpnData.itemId + " = " + diff.ToString());
            if(diff.firePower <= 0 &&
                diff.icePower <= 0 &&
                diff.lightningPower <= 0 &&
                diff.windPower <= 0 &&
                diff.earthPower <= 0 &&
                diff.lightPower <= 0 &&
                diff.beastPower <= 0 &&
                diff.scalesPower <= 0 &&
                diff.techPower <= 0 &&
                curWpn.stats.attack >= newWpnData.baseAttack &&
                //curWpn.stats.durability >= newWpnData.baseDurability &&
                curWpn.stats.stability >= newWpnData.baseStability //&&
                //curWpn.stats.block >= newWpnData.baseBlock
                 )
            {
                availableEvolves.Add(evolve);
            }
        }
        return availableEvolves;
    }
    public WeaponScript GetBaseWeaponByType(WeaponType weaponType)
    {
        return baseWeapons[(int)weaponType].GetComponent<WeaponScript>();
    }
}

//used for JSON array
[Serializable]
public class WeaponsArray
{
    public WeaponStats[] weaponStats;
}
/** Change Log  
 *  Date         Developer  Description
 *  09/16/2024   Alec       New.
 *  
 * */