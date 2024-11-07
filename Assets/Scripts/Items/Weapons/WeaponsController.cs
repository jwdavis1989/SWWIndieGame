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