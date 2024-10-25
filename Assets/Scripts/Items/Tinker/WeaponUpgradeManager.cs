using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponUpgradeManager : MonoBehaviour
{
    //****DEBUG ASTEST
    [Header("DEBUG")]
    public bool debugMode = false;
    public bool dropItem = false;
    public void Update()
    {//astest
        if (dropItem)
        {
            dropItem = false;
            int i = UnityEngine.Random.Range(0, baseComponents.Length);
            Instantiate(baseComponents[i]);
        }
    }
    //****DEBUG
    [Header("WeaponUpgradeManager is a singleton containing prefabs for each tinker component\n" +
        ", methods for upgrading/breaking down weapons\n" +
        ", and a record of the players current tinker components\n")]
    public static WeaponUpgradeManager instance;
    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void Start()
    {
        DontDestroyOnLoad(gameObject);
        LoadAllComponentTypes();
    }
    [Header("Array of normal components")]
    public GameObject[] baseComponents;
    [Header("List of deconstructed weapon components")]
    public List<GameObject> ownedWeaponBreakdowns = new List<GameObject>();
    [Header("JSON file contaning base stats")]
    public TextAsset baseComponentJsonFile; // json file with intilizing stats that will overwrite prefab
    /**
     * CanUseComponent will return true if any stat will be upgraded. I.e. if any matching stat is not currently maxed
     */
    public bool CanUseComponent(WeaponScript weapon, TinkerComponent tinkerComponent)
    {
        return AddTinkerComponentToWeapon(weapon, tinkerComponent, false);
    }
    public bool UseComponent(WeaponScript weapon, TinkerComponent tinkerComponent)
    {
        return AddTinkerComponentToWeapon(weapon, tinkerComponent, true);
    }
    public bool AddTinkerComponentToWeapon(WeaponScript weapon, TinkerComponent tinkerComponent, bool doUpdate)
    {
        bool canUpgrade = false;
        float newAttack = weapon.stats.attack;
        //calulate new attack
        if (tinkerComponent.stats.isWeapon)
        {
            if (weapon.stats.attack < tinkerComponent.stats.attack)
            {
                newAttack += tinkerComponent.stats.attack * 0.25f;
            }
            else
            {
                newAttack += tinkerComponent.stats.attack * 0.60f;
            }
        }
        else
        {
            newAttack += tinkerComponent.stats.attack;
        }
        newAttack = Mathf.Min(newAttack, weapon.stats.maxAttack);
        //calculate new elemental
        ElementalStats newStats = weapon.stats.elemental.Add(tinkerComponent.stats.elementalStats);
        newStats.firePower = Mathf.Min(newStats.firePower, weapon.stats.maxElemental.firePower);
        newStats.icePower = Mathf.Min(newStats.icePower, weapon.stats.maxElemental.icePower);
        newStats.lightningPower = Mathf.Min(newStats.lightningPower, weapon.stats.maxElemental.lightningPower);
        newStats.windPower = Mathf.Min(newStats.windPower, weapon.stats.maxElemental.windPower);
        newStats.earthPower = Mathf.Min(newStats.earthPower, weapon.stats.maxElemental.earthPower);
        newStats.lightPower = Mathf.Min(newStats.lightPower, weapon.stats.maxElemental.lightPower);
        newStats.beastPower = Mathf.Min(newStats.beastPower, weapon.stats.maxElemental.beastPower);
        newStats.scalesPower = Mathf.Min(newStats.scalesPower, weapon.stats.maxElemental.scalesPower);
        newStats.techPower = Mathf.Min(newStats.techPower, weapon.stats.maxElemental.techPower);
        ElementalStats diffWithPrev = newStats.CalculateElementDiff(weapon.stats.elemental);
        //if any stat will be upgraded then we can upgrade
        if (diffWithPrev.firePower > 0 ||
            diffWithPrev.icePower > 0 ||
            diffWithPrev.lightningPower > 0 ||
            diffWithPrev.windPower > 0 ||
            diffWithPrev.earthPower > 0 ||
            diffWithPrev.lightPower > 0 ||
            diffWithPrev.beastPower > 0 ||
            diffWithPrev.scalesPower > 0 ||
            diffWithPrev.techPower > 0 ||
            weapon.stats.attack < newAttack)
        {
            canUpgrade = true;
            if (doUpdate)
            {
                weapon.stats.elemental = newStats;
                weapon.stats.attack = newAttack;
                tinkerComponent.stats.count--;
            }
        }
        return canUpgrade;
    }
    public void LoadAllComponentTypes()
    {
        if(baseComponentJsonFile == null)
        {
            Debug.Log("WeaponUpgradeManager.baseComponentJsonFile is missing!");
            return;
        }
        //add prefabs to initilizer array
        GameObject[] componentInitilizer = new GameObject[(int)TinkerComponentType.Amethyst];//.Weapon];//Enum.GetValues(typeof(WeaponType)).Cast<int>().Max()];
        foreach (var component in baseComponents)
        {
            componentInitilizer[(int)component.GetComponent<TinkerComponent>().stats.componentType] = component;
        }
        //read json file and initilize stats
        ComponentsArray componentJsons = JsonUtility.FromJson<ComponentsArray>(baseComponentJsonFile.text);
        foreach (TinkerComponentStats componentStats in componentJsons.components)
        {
            int i = (int)componentStats.componentType;
            if (componentInitilizer[i] != null)
            {   // prefab is loaded, copy stats over
                componentInitilizer[i].GetComponent<TinkerComponent>().stats = componentStats;
                if (debugMode) Debug.Log("Prefab loaded for " + i + " type:" + componentInitilizer[i].GetComponent<TinkerComponent>().stats.itemName);//astest
            }
            //    else
            //    {   // no prefab, create a new empty object
            //        // Warning: currently attack() is virtual & cannot be called for base weapons, need SwordScript or WrenchScript
            //        //          Stats can be viewed for these however
            //        weaponsInitilizer[i] = new GameObject("Empty");
            //        weaponsInitilizer[i].AddComponent(typeof(WeaponScript));
            //        weaponsInitilizer[i].GetComponent<WeaponScript>().stats = weaponStat;
            //        if (debugMode) Debug.Log("created object for " + i + " type:" + weaponsInitilizer[i].GetComponent<WeaponScript>().stats.weaponName); //astest
            //    }
            //    if (debugMode) Debug.Log("Weapon " + i + ": WeaponType:" + weaponsInitilizer[i].GetComponent<WeaponScript>().stats.weaponType
            //        + " Atk:" + weaponsInitilizer[i].GetComponent<WeaponScript>().stats.attack); //astest
        }
    ////Set weapons here
    //baseWeapons = weaponsInitilizer;
}
    //used for JSON. Loading all types and for loading players components
    [Serializable]
    public class ComponentsArray
    {
        public TinkerComponentStats[] components;
    }
}
