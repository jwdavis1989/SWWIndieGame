using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.Collections.AllocatorManager;
using static UnityEditor.Rendering.FilterWindow;
using static UnityEngine.InputManagerEntry;
/** 
 * Enum of all weapon types.
 * 
 * Note: Since enums are sometimes stored as integers adding a new type anywhere but right above UNKOWN 
 *       may cause previously set values to be offset. Could convert to HashTable keyed with constant 
 *       Strings to prevent this. Could also add several PLACEHOLDER1,etc enums and rename them later.
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

public enum WeaponFamily
{
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
[Serializable] public class WeaponStats
{
    [Header("Weapon Type")]
    public WeaponType weaponType = 0;
    public bool isMonsterWeapon = false;

    [Header("Weapon Attributes")]
    public float attack = 1.0f;
    public float maxAttack = 1.0f;
    public float basePoiseDamage = 35f; //Base 35 in case it's caused by traps
    private float traitShrapnelPoiseDamageModifier = 1.25f;
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

    //Jump Attacks
    public float lightJumpAttack01StaminaCostModifier = 1f;
    public float heavyJumpAttack01StaminaCostModifier = 4f;

    //Running
    public float lightRunningAttack01StaminaCostModifier = 1f;

    //Rolling
    public float lightRollingAttack01StaminaCostModifier = 1f;

    //Backstepping
    public float lightBackstepAttack01StaminaCostModifier = 1f;

    //Spells
    public float areaSpellAttack01StaminaCostModifier = 1f;

    //Guns
    public float gunAttack01StaminaCostModifier = 1f;

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

    //Jump Attacks
    public float lightJumpAttack01DamageMotionValue = 1f;
    public float heavyJumpAttack01DamageMotionValue = 1.8f;

    //Running
    public float lightRunningAttack01DamageMotionValue = 1f;

    //Rolling
    public float lightRollingAttack01DamageMotionValue = 1f;

    //Backstepping
    public float lightBackstepAttack01DamageMotionValue = 1f;

    //Spells
    public float areaSpellAttack01DamageMotionValue = 1f;

    //Guns
    public float singleTargetBulletAttack01DamageMotionValue = 1f;


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
    public ElementalDamageType currentHighestElementalStat;
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
    private CharacterManager characterThatOwnsThisWeapon;
    [Header("Weapon Family - Important - Set in Prefab")]
    public WeaponFamily weaponFamily = 0;

    [Header("Weapon Damage Colliders")]
    [SerializeField] public MeleeWeaponDamageCollider weaponDamageCollider;
    [SerializeField] public GameObject jumpAttackWeaponDamageCollider;

    [Header("Currently set on prefab")]
    public bool isSpecialWeapon = false;
    public bool isWristWeapon = false;
    public float specialWeaponDamageMultiplier = 1.25f;

    [Header("Image used for menu icon")]
    public Sprite spr = null;
    [Header("Will appear as ??? on weapon sheet until obtained")]
    public bool hasObtained = false;
    [Header("These are all written to JSON when saving a game.")]
    public WeaponStats stats;

    [Header("Actions")]
    public WeaponItemAction mainHandLightAttackAction;  //One hand light attack
    public WeaponItemAction mainHandHeavyAttackAction;  //One hand heavy attack
    public WeaponItemAction offHandCastMagicAttackAction;   //Off hand Magic Special Attack
    public WeaponItemAction offHandShootGunAttackAction;   //Off hand Gun Special Attack



    [Header("Animations")]
    public AnimatorOverrideController weaponAnimatorOverride;
    [SerializeField] protected string offHandSpellAnimation;
    [SerializeField] protected string offHandGunShootAnimation;

    [Header("Weapon Family Specific")]
    [Header("Magic Special Weapons")]
    public float fullChargingTraitModifier = 1.25f;

    [Header("Gun Special Weapons")]
    [Header("Gun Transforms")]
    public GameObject gunModel;
    public BulletOriginLocation bulletOriginLocation;
    private Vector3 baseModelLocation;
    private Quaternion baseModelRotation;
    public Transform shootingTransform;
    public bool isSettingGunToFiringTransform = false;
    public bool isSetGunToHandTransform = false;

    [Header("Gun Projectile")]
    [SerializeField] public GameObject gunProjectile = null;
    [SerializeField] public GameObject gunShotWarmUpVFX;
    public float projectileForwardVelocityMultiplier = 7f;
    public float projectileUpwardVelocityMultiplier = 4f;
    public float projectileMass = 0.01f;


    [Header("Spell Projectile")]
    public float spellForwardVelocityMultiplier = 7f;
    public float spellUpwardVelocityMultiplier = 4f;
    [SerializeField] public GameObject spellCastWarmUpVFX;
    [SerializeField] public GameObject spellChargeVFX;
    [SerializeField] public GameObject spellProjectileVFX;
    [SerializeField] public GameObject spellProjectileFullChargeVFX;

    [Header("Ranged SFX")]
    public AudioClip spellReleaseSFX;
    public AudioClip spellProjectileSFX;
    public AudioClip gunAimSFX;
    public AudioClip gunFireSFX;

    [Header("Debug Mode")]
    public bool isInDebugMode = false;




    public void Awake()
    {
        if (isSpecialWeapon)
        {
            // if (projectile != null)
            // {
            //     weaponDamageCollider = projectile.GetComponent<MeleeWeaponDamageCollider>();
            // }
            // else
            // {
            //     weaponDamageCollider = GetComponentInChildren<MeleeWeaponDamageCollider>();
            // }
            InitializeGunTransform();
        }
        else
        {
            weaponDamageCollider = GetComponentInChildren<MeleeWeaponDamageCollider>();
        }

        if (weaponDamageCollider)
        {
            SetWeaponDamage(weaponDamageCollider);
            UpdateHighestElementalStat();
        }

        stats.elemental.currentHighestElementalStat = GetHighestElementalStat();

        //Initialize Weapon Owner
        characterThatOwnsThisWeapon = GetComponentInParent<CharacterManager>();

        //Activate Debug Mode if Weapon Manager is in Debug Mode
        if (characterThatOwnsThisWeapon != null)
            isInDebugMode = characterThatOwnsThisWeapon.isInDebugMode;
    }

    private void Update()
    {
        if (isSpecialWeapon)
        {
            HandleSettingGunToFiringTransform();
            HandleSetGunToHandTransform();
        }

    }
    //TODO: Call this when you upgrade weapons too!
    public void SetWeaponDamage(MeleeWeaponDamageCollider weaponDamageCollider)
    {
        if (weaponDamageCollider == null)
        {
            return;
        }

        if (GetComponentInParent<CharacterWeaponManager>() != null)
            weaponDamageCollider.characterCausingDamage = GetComponentInParent<CharacterWeaponManager>().characterThatOwnsThisArsenal;
        weaponDamageCollider.isMainHand = !isSpecialWeapon;
        weaponDamageCollider.enabled = true;

        weaponDamageCollider.weaponFamily = weaponFamily;

        weaponDamageCollider.physicalDamage = stats.attack;
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
        //if (isInDebugMode) Debug.Log("Adding " + exp + " exp to " + stats.weaponName);//astest
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
    //    if (isInDebugMode) Debug.Log("BaseWeaponScript stats.attackTarget called.");//ASTEST
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
        if (stats.durability > 0)
        {
            if (!InventionManager.instance.CheckHasUpgrade(InventionType.DaedalusNanoMaterials)) //no upgrade
                stats.durability--; // Reduce durability
            else if (UnityEngine.Random.Range(0, 10) != 1) // 90% chance to reduce durability
                stats.durability--; // Reduce durability
        }
        else
            return 0; // The weapon is broken. Return without doing damage

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
            //Special Weapons Deal 25% bonus Damage
            if (isSpecialWeapon)
            {
                result *= specialWeaponDamageMultiplier;
            }

            if (targetCharacter.isBlocking)
            {
                if (targetCharacter.characterWeaponManager != null && targetCharacter.characterWeaponManager.ownedWeapons.Count > 0)
                {
                    return result * attackMotionValue * fullChargeModifier * (1 - (blockingState * targetCharacter.characterWeaponManager.GetMainHand().stats.block) / 100f);
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
        if (!CanIUseThisSpecialAttack(character))
        {
            return;
        }

        character.characterAnimatorManager.PlayTargetActionAnimation(offHandSpellAnimation, true);
    }

    public virtual void SuccessfullyCastSpell(CharacterManager character)
    {
        //1. Destroy any Warm Up FX remaining from the spell
        character.characterCombatManager.DestroyAllCurrentActionFX();

        //2. Instantiate the Projectile
        SpellOriginLocation spellOriginLocation = character.characterWeaponManager.GetEquippedWeapon(true).GetComponentInChildren<SpellOriginLocation>();
        GameObject instantiatedSpellProjectileFX = Instantiate(spellProjectileVFX);

        //Update the VFX to match the highest element of the magic weapon
        instantiatedSpellProjectileFX.GetComponent<SpellElementalVFXManager>().ChangeVFXBasedOnElement(stats.elemental.currentHighestElementalStat);

        FireBallManager fireBallManager = instantiatedSpellProjectileFX.GetComponent<FireBallManager>();
        fireBallManager.InitializeFireBall(character, stats.elemental.currentHighestElementalStat);

        //3. Zero out its location and unparent it
        instantiatedSpellProjectileFX.transform.parent = spellOriginLocation.transform;
        instantiatedSpellProjectileFX.transform.localPosition = Vector3.zero;
        instantiatedSpellProjectileFX.transform.localRotation = Quaternion.identity;
        instantiatedSpellProjectileFX.transform.parent = null;
        instantiatedSpellProjectileFX.transform.localScale = Vector3.one;


        // instantiatedSpellProjectileFX.transform.position = spellOriginLocation.transform.position;
        // instantiatedSpellProjectileFX.transform.rotation = spellOriginLocation.transform.rotation;

        //Alternative way to avoid colliding with self, but isn't needed as we already checked this in the SpellProjectileDamageCollider 
        // Collider[] characterColliders = character.GetComponentsInChildren<Collider>();
        // Collider characterCollisionCollider = character.GetComponent<Collider>();
        //
        // foreach (var collider in characterColliders)
        // {
        //     Physics.IgnoreCollision(collider, fireBallManager.damageCollider.GetComponent<Collider>(), true);
        // }

        //4. Set the projectile's direction
        if (character.isLockedOn)
        {
            instantiatedSpellProjectileFX.transform.LookAt(character.characterCombatManager.currentTarget.transform.position);
        }
        else
        {
            //instantiatedSpellProjectileFX.transform.forward = character.transform.forward;
            Vector3 newForward = character.transform.forward + new UnityEngine.Vector3(0, 0, 0);
            instantiatedSpellProjectileFX.transform.forward = newForward;
        }

        //6. Set the projectile's velocity
        Rigidbody spellRigidbody = instantiatedSpellProjectileFX.GetComponent<Rigidbody>();
        Vector3 upwardVelocity = instantiatedSpellProjectileFX.transform.up * spellUpwardVelocityMultiplier;
        Vector3 forwardVelocity = instantiatedSpellProjectileFX.transform.forward * spellForwardVelocityMultiplier;
        Vector3 totalVelocity = upwardVelocity + forwardVelocity;
        spellRigidbody.velocity = totalVelocity;
    }

    public virtual void SuccessfullyCastSpellFullCharge(CharacterManager character)
    {
        //1. Destroy any Warm Up FX remaining from the spell
        character.characterCombatManager.DestroyAllCurrentActionFX();

        //2. Instantiate the Projectile
        SpellOriginLocation spellOriginLocation = character.characterWeaponManager.GetEquippedWeapon(true).GetComponentInChildren<SpellOriginLocation>();
        GameObject instantiatedSpellProjectileFX = Instantiate(spellProjectileFullChargeVFX);

        //Update the VFX to match the highest element of the magic weapon
        instantiatedSpellProjectileFX.GetComponent<SpellElementalVFXManager>().ChangeVFXBasedOnElement(stats.elemental.currentHighestElementalStat);

        //3. Apply Damage to the projectiles damage collider
        FireBallManager fireBallManager = instantiatedSpellProjectileFX.GetComponent<FireBallManager>();
        fireBallManager.isFullyCharged = true;
        fireBallManager.InitializeFireBall(character, stats.elemental.currentHighestElementalStat);

        //4. Zero out its location and unparent it
        instantiatedSpellProjectileFX.transform.parent = spellOriginLocation.transform;
        instantiatedSpellProjectileFX.transform.localPosition = Vector3.zero;
        instantiatedSpellProjectileFX.transform.localRotation = Quaternion.identity;
        instantiatedSpellProjectileFX.transform.parent = null;

        instantiatedSpellProjectileFX.transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);

        //5. Set the projectile's direction
        if (character.isLockedOn)
        {
            instantiatedSpellProjectileFX.transform.LookAt(character.characterCombatManager.currentTarget.transform.position);
        }
        else
        {
            instantiatedSpellProjectileFX.transform.forward = character.transform.forward;
        }

        //6. Set the projectile's velocity
        Rigidbody spellRigidbody = instantiatedSpellProjectileFX.GetComponent<Rigidbody>();
        Vector3 upwardVelocity = instantiatedSpellProjectileFX.transform.up * spellUpwardVelocityMultiplier;
        Vector3 forwardVelocity = instantiatedSpellProjectileFX.transform.forward * spellForwardVelocityMultiplier;
        Vector3 totalVelocity = upwardVelocity + forwardVelocity;
        spellRigidbody.velocity = totalVelocity;

        fireBallManager.isFullyCharged = true;
    }

    public void SuccessfullyChargeSpell(CharacterManager character)
    {
        //Destroy any Warm Up FX remaining from the spell
        character.characterCombatManager.DestroyAllCurrentActionFX();

        //Instantiate the VFZ
        SpellOriginLocation spellOriginLocation = character.characterWeaponManager.GetEquippedWeapon(true).GetComponentInChildren<SpellOriginLocation>();
        GameObject instantiatedSpellChargeVFX = Instantiate(spellChargeVFX);

        //Save the VFX to delete later
        character.characterEffectsManager.activeSpellWarmUpFX = instantiatedSpellChargeVFX;

        //Zero out its location and unparent it
        instantiatedSpellChargeVFX.transform.parent = spellOriginLocation.transform;
        instantiatedSpellChargeVFX.transform.localPosition = Vector3.zero;
        instantiatedSpellChargeVFX.transform.localRotation = Quaternion.identity;

        //Update the VFX to match the highest element of the magic weapon
        instantiatedSpellChargeVFX.GetComponent<SpellElementalVFXManager>().ChangeVFXBasedOnElement(stats.elemental.currentHighestElementalStat);

        instantiatedSpellChargeVFX.transform.localScale = new Vector3(0.7f, 0.7f / 0.9f, 0.7f);

    }

    public virtual void InstantiateWarmUpSpellFX(CharacterManager character)
    {
        //1. Instantiate Warm Up at the correct place
        SpellOriginLocation spellOriginLocation = character.characterWeaponManager.GetOffHand().gameObject.GetComponentInChildren<SpellOriginLocation>();

        //2. "Save" the warm up FX as a variable so it can be destroyed if the player is knocked out of the animation
        GameObject instantiatedSpellWarmUpFX = Instantiate(spellCastWarmUpVFX);
        instantiatedSpellWarmUpFX.transform.parent = spellOriginLocation.transform;
        instantiatedSpellWarmUpFX.transform.localPosition = Vector3.zero;
        instantiatedSpellWarmUpFX.transform.localRotation = Quaternion.identity;

        //Update the VFX to match the highest element of the magic weapon
        instantiatedSpellWarmUpFX.GetComponent<SpellElementalVFXManager>().ChangeVFXBasedOnElement(stats.elemental.currentHighestElementalStat);
        character.characterEffectsManager.activeSpellWarmUpFX = instantiatedSpellWarmUpFX;

        //3. Drain Stamina
        character.characterWeaponManager.currentAttackType = AttackType.AreaSpellAttack01;
        character.CallDrainStaminaBasedOnAttack();
    }

    public virtual void InstantiateReleaseFX(CharacterManager character)
    {
        if (isInDebugMode) Debug.Log("Instantiate Release FX");
    }

    public void InitializeGunTransform()
    {
        if (weaponFamily == WeaponFamily.SemiAutoGuns || weaponFamily == WeaponFamily.BurstFireGuns
                || weaponFamily == WeaponFamily.LaserGuns || weaponFamily == WeaponFamily.Shotguns
                || weaponFamily == WeaponFamily.GrenadeLaunchers)
        {
            // Debug.Log("baseTransform?" + baseTransform != null);
            // Debug.Log("gunModel?" + gunModel != null);
            // Debug.Log("gunModel.transform?" + gunModel.transform != null);
            // Debug.Log("gunModel.transform.localPosition?" + gunModel.transform.localPosition != null);
            // Debug.Log("gunModel.transform.localRotation?" + gunModel.transform.localRotation != null);
            // baseTransform.position = gunModel.transform.localPosition;
            // baseTransform.rotation = gunModel.transform.localRotation;
            // Debug.Log("baseTransform.localPosition?" + baseTransform.localPosition != null);
            // Debug.Log("baseTransform.localRotation?" + baseTransform.localRotation != null);
            baseModelLocation = gunModel.transform.localPosition;
            baseModelRotation = gunModel.transform.localRotation;
            bulletOriginLocation = gunModel.GetComponentInChildren<BulletOriginLocation>();
        }
    }

    public void SetGunToFiringTransform()
    {
        // Vector3 velocity = Vector3.zero;
        // gunModel.transform.localPosition = Vector3.SmoothDamp(gunModel.transform.localPosition, shootingTransform.localPosition, ref velocity, 0.05f);
        // gunModel.transform.localRotation = Quaternion.Slerp(shootingTransform.localRotation, Quaternion.Euler(0, 0, 0), 0.2f);

        // gunModel.transform.localPosition = shootingTransform.localPosition;
        // gunModel.transform.localRotation = shootingTransform.localRotation;

        isSettingGunToFiringTransform = true;
        isSetGunToHandTransform = false;
    }


    public void SetGunToHandTransform()
    {
        // gunModel.transform.localPosition = baseModelLocation;
        // gunModel.transform.localRotation = baseModelRotation;

        // Vector3 velocity = Vector3.zero;
        // gunModel.transform.localPosition = Vector3.SmoothDamp(gunModel.transform.localPosition, baseModelLocation, ref velocity, 0.05f);
        // gunModel.transform.localRotation = Quaternion.Slerp(gunModel.transform.localRotation, baseModelRotation, 0.8f);

        isSettingGunToFiringTransform = false;
        isSetGunToHandTransform = true;
    }

    public void ResetGunTransformBools()
    {
        isSettingGunToFiringTransform = false;
        isSetGunToHandTransform = false;
    }

    private void HandleSetGunToHandTransform()
    {
        if (isSetGunToHandTransform)
        {
            Vector3 velocity = Vector3.zero;
            gunModel.transform.localPosition = Vector3.SmoothDamp(gunModel.transform.localPosition, baseModelLocation, ref velocity, 0.05f);
            gunModel.transform.localRotation = Quaternion.Slerp(gunModel.transform.localRotation, baseModelRotation, 1f);
        }
    }

    private void HandleSettingGunToFiringTransform()
    {
        if (isSettingGunToFiringTransform)
        {
            Vector3 velocity = Vector3.zero;
            gunModel.transform.localPosition = Vector3.SmoothDamp(gunModel.transform.localPosition, shootingTransform.localPosition, ref velocity, 0.05f);
            gunModel.transform.localRotation = Quaternion.Slerp(shootingTransform.localRotation, Quaternion.Euler(0, 0, 0), 0f);
        }
    }

    public virtual void AttemptToShootGun(CharacterManager character)
    {
        if (!CanIUseThisSpecialAttack(character))
        {
            return;
        }

        character.characterAnimatorManager.PlayTargetActionAnimation(offHandGunShootAnimation, true);

        //Change character model Rotation to counter animation's root motion rotation in the nation
        character.SetShootingModelAlignment();
    }

    public virtual void InstantiateWarmUpGunFX(CharacterManager character)
    {
        //1. Instantiate Warm Up at the correct place
        // BulletOriginLocation bulletOriginLocation = character.characterWeaponManager.GetOffHand().gameObject.GetComponentInChildren<BulletOriginLocation>();

        //2. "Save" the warm up FX as a variable so it can be destroyed if the player is knocked out of the animation
        // GameObject instantiatedGunWarmUpFX = Instantiate(gunShotWarmUpVFX);
        // instantiatedGunWarmUpFX.transform.parent = bulletOriginLocation.transform;
        // instantiatedGunWarmUpFX.transform.localPosition = Vector3.zero;
        // instantiatedGunWarmUpFX.transform.localRotation = Quaternion.identity;

        //Alt 1 & 2
        gunShotWarmUpVFX.SetActive(true);
        gunShotWarmUpVFX.GetComponent<SpellElementalVFXManager>().ChangeVFXBasedOnElement(stats.elemental.currentHighestElementalStat);

        //Update the VFX to match the highest element of the magic weapon
        // instantiatedGunWarmUpFX.GetComponent<SpellElementalVFXManager>().ChangeVFXBasedOnElement(stats.elemental.currentHighestElementalStat);
        // character.characterEffectsManager.activeSpellWarmUpFX = instantiatedGunWarmUpFX;

        //3. Drain Stamina
        character.characterWeaponManager.currentAttackType = AttackType.SingleTargetBulletAttack01;
        character.CallDrainStaminaBasedOnAttack();
    }

    public virtual void SuccessfullyShootGun(CharacterManager character)
    {
        //1. Destroy any Warm Up FX remaining from the gun shot
        character.characterCombatManager.DestroyAllCurrentActionFX();

        //Alt 1
        gunShotWarmUpVFX.GetComponent<SpellElementalVFXManager>().ResetActiveElementalVFX();
        gunShotWarmUpVFX.SetActive(false);

        //2. Instantiate the Projectile
        BulletOriginLocation bulletOriginLocation = character.characterWeaponManager.GetEquippedWeapon(true).GetComponentInChildren<BulletOriginLocation>();
        GameObject instantiatedGunProjectile = Instantiate(gunProjectile);

        //Update the VFX to match the highest element of the magic weapon
        instantiatedGunProjectile.GetComponent<SpellElementalVFXManager>().ChangeVFXBasedOnElement(stats.elemental.currentHighestElementalStat);

        FireBallManager fireBallManager = instantiatedGunProjectile.GetComponent<FireBallManager>();
        fireBallManager.InitializeFireBall(character, stats.elemental.currentHighestElementalStat);

        //3. Zero out its location and unparent it
        instantiatedGunProjectile.transform.parent = bulletOriginLocation.transform;
        instantiatedGunProjectile.transform.localPosition = Vector3.zero;
        instantiatedGunProjectile.transform.localRotation = Quaternion.identity;
        instantiatedGunProjectile.transform.parent = null;
        instantiatedGunProjectile.transform.localScale = Vector3.one;

        //4. Set the projectile's direction
        if (character.isLockedOn)
        {
            instantiatedGunProjectile.transform.LookAt(character.characterCombatManager.currentTarget.transform.position);
        }
        else
        {
            //instantiatedSpellProjectileFX.transform.forward = character.transform.forward;
            Vector3 newForward = character.transform.forward + new UnityEngine.Vector3(0, 0, 0);
            instantiatedGunProjectile.transform.forward = newForward;
        }

        //6. Set the projectile's velocity
        Rigidbody bulletRigidbody = instantiatedGunProjectile.GetComponent<Rigidbody>();
        Vector3 upwardVelocity = instantiatedGunProjectile.transform.up * projectileUpwardVelocityMultiplier;
        Vector3 forwardVelocity = instantiatedGunProjectile.transform.forward * projectileForwardVelocityMultiplier;
        Vector3 totalVelocity = upwardVelocity + forwardVelocity;
        bulletRigidbody.velocity = totalVelocity;
    }

    protected virtual bool CanIUseThisSpecialAttack(CharacterManager character)
    {
        if (character.isPerformingAction || character.isJumping || character.characterStatsManager.currentStamina <= 0)
        {
            return false;
        }

        return true;
    }

    public void InstantiateJumpAttackCollider()
    {
        GameObject newJumpAttackColliderObject = Instantiate(jumpAttackWeaponDamageCollider, transform.position, Quaternion.identity);
        MeleeJumpAttackDamageCollider newJumpAttackDamageCollider = newJumpAttackColliderObject.GetComponent<MeleeJumpAttackDamageCollider>();
        SetWeaponDamage(newJumpAttackDamageCollider);

        //DEBUG: Allows you to change the elements in editor during live session to update which VFX plays
        if (isInDebugMode)
        {
            UpdateHighestElementalStat();
        }

        newJumpAttackDamageCollider.enabled = true;
        newJumpAttackDamageCollider.EnableDamageCollider();

        //Switch for Greatest Element:
        switch (stats.elemental.currentHighestElementalStat)
        {
            case ElementalDamageType.Fire:
                newJumpAttackDamageCollider.fireJumpAttackVFX.SetActive(true);
                newJumpAttackDamageCollider.MeteorImpactDecal.currentColorIndex = 0;
                if (isInDebugMode) Debug.Log("Highest Element: Fire");
                break;
            case ElementalDamageType.Ice:
                newJumpAttackDamageCollider.iceJumpAttackVFX.SetActive(true);
                newJumpAttackDamageCollider.MeteorImpactDecal.currentColorIndex = 1;
                if (isInDebugMode) Debug.Log("Highest Element: Ice");
                break;
            case ElementalDamageType.Lightning:
                newJumpAttackDamageCollider.lightningJumpAttackVFX.SetActive(true);
                newJumpAttackDamageCollider.MeteorImpactDecal.currentColorIndex = 2;
                if (isInDebugMode) Debug.Log("Highest Element: Lightning");
                break;
            case ElementalDamageType.Wind:
                newJumpAttackDamageCollider.windJumpAttackVFX.SetActive(true);
                newJumpAttackDamageCollider.MeteorImpactDecal.currentColorIndex = 3;
                if (isInDebugMode) Debug.Log("Highest Element: Wind");
                break;
            case ElementalDamageType.Earth:
                newJumpAttackDamageCollider.earthJumpAttackVFX.SetActive(true);
                newJumpAttackDamageCollider.MeteorImpactDecal.currentColorIndex = 4;
                if (isInDebugMode) Debug.Log("Highest Element: Earth");
                break;
            case ElementalDamageType.Light:
                newJumpAttackDamageCollider.lightJumpAttackVFX.SetActive(true);
                newJumpAttackDamageCollider.MeteorImpactDecal.currentColorIndex = 5;
                if (isInDebugMode) Debug.Log("Highest Element: Light");
                break;
            case ElementalDamageType.Beast:
                newJumpAttackDamageCollider.beastJumpAttackVFX.SetActive(true);
                newJumpAttackDamageCollider.MeteorImpactDecal.currentColorIndex = 6;
                if (isInDebugMode) Debug.Log("Highest Element: Beast");
                break;
            case ElementalDamageType.Scales:
                newJumpAttackDamageCollider.scalesJumpAttackVFX.SetActive(true);
                newJumpAttackDamageCollider.MeteorImpactDecal.currentColorIndex = 7;
                if (isInDebugMode) Debug.Log("Highest Element: Scales");
                break;
            case ElementalDamageType.Tech:
                newJumpAttackDamageCollider.techJumpAttackVFX.SetActive(true);
                newJumpAttackDamageCollider.MeteorImpactDecal.currentColorIndex = 8;
                if (isInDebugMode) Debug.Log("Highest Element: Tech");
                break;
            default:
                newJumpAttackDamageCollider.fireJumpAttackVFX.SetActive(true);
                newJumpAttackDamageCollider.MeteorImpactDecal.currentColorIndex = 0;
                if (isInDebugMode) Debug.Log("Highest Element: Default Case");
                break;
        }

        //Initialize MeteorSmashDecal's color to match highest element of weapon
        newJumpAttackDamageCollider.MeteorImpactDecal.InitializeColorFading();
    }
    

    // Minimum Threshold determines a cut off before we start allowing an element to be high enough to count for elemental graphics and effects
    public ElementalDamageType GetHighestElementalStat(float minimumThreshold = 0)
    {
        ElementalDamageType highestElement;
        List<float> elementalComparisonList = new List<float>();
        int highestElementStatIndex = -1;
        float currentHighestElementStat = minimumThreshold;

        //Set the values of the Comparison List to the weapon's elements
        elementalComparisonList.Add(stats.elemental.firePower);
        elementalComparisonList.Add(stats.elemental.icePower);
        elementalComparisonList.Add(stats.elemental.lightningPower);
        elementalComparisonList.Add(stats.elemental.windPower);
        elementalComparisonList.Add(stats.elemental.earthPower);
        elementalComparisonList.Add(stats.elemental.lightPower);
        elementalComparisonList.Add(stats.elemental.beastPower);
        elementalComparisonList.Add(stats.elemental.scalesPower);
        elementalComparisonList.Add(stats.elemental.techPower);

        //Determine highest elemental stat
        for (int i = 0; i < elementalComparisonList.Count; i++)
        {
            if (elementalComparisonList[i] > currentHighestElementStat)
            {
                currentHighestElementStat = elementalComparisonList[i];
                highestElementStatIndex = i;
            }
        }

        switch (highestElementStatIndex)
        {
            case 0:
                highestElement = ElementalDamageType.Fire;
                //if (isInDebugMode) Debug.Log("Highest Element: Fire");
                break;
            case 1:
                highestElement = ElementalDamageType.Ice;
                //if (isInDebugMode) Debug.Log("Highest Element: Ice");
                break;
            case 2:
                highestElement = ElementalDamageType.Lightning;
                //if (isInDebugMode) Debug.Log("Highest Element: Lightning");
                break;
            case 3:
                highestElement = ElementalDamageType.Wind;
                //if (isInDebugMode) Debug.Log("Highest Element: Wind");
                break;
            case 4:
                highestElement = ElementalDamageType.Earth;
                //if (isInDebugMode) Debug.Log("Highest Element: Earth");
                break;
            case 5:
                highestElement = ElementalDamageType.Light;
                //if (isInDebugMode) Debug.Log("Highest Element: Light");
                break;
            case 6:
                highestElement = ElementalDamageType.Beast;
                //if (isInDebugMode) Debug.Log("Highest Element: Beast");
                break;
            case 7:
                highestElement = ElementalDamageType.Scales;
                //if (isInDebugMode) Debug.Log("Highest Element: Scales");
                break;
            case 8:
                highestElement = ElementalDamageType.Tech;
                //if (isInDebugMode) Debug.Log("Highest Element: Tech");
                break;
            default:
                highestElement = ElementalDamageType.Unaspected;
                //if (isInDebugMode) Debug.Log("Highest Element: Unaspected");
                break;
        }

        return highestElement;
    }

    public void UpdateHighestElementalStat()
    {
        //Update Highest Elemental Value
        stats.elemental.currentHighestElementalStat = GetHighestElementalStat();
    }
    /** Returns type as string with spaces added between captial letters. I.E. Great Sword
     */
    public string GetWeaponFamilyFormatted()
    {
        if (weaponFamily == WeaponFamily.HammersOrWrenches)
            return WeaponType.Wrench == stats.weaponType || stats.weaponType == WeaponType.ReinforcedWrench ? "Wrench": "Hammer";
        string name = "" + weaponFamily;
        string formatted = "";
        foreach (char letter in name)
        {
            if (char.IsUpper(letter))
            {
                formatted += " " + letter;
            }
            else
            {
                formatted += letter;
            }
        }
        formatted = formatted.Trim();//remove extra space
        if (formatted.Length > 0 && formatted[formatted.Length - 1] == 's')
        {//remove plurality
            formatted = formatted.Substring(0,formatted.Length - 1);
        }
        return formatted;
    }
    public Dictionary<string, float> GetPrimaryStats()
    {
        Dictionary<string, float> rv = new Dictionary<string, float>();
        rv.Add("Attack", stats.attack);
        rv.Add("Block", stats.block);
        rv.Add("Durability", stats.durability);
        rv.Add("Stability", stats.stability);
        return rv;
    }
    public Dictionary<string, float> GetElementalStats()
    {
        Dictionary<string, float> rv = new Dictionary<string, float>();
        rv.Add("Fire", stats.elemental.firePower);
        rv.Add("Earth", stats.elemental.earthPower);
        rv.Add("Ice", stats.elemental.icePower);
        rv.Add("Light", stats.elemental.lightPower);
        rv.Add("Lightning", stats.elemental.lightningPower);
        rv.Add("Beast", stats.elemental.beastPower);
        rv.Add("Wind", stats.elemental.windPower);
        rv.Add("Scales", stats.elemental.scalesPower);
        rv.Add("Tech", stats.elemental.techPower);
        return rv;
    }
    public static string GetStatTooltip(string stat)
    {
        switch (stat)
        {
            case "Attack":
                return "Attack improves all damage";
            case "Block":
                return "Block lowers damage";
            case "Durability":
                return "Weapon is broken when durability is zero";
            case "Stability":
                return "Stability reduces stagger";
            case "Fire":
                return "Fire is strong against grass types";
            case "Earth":
                return "Earth is strong against newbs";
            case "Ice":
                return "Ices is strong against fire types";
            case "Light":
                return "Light is good against undead";
            case "Lightning":
                return "Lightning is good against water types";
            case "Beast":
                return "Beast improves damage against natural animals";
            case "Wind":
                return "Wind improves damage against flying enemies";
            case "Scales":
                return "Scales improves damage against fish and reptiles";
            case "Tech":
                return "Tech improves damage to robots";
            default:
                return stat + " is good";
        }
    }
}
/** Change Log  
 *  Date         Developer  Description
 *  09/16/2024   Alec       New.
 *  06/23/2025   Jerry      Added Block/Perfect Block Mechanics.
 *  08/04/2025   Jerry      Added Spell Casting Weapon Mechanics.
 *  01/15/2026   Alec       Added helper script for tooltip, Stats can be iterated as a dictionary
 * */