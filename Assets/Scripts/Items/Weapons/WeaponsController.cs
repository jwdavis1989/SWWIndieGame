using Palmmedia.ReportGenerator.Core.Common;
using Palmmedia.ReportGenerator.Core.Parser.Analysis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;



public class WeaponsController : MonoBehaviour
{
    [Header("WeaponsController\nDescription - Contains: List of all Weapon Types\n\t\t\tPrefafs of each weapon\n\t\t\tList of Player's current wepaons\n\n")]
    [Header("List of all weapons. Will use prefab added in Editor. Intialized by JSON")]
    public GameObject[] weapons; // list of all weapons, load with prefabs in Unity Editor. Initilized in Start()
    public TextAsset jsonFile; // json file with intilizing stats that will overwrite prefab
    public bool debugMode = false; // Debug Text, adds shortsword to current weapons
    [Header("Player Inventory")]
    public List<GameObject> currentlyOwnedWeapons;
    public int indexOfCurrentlyEquippedWeapon = 0;
    // Start is called before the first frame update
    void Start()
    {
        //add prefabs to initilizer array
        GameObject[] weaponsInitilizer = new GameObject[(int)WeaponType.UNKNOWN];//Enum.GetValues(typeof(WeaponType)).Cast<int>().Max()];
        foreach (var weapon in weapons)
        {
            weaponsInitilizer[(int)weapon.GetComponent<BaseWeaponScript>().weaponType] = weapon;
        }
        //read json file and initilize stats
        WeaponsArray weaponsJsons = JsonUtility.FromJson<WeaponsArray>(jsonFile.text);
        foreach (BaseWeaponStats weaponStat in weaponsJsons.weapons)
        {
            int i = (int)weaponStat.weaponType;
            if (weaponsInitilizer[i] != null)
            {   // prefab is loaded, copy stats over
                if (debugMode) Debug.Log("Prefab loaded for " + i + " type:" + weaponsInitilizer[i].GetComponent<BaseWeaponScript>().weaponName);//astest
                weaponsInitilizer[i].GetComponent<BaseWeaponScript>().copy(weaponStat);
            }
            else
            {   // no prefab, create a new empty object
                // Warning: currently attack() is virtual & cannot be called for base weapons, need SwordScript or WrenchScript
                //          Stats can be viewed for these however
                weaponsInitilizer[i] = new GameObject("Empty");
                weaponsInitilizer[i].AddComponent(typeof(BaseWeaponScript));
                weaponsInitilizer[i].GetComponent<BaseWeaponScript>().copy(weaponStat);
                if (debugMode) Debug.Log("created object for " + i + " type:" + weaponsInitilizer[i].GetComponent<BaseWeaponScript>().weaponName); //astest
            }
            if (debugMode) Debug.Log("Weapon " + i + ": WeaponType:" + weaponsInitilizer[i].GetComponent<BaseWeaponScript>().weaponType 
                + " Atk:"+ weaponsInitilizer[i].GetComponent<BaseWeaponScript>().attack); //astest
        }
        //Set weapons here
        weapons = weaponsInitilizer;
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
            currentlyOwnedWeapons[0].GetComponent<BaseWeaponScript>().attackTarget(gameObject);
            AttackTargetWithCurrentlyEquippedWeapon(gameObject);
            //Change to Wrench, the third weapon
            ChangeWeapon(2); 
            AttackTargetWithCurrentlyEquippedWeapon(gameObject);
            Debug.Log("============== LIST OF ALL WEAPONS =====================");
            foreach(GameObject weaponObj in weapons)
            {
                BaseWeaponScript weapon = weaponObj.GetComponent<BaseWeaponScript>();
                Debug.Log("Weapon:" + weapon.weaponName + " Atk: " + weapon.attack + " Fire:" + weapon.firePower + " Type:"+ weapon.weaponType);
            }
        }
    }
    public void AddWeaponToCurrentWeapons(WeaponType weaponType)
    {
        int i = (int)weaponType;
        if (weapons[i] != null) {
            currentlyOwnedWeapons.Add(Instantiate(weapons[i]));
        }
        else
        {
            Debug.Log("Warning in addWeaponToCurrentWeapons(" + weaponType + "). Not found");
        }
    }
    public void ChangeWeapon(int index)
    {
        if (currentlyOwnedWeapons[index] != null)
            indexOfCurrentlyEquippedWeapon = index;
    }
    public void AttackTargetWithCurrentlyEquippedWeapon(GameObject target)
    {
        if (target == null) return;
        if (currentlyOwnedWeapons[indexOfCurrentlyEquippedWeapon] != null)
        {
            currentlyOwnedWeapons[indexOfCurrentlyEquippedWeapon].GetComponent<BaseWeaponScript>().attackTarget(target);
        }
    }
    /**
     * Perhaps could use a method to calculate the attributes of a weapon of a particular level
     */
    public float GetWeaponAttack(WeaponType type, int level)
    {
        if(weapons[(int)type] != null)
            return weapons[(int)type].GetComponent<BaseWeaponScript>().attack * level;
        return -1;
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
    FlintlockEvolve1,
    FlintlockEvolve2,
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
    public BaseWeaponStats[] weapons;
}
//used for JSON object
[Serializable]
public class BaseWeaponStats
{
    public float attack = 1.0f;
    public float speed = 1.0f;
    public float specialtyCooldown = 0;
    public float block = 1.0f;
    public float stability = 1.0f;
    public float xpToLevel = 100.0f;
    public int maxDurability = 1;
    public int firePower = 0;
    public int icePower = 0;
    public int lightningPower = 0;
    public int windPower = 0;
    public int earthPower = 0;
    public int lightPower = 0;
    public int beastPower = 0;
    public int scalesPower = 0;
    public int techPower = 0;
    public int tinkerPointsPerLvl = 0;
    public WeaponType weaponType = 0;
    public float currentDurability = 1.0f;
    public int level = 1;
    public float currentExperiencePoints = 0.0f;
    public int currentTinkerPoints = 0;
    public String weaponName = "BaseWeaponName";
}
/** Change Log  
 *  Date         Developer  Description
 *  09/16/2024   Alec       New.
 *  
 * */