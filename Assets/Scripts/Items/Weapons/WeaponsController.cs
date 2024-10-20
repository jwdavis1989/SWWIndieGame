using Palmmedia.ReportGenerator.Core.Common;
using Palmmedia.ReportGenerator.Core.Parser.Analysis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;



public class WeaponsController : MonoBehaviour
{
    public static WeaponsController instance;

    [Header("WeaponsController\nDescription - Contains: List of all Weapon Types\n\t\t\tPrefafs of each weapon\n\t\t\tList of Player's current wepaons\n\n")]
    [Header("List of all weapons. Will use prefab added in Editor. Intialized by JSON")]
    public GameObject[] weapons; // list of all weapons, load with prefabs in Unity Editor. Initilized in Start()
    public TextAsset jsonFile; // json file with intilizing stats that will overwrite prefab
    public bool debugMode = false; // Debug Text, adds shortsword to current weapons
    [Header("Player Inventory")]
    public List<GameObject> currentlyOwnedWeapons;
    public int indexOfCurrentlyEquippedWeapon = 0;
    // Start is called before the first frame 
    
    public void Awake() {
        if (instance == null) {
            instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        //Avoids destroying this object when changing scenes
        DontDestroyOnLoad(gameObject);
        LoadAllWeaponTypes();
        RunTests();
    }
    public void LoadAllWeaponTypes()
    {
        //add prefabs to initilizer array
        GameObject[] weaponsInitilizer = new GameObject[(int)WeaponType.UNKNOWN];//Enum.GetValues(typeof(WeaponType)).Cast<int>().Max()];
        foreach (var weapon in weapons)
        {
            weaponsInitilizer[(int)weapon.GetComponent<WeaponScript>().stats.weaponType] = weapon;
        }
        //read json file and initilize stats
        WeaponsArray weaponsJsons = JsonUtility.FromJson<WeaponsArray>(jsonFile.text);
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
                if (debugMode) Debug.Log("created object for " + i + " type:" + weaponsInitilizer[i].GetComponent<WeaponScript>().stats.weaponName); //astest
            }
            if (debugMode) Debug.Log("Weapon " + i + ": WeaponType:" + weaponsInitilizer[i].GetComponent<WeaponScript>().stats.weaponType
                + " Atk:" + weaponsInitilizer[i].GetComponent<WeaponScript>().stats.attack); //astest
        }
        //Set weapons here
        weapons = weaponsInitilizer;
    }

    public void AddWeaponToCurrentWeapons(WeaponType weaponType)
    {
        int i = (int)weaponType;
        if (weapons[i] != null) {
            currentlyOwnedWeapons.Add(Instantiate(weapons[i]));
            weapons[i].GetComponent<WeaponScript>().hasObtained = true;
        }
        else
        {
            Debug.Log("Warning in addWeaponToCurrentWeapons(" + weaponType + "). Not found");
        }
    }
    public void ChangeWeapon(int index)
    {
        if (currentlyOwnedWeapons[index] != null)
        {
            currentlyOwnedWeapons[indexOfCurrentlyEquippedWeapon].SetActive(false);
            indexOfCurrentlyEquippedWeapon = index;
            currentlyOwnedWeapons[indexOfCurrentlyEquippedWeapon].SetActive(true);
        }
    }
    public void nextWeapon()
    {
        int totalWeapons = currentlyOwnedWeapons.Count;
        int newWeaponIndex = indexOfCurrentlyEquippedWeapon;
        if (currentlyOwnedWeapons != null && totalWeapons > 0)
        {
            if(newWeaponIndex + 1 > totalWeapons - 1)
            {
                newWeaponIndex = 0;
            }
            else
            {
                newWeaponIndex++;
            }
            ChangeWeapon(newWeaponIndex);
        }
    }
    public void prevWeapon()
    {
        int totalWeapons = currentlyOwnedWeapons.Count;
        int newWeaponIndex = indexOfCurrentlyEquippedWeapon;
        if (currentlyOwnedWeapons != null && totalWeapons > 0)
        {
            if (newWeaponIndex - 1 < 0)
            {
                newWeaponIndex = totalWeapons - 1;
            }
            else
            {
                newWeaponIndex--;
            }
            ChangeWeapon(newWeaponIndex);
        }
    }
    public void AttackTargetWithCurrentlyEquippedWeapon(GameObject target)
    {
        if (target == null) return;
        if (currentlyOwnedWeapons[indexOfCurrentlyEquippedWeapon] != null)
        {
            currentlyOwnedWeapons[indexOfCurrentlyEquippedWeapon].GetComponent<WeaponScript>().attackTarget(target);
        }
    }
    /**
     * Perhaps could use a method to calculate the attributes of a weapon of a particular level
     */
    public float GetWeaponAttack(WeaponType type, int level)
    {
        if(weapons[(int)type] != null)
            return weapons[(int)type].GetComponent<WeaponScript>().stats.attack * level;
        return -1;
    }
    //For loading weapons from save file json
    public void setCurrentWeapons(WeaponsArray weaponsJson)
    {
        currentlyOwnedWeapons = new List<GameObject>();
        int i = 0;
        foreach (WeaponStats weaponStat in weaponsJson.weaponStats)
        {
            AddWeaponToCurrentWeapons(weaponStat.weaponType);
            //currentlyOwnedWeapons[i].SetActive(false);
            currentlyOwnedWeapons[i++].GetComponent<WeaponScript>().stats = weaponStat;
        }
        if(currentlyOwnedWeapons.Count > 0)
        {
            currentlyOwnedWeapons[indexOfCurrentlyEquippedWeapon].SetActive(true);
        }
    }
    //for saving current weapons to save file json
    public WeaponsArray GetCurrentWeapons()
    {
        WeaponsArray weaponsPojo = new WeaponsArray();
        weaponsPojo.weaponStats = new WeaponStats[currentlyOwnedWeapons.Count];
        int i = 0;
        foreach (GameObject weapon in currentlyOwnedWeapons)
        {
            weaponsPojo.weaponStats[i++] = weapon.GetComponent<WeaponScript>().stats;
        }
        return weaponsPojo;
    }
    void RunTests()
    {
        if (debugMode)//astest
        {
            //Tests
            //display attack by weapon type
            Debug.Log("Attack of level 1 BastardSword:" + GetWeaponAttack(WeaponType.BastardSword, 1));//astest
            //Add 2 Shortswords and a Wrench to currently owned weapons
            AddWeaponToCurrentWeapons(WeaponType.Shortsword);
            AddWeaponToCurrentWeapons(WeaponType.Shortsword);
            AddWeaponToCurrentWeapons(WeaponType.Wrench);
            //Two methods of attacking
            currentlyOwnedWeapons[0].GetComponent<WeaponScript>().attackTarget(gameObject);
            AttackTargetWithCurrentlyEquippedWeapon(gameObject);
            //Change to Wrench, the third weapon
            ChangeWeapon(2);
            AttackTargetWithCurrentlyEquippedWeapon(gameObject);
            Debug.Log("============== LIST OF ALL WEAPONS =====================" + weapons.Length + " :" + weapons.ToString());
            int i = 0;
            foreach (GameObject weaponObj in weapons)
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

// Enum type of all weapon types
public enum WeaponType
{
    Shortsword,
    Wrench,
    BastardSword,
    BroadSword,
    BoneBlade,
    ReinforcedWrench,
    //specialty weapons
    Dagger,
    Flintlock,
    SparkCaster,
    BowieKnife,
    Revolver,
    ScrapGun,
    ZapCaster,
    BurnCaster,
    FreezeCaster,
    //T3 Weapons
    DiamondSword,

    //Limit
    UNKNOWN
}
//used for JSON array
[Serializable]
public class WeaponsArray
{
    public WeaponStats[] weaponStats;
}
//used for JSON object
[Serializable]
public class WeaponStats
{
    [Header("Weapon Type - Important - Set in Prefab")]
    public WeaponType weaponType = 0;
    [Header("Weapon Attributes (Intialized by JSON)")]
    public float attack = 1.0f;
    public float maxAttack = 1.0f;
    public int durability = 1;
    public int maxDurability = 1;
    public float block = 1.0f;
    public float maxBlock = 1.0f;
    public float stability = 1.0f;
    public float maxStability = 1.0f;
    public ElementalStats elemental;
    public ElementalStats maxElemental;
    public float speed = 1.0f;
    public float maxSpeed = 1.0f;
    public float specialtyCooldown = 0;
    public float maxSpecialtyCooldown = 0;
    public float xpToLevel = 100.0f;
    public int tinkerPointsPerLvl = 0;
    public float currentDurability = 1.0f;
    public int level = 1;
    public float currentExperiencePoints = 0.0f;
    public int currentTinkerPoints = 0;
    public String weaponName = "BaseWeaponName";
}
[Serializable]
public class ElementalStats
{
    public int firePower = 0;
    public int icePower = 0;
    public int lightningPower = 0;
    public int windPower = 0;
    public int earthPower = 0;
    public int lightPower = 0;
    public int beastPower = 0;
    public int scalesPower = 0;
    public int techPower = 0;
}
/** Change Log  
 *  Date         Developer  Description
 *  09/16/2024   Alec       New.
 *  
 * */