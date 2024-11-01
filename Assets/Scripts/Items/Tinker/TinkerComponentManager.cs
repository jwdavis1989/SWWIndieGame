using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TinkerComponentManager : MonoBehaviour
{
    //****DEBUG ASTEST
    [Header("DEBUG")]
    public bool debugMode = false;
    public bool dropRandomItem = false;
    public bool breakDownEquippedWeapon = false;
    public bool addAWeaponComponentToEquippedWeapon = false;
    public void Update()
    {//astest
        if (dropRandomItem)
        {
            dropRandomItem = false;
            DropRandomItem(transform);
        }
        if (breakDownEquippedWeapon)
        {
            breakDownEquippedWeapon = false;
            //breakdown equipped weapon
            BreakDownWeapon(PlayerWeaponManager.instance.indexOfEquippedWeapon, false);
            //equip first weapon
            PlayerWeaponManager.instance.indexOfEquippedWeapon = 0;
            if(PlayerWeaponManager.instance.ownedWeapons.Count > 0)
                PlayerWeaponManager.instance.ownedWeapons[0].SetActive(true);
        }
        if (addAWeaponComponentToEquippedWeapon)
        {
            addAWeaponComponentToEquippedWeapon = false;
            if(weaponComponents.Count > 0)
                AddTinkerComponentToWeapon(PlayerWeaponManager.instance.ownedWeapons[PlayerWeaponManager.instance.indexOfEquippedWeapon], weaponComponents[0], true);
        }
    }
    public void DropRandomItem(Transform transform, float distance = 0)
    {
        int i = UnityEngine.Random.Range(0, baseComponents.Length-1);
        if (baseComponents[i] == null) return;
        Instantiate(baseComponents[i], transform.position + (transform.forward * distance), transform.rotation);
    }
    //****DEBUG

    [Header("TinkerComponentManager is a singleton containing prefabs for each tinker component\n" +
        ", methods for upgrading/breaking down weapons\n" +
        ", and a record of the players current tinker components\n")]
    public static TinkerComponentManager instance;
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
    //Array of Tinker Component Pre-Fabs. Use to instantiate components ingame and track players count of each component
    [Header("Array of Tinker Component Pre-Fabs. \n" +
        "Use to instantiate components ingame and\n track players count of each component")]
    public GameObject[] baseComponents;
    //List of weapons turned in to tinker components
    [Header("List of weapons turned in to tinker components")]
    public List<GameObject> weaponComponents = new List<GameObject>();
    //JSON file contaning base stats. Will overwrite prefab!
    [Header("JSON file contaning base stats")]
    public TextAsset baseComponentJsonFile;
    /**
    * break down weapon into a tinker component and add to owned components
    * @Returns the component that was added
    */
    public TinkerComponent BreakDownWeapon(int index, bool specialWeapon)
    {
        
        int weaponIndex = specialWeapon ? PlayerWeaponManager.instance.indexOfEquippedSpecialWeapon : PlayerWeaponManager.instance.indexOfEquippedWeapon;
        List <GameObject> weaponsList = specialWeapon ? PlayerWeaponManager.instance.ownedSpecialWeapons : PlayerWeaponManager.instance.ownedWeapons;
        if (weaponsList.Count < index)
            return null;
        GameObject ownedWeapon = weaponsList[index];
        WeaponScript weapon = ownedWeapon.GetComponent<WeaponScript>();
        ownedWeapon.AddComponent<TinkerComponent>();
        TinkerComponent rv = ownedWeapon.GetComponent<TinkerComponent>();
        rv.stats.elementalStats = weapon.stats.elemental;
        rv.stats.attack = weapon.stats.attack;
        rv.stats.durability = weapon.stats.durability;
        rv.stats.stability = weapon.stats.stability;
        rv.stats.block = weapon.stats.block;
        rv.stats.isWeapon = true;
        rv.stats.count = 1;
        weaponComponents.Add(ownedWeapon);
        weaponsList.Remove(ownedWeapon);
        ownedWeapon.SetActive(false);
        return rv;
    }
    /**
    * add or subtract a tinker component from the player
    */
    public void AddBaseComponentToPlayer(TinkerComponentType type, int count)
    {
        TinkerComponentManager.instance.baseComponents[(int)type].GetComponent<TinkerComponent>().stats.count += count;
    }
    /**
     * CanUseComponent will return true if any stat will be upgraded. I.e. if any matching stat is not currently maxed
     */
    public bool CanUseComponent(GameObject weapon, GameObject tinkerComponent)
    {
        return AddTinkerComponentToWeapon(weapon, tinkerComponent, false);
    }
    /**
     * Adds a component to a weapon
     */
    public bool UseComponent(GameObject weapon, GameObject tinkerComponent)
    {
        return AddTinkerComponentToWeapon(weapon, tinkerComponent, true);
    }
    public bool AddTinkerComponentToWeapon(GameObject weaponToUpgrade, GameObject tinkerComponentPassed, bool doUpdate)
    {
        TinkerComponent tinkerComponentToAdd = tinkerComponentPassed.GetComponent<TinkerComponent>();
        WeaponScript weapon = weaponToUpgrade.GetComponent<WeaponScript>();
        //used passed component for weapons. Ensure using base components for regular components
        TinkerComponent tinkerComponent = tinkerComponentToAdd.stats.isWeapon ? tinkerComponentToAdd : baseComponents[(int)tinkerComponentToAdd.stats.componentType].GetComponent<TinkerComponent>();
        //can't add if out of that component
        if (tinkerComponent.stats.count <= 0) return false;
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
        ElementalStats diffWithPrev = newStats.Subract(weapon.stats.elemental);
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
                if (tinkerComponent.stats.isWeapon)
                {
                    weaponComponents.Remove(tinkerComponentPassed);
                }
            }
        }
        return canUpgrade;
    }
    public void LoadAllComponentTypes()
    {
        if(baseComponentJsonFile == null)
        {
            Debug.Log("TinkerComponentManager.baseComponentJsonFile is missing!");
            return;
        }
        //add prefabs to initilizer array
        GameObject[] componentInitilizer = new GameObject[(int)TinkerComponentType.Weapon];//Enum.GetValues(typeof(WeaponType)).Cast<int>().Max()];
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
        }
        //Set baseComponents here
        baseComponents = componentInitilizer;
}
    //used for JSON. Loading all types and for loading players components
    [Serializable]
    public class ComponentsArray
    {
        public TinkerComponentStats[] components;
    }
}
