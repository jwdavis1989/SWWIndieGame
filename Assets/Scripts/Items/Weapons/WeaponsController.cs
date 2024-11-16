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
    [Header("List of all weapons. Will use prefab added in Editor. Stats intialized by JSON")]
    public GameObject[] baseWeapons; // list of all weapons, load with prefabs in Unity Editor. Initilized in Start()
    [Header("JSON containing base stats")]
    public TextAsset baseWeaponJsonFile; // json file with intilizing stats that will overwrite prefab
    public bool debugMode = false; // Debug Text, adds to current weapons on Start

    /**
    * Creates and returns a weapon of any type at any location
    */
    public GameObject CreateWeapon(WeaponType type, Transform location)
    {
        return Instantiate(baseWeapons[(int)type], location);
    }
    void Start()
    {
        // Avoids destroying this object when changing scenes
        DontDestroyOnLoad(gameObject);
        // Load base stats for weapons from json
        LoadAllWeaponTypes();
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

    public void LoadAllWeaponTypes()
    {
        //add prefabs to initilizer array sorted by weapon type
        GameObject[] weaponsInitilizer = new GameObject[(int)WeaponType.UNKNOWN];//Enum.GetValues(typeof(WeaponType)).Cast<int>().Max()];
        foreach (var weapon in baseWeapons)
        {
            weaponsInitilizer[(int)weapon.GetComponent<WeaponScript>().stats.weaponType] = weapon;
        }
        //read json file and initilize stats
        WeaponsArray weaponsJsons = JsonUtility.FromJson<WeaponsArray>(baseWeaponJsonFile.text);
        foreach (WeaponStats weaponStat in weaponsJsons.weaponStats)
        {
            int i = (int)weaponStat.weaponType;
            if (weaponsInitilizer[i] != null)
            {   // prefab is loaded, copy stats over
                if (debugMode) Debug.Log("Prefab loaded for " + i + " type:" + weaponsInitilizer[i].GetComponent<WeaponScript>().stats.weaponName);//astest
                weaponsInitilizer[i].GetComponent<WeaponScript>().stats = weaponStat;
            }
            else
            {   // no prefab, create a new empty object
                // Warning: currently attack() is virtual & cannot be called for base weapons, need SwordScript or WrenchScript
                //          Stats can be viewed for these however
                weaponsInitilizer[i] = new GameObject("Empty");
                weaponsInitilizer[i].AddComponent(typeof(WeaponScript));
                weaponsInitilizer[i].GetComponent<WeaponScript>().stats = weaponStat;
                weaponsInitilizer[i].GetComponent<WeaponScript>().SetWeaponDamage();
                if (debugMode) Debug.Log("created object for " + i + " type:" + weaponsInitilizer[i].GetComponent<WeaponScript>().stats.weaponName); //astest
            }
            if (debugMode) Debug.Log("Weapon " + i + ": WeaponType:" + weaponsInitilizer[i].GetComponent<WeaponScript>().stats.weaponType
                + " Atk:" + weaponsInitilizer[i].GetComponent<WeaponScript>().stats.attack); //astest
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
    public GameObject EvolveWeapon(GameObject oldWpn, WeaponType newWeaponType, CharacterWeaponManager character)
    {
        WeaponScript oldWpnScrpt = oldWpn.GetComponent<WeaponScript>();
        WeaponStats oldStats = oldWpnScrpt.stats;
        bool isSpecial = oldWpnScrpt.isSpecialWeapon;
        GameObject newWpn = baseWeapons[(int)newWeaponType];
        WeaponStats newStats = newWpn.GetComponent<WeaponScript>().stats;
        newStats.attack = oldStats.attack;
        newStats.durability = oldStats.durability;
        newStats.block = oldStats.block;
        newStats.stability = oldStats.stability;
        newStats.elemental = oldStats.elemental;
        newStats.currentTinkerPoints = oldStats.currentTinkerPoints;
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
    public List<WeaponType> GetAvailableEvolves(WeaponScript curWpn)
    {
        List<WeaponType> evolves = GetAllEvolutions(curWpn.stats.weaponType);
        List<WeaponType> availableEvolves = new List<WeaponType>();
        foreach (WeaponType evolve in evolves)
        {
            WeaponScript newWpn = baseWeapons[(int)evolve].GetComponent<WeaponScript>();
            //check diff between req stats and current stats
            ElementalStats diff = newWpn.stats.elemental.Subract(curWpn.stats.elemental);
            if(diff.firePower <= 0 &&
                diff.icePower <= 0 &&
                diff.lightningPower <= 0 &&
                diff.windPower <= 0 &&
                diff.earthPower <= 0 &&
                diff.lightPower <= 0 &&
                diff.beastPower <= 0 &&
                diff.scalesPower <= 0 &&
                diff.techPower <= 0 &&
                curWpn.stats.attack >= newWpn.stats.attack &&
                curWpn.stats.durability >= newWpn.stats.durability &&
                curWpn.stats.stability >= newWpn.stats.stability &&
                curWpn.stats.block >= newWpn.stats.block)
            {
                availableEvolves.Add(evolve);
            }
        }
        return availableEvolves;
    }
    public List<WeaponType> GetAllEvolutions(WeaponType weaponType)
    {
        List<WeaponType> evolutions = new List<WeaponType>();
        switch(weaponType){
            case WeaponType.Shortsword:
                evolutions.Add(WeaponType.BastardSword);
                evolutions.Add(WeaponType.BroadSword); break;
            case WeaponType.Wrench:
                evolutions.Add(WeaponType.ReinforcedWrench);
                break;
            case WeaponType.BastardSword:
                break;
            case WeaponType.BroadSword:
                break;
            case WeaponType.BoneBlade:
                //evolutions.Add(WeaponType.Deathknell);
                break;
            case WeaponType.ReinforcedWrench:
                break;
            case WeaponType.Dagger:
                evolutions.Add(WeaponType.BowieKnife);
                break;
            case WeaponType.Flintlock:
                evolutions.Add(WeaponType.Revolver);
                evolutions.Add(WeaponType.ScrapGun); break;
            case WeaponType.SparkCaster:
                evolutions.Add(WeaponType.ZapCaster);
                evolutions.Add(WeaponType.BurnCaster); break;
            case WeaponType.BowieKnife:
                break;
            case WeaponType.Revolver:
                break;
            case WeaponType.ScrapGun:
                break;
            case WeaponType.ZapCaster:
                break;
            case WeaponType.BurnCaster:
                break;
            case WeaponType.FreezeCaster:
                //evolutions.Add(WeaponType.SplashCaster);
                break;
            case WeaponType.DiamondSword:
                break;
        }
        return evolutions;
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