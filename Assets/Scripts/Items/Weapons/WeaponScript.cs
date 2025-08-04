using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
/** 
 * Enum of all weapon types.
 */
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

    //Monster Weapons
    SkeleShortSword,
    //Limit - Nothing past here
    UNKNOWN
}

public enum WeaponFamily {
    Swords,
    GreatSwords,
    HammersOrWrenches,
    Scythes,
    Daggers,
    SemiAutoGuns,
    BurstFireGuns,
    LaserGuns,
    Shotguns,
    GrenadeLaunchers,
    MagicRosary,
    MagicWands,
    MagicStaves,
    MagicRings,
    Drones,
    NotYetSet
}

/*
 * Serializable WeaponStats used for JSON saving
 */
[Serializable]
public class WeaponStats
{
    [Header("Weapon Type - Important - Set in Prefab")]
    public WeaponType weaponType = 0;
    public bool isMonsterWeapon = false;

    [Header("Weapon Attributes")]
    public float attack = 1.0f;
    public float maxAttack = 1.0f;
    public float basePoiseDamage = 35f; //Base 35 in case it's caused by traps
    public float durability = 1;
    public float maxDurability = 1;
    public float block = 1.0f;
    public float maxBlock = 1.0f;
    public float stability = 1.0f;
    public float maxStability = 1.0f;
    public ElementalStats elemental;
    public ElementalStats maxElemental;
    public float speed = 1.0f;
    public float maxSpeed = 1.0f;
    public float specialtyCooldown = 0;
    public float maxSpecialtyCooldown = 0; // Unused?
    public float xpToLevel = 100.0f;
    public int tinkerPointsPerLvl = 1;
    public float currentDurability = 1.0f;
    public int level = 1;
    public float currentExperiencePoints = 0.0f;
    public float experiencePointsToNextLevel = 100.0f;
    public int currentTinkerPoints = 0;
    public String weaponName = "BaseWeaponName";

    [Header("Stamina Costs")]
    public float baseStaminaCost = 20f;

    //Light
    public float lightAttack01StaminaCostModifier = 1f;
    public float lightAttack02StaminaCostModifier = 1f;
    public float lightAttack03StaminaCostModifier = 1f;

    //Heavy
    public float heavyAttack01StaminaCostModifier = 1.2f;
    public float heavyAttack02StaminaCostModifier = 1.2f;

    //Running
    public float lightRunningAttack01StaminaCostModifier = 1f;

    //Rolling
    public float lightRollingAttack01StaminaCostModifier = 1f;

    //Backstepping
    public float lightBackstepAttack01StaminaCostModifier = 1f;

    [Header("Motion Values")]
    //Light
    public float lightAttack01DamageMotionValue = 1f;
    public float lightAttack02DamageMotionValue = 1.1f;
    public float lightAttack03DamageMotionValue = 1.2f;

    //Heavy
    public float heavyAttack01DamageMotionValue = 1.4f;
    public float heavyAttack02DamageMotionValue = 1.6f;

    //Charged Heavy
    public float heavyChargedAttack01DamageMotionValue = 2.0f;
    public float heavyChargedAttack02DamageMotionValue = 2.2f;

    //Running
    public float lightRunningAttack01DamageMotionValue = 1f;

    //Rolling
    public float lightRollingAttack01DamageMotionValue = 1f;

    //Backstepping
    public float lightBackstepAttack01DamageMotionValue = 1f;


}
/*
 * Serializable ElementalStats used for JSON saving
 */
[Serializable]
public class ElementalStats
{
    public float firePower = 0;
    public float icePower = 0;
    public float lightningPower = 0;
    public float windPower = 0;
    public float earthPower = 0;
    public float lightPower = 0;
    public float beastPower = 0;
    public float scalesPower = 0;
    public float techPower = 0;
    public ElementalStats Subract(ElementalStats subtractor)
    {
        ElementalStats diff = new ElementalStats();
        diff.firePower = firePower - subtractor.firePower;
        diff.icePower = icePower - subtractor.icePower;
        diff.lightningPower = lightningPower - subtractor.lightningPower;
        diff.windPower = windPower - subtractor.windPower;
        diff.earthPower = earthPower - subtractor.earthPower;
        diff.lightPower = lightPower - subtractor.lightPower;
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
        sum.lightPower = lightPower + other.lightPower;
        sum.beastPower = beastPower + other.beastPower;
        sum.scalesPower = scalesPower + other.scalesPower;
        sum.techPower = techPower + other.techPower;
        return sum;
    }
}
/** 
 * Base weapon script containing weapon stats and behaviors
 * MonoBehaviour - Can add to Game Objects
 */
public class WeaponScript : MonoBehaviour
{
    [Header("Weapon Family - Important - Set in Prefab")]
    public WeaponFamily weaponFamily = 0;

    [Header("Weapon Damage Collider")]
    [SerializeField] public MeleeWeaponDamageCollider weaponDamageCollider;

    [Header("Currently set on prefab")]
    public bool isSpecialWeapon = false;
    [Header("Image used for menu icon")]
    public Sprite spr = null;
    [Header("Will appear as ??? on weapon sheet until obtained")]
    public bool hasObtained = false;
    [Header("These are all written to JSON when saving a game.")]
    public WeaponStats stats;

    [Header("Actions")]
    public WeaponItemAction mainHandLightAttackAction;  //One hand light attack
    public WeaponItemAction mainHandHeavyAttackAction;  //One hand heavy attack

    [Header("Projectile")]
    public GameObject projectile = null;

    [Header("Animations")]
    public AnimatorOverrideController weaponAnimatorOverride;
    [SerializeField] protected string offHandSpellAnimation;

    [Header("Weapon Family Specific")]
    [Header("Magic Special Weapons")]
    public float fullChargingTraitModifier = 1.25f;
    [SerializeField] public GameObject spellCastWarmUpVFX;
    [SerializeField] public GameObject spellProjectileVFX;
    //TODO: Add full charge version of spell VFX
    public AudioClip spellReleaseSFX;
    public AudioClip spellProjectileSFX;




    public void Awake()
    {
        if (isSpecialWeapon)
        {
            if (projectile != null)
            {
                weaponDamageCollider = projectile.GetComponent<MeleeWeaponDamageCollider>();
            }
            else
            {
                weaponDamageCollider = GetComponentInChildren<MeleeWeaponDamageCollider>();
            }
        }
        else
        {
            weaponDamageCollider = GetComponentInChildren<MeleeWeaponDamageCollider>();
        }
        if (weaponDamageCollider)
        {
            SetWeaponDamage();
        }
    }
    //TODO: Call this when you upgrade weapons too!
    public void SetWeaponDamage()
    {
        if (weaponDamageCollider == null)
        {
            return;
        }

        weaponDamageCollider.characterCausingDamage = GetComponentInParent<CharacterWeaponManager>().characterThatOwnsThisArsenal;
        weaponDamageCollider.isMainHand = !isSpecialWeapon;
        weaponDamageCollider.enabled = true;

        weaponDamageCollider.weaponFamily = weaponFamily;

        weaponDamageCollider.elementalStats = stats.elemental;
        weaponDamageCollider.poiseDamage = stats.basePoiseDamage;

        //Turn the collider back off so it doesn't hurt anyone, ow
        weaponDamageCollider.enabled = false;

        //Add Motion Value
        //Light
        weaponDamageCollider.lightAttack01DamageMotionValue = stats.lightAttack01DamageMotionValue;
        weaponDamageCollider.lightAttack02DamageMotionValue = stats.lightAttack02DamageMotionValue;
        weaponDamageCollider.lightAttack03DamageMotionValue = stats.lightAttack03DamageMotionValue;

        //Heavy
        weaponDamageCollider.heavyAttack01DamageMotionValue = stats.heavyAttack01DamageMotionValue;
        weaponDamageCollider.heavyAttack02DamageMotionValue = stats.heavyAttack02DamageMotionValue;

        //Charged Heavy
        weaponDamageCollider.heavyChargedAttack01DamageMotionValue = stats.heavyChargedAttack01DamageMotionValue;
        weaponDamageCollider.heavyChargedAttack02DamageMotionValue = stats.heavyChargedAttack02DamageMotionValue;

        //Running
        weaponDamageCollider.lightRunningAttack01DamageMotionValue = stats.lightRunningAttack01DamageMotionValue;

        //Rolling
        weaponDamageCollider.lightRollingAttack01DamageMotionValue = stats.lightRollingAttack01DamageMotionValue;

        //Backstepping
        weaponDamageCollider.lightBackstepAttack01DamageMotionValue = stats.lightBackstepAttack01DamageMotionValue;

    }
    /**
     * Add Exp to a weapon and level it up if possible
     */
    public void AddExp(float exp)
    {
        //Debug.Log("Adding " + exp + " exp to " + stats.weaponName);//astest
        stats.currentExperiencePoints += exp;
        while (stats.currentExperiencePoints >= stats.experiencePointsToNextLevel)
        {
            stats.level++;
            stats.currentTinkerPoints += stats.tinkerPointsPerLvl;
            //Currently add 100 to exp needed for each level
            stats.experiencePointsToNextLevel = 100 + 100 * stats.level;
            //TODO PLAY LEVEL UP NOISE/ANIMATION
        }
    }

    //public virtual void attackTarget(GameObject target)
    //{
    //    Debug.Log("BaseWeaponScript stats.attackTarget called.");//ASTEST
    //    if (target != null) {
    //        //calculateElementalDamage(stats.attack, target);
    //        //target.GetComponent<EnemyController>().hp -= stats.attack;
    //        //TODO
    //        //play weapon animation
    //        //set reload/recharge
    //    }
    //}
    public float CalculateTotalDamage(CharacterManager targetCharacter, float attackMotionValue = 1f, float fullChargeModifier = 1f)
    {
        float result = stats.attack * (1 - targetCharacter.characterStatsManager.physicalDefense);

        //I feel like there should be a way to do this iteratively, but with the ElementalStats class as it is, I don't know of any way to do so atm.
        result += stats.attack * (stats.elemental.firePower * 0.005f) * (1 - targetCharacter.characterStatsManager.elementalDefenses.firePower);
        result += stats.attack * (stats.elemental.icePower * 0.005f) * (1 - targetCharacter.characterStatsManager.elementalDefenses.icePower);
        result += stats.attack * (stats.elemental.lightningPower * 0.005f) * (1 - targetCharacter.characterStatsManager.elementalDefenses.lightningPower);
        result += stats.attack * (stats.elemental.windPower * 0.005f) * (1 - targetCharacter.characterStatsManager.elementalDefenses.windPower);
        result += stats.attack * (stats.elemental.earthPower * 0.005f) * (1 - targetCharacter.characterStatsManager.elementalDefenses.earthPower);
        result += stats.attack * (stats.elemental.lightPower * 0.005f) * (1 - targetCharacter.characterStatsManager.elementalDefenses.lightPower);
        result += stats.attack * (stats.elemental.beastPower * 0.005f) * (1 - targetCharacter.characterStatsManager.elementalDefenses.beastPower);
        result += stats.attack * (stats.elemental.scalesPower * 0.005f) * (1 - targetCharacter.characterStatsManager.elementalDefenses.scalesPower);
        result += stats.attack * (stats.elemental.techPower * 0.005f) * (1 - targetCharacter.characterStatsManager.elementalDefenses.techPower);

        //Calculate block modifier
        float blockingState = targetCharacter.isPerfectBlocking ? targetCharacter.perfectBlockModifier : 1f;

        if (result > 0)
        {
            if (targetCharacter.isBlocking)
            {
                if (targetCharacter.characterWeaponManager != null && targetCharacter.characterWeaponManager.ownedWeapons.Count > 0)
                {
                    return result * attackMotionValue * fullChargeModifier * (1 - (blockingState * targetCharacter.characterWeaponManager.ownedWeapons[targetCharacter.characterWeaponManager.indexOfEquippedWeapon].GetComponent<WeaponScript>().stats.block) / 100f);
                }
                else
                {
                    return result * attackMotionValue * fullChargeModifier * (1 - (blockingState * targetCharacter.nonWeaponBlockingStrength) / 100f);
                }
            }
            else
            {
                return result * attackMotionValue * fullChargeModifier;
            }

        }
        else return 0;
    }

    public virtual void AttemptToCastSpell(CharacterManager character)
    {
        //
    }

    public virtual void SuccessfullyCastSpell(CharacterManager character)
    {
        //
    }

    protected virtual void InstantiateWarmUpSpellFX(CharacterManager character)
    {
        //
    }

    protected virtual void InstantiateWarmUpReleaseFX(CharacterManager character)
    {
        //
    }


}
/** Change Log  
 *  Date         Developer  Description
 *  09/16/2024   Alec       New.
 *  06/23/2025   Jerry      Added Block/Perfect Block Mechanics.
 *  
 * */