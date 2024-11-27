using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeaponDamageCollider : DamageCollider
{
    [Header("Attacking Character")]
    public CharacterManager characterCausingDamage;

    [Header("Weapon Attack Modifiers")]
    public float lightAttack01DamageMotionValue;

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

        //Set the Weapon Family of the attack for sound and visual effects
        damageEffect.weaponFamily = weaponFamily;

        //Base Attack Power
        damageEffect.physicalDamage = physicalDamage;

        //Elemental
        damageEffect.fireDamage = fireDamage;
        damageEffect.iceDamage = iceDamage;
        damageEffect.lightningDamage = lightningDamage;
        damageEffect.windDamage = windDamage;

        //Anti-Type
        damageEffect.earthDamage = earthDamage;
        damageEffect.lightDamage = lightDamage;
        damageEffect.beastDamage = beastDamage;
        damageEffect.scalesDamage = scalesDamage;
        damageEffect.techDamage = techDamage;

        //Armore Penetration
        damageEffect.isReducedByArmor = isReducedByArmor;

        //Determine Motion Value
        if (characterCausingDamage.characterWeaponManager != null) {
            switch (characterCausingDamage.characterWeaponManager.currentAttackType) {
                case AttackType.LightAttack01:
                    attackMotionValue = characterCausingDamage.characterWeaponManager.ownedWeapons[characterCausingDamage.characterWeaponManager.indexOfEquippedWeapon].GetComponent<WeaponScript>().stats.lightAttack01DamageMotionValue;
                    break;
                default:
                    break;
            }
        }

        //Apply Motion Value
        damageEffect.attackMotionValue = attackMotionValue;

        //TODO: Determine Full Charge Bonus Damage Modifier for Charged Heavy & Charged Spell Attacks
        //Probably a switch

        //Apply Full Charge Bonus Damage Modifier for Charged Heavy & Charged Spell Attacks
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
