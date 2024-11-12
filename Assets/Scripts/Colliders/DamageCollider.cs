using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCollider : MonoBehaviour
{
    [Header("Collider")]
    protected Collider damageCollider;

    [Header("Stats")]
    //public WeaponStats stats;
    public float physicalDamage = 0f;
    public float fireDamage = 0f;
    public float iceDamage = 0f;
    public float lightningDamage = 0f;
    public float windDamage = 0f;
    public float earthDamage = 0f;
    public float lightDamage = 0f;
    public float beastDamage = 0f;
    public float scalesDamage = 0f;
    public float techDamage = 0f;

    //1 = True, 0 = False
    [Header("Armor Reduces? 1 = T, 0 = F")]
    public int isReducedByArmor = 1;

    [Header("Contact Point")]
    protected Vector3 contactPoint;

    [Header("Characters Damaged")]
    protected List<CharacterManager> charactersDamaged = new List<CharacterManager>();

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("Character")) {
            CharacterManager damageTarget = other.GetComponent<CharacterManager>();

            if (damageTarget != null) {
                contactPoint = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);

                //Check if we can damage this target based on friendly fire

                //Check if target is blocking

                //Check if target is invulnerable

                //Damage
                DamageTarget(damageTarget);
            }
        }
    }

    protected virtual void DamageTarget(CharacterManager damageTarget) {
        //We don't want to damage the same target more than once in a single attack
        //So we add them to a list that checks before applying damage
        //Create a copy of the damage effect to not change the original
        TakeHealthDamageCharacterEffect damageEffect = Instantiate(WorldCharacterEffectsManager.instance.takeHealthDamageEffect);

        //Base Attack Power
        damageEffect.physicalDamage = physicalDamage;
        //damageEffect.weaponScript.stats = stats;

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
        
        //Apply the copy's damage effect to the target
        damageTarget.characterEffectsManager.ProcessInstantEffect(damageEffect);
    }

    public virtual void EnableDamageCollider() {
        damageCollider.enabled = true;
    }

    public virtual void DisableDamageCollider() {
        damageCollider.enabled = false;

        //Reset list of hit enemies so they can be damaged again on next activation
        charactersDamaged.Clear();
    }

}
