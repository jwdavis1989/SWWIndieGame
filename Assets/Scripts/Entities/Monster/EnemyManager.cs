using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyManager : CharacterManager
{
    public bool isAggro = false;
    public float aggroRange = 1.0f;
    public AggroCollider aggroCollider;

    public float atkRange = 1.0f;
    public CharacterCombatManager combatManager;

    private bool movingToTarget = false;
    public float turnSpeed = 1.0f;
    [Header("Determines which type of exp to drop on death")]
    public bool lastHitByMainHand = true;
    protected override void Awake()
    {
        base.Awake();
        if (combatManager == null) combatManager = GetComponent<CharacterCombatManager>();
        if(aggroCollider) aggroCollider.SetRange(aggroRange);
    }
    protected override void LateUpdate()
    {
        if (!isDead && isLockedOn)
        {
            TurnTowardsTarget();
            if(movingToTarget)
                MoveToTarget();
        }
    }

    public void TurnTowardsTarget()
    {
        //This rotates this gameObject
        Vector3 rotationDirection = combatManager.currentTarget.characterCombatManager.LockOnTransform.position - transform.position;
        rotationDirection.Normalize();
        rotationDirection.y = 0;

        Quaternion targetRotation = Quaternion.LookRotation(rotationDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed);

        //This rotates the pivot object
        //We don't set rotationDirection.y = 0 because this is the up/down rotation
        rotationDirection = combatManager.currentTarget.characterCombatManager.LockOnTransform.position - transform.position;
        rotationDirection.Normalize();

        targetRotation = Quaternion.LookRotation(rotationDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed);

        //Save our rotation values, so when we unlock it doesn't snap too far away
        //float leftAndRightLookAngle = transform.eulerAngles.y;
        //float upAndDownLookAngle = transform.eulerAngles.x;
        //leftAndRightLookAngle = transform.eulerAngles.y;
        //upAndDownLookAngle = transform.eulerAngles.x;
    }
    public void BeginAttack01()
    {
        movingToTarget = true;
        StartCoroutine(LungeForward(3.0f));
    }
    public IEnumerator LungeForward(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        movingToTarget = false;

    }
    public float speed = 1.0f;
    public void MoveToTarget()
    {
        var step = speed * Time.deltaTime; // calculate distance to move
        speed *= 1.0f + (0.5f * Time.deltaTime);//accelerate
        transform.parent.position = Vector3.MoveTowards(transform.parent.position, combatManager.currentTarget.transform.position, step);
    }
    public void AggroPlayer(GameObject player)
    {
        if (combatManager != null)
        {
            isLockedOn = true;
            combatManager.LockOnTransform = player.transform;
            combatManager.SetTarget(player.GetComponent<CharacterManager>());
            movingToTarget = true;
        }
        else Debug.Log("Combat manager is null");
    }
    public override IEnumerator ProcessDeathEvent(bool manuallySelectDeathAnimation = false)
    {
        characterStatsManager.currentHealth = 0;
        canMove = false;
        isDead = true;

        //Reset any Flags here that need to be reset
        //Todo: Add these later

        //If not grounded, play an aerial death animation

        if (!manuallySelectDeathAnimation)
        {
            //Could change this to choose a random death animation in the future if we wanted to.
            characterAnimatorManager.PlayTargetActionAnimation("Dead_01", true);
        }

        //Play Death SFX
        //characterSoundFXManager.audioSource.PlayOneShot(WorldSoundFXManager.instance.deathSFX);

        if (!isPlayer)
        {
            //If monster: Award players with Gold or items
            GetComponent<EnemyStatsManager>().DoAllDrops(lastHitByMainHand);
        }
        
        yield return new WaitForSeconds(5);

    }
}
