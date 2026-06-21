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
    public GameObject wristOffHandWeaponAnchor;

    [Header("Weapon Combo System")]
    public AttackType currentAttackType;

    [Header("Special Weapon Cooldown")]
    public float specialtyCooldown = 5f;
    public float specialtyCooldownTimer = 0;
    public bool isSpecialWeaponOffCooldown = true;
    public float quickChargeCapacitorCooldownMultiplier = 0.8f;

    /**
     * Shorthands to access Main/Off hand weapons
     */
    public WeaponScript GetMainHand()
    {
        if (GetEquippedWeapon() != null)
            return GetEquippedWeapon().GetComponent<WeaponScript>();
        return null;
    }
    public WeaponScript GetOffHand()
    {
        if (GetEquippedWeapon(true) != null)
            return GetEquippedWeapon(true).GetComponent<WeaponScript>();
        return null;
    }
    public void Awake()
    {
        if(characterThatOwnsThisArsenal == null && gameObject.GetComponent<CharacterManager>() != null)
        {
            characterThatOwnsThisArsenal = gameObject.GetComponent<CharacterManager>();
        }

        if (ownedWeapons.Count > 0)
        {
            List<WeaponType> weaponTypes = new List<WeaponType>();
            foreach (var weapon in ownedWeapons)
            {
                weaponTypes.Add(weapon.GetComponent<WeaponScript>().stats.weaponType);
            }
            ownedWeapons = new List<GameObject>();

            foreach (WeaponType weaponType in weaponTypes)
            {
                AddWeaponToCurrentWeapons(weaponType);
            }
        }
    }

    public void Update()
    {
        HandleSpecialWeaponCooldown();
    }
    //Helper
    public int TotalWeapons()
    {
        int total = 0;
        if(ownedWeapons != null)
            total += ownedWeapons.Count;
        if(ownedSpecialWeapons != null)
            total += ownedSpecialWeapons.Count;
        return total;
    }
    /**
     * Adds weapon of any type to current weapons
     * Returns a reference to the weapon that was added
     */
    public WeaponScript AddWeaponToCurrentWeapons(WeaponType weaponType)
    {
        int i = (int)weaponType;
        WeaponScript newWeapon = WeaponsController.instance.baseWeapons[i].GetComponent<WeaponScript>();
        GameObject weaponToAdd;
        if (newWeapon.isSpecialWeapon)
        {
            if (!newWeapon.isWristWeapon)
            {
                weaponToAdd = WeaponsController.instance.CreateWeapon(weaponType, offHandWeaponAnchor.transform);
            }
            else
            {
                weaponToAdd = WeaponsController.instance.CreateWeapon(weaponType, wristOffHandWeaponAnchor.transform);
            }
            
            ownedSpecialWeapons.Add(weaponToAdd);

            //Update Equipped Weapon Icon if this is your first special weapon
            //Alec, this might cause bugs later if I goofed so heads-up lol
            if (characterThatOwnsThisArsenal.isPlayer && indexOfEquippedSpecialWeapon == 0)
            {
                PlayerUIManager.instance.playerUIHudManager.SetLeftWeaponQuickSlotIcon();
            }
        }
        else
        {
            weaponToAdd = WeaponsController.instance.CreateWeapon(weaponType, mainHandWeaponAnchor.transform);
            ownedWeapons.Add(weaponToAdd);

            //Update Equipped Weapon Icon if this is your first weapon
            //Alec, this might cause bugs later if I goofed so heads-up lol
            if (characterThatOwnsThisArsenal.isPlayer && indexOfEquippedWeapon == 0)
            {
                PlayerUIManager.instance.playerUIHudManager.SetRightWeaponQuickSlotIcon();
            }
        }
        //Warning if using for npc - Currently still tracking single pokedex
        WeaponScript currentWeaponScript = WeaponsController.instance.baseWeapons[i].GetComponent<WeaponScript>();
        currentWeaponScript.stats.currentDurability = currentWeaponScript.stats.durability;
        currentWeaponScript.hasObtained = true;

        //Initialize Weapon Owner to avoid a race condition in Awake()
        currentWeaponScript.characterThatOwnsThisWeapon = characterThatOwnsThisArsenal;
        
        return weaponToAdd.GetComponent<WeaponScript>();
    }
    public void EquipWeapon(GameObject weapon)
    {
        Debug.Log("Active weapon is " + weapon.name);//astest
        int index = -1;
        index = ownedWeapons.FindIndex(wpn => wpn == weapon);
        if (index == -1)
        {
            index = ownedSpecialWeapons.FindIndex(wpn => wpn == weapon);
            if (index != -1)
                ChangeSpecialWeapon(index);
            else
                Debug.LogWarning("EquipWeapon called and not found");
        }
        else
            ChangeWeapon(index);
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
            characterThatOwnsThisArsenal.characterAnimatorManager.PlayTargetActionAnimation("Swap_Right_Weapon_01", false, false, true, true);

            //Update Weapon Animator Controller to fit new Weapon
            characterThatOwnsThisArsenal.characterAnimatorManager.UpdateAnimatorControllerByWeapon(ownedWeapons[indexOfEquippedWeapon].GetComponent<WeaponScript>());

            //Update Weapon Slot UI for the player only
            if (characterThatOwnsThisArsenal.isPlayer)
            {
                PlayerUIManager.instance.playerUIHudManager.SetRightWeaponQuickSlotIcon();
            }
        }
    }
    //find next weapon and call ChangeWeapon
    public void NextWeapon()
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
    public void PrevWeapon()
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
            characterThatOwnsThisArsenal.characterAnimatorManager.PlayTargetActionAnimation("Swap_Left_Weapon_01", false, false, true, true);

            //Update Weapon Slot UI for the player only
            if (characterThatOwnsThisArsenal.isPlayer) {
                PlayerUIManager.instance.playerUIHudManager.SetLeftWeaponQuickSlotIcon();
            }
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

    public void HandleSpecialWeaponCooldown()
    {
        if (!isSpecialWeaponOffCooldown && specialtyCooldownTimer >= 0)
        {
            specialtyCooldownTimer -= Time.deltaTime;
        }
        else
        {
            isSpecialWeaponOffCooldown = true;
        }
    }

    public virtual void ResetSpecialWeaponCooldownTimer()
    {
        specialtyCooldownTimer = specialtyCooldown;
        if (characterThatOwnsThisArsenal.isPlayer && InventionManager.instance.CheckHasUpgrade(InventionID.QUICKCHARGE_CAPACITORY))
        {
            specialtyCooldownTimer *= quickChargeCapacitorCooldownMultiplier;
        }
        isSpecialWeaponOffCooldown = false;
    }

    public void PerformWeaponBasedAction(WeaponItemAction weaponAction, WeaponScript weaponPerformingAction)
    {
        //Way shown in tutorial, might not make sense for our version
        //weaponAction.AttemptToPerformAction(characterThatOwnsThisArsenal, weaponPerformingAction);

        //Work-around based on us not needing to store the weapon being used afaik
        weaponAction.AttemptToPerformAction(characterThatOwnsThisArsenal);
    }

    public void OpenDamageCollider() {
        WeaponScript currentWeapon = ownedWeapons[indexOfEquippedWeapon].GetComponent<WeaponScript>();
        currentWeapon.weaponDamageCollider.EnableDamageCollider();
        if (currentWeapon.bladeTrailVFX)
        {   
            currentWeapon.bladeTrailVFX.gameObject.SetActive(true);
        }
        //Play Whoosh SFX
        PlayMeleeWeaponSwingSFX();
    }

    public void CloseDamageCollider() {
        WeaponScript currentWeapon = ownedWeapons[indexOfEquippedWeapon].GetComponent<WeaponScript>();
        currentWeapon.weaponDamageCollider.DisableDamageCollider();
        if (currentWeapon.bladeTrailVFX)
        {   
            currentWeapon.bladeTrailVFX.gameObject.SetActive(false);
        }
    }

    public void EnableBladeTrailVFX()
    {
        WeaponScript currentWeapon = ownedWeapons[indexOfEquippedWeapon].GetComponent<WeaponScript>();
        if (currentWeapon.bladeTrailVFX)
        {   
            currentWeapon.bladeTrailVFX.gameObject.SetActive(true);
        }
    }

    public void DisableBladeTrailVFX()
    {
        WeaponScript currentWeapon = ownedWeapons[indexOfEquippedWeapon].GetComponent<WeaponScript>();
        if (currentWeapon.bladeTrailVFX)
        {   
            currentWeapon.bladeTrailVFX.gameObject.SetActive(false);
        }
    }

    public void EnableSpecialWeaponTrailVFX()
    {
        WeaponScript currentWeapon = ownedSpecialWeapons[indexOfEquippedSpecialWeapon].GetComponent<WeaponScript>();
        if (currentWeapon.bladeTrailVFX)
        {   
            currentWeapon.bladeTrailVFX.gameObject.SetActive(true);
        }
    }

    public void DisableSpecialWeaponTrailVFX()
    {
        WeaponScript currentWeapon = ownedSpecialWeapons[indexOfEquippedSpecialWeapon].GetComponent<WeaponScript>();
        if (currentWeapon.bladeTrailVFX)
        {   
            currentWeapon.bladeTrailVFX.gameObject.SetActive(false);
        }
    }
    
    public void OpenJumpAttackDamageCollider() {
        //Play Whoosh SFX
        PlayMeleeWeaponSwingSFX();
    }

    public void CloseJumpAttackDamageCollider() {
        //ownedWeapons[indexOfEquippedWeapon].GetComponent<WeaponScript>().jumpAttackWeaponDamageCollider.DisableDamageCollider();
    }

    public void PlayJumpAttackImpactVFX()
    {
        ownedWeapons[indexOfEquippedWeapon].GetComponent<WeaponScript>().InstantiateJumpAttackCollider();
    }

    public void DrainStaminaBasedOnAttack()
    {
        WeaponScript currentWeapon = ownedWeapons[indexOfEquippedWeapon].GetComponent<WeaponScript>();

        if (currentWeapon == null)
        {
            return;
        }

        float staminaDeducted = currentWeapon.stats.baseStaminaCost;

        switch (currentAttackType)
        {
            //Light Attacks
            case AttackType.LightAttack01:
                staminaDeducted *= currentWeapon.stats.lightAttack01StaminaCostModifier;
                break;
            case AttackType.LightAttack02:
                staminaDeducted *= currentWeapon.stats.lightAttack02StaminaCostModifier;
                break;
            case AttackType.LightAttack03:
                staminaDeducted *= currentWeapon.stats.lightAttack03StaminaCostModifier;
                break;

            //Heavy Attacks
            case AttackType.HeavyAttack01:
                staminaDeducted *= currentWeapon.stats.heavyAttack01StaminaCostModifier;
                break;
            case AttackType.HeavyAttack02:
                staminaDeducted *= currentWeapon.stats.heavyAttack02StaminaCostModifier;
                break;

            //Charge Heavy Attacks
            case AttackType.ChargedAttack01:
                staminaDeducted *= currentWeapon.stats.heavyAttack01StaminaCostModifier;
                break;
            case AttackType.ChargedAttack02:
                staminaDeducted *= currentWeapon.stats.heavyAttack02StaminaCostModifier;
                break;

            //Jumping Attacks
            case AttackType.LightJumpAttack01:
                staminaDeducted *= currentWeapon.stats.lightJumpAttack01StaminaCostModifier;
                break;
            case AttackType.HeavyJumpAttack01:
                staminaDeducted *= currentWeapon.stats.heavyJumpAttack01StaminaCostModifier;
                break;

            //Running Attacks
            case AttackType.RunningLightAttack01:
                staminaDeducted *= currentWeapon.stats.lightRunningAttack01StaminaCostModifier;
                break;

            //Rolling Attacks
            case AttackType.RollingLightAttack01:
                staminaDeducted *= currentWeapon.stats.lightRollingAttack01StaminaCostModifier;
                break;

            //Backstep Attacks
            case AttackType.BackstepLightAttack01:
                staminaDeducted *= currentWeapon.stats.lightBackstepAttack01StaminaCostModifier;
                break;

            //Magic Attacks
            case AttackType.AreaSpellAttack01:
                staminaDeducted *= currentWeapon.stats.areaSpellAttack01StaminaCostModifier;
                break;

            //Default
            default:
                break;
        }

        characterThatOwnsThisArsenal.characterStatsManager.currentStamina -= staminaDeducted;
    }

    private void PlayMeleeWeaponSwingSFX() {
        AudioClip meleeWeaponSwingSFX;
        //e.g. If Fire damage is greater, play burn SFX
        //e.g. If Lightning damage is greater, play Zap SFX

        switch (GetMainHand().weaponFamily) {
                case WeaponFamily.Swords:
                    meleeWeaponSwingSFX = WorldSoundFXManager.instance.ChooseRandomSFXFromArray(WorldSoundFXManager.instance.slashingWeaponSwingSFX);
                    characterThatOwnsThisArsenal.characterSoundFXManager.PlayAdvancedSoundFX(meleeWeaponSwingSFX, 0.5f, 1f, true, 0.1f);
                    break;
                case WeaponFamily.GreatSwords:
                    meleeWeaponSwingSFX = WorldSoundFXManager.instance.ChooseRandomSFXFromArray(WorldSoundFXManager.instance.heavySlashingWeaponSwingSFX);
                    characterThatOwnsThisArsenal.characterSoundFXManager.PlayAdvancedSoundFX(meleeWeaponSwingSFX, 0.5f, 0.8f, true, 0.1f);
                    break;
                case WeaponFamily.HammersOrWrenches:
                    meleeWeaponSwingSFX = WorldSoundFXManager.instance.ChooseRandomSFXFromArray(WorldSoundFXManager.instance.bludgeoningWeaponSwingSFX);
                    characterThatOwnsThisArsenal.characterSoundFXManager.PlayAdvancedSoundFX(meleeWeaponSwingSFX, 0.5f, 1f, true, 0.1f);
                    break;
                case WeaponFamily.Scythes:
                    meleeWeaponSwingSFX = WorldSoundFXManager.instance.ChooseRandomSFXFromArray(WorldSoundFXManager.instance.scytheWeaponSwingSFX);
                    characterThatOwnsThisArsenal.characterSoundFXManager.PlayAdvancedSoundFX(meleeWeaponSwingSFX, 0.5f, 0.8f, true, 0.1f);
                    break;
                case WeaponFamily.Daggers:
                    meleeWeaponSwingSFX = WorldSoundFXManager.instance.ChooseRandomSFXFromArray(WorldSoundFXManager.instance.piercingWeaponSwingSFX);
                    characterThatOwnsThisArsenal.characterSoundFXManager.PlayAdvancedSoundFX(meleeWeaponSwingSFX, 0.5f, 1f, true, 0.1f);
                    break;
                case WeaponFamily.NotYetSet:
                    Debug.Log("Weapon Family not set on Prefab!");
                    break;
                default:
                    Debug.Log("Weapon Family not set on Prefab!");
                    break;
            }
    }
}
