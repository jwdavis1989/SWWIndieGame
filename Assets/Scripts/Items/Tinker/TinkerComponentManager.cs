using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TinkerComponentManager : MonoBehaviour
{
    /**
    * Drops a Tinker Component of any type other than weapon at the specified location
    * Returns a reference to the dropped component
    */
    public GameObject DropComponent(TinkerComponentType type, Transform location)
    {
        return Instantiate(baseComponents[(int)type], location);
    }
    //**** DEBUG AREA ASTEST
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
            //breakdown equipped weapon of player
            BreakDownWeapon(PlayerWeaponManager.instance.indexOfEquippedWeapon, false, PlayerWeaponManager.instance);
            //equip first weapon
            PlayerWeaponManager.instance.indexOfEquippedWeapon = 0;
            if (PlayerWeaponManager.instance.ownedWeapons.Count > 0)
                PlayerWeaponManager.instance.ownedWeapons[0].SetActive(true);
        }
        if (addAWeaponComponentToEquippedWeapon)
        {
            addAWeaponComponentToEquippedWeapon = false;
            if (weaponComponents.Count > 0)
                AddTinkerComponentToWeapon(PlayerWeaponManager.instance.ownedWeapons[PlayerWeaponManager.instance.indexOfEquippedWeapon], weaponComponents[0], true);
        }
    }
    public void DropRandomItem(Transform transform, float distance = 0)
    {
        int i = UnityEngine.Random.Range(0, baseComponents.Length - 1);
        if (baseComponents[i] == null) return;
        Instantiate(baseComponents[i], transform.position + (transform.forward * distance), transform.rotation);
    }
    //****END DEBUG AREA
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
        //Load base stats from json
        LoadAllComponentTypes();
    }
    //Array of Tinker Component Pre-Fabs. Use to instantiate components ingame and track players count of each component
    [Header("TinkerComponentManager is a singleton containing:\n " +
    "\tPrefabs for each tinker component\n" +
    "\tMethod for dropping components to the Game world\n" +
    "\tMethods for upgrading/breaking down weapons\n" +
    "\tA record of the players current tinker components\n")]
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
    * @Returns a reference to the component that was added
    */
    public TinkerComponent BreakDownWeapon(int index, bool specialWeapon, CharacterWeaponManager characterWeapons)
    {
        
        int weaponIndex = specialWeapon ? characterWeapons.indexOfEquippedSpecialWeapon : characterWeapons.indexOfEquippedWeapon;
        List <GameObject> weaponsList = specialWeapon ? characterWeapons.ownedSpecialWeapons : characterWeapons.ownedWeapons;
        if (weaponsList.Count < index)
            return null;
        GameObject ownedWeapon = weaponsList[index];
        WeaponScript weapon = ownedWeapon.GetComponent<WeaponScript>();
        if (weapon.stats.level < 5)
            throw new ArgumentException("Weapon must be level 5");
        ownedWeapon.AddComponent<TinkerComponent>();
        TinkerComponent rv = ownedWeapon.GetComponent<TinkerComponent>();
        rv.stats.elementalStats = weapon.stats.elemental;
        rv.stats.attack = weapon.stats.attack;
        rv.stats.durability = weapon.stats.durability;
        rv.stats.stability = weapon.stats.stability;
        rv.stats.block = weapon.stats.block;
        rv.stats.isWeapon = true;
        rv.stats.count = 1;
        rv.stats.itemName = weapon.stats.weaponName;
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
     * Adds a component to a weapon. returns true if succesfully updated
     */
    public bool UseComponent(GameObject weapon, GameObject tinkerComponent)
    {
        return AddTinkerComponentToWeapon(weapon, tinkerComponent, true);
    }
    public bool AddTinkerComponentToWeapon(GameObject weaponToUpgrade, GameObject tinkerComponentPassed, bool doUpdate)
    {
        TinkerComponent tinkerComponentToAdd = tinkerComponentPassed.GetComponent<TinkerComponent>();
        WeaponScript weapon = weaponToUpgrade.GetComponent<WeaponScript>();
        // new tinker points
        if (weapon.stats.currentTinkerPoints == 0) return false;
        //used passed component for weapons. Ensure using base components for regular components
        TinkerComponent tinkerComponent = tinkerComponentToAdd.stats.isWeapon ? tinkerComponentToAdd : baseComponents[(int)tinkerComponentToAdd.stats.componentType].GetComponent<TinkerComponent>();
        Debug.Log("AddTinkerComponentToWeapon " + tinkerComponent.stats.itemName);//astest
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
        //calc other stats
        float newStab = Mathf.Min(weapon.stats.stability + tinkerComponent.stats.stability, weapon.stats.maxStability);
        float newBlock = Mathf.Min(weapon.stats.block + tinkerComponent.stats.block, weapon.stats.maxBlock);
        float newDur = Mathf.Min(weapon.stats.durability + tinkerComponent.stats.durability, weapon.stats.maxDurability);
        //if any stat will be upgraded then we can upgrade
        if (diffWithPrev.firePower > 0 ||
            diffWithPrev.icePower > 0  ||
            diffWithPrev.lightningPower > 0 ||
            diffWithPrev.windPower > 0  ||
            diffWithPrev.earthPower > 0 ||
            diffWithPrev.lightPower > 0 ||
            diffWithPrev.beastPower > 0 ||
            diffWithPrev.scalesPower > 0 ||
            weapon.stats.stability < newStab ||
            weapon.stats.block < newBlock ||
            weapon.stats.durability < newDur ||
            diffWithPrev.techPower > 0 ||
            weapon.stats.attack < newAttack)
        {
            canUpgrade = true;
            if (doUpdate)
            {
                weapon.stats.elemental = newStats;
                weapon.stats.attack = newAttack;
                weapon.stats.stability = newStab;
                weapon.stats.block = newBlock;
                weapon.stats.durability = newDur;
                tinkerComponent.stats.count--;
                weapon.stats.currentTinkerPoints--;
                if (tinkerComponent.stats.isWeapon)
                {
                    weaponComponents.Remove(tinkerComponentPassed);
                }
                weapon.SetWeaponDamage();
            }
        }
        Debug.Log("AddTinkerComponentToWeapon ret " + canUpgrade);//astest
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
