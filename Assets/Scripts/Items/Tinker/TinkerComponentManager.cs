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
        List<GameObject> weaponsList = specialWeapon ? characterWeapons.ownedSpecialWeapons : characterWeapons.ownedWeapons;
        if (weaponsList.Count < index)
            return null;
        GameObject wpnToBreak = weaponsList[index];
        WeaponScript weapon = wpnToBreak.GetComponent<WeaponScript>();
        if (weapon.stats.level < 5)
            throw new ArgumentException("Weapon must be level 5");
        wpnToBreak.AddComponent<TinkerComponent>();
        TinkerComponent rv = wpnToBreak.GetComponent<TinkerComponent>();
        rv.stats.elementalStats = weapon.stats.elemental;
        rv.stats.attack = weapon.stats.attack;
        rv.stats.durability = weapon.stats.durability;
        rv.stats.stability = weapon.stats.stability;
        rv.stats.block = weapon.stats.block;
        rv.stats.isWeapon = true;
        rv.stats.isSpecialWpn = specialWeapon;
        rv.stats.count = 1;
        rv.stats.itemName = weapon.stats.weaponName;
        rv.spr = weapon.spr;
        weaponComponents.Add(wpnToBreak); //add component
        GameObject eqWpn = characterWeapons.GetEquippedWeapon(specialWeapon);
        weaponsList.Remove(wpnToBreak);
        //reset equipped weapon index as it may have changed
        if (specialWeapon)
        {
            if (eqWpn == wpnToBreak)
                characterWeapons.indexOfEquippedSpecialWeapon = 0;
            else
                characterWeapons.indexOfEquippedSpecialWeapon = characterWeapons.ownedSpecialWeapons.IndexOf(eqWpn);
        }
        else
        {
            if (eqWpn == wpnToBreak)
                characterWeapons.indexOfEquippedWeapon = 0;
            else
                characterWeapons.indexOfEquippedWeapon = characterWeapons.ownedWeapons.IndexOf(eqWpn);
        }
        wpnToBreak.SetActive(false);
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
    private bool AddTinkerComponentToWeapon(GameObject weaponToUpgrade, GameObject tinkerComponentPassed, bool doUpdate)
    {
        if (weaponToUpgrade == null) { return false; }
        TinkerComponent tinkerComponentToAdd = tinkerComponentPassed.GetComponent<TinkerComponent>();
        WeaponScript weapon = weaponToUpgrade.GetComponent<WeaponScript>();
        // new tinker points
        if (weapon.stats.currentTinkerPoints == 0) return false;
        //used passed component for weapons. Ensure using base components for regular components
        TinkerComponent tinkerComponent = tinkerComponentToAdd.stats.isWeapon ? tinkerComponentToAdd : baseComponents[(int)tinkerComponentToAdd.stats.componentType].GetComponent<TinkerComponent>();
        //can't add if out of that component
        if (tinkerComponent.stats.count <= 0) return false;
        bool canUpgrade = false;
        float newAttack = weapon.stats.attack;
        //calulate new attack
        if (tinkerComponent.stats.isWeapon)// broken down weapon component
        {
            if (weapon.isSpecialWeapon && !tinkerComponent.stats.isSpecialWpn)
                return false;//can only add main hand to main hand / off hand to off hand
            if (!weapon.isSpecialWeapon && tinkerComponent.stats.isSpecialWpn)
                return false;
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
        //calc other stats
        float newStab = Mathf.Min(weapon.stats.stability + tinkerComponent.stats.stability, weapon.stats.maxStability);
        float newBlock = Mathf.Min(weapon.stats.block + tinkerComponent.stats.block, weapon.stats.maxBlock);
        float newDur = Mathf.Min(weapon.stats.durability + tinkerComponent.stats.durability, weapon.stats.maxDurability);
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
            (diffWithPrev.techPower > 0) ||
            (newStab > weapon.stats.stability) ||
            (newBlock > weapon.stats.block) ||
            (newDur > weapon.stats.durability) ||
            (newAttack > weapon.stats.attack))
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
        return canUpgrade;
    }
    /** After removing json part this is just sorting the components by type which is still important */
    public void LoadAllComponentTypes()
    {
        //add prefabs to initilizer array
        GameObject[] componentInitilizer = new GameObject[(int)TinkerComponentType.Weapon];//Enum.GetValues(typeof(WeaponType)).Cast<int>().Max()];
        foreach (var component in baseComponents)
        {
            componentInitilizer[(int)component.GetComponent<TinkerComponent>().stats.componentType] = component;
        }
        //Set baseComponents here
        baseComponents = componentInitilizer;
    }
    public ComponentsArray CreateSaveData(bool weapon = false)
    {
        ComponentsArray saveData = new ComponentsArray();
        if (weapon)
        {
            saveData.components = new TinkerComponentStats[weaponComponents.Count];
            foreach (GameObject component in weaponComponents)
            {
                TinkerComponentStats stats = component.GetComponent<TinkerComponent>().stats;
                saveData.components[(int)stats.componentType] = stats;
            }
        }
        else
        {
            saveData.components = new TinkerComponentStats[baseComponents.Length];
            foreach (GameObject component in baseComponents)
            {
                TinkerComponentStats stats = component.GetComponent<TinkerComponent>().stats;
                saveData.components[(int)stats.componentType] = stats;
            }
        }
        return saveData;
    }
    public void LoadSaveData(ComponentsArray saveData, bool weapon = false)
    {
        foreach (TinkerComponentStats stats in saveData.components)
        {
            if (weapon)
            {//TODO unfinished for weapons
                GameObject newComponent = new GameObject();
                newComponent.AddComponent<TinkerComponent>();
                newComponent.GetComponent<TinkerComponent>().stats = stats;
                weaponComponents.Add(newComponent);
            }
            else
            {
                baseComponents[(int)stats.componentType].GetComponent<TinkerComponent>().stats = stats;
            }
        }
    }
}
[Serializable] //used for JSON. Loading all types and for loading players components
public class ComponentsArray
{
    public TinkerComponentStats[] components;
}