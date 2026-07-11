using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BulletManager : SpellManager
{
    //This script serves as the central hub to manipulate and adjust the fireball spell once it's active:
    //1. Give the spell slight homing toward lock-on targets
    //2. Assign damage
    //3. Enable/Disable VFX & SFX including the Contact explosion, trails, etc.
    [Header("Colliders")]
    public BulletProjectileDamageCollider damageCollider;

    [Header("Instantiated FX")]
    protected GameObject instantiatedDestructionFX;
    public ElementalDamageType highestElementalDamageType;

    protected bool hasCollided = false;
    public bool isFullyCharged = false;
    protected Rigidbody fireBallRigidBody;
    protected Coroutine destructionFXCoroutine;

    protected Transform spawnTransform;

    protected override void Awake()
    {
        base.Awake();

        fireBallRigidBody = GetComponent<Rigidbody>();
        damageCollider = GetComponentInChildren<BulletProjectileDamageCollider>();
    }
    protected override void Update()
    {
        base.Update();

        if (spellTargetCharacter != null)
        {
            transform.LookAt(spellTargetCharacter.characterCombatManager.LockOnTransform.position);
        }

        //TODO: Technichally erroneous, consider commenting out then increasing the momentum of the spell's forward/upward velocities
            if (fireBallRigidBody != null)
            {
                Vector3 currentVelocity = fireBallRigidBody.velocity;
                fireBallRigidBody.velocity = transform.forward + currentVelocity;
            }
        //END OF COMMENT OUT SECTION
    }

    protected void OnCollisionEnter(Collision collision)
    {
        //Ignore collisions with Character layer. The Damage collider will handle character collisions while this is only for impact VFX
        // if (collision.gameObject.layer == 6 /*Character Layer*/)
        // {
        //     return;
        // }

        if (!hasCollided)
        {
            hasCollided = true;
            InstantiateSpellDestructionFX();
        }
    }

    public virtual void InitializeBullet(CharacterManager characterCausingDamage, ElementalDamageType currentHighestElementalDamageType, float projectileLifetimeInSeconds, Transform spawnTransform, float projectileUpwardVelocityMultiplier, float projectileForwardVelocityMultiplier, bool hasGravity = true)
    {
        damageCollider.characterCausingDamage = characterCausingDamage;
        damageCollider.InitializeStats();
        if (isFullyCharged)
        {
            damageCollider.fullChargeModifier = characterCausingDamage.characterWeaponManager.GetEquippedWeapon(true).GetComponent<WeaponScript>().fullChargingTraitModifier;
        }
        highestElementalDamageType = currentHighestElementalDamageType;

        this.spawnTransform = spawnTransform;

        //Set whether the projectile will fall over time or fly straight
        fireBallRigidBody.useGravity = hasGravity;

        //Destroy bullet after its lifetime ends
        StartCoroutine(DestroyAfterTime(projectileLifetimeInSeconds));

        GetComponent<SpellElementalVFXManager>().ChangeVFXBasedOnElement(highestElementalDamageType);
    }

    public virtual void InstantiateSpellDestructionFX()
    {
        if (isFullyCharged)
        {
            instantiatedDestructionFX = Instantiate(impactParticleFullChargeVFX, transform.position, Quaternion.identity);
        }
        else
        {
            instantiatedDestructionFX = Instantiate(impactParticleVFX, transform.position, Quaternion.identity);
        }

        //Update Explosion VFX based on highest element of the magic weapon used to cast it
        instantiatedDestructionFX.GetComponent<SpellElementalVFXManager>().ChangeVFXBasedOnElement(highestElementalDamageType);
        Destroy(gameObject);
    }

    public void WaitThenInstantiateSpellDestructionFX(float timeToWaitInSeconds)
    {
        if (destructionFXCoroutine != null)
        {
            StopCoroutine(destructionFXCoroutine);
        }

        destructionFXCoroutine = StartCoroutine(WaitThenInstantiateFX(timeToWaitInSeconds));
        StartCoroutine(WaitThenInstantiateFX(timeToWaitInSeconds));
    }

    protected IEnumerator WaitThenInstantiateFX(float timeToWaitInSeconds)
    {
        yield return new WaitForSeconds(timeToWaitInSeconds);

        InstantiateSpellDestructionFX();
    }

    protected IEnumerator DestroyAfterTime(float lifeSpanInSeconds)
    {
        yield return new WaitForSeconds(lifeSpanInSeconds);

        InstantiateSpellDestructionFX();
    }
    
}
