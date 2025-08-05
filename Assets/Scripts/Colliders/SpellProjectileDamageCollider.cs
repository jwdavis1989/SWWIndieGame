using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellProjectileDamageCollider : DamageCollider
{
    private FireBallManager fireBallManager;

    [Header("Attacking Character")]
    public CharacterManager characterCausingDamage;

    [Header("Spell Attack Modifiers")]
    public float spellDamageMotionValue;
    

    private WeaponScript weaponThatOwnsThisCollider;
    public WeaponFamily weaponFamily;

    public float timeUntilSpellDestruction = 0.4f;

    protected override void Awake()
    {
        base.Awake();

        fireBallManager = GetComponentInParent<FireBallManager>();
        Debug.Log("fireBallManager: " + (fireBallManager != null));
        Debug.Log("characterCausingDamage: " + (characterCausingDamage != null));
        //Debug.Log("characterCausingDamage.characterWeaponManager: " + (characterCausingDamage.characterWeaponManager != null));
        weaponThatOwnsThisCollider = characterCausingDamage.characterWeaponManager.GetEquippedWeapon(true).GetComponent<WeaponScript>();
        Debug.Log("weaponThatOwnsThisCollider: " + (weaponThatOwnsThisCollider != null));
        InitializeStats();
    }

    protected void InitializeStats()
    {
        //Base Attack Power
        physicalDamage = weaponThatOwnsThisCollider.stats.attack;
        //damageEffect.weaponScript.stats = stats;
        fullChargeModifier = weaponThatOwnsThisCollider.fullChargingTraitModifier;
        //Elemental
        elementalStats = weaponThatOwnsThisCollider.stats.elemental;
    }

    protected override void OnTriggerEnter(Collider other)
    {
        //if (other.gameObject.layer == LayerMask.NameToLayer("Character")) {
        CharacterManager damageTarget = other.GetComponentInParent<CharacterManager>();
        //Uncomment below if we want to search on both the damageable character colliders and the character controller collider:
        // if (damageTarget == null) {
        //     damageTarget = other.GetComponent<CharacterManager>();
        // }

        if (damageTarget != null)
        {
            //"Friendly Fire", isn't.
            if (damageTarget == characterCausingDamage)
            {
                return;
            }

            contactPoint = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);

            //Check if we can damage this target based on friendly fire

            //Check if target is blocking
            CheckForBlock(damageTarget);

            //Check if target is invulnerable

            //Damage
            DamageTarget(damageTarget);

            fireBallManager.WaitThenInstantiateSpellDestructionFX(timeUntilSpellDestruction);
        }
        //}
    }

    protected override void DamageTarget(CharacterManager damageTarget)
    {
        //We don't want to damage the same target more than once in a single attack
        //So we add them to a list that checks before applying damage
        if (charactersDamaged.Contains(damageTarget))
        {
            return;
        }

        charactersDamaged.Add(damageTarget);

        //Create a copy of the damage effect to not change the original
        TakeHealthDamageCharacterEffect damageEffect = Instantiate(WorldCharacterEffectsManager.instance.takeHealthDamageEffect);
        damageEffect.isMainHand = isMainHand;

        //Set the Weapon Family of the attack for sound and visual effects
        damageEffect.weaponFamily = weaponFamily;

        //Set Character Causing the damage, if it exits
        if (characterCausingDamage != null)
        {
            damageEffect.characterCausingDamage = characterCausingDamage;
        }

        //Base Attack Power
        damageEffect.physicalDamage = physicalDamage;
        Debug.Log("Base Damage: " + physicalDamage);

        //Elemental
        damageEffect.elementalDamage = elementalStats;

        //Armore Penetration
        damageEffect.isReducedByArmor = isReducedByArmor;

        //Determine Motion Value
        if (characterCausingDamage.characterWeaponManager != null)
        {
            switch (characterCausingDamage.characterWeaponManager.currentAttackType)
            {

                //Spell Attacks
                case AttackType.AreaSpellAttack01:
                    attackMotionValue = characterCausingDamage.characterWeaponManager.GetEquippedWeapon(true).GetComponent<WeaponScript>().stats.areaSpellAttack01DamageMotionValue;
                    break;

                //Default
                default:
                    break;
            }

            //Calculate Poise Damage
            damageEffect.poiseDamage = attackMotionValue * characterCausingDamage.characterWeaponManager.GetEquippedWeapon(true).GetComponent<WeaponScript>().stats.basePoiseDamage;
        }
        else
        {
            //No WeaponManager found in character causing damage
            damageEffect.poiseDamage = poiseDamage;
        }

        //Apply Motion Value
        damageEffect.attackMotionValue = attackMotionValue;

        //TODO: Determine Full Charge Bonus Damage Modifier for Charged Spell Attacks
        //UPDATE: Using a different system for melee charge attacking, might still use this for spell charge attacks(?)

        //Apply Full Charge Bonus Damage Modifier for Charged Spell Attacks
        damageEffect.fullChargeModifier = fullChargeModifier;

        //Update Contact Point for VFX
        damageEffect.contactPoint = contactPoint;

        //Update the angle the target is hit from for determining animations
        damageEffect.angleHitFrom = Vector3.SignedAngle(characterCausingDamage.transform.forward, damageTarget.transform.forward, Vector3.up);

        //Apply the copy's damage effect to the target
        damageTarget.characterEffectsManager.ProcessInstantEffect(damageEffect);
    }

    protected override void GetBlockingDotValues(CharacterManager damageTarget)
    {
        //Warning: This might need changed to represent the direction from the fireball collider in some way. If bugs, check here.
        directionFromAttackToDamageTarget = transform.position - damageTarget.transform.position;
        dotValueFromAttackToDamageTarget = Vector3.Dot(directionFromAttackToDamageTarget, damageTarget.transform.forward);
    }

}
