using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FireBallManager : SpellManager
{
    //This script serves as the central hub to manipulate and adjust the fireball spell once it's active:
    //1. Give the spell slight homing toward lock-on targets
    //2. Assign damage
    //3. Enable/Disable VFX & SFX including the Contact explosion, trails, etc.
    [Header("Colliders")]
    public SpellProjectileDamageCollider damageCollider;

    [Header("Instantiated FX")]
    private GameObject instantiatedDestructionFX;

    private bool hasCollided = false;
    public bool isFullyCharged = false;
    private Rigidbody fireBallRigidBody;
    private Coroutine destructionFXCoroutine;

    protected override void Awake()
    {
        base.Awake();

        fireBallRigidBody = GetComponent<Rigidbody>();
        damageCollider = GetComponentInChildren<SpellProjectileDamageCollider>();
    }
    protected override void Update()
    {
        base.Update();

        if (spellTargetCharacter != null)
        {
            transform.LookAt(spellTargetCharacter.transform);
        }

        if (fireBallRigidBody != null)
        {
            Vector3 currentVelocity = fireBallRigidBody.velocity;
            fireBallRigidBody.velocity = transform.forward + currentVelocity;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Ignore collisions with Character layer. The Damage collider will handle character collisions while this is only for impact VFX
        if (collision.gameObject.layer == 6 /*Character Layer*/)
        {
            return;
        }

        if (!hasCollided)
        {
            hasCollided = true;
            InstantiateSpellDestructionFX();
        }
    }

    public void InitializeFireBall(CharacterManager characterCausingDamage)
    {
        damageCollider.characterCausingDamage = characterCausingDamage;
        damageCollider.InitializeStats();
    }

    public void InstantiateSpellDestructionFX()
    {
        if (isFullyCharged)
        {
            instantiatedDestructionFX = Instantiate(impactParticleFullChargeVFX, transform.position, Quaternion.identity);
        }
        else
        {   
            instantiatedDestructionFX = Instantiate(impactParticleVFX, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }

    public void WaitThenInstantiateSpellDestructionFX(float timeToWaitInSeconds)
    {
        if (destructionFXCoroutine != null)
        {
            StopCoroutine(destructionFXCoroutine);
        }

        destructionFXCoroutine = StartCoroutine(WaitThenInstantiateFX(timeToWaitInSeconds));
        //StartCoroutine(WaitThenInstantiateFX(timeToWaitInSeconds));
    }

    private IEnumerator WaitThenInstantiateFX(float timeToWaitInSeconds)
    {
        yield return new WaitForSeconds(timeToWaitInSeconds);

        InstantiateSpellDestructionFX();
    }
    
}
