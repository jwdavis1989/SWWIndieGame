using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterWeaponManager : MonoBehaviour
{
    [Header("Owner CharacterManager")]
    public CharacterManager characterThatOwnsThisArsenal;
    [Header("Inventory")]
    public List<GameObject> ownedWeapons;
    public List<GameObject> ownedSpecialWeapons;
    public int indexOfEquippedWeapon = 0;
    public int indexOfEquippedSpecialWeapon = 0;
    [Header("Weapon Attachment to Character")]
    public GameObject mainHandWeaponAnchor;
    public GameObject offHandWeaponAnchor;

    [Header("Weapon Combo System")]
    public AttackType currentAttackType;

    /**
     * Adds weapon of any type to current weapons
     * Returns a reference to the weapon that was added
     */
    public WeaponScript AddWeaponToCurrentWeapons(WeaponType weaponType)
    {
        int i = (int)weaponType;
        bool isSpecial = WeaponsController.instance.baseWeapons[i].GetComponent<WeaponScript>().isSpecialWeapon;
        GameObject weaponToAdd;
        if (isSpecial)
        {
            weaponToAdd = WeaponsController.instance.CreateWeapon(weaponType, offHandWeaponAnchor.transform);
            ownedSpecialWeapons.Add(weaponToAdd);
        }
        else
        {
            weaponToAdd = WeaponsController.instance.CreateWeapon(weaponType, mainHandWeaponAnchor.transform);
            ownedWeapons.Add(weaponToAdd);
        }
        //Warning if using for npc - Currently still tracking single pokedex
        WeaponScript currentWeaponScript = WeaponsController.instance.baseWeapons[i].GetComponent<WeaponScript>();
        currentWeaponScript.hasObtained = true;
        return weaponToAdd.GetComponent<WeaponScript>();
    }
    /**
     * Change equipped weapon
     */
    public void ChangeWeapon(int index)
    {
        if (index < ownedWeapons.Count && ownedWeapons[index] != null)
        {
            ownedWeapons[indexOfEquippedWeapon].SetActive(false);
            indexOfEquippedWeapon = index;
            ownedWeapons[indexOfEquippedWeapon].SetActive(true);
            //Play the weapon swap animation
            characterThatOwnsThisArsenal.characterAnimatorManager.PlayTargetActionAnimation("Swap_Right_Weapon_01", false, true, true, true);
        }
    }
    //find next weapon and call ChangeWeapon
    public void nextWeapon()
    {
        int totalWeapons = ownedWeapons.Count;
        int newWeaponIndex = indexOfEquippedWeapon;
        if (ownedWeapons != null && totalWeapons > 0)
        {
            newWeaponIndex = (newWeaponIndex + 1 > totalWeapons - 1) ? 0 : newWeaponIndex + 1;
            ChangeWeapon(newWeaponIndex);
        }
    }
    //find prev weapon and call ChangeWeapon
    public void prevWeapon()
    {
        int totalWeapons = ownedWeapons.Count;
        int newWeaponIndex = indexOfEquippedWeapon;
        if (ownedWeapons != null && totalWeapons > 0)
        {
            newWeaponIndex = (newWeaponIndex - 1 < 0) ? totalWeapons - 1 : newWeaponIndex - 1;
            ChangeWeapon(newWeaponIndex);
        }
    }
    /**
     * Change equipped special weapon
     */
    public void ChangeSpecialWeapon(int index)
    {

        if (index < ownedSpecialWeapons.Count && ownedSpecialWeapons[index] != null)
        {
            ownedSpecialWeapons[indexOfEquippedSpecialWeapon].SetActive(false);
            indexOfEquippedSpecialWeapon = index;
            ownedSpecialWeapons[indexOfEquippedSpecialWeapon].SetActive(true);
            //Play the weapon swap animation
            characterThatOwnsThisArsenal.characterAnimatorManager.PlayTargetActionAnimation("Swap_Left_Weapon_01", false, true, true, true);
        }
    }
    //find next weapon and call ChangeSpecialWeapon
    public void nextSpecialWeapon()
    {
        int totalWeapons = ownedSpecialWeapons.Count;
        int newWeaponIndex = indexOfEquippedSpecialWeapon;
        if (ownedSpecialWeapons != null && totalWeapons > 0)
        {
            newWeaponIndex = (newWeaponIndex + 1 > totalWeapons - 1) ? 0 : newWeaponIndex + 1;
            ChangeSpecialWeapon(newWeaponIndex);
        }
    }
    //find prev weapon and call ChangeSpecialWeapon
    public void prevSpecialWeapon()
    {
        int totalWeapons = ownedSpecialWeapons.Count;
        int newWeaponIndex = indexOfEquippedSpecialWeapon;
        if (ownedSpecialWeapons != null && totalWeapons > 0)
        {
            newWeaponIndex = (newWeaponIndex - 1 < 0) ? totalWeapons - 1 : newWeaponIndex - 1;
            ChangeSpecialWeapon(newWeaponIndex);
        }
    }
    /**
     * Attempt to attack target with equiped weapon. If this behavior is elsewhere then this should be deleted - Removed attack from weapon Alec 11/2/24
     */
    //public void AttackTargetWithEquippedWeapon(GameObject target)
    //{
    //    if (target == null) return;
    //    if (ownedWeapons.Count > indexOfEquippedWeapon & ownedWeapons[indexOfEquippedWeapon] != null)
    //    {
    //        ownedWeapons[indexOfEquippedWeapon].GetComponent<WeaponScript>().attackTarget(target);
    //    }
    //}
    /**
     * Returns equipped weapon or special weapon if specified. Null if nothing equipped
     */
    public GameObject GetEquippedWeapon(bool special = false)
    {
        if (special)
        {
            if (ownedSpecialWeapons.Count == 0)
                return null;
            return ownedSpecialWeapons[indexOfEquippedSpecialWeapon];
        }
        if (ownedWeapons.Count == 0) 
            return null;
        return ownedWeapons[indexOfEquippedWeapon];
    }
    /**
 *  Loads weapons from Array
 *  Used by load game systems
 */
    public void setCurrentWeapons(WeaponsArray weaponsJson)
    {
        ownedWeapons = new List<GameObject>();
        ownedSpecialWeapons = new List<GameObject>();
        int i = 0;
        int specialI = 0;
        foreach (WeaponStats weaponStat in weaponsJson.weaponStats)
        {
            WeaponScript weaponScript = AddWeaponToCurrentWeapons(weaponStat.weaponType);
            if (weaponScript.isSpecialWeapon)
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
        if (ownedWeapons.Count > 0)
        {
            ownedWeapons[indexOfEquippedWeapon].SetActive(true);
        }
        if (ownedSpecialWeapons.Count > 0)
        {
            ownedSpecialWeapons[indexOfEquippedSpecialWeapon].SetActive(true);
        }
    }
    //Retruns JSON data used for save game
    public WeaponsArray GetCurrentWeapons()
    {
        WeaponsArray weaponsData = new WeaponsArray();
        weaponsData.weaponStats = new WeaponStats[ownedWeapons.Count + ownedSpecialWeapons.Count];
        int i = 0;
        foreach (GameObject weapon in ownedWeapons)
        {
            weaponsData.weaponStats[i++] = weapon.GetComponent<WeaponScript>().stats;
        }
        foreach (GameObject weapon in ownedSpecialWeapons)
        {
            weaponsData.weaponStats[i++] = weapon.GetComponent<WeaponScript>().stats;
        }
        return weaponsData;
    }
    //sets all weapons or special weapons to inactive
    public void SetAllWeaponsToInactive(bool targetSpecialWeaponStatus)
    {
        List<GameObject> weapons = targetSpecialWeaponStatus ? ownedSpecialWeapons : ownedWeapons;
        if (weapons.Count <= 0)
            return;
        foreach (GameObject weapon in weapons)
        {
            weapon.SetActive(false);
        }
    }

    public void PerformWeaponBasedAction(WeaponItemAction weaponAction, WeaponScript weaponPerformingAction) {
        //Way shown in tutorial, might not make sense for our version
        //weaponAction.AttemptToPerformAction(characterThatOwnsThisArsenal, weaponPerformingAction);

        //Work-around based on us not needing to store the weapon being used afaik
        weaponAction.AttemptToPerformAction(characterThatOwnsThisArsenal);
    }

    public void OpenDamageCollider() {
        ownedWeapons[indexOfEquippedWeapon].GetComponent<WeaponScript>().meleeWeaponDamageCollider.EnableDamageCollider();

        //TODO: Play Whoosh SFX

    }

    public void CloseDamageCollider() {
        ownedWeapons[indexOfEquippedWeapon].GetComponent<WeaponScript>().meleeWeaponDamageCollider.DisableDamageCollider();
    }
    public void DrainStaminaBasedOnAttack() {
        WeaponScript currentWeapon = ownedWeapons[indexOfEquippedWeapon].GetComponent<WeaponScript>();
        
        if (currentWeapon == null) {
            return;
        }

        float staminaDeducted = 0f;

        switch (currentAttackType) {
            case AttackType.LightAttack01:
            staminaDeducted = currentWeapon.stats.baseStaminaCost * currentWeapon.stats.lightAttack01StaminaCostModifier;
                break;
            default:
                break;
        }

        characterThatOwnsThisArsenal.characterStatsManager.currentStamina -= staminaDeducted;
    }
}
