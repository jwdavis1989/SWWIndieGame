using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeaponDamageCollider : DamageCollider
{
    [Header("Attacking Character")]
    public CharacterManager characterCausingDamage;

    [Header("Weapon Attack Modifiers")]
    //Light
    public float lightAttack01DamageMotionValue;
    public float lightAttack02DamageMotionValue;
    public float lightAttack03DamageMotionValue;

    //Heavy
    public float heavyAttack01DamageMotionValue;
    public float heavyAttack02DamageMotionValue;

    //Charged Heavy
    public float heavyChargedAttack01DamageMotionValue;
    public float heavyChargedAttack02DamageMotionValue;

    //Running
    public float lightRunningAttack01DamageMotionValue;

    //Rolling
    public float lightRollingAttack01DamageMotionValue;

    //Backstepping
    public float lightBackstepAttack01DamageMotionValue;

    private WeaponScript weaponThatOwnsThisCollider;
    public WeaponFamily weaponFamily;

    protected override void Awake() {
        base.Awake();

        // weaponThatOwnsThisCollider = GetComponentInParent<WeaponScript>();
        // weaponFamily = weaponThatOwnsThisCollider.stats.weaponFamily;
        // Debug.Log("weaponFamily of collider set to: " + weaponFamily);

        //Disable hit box on awake
        damageCollider.enabled = false;
    }

    protected override void OnTriggerEnter(Collider other) {
        //if (other.gameObject.layer == LayerMask.NameToLayer("Character")) {
            CharacterManager damageTarget = other.GetComponentInParent<CharacterManager>();
            
            //Uncomment below if we want to search on both the damageable character colliders and the character controller collider:
            // if (damageTarget == null) {
            //     damageTarget = other.GetComponent<CharacterManager>();
            // }

                //"Friendly Fire", isn't.
                if (damageTarget != null) {
                    if (damageTarget == characterCausingDamage) {
                    return;
                }

                contactPoint = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);

                //Check if we can damage this target based on friendly fire

                //Check if target is blocking

                //Check if target is invulnerable

                //Damage
                DamageTarget(damageTarget);
            }
        //}
    }

    protected override void DamageTarget(CharacterManager damageTarget) {
        //We don't want to damage the same target more than once in a single attack
        //So we add them to a list that checks before applying damage
        if (charactersDamaged.Contains(damageTarget)) {
            return;
        }

        charactersDamaged.Add(damageTarget);

        //Create a copy of the damage effect to not change the original
        TakeHealthDamageCharacterEffect damageEffect = Instantiate(WorldCharacterEffectsManager.instance.takeHealthDamageEffect);
        damageEffect.isMainHand = isMainHand;

        //Set the Weapon Family of the attack for sound and visual effects
        damageEffect.weaponFamily = weaponFamily;

        //Set Character Causing the damage, if it exits
        if (characterCausingDamage != null) {
            damageEffect.characterCausingDamage = characterCausingDamage;
        }

        //Base Attack Power
        damageEffect.physicalDamage = physicalDamage;

        //Elemental
        damageEffect.elementalDamage = elementalStats;
        //damageEffect.fireDamage = fireDamage;
        //damageEffect.iceDamage = iceDamage;
        //damageEffect.lightningDamage = lightningDamage;
        //damageEffect.windDamage = windDamage;

        ////Anti-Type
        //damageEffect.earthDamage = earthDamage;
        //damageEffect.lightDamage = lightDamage;
        //damageEffect.beastDamage = beastDamage;
        //damageEffect.scalesDamage = scalesDamage;
        //damageEffect.techDamage = techDamage;

        //Armore Penetration
        damageEffect.isReducedByArmor = isReducedByArmor;

        //Determine Motion Value
        if (characterCausingDamage.characterWeaponManager != null) {
            switch (characterCausingDamage.characterWeaponManager.currentAttackType) {

                //Light Attacks
                case AttackType.LightAttack01:
                    attackMotionValue = characterCausingDamage.characterWeaponManager.ownedWeapons[characterCausingDamage.characterWeaponManager.indexOfEquippedWeapon].GetComponent<WeaponScript>().stats.lightAttack01DamageMotionValue;
                    break;
                case AttackType.LightAttack02:
                    attackMotionValue = characterCausingDamage.characterWeaponManager.ownedWeapons[characterCausingDamage.characterWeaponManager.indexOfEquippedWeapon].GetComponent<WeaponScript>().stats.lightAttack02DamageMotionValue;
                    break;
                case AttackType.LightAttack03:
                    attackMotionValue = characterCausingDamage.characterWeaponManager.ownedWeapons[characterCausingDamage.characterWeaponManager.indexOfEquippedWeapon].GetComponent<WeaponScript>().stats.lightAttack03DamageMotionValue;
                    break;

                //Heavy Attacks
                case AttackType.HeavyAttack01:
                    attackMotionValue = characterCausingDamage.characterWeaponManager.ownedWeapons[characterCausingDamage.characterWeaponManager.indexOfEquippedWeapon].GetComponent<WeaponScript>().stats.heavyAttack01DamageMotionValue;
                    break;
                case AttackType.HeavyAttack02:
                    attackMotionValue = characterCausingDamage.characterWeaponManager.ownedWeapons[characterCausingDamage.characterWeaponManager.indexOfEquippedWeapon].GetComponent<WeaponScript>().stats.heavyAttack02DamageMotionValue;
                    break;

                //Charge Heavy Attacks
                case AttackType.ChargedAttack01:
                    attackMotionValue = characterCausingDamage.characterWeaponManager.ownedWeapons[characterCausingDamage.characterWeaponManager.indexOfEquippedWeapon].GetComponent<WeaponScript>().stats.heavyChargedAttack01DamageMotionValue;
                    break;
                case AttackType.ChargedAttack02:
                    attackMotionValue = characterCausingDamage.characterWeaponManager.ownedWeapons[characterCausingDamage.characterWeaponManager.indexOfEquippedWeapon].GetComponent<WeaponScript>().stats.heavyChargedAttack02DamageMotionValue;
                    break;

                //Default
                default:
                    break;
            }
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

    // private void ApplyAttackDamageModifiers(float modifier, TakeHealthDamageCharacterEffect damage) {
    //     damage.
    // }
    
}
