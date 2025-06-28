using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCollider : MonoBehaviour
{
    [Header("Collider")]
    protected Collider damageCollider;

    [Header("Stats")]
    public float physicalDamage = 0f;
    public ElementalStats elementalStats = new ElementalStats();

    [Header("Main Hand / Off Hand weapon")]
    public bool isMainHand = false;

    //Damage modifier for specific attack, which differs between attacks in a combo
    public float attackMotionValue = 1f;

    //Damage modifier for successfully charging an attack fully (e.g. Heavy melee or Magic)
    public float fullChargeModifier = 1f;

    //1 = True, 0 = False
    [Header("Armor Reduces? 1 = T, 0 = F")]
    public int isReducedByArmor = 1;

    [Header("Environmental Hazard Fields")]
    public bool isEnvironmentalHazard = false;
    public float environmentalHazardTickRate = 2f;

    [Header("Contact Point")]
    protected Vector3 contactPoint;

    [Header("Characters Damaged")]
    protected List<CharacterManager> charactersDamaged = new List<CharacterManager>();

    [Header("Block")]
    protected Vector3 directionFromAttackToDamageTarget;
    protected float dotValueFromAttackToDamageTarget;

    protected virtual void Awake()
    {
        damageCollider = GetComponent<Collider>();

        if (isEnvironmentalHazard)
        {
            InvokeRepeating("ResetDamageList", environmentalHazardTickRate, environmentalHazardTickRate);
        }
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        //if (other.gameObject.layer == LayerMask.NameToLayer("Character")) {
        CharacterManager damageTarget = other.GetComponentInParent<CharacterManager>();

        //Uncomment below if we want to search on both the damageable character colliders and the character controller collider:
        // if (damageTarget == null) {
        //     damageTarget = other.GetComponent<CharacterManager>();
        // }
        if (damageTarget != null)
        {
            contactPoint = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);

            //Check if we can damage this target based on friendly fire

            //Check if target is blocking
            CheckForBlock(damageTarget);

            //Check if target is invulnerable

            //Damage
            DamageTarget(damageTarget);
        }
        //}
    }

    protected virtual void CheckForBlock(CharacterManager damageTarget)
    {
        //Character has already been processed
        if (charactersDamaged.Contains(damageTarget))
        {
            return;
        }

        GetBlockingDotValues(damageTarget);

        //TODO: Check for Weapon Reflection (e.g. a one handed spear bouncing off a shield)

        //Check if the character is blocking, and check if they are facing in the correct direction to block successfully
        Debug.Log("isBlocking: " + damageTarget.isBlocking);
        if (damageTarget.isBlocking /*&& dotValueFromAttackToDamageTarget > 0.1f*/)
        {
            charactersDamaged.Add(damageTarget);
            TakeBlockedHealthDamageCharacterEffect blockingTakeDamageEffect = Instantiate(WorldCharacterEffectsManager.instance.takeBlockedHealthDamageCharacterEffect);

            blockingTakeDamageEffect.isMainHand = isMainHand;

            //Base Attack Power
            blockingTakeDamageEffect.physicalDamage = physicalDamage;

            //Elemental
            blockingTakeDamageEffect.elementalDamage = elementalStats;

            //Armore Penetration
            blockingTakeDamageEffect.isReducedByArmor = isReducedByArmor;

            //Apply Motion Value
            blockingTakeDamageEffect.attackMotionValue = attackMotionValue;

            //Apply Charge Bonus Damage Modifier
            blockingTakeDamageEffect.fullChargeModifier = fullChargeModifier;

            //Update Contact Point for VFX
            blockingTakeDamageEffect.contactPoint = contactPoint;

            //Apply copy's blocked character damage to target
            damageTarget.characterEffectsManager.ProcessInstantEffect(blockingTakeDamageEffect);

        }

    }

    //Override this for attacks that don't use a weapon collider such as zombie fist baps
    protected virtual void GetBlockingDotValues(CharacterManager damageTarget)
    {
        directionFromAttackToDamageTarget = transform.position - damageTarget.transform.position;
        dotValueFromAttackToDamageTarget = Vector3.Dot(directionFromAttackToDamageTarget, damageTarget.transform.forward);
    }

    protected virtual void DamageTarget(CharacterManager damageTarget)
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

        //Base Attack Power
        damageEffect.physicalDamage = physicalDamage;

        //Elemental
        damageEffect.elementalDamage = elementalStats;

        //Armore Penetration
        damageEffect.isReducedByArmor = isReducedByArmor;

        //Apply Motion Value
        damageEffect.attackMotionValue = attackMotionValue;

        //Apply Charge Bonus Damage Modifier
        damageEffect.fullChargeModifier = fullChargeModifier;

        //Update Contact Point for VFX
        damageEffect.contactPoint = contactPoint;

        //Apply the copy's damage effect to the target
        damageTarget.characterEffectsManager.ProcessInstantEffect(damageEffect);
    }

    public virtual void EnableDamageCollider()
    {
        damageCollider.enabled = true;
    }

    public virtual void DisableDamageCollider()
    {

        //Reset list of hit enemies so they can be damaged again on next activation
        charactersDamaged.Clear();

        damageCollider.enabled = false;
    }

    private void ResetDamageList()
    {
        charactersDamaged.Clear();
        damageCollider.enabled = false;
        damageCollider.enabled = true;
    }

}
