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



public class WeaponsController : MonoBehaviour
{
    public static WeaponsController instance;

    [Header("WeaponsController\nDescription - Contains: List of all Weapon Types\n\t\t\tPrefafs of each weapon\n\t\t\tList of Player's current wepaons\n\n")]
    [Header("List of all weapons. Will use prefab added in Editor. Intialized by JSON")]
    public GameObject[] baseWeapons; // list of all weapons, load with prefabs in Unity Editor. Initilized in Start()
    public TextAsset baseWeaponJsonFile; // json file with intilizing stats that will overwrite prefab
    public bool debugMode = false; // Debug Text, adds shortsword to current weapons
    [Header("Player Inventory")]
    public List<GameObject> ownedWeapons;
    public List<GameObject> ownedSpecialWeapons;
    public int indexOfEquippedWeapon = 0;
    public int indexOfEquippedSpecialWeapon = 0;
    // Start is called before the first frame 

    [Header("Weapon Attachment")]
    public GameObject mainHandWeaponAnchor;
    public GameObject offHandWeaponAnchor;
    
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
                if (debugMode) Debug.Log("created object for " + i + " type:" + weaponsInitilizer[i].GetComponent<WeaponScript>().stats.weaponName); //astest
            }
            if (debugMode) Debug.Log("Weapon " + i + ": WeaponType:" + weaponsInitilizer[i].GetComponent<WeaponScript>().stats.weaponType
                + " Atk:" + weaponsInitilizer[i].GetComponent<WeaponScript>().stats.attack); //astest
        }
        //Set weapons here
        baseWeapons = weaponsInitilizer;
    }

    public void AddWeaponToCurrentWeapons(WeaponType weaponType)
    {
        int i = (int)weaponType;
        if (baseWeapons[i] != null) {
            if (baseWeapons[i].GetComponent<WeaponScript>().isSpecialWeapon) {
                ownedSpecialWeapons.Add(Instantiate(baseWeapons[i], offHandWeaponAnchor.transform));
            }
            else {
                ownedWeapons.Add(Instantiate(baseWeapons[i], mainHandWeaponAnchor.transform));
            }
            WeaponScript currentWeaponScript = baseWeapons[i].GetComponent<WeaponScript>();
            currentWeaponScript.hasObtained = true;
        }
        else
        {
            Debug.Log("Warning in addWeaponToCurrentWeapons(" + weaponType + "). Not found");
        }
    }
    public void ChangeWeapon(int index)
    {
        if (index < ownedWeapons.Count && ownedWeapons[index] != null)
        {
            ownedWeapons[indexOfEquippedWeapon].SetActive(false);
            indexOfEquippedWeapon = index;
            ownedWeapons[indexOfEquippedWeapon].SetActive(true);
        }
    }
    public void nextWeapon()
    {
        int totalWeapons = ownedWeapons.Count;
        int newWeaponIndex = indexOfEquippedWeapon;
        if (ownedWeapons != null && totalWeapons > 0)
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
        int totalWeapons = ownedWeapons.Count;
        int newWeaponIndex = indexOfEquippedWeapon;
        if (ownedWeapons != null && totalWeapons > 0)
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
    public void ChangeSpecialWeapon(int index)
    {

        if (index < ownedSpecialWeapons.Count && ownedSpecialWeapons[index] != null)
        {
            ownedSpecialWeapons[indexOfEquippedSpecialWeapon].SetActive(false);
            indexOfEquippedSpecialWeapon = index;
            ownedSpecialWeapons[indexOfEquippedSpecialWeapon].SetActive(true);
        }
    }
    public void nextSpecialWeapon()
    {
        int totalWeapons = ownedSpecialWeapons.Count;
        int newWeaponIndex = indexOfEquippedSpecialWeapon;
        if (ownedSpecialWeapons != null && totalWeapons > 0)
        {
            if (newWeaponIndex + 1 > totalWeapons - 1)
            {
                newWeaponIndex = 0;
            }
            else
            {
                newWeaponIndex++;
            }
            ChangeSpecialWeapon(newWeaponIndex);
        }
    }
    public void prevSpecialWeapon()
    {
        int totalWeapons = ownedSpecialWeapons.Count;
        int newWeaponIndex = indexOfEquippedSpecialWeapon;
        if (ownedSpecialWeapons != null && totalWeapons > 0)
        {
            if (newWeaponIndex - 1 < 0)
            {
                newWeaponIndex = totalWeapons - 1;
            }
            else
            {
                newWeaponIndex--;
            }
            ChangeSpecialWeapon(newWeaponIndex);
        }
    }
    public void AttackTargetWithCurrentlyEquippedWeapon(GameObject target)
    {
        if (target == null) return;
        if (ownedWeapons[indexOfEquippedWeapon] != null)
        {
            ownedWeapons[indexOfEquippedWeapon].GetComponent<WeaponScript>().attackTarget(target);
        }
    }
    /**
     * Perhaps could use a method to calculate the attributes of a weapon of a particular level
     */
    public float GetWeaponAttack(WeaponType type, int level)
    {
        if(baseWeapons[(int)type] != null)
            return baseWeapons[(int)type].GetComponent<WeaponScript>().stats.attack * level;
        return -1;
    }
    //For loading weapons from save file json
    public void setCurrentWeapons(WeaponsArray weaponsJson)
    {
        ownedWeapons = new List<GameObject>();
        ownedSpecialWeapons = new List<GameObject>();
        int i = 0;
        int specialI = 0;
        foreach (WeaponStats weaponStat in weaponsJson.weaponStats)
        {
            AddWeaponToCurrentWeapons(weaponStat.weaponType);
            if (baseWeapons[(int)weaponStat.weaponType].GetComponent<WeaponScript>().isSpecialWeapon)
            {
                ownedSpecialWeapons[specialI].GetComponent<WeaponScript>().stats = weaponStat;
                ownedSpecialWeapons[specialI++].SetActive(false);
            }
            else
            {
                ownedWeapons[i].GetComponent<WeaponScript>().stats = weaponStat;
                ownedWeapons[i++].SetActive(false);
            }
        }
        if(ownedWeapons.Count > 0)
        {
            ownedWeapons[indexOfEquippedWeapon].SetActive(true);
        }
        if (ownedSpecialWeapons.Count > 0)
        {
            ownedSpecialWeapons[indexOfEquippedSpecialWeapon].SetActive(true);
        }
    }
    //for saving current weapons to save file json
    public WeaponsArray GetCurrentWeapons()
    {
        WeaponsArray weaponsPojo = new WeaponsArray();
        weaponsPojo.weaponStats = new WeaponStats[ownedWeapons.Count + ownedSpecialWeapons.Count];
        int i = 0;
        foreach (GameObject weapon in ownedWeapons)
        {
            weaponsPojo.weaponStats[i++] = weapon.GetComponent<WeaponScript>().stats;
        }
        foreach (GameObject weapon in ownedSpecialWeapons)
        {
            weaponsPojo.weaponStats[i++] = weapon.GetComponent<WeaponScript>().stats;
        }
        return weaponsPojo;
    }

    public void SetAllWeaponsToInactive(bool targetSpecialWeaponStatus) {
        List<GameObject> weapons = targetSpecialWeaponStatus? ownedSpecialWeapons: ownedWeapons;
        if (weapons.Count <= 0)
            return;
        foreach (GameObject weapon in weapons) {
                weapon.SetActive(false);
        }
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
            ownedWeapons[0].GetComponent<WeaponScript>().attackTarget(gameObject);
            AttackTargetWithCurrentlyEquippedWeapon(gameObject);
            //Change to Wrench, the third weapon
            ChangeWeapon(2);
            AttackTargetWithCurrentlyEquippedWeapon(gameObject);
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
    public ElementalStats CalculateElementDiff(ElementalStats subtractor)
    {
        ElementalStats diff = new ElementalStats();
        diff.firePower = firePower - subtractor.firePower;
        diff.icePower = icePower - subtractor.icePower;
        diff.lightningPower = lightningPower - subtractor.lightningPower;
        diff.windPower = windPower - subtractor.windPower;
        diff.earthPower = earthPower - subtractor.earthPower;
        diff.lightPower = lightPower - subtractor.lightningPower;
        diff.beastPower = beastPower - subtractor.beastPower;
        diff.scalesPower = scalesPower - subtractor.scalesPower;
        diff.techPower = techPower - subtractor.techPower;
        return diff;
    }
    public ElementalStats Add(ElementalStats other)
    {
        ElementalStats sum = new ElementalStats();
        sum.firePower = firePower + other.firePower;
        sum.icePower = icePower + other.icePower;
        sum.lightningPower = lightningPower + other.lightningPower;
        sum.windPower = windPower + other.windPower;
        sum.earthPower = earthPower + other.earthPower;
        sum.lightPower = lightPower + other.lightningPower;
        sum.beastPower = beastPower + other.beastPower;
        sum.scalesPower = scalesPower + other.scalesPower;
        sum.techPower = techPower + other.techPower;
        return sum;
    }
}
/** Change Log  
 *  Date         Developer  Description
 *  09/16/2024   Alec       New.
 *  
 * */