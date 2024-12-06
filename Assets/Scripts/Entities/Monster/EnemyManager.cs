using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyManager : CharacterManager
{
    [Header("Aggro")]
    public bool isAggro = false;
    public float aggroRange = 5.0f;
    public AggroCollider aggroCollider;
    public AtkRangeCollider atkCollider;
    [Header("Attack")]
    public float atkRange = 2.0f;
    bool chargingAtk1 = false;
    public float atk1ChargeTime = 2.0f;
    public CharacterCombatManager combatManager;
    [Header("Movement")]
    private bool movingToTarget = false;
    public float speed = 2.0f;
    public float turnSpeed = 1.0f;
    bool isFlying = true;
    public float upwardSpeed = 1.0f;
    [Header("Determines which type of exp to drop on death")]
    public bool lastHitByMainHand = true;
    protected override void Awake()
    {
        base.Awake();
        if(combatManager == null) combatManager = GetComponent<CharacterCombatManager>();
        if(aggroCollider) aggroCollider.SetRange(aggroRange);
        if(atkCollider) atkCollider.SetRange(atkRange);
    }
    protected override void LateUpdate()
    {
        base.LateUpdate();
        if (!isDead && isLockedOn)
        {
            if(canRotate)
                TurnTowardsTarget();
            if (canMove && movingToTarget)
            {
                MoveToTarget();
                if(isFlying)
                    MoveUpward();
            }
        }
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
    public void BeginAttack01()
    {
        if (!chargingAtk1)
        {
            chargingAtk1 = true;
            movingToTarget = false;
            StartCoroutine(EndAttack01(atk1ChargeTime));
            //Test animation... I HAVE NO IDEA WHATS IM DOING. I thikn atk ani should start here - alec 
            //string light_attack_01 = "Main_Hand_Light_Attack_01";
            //characterAnimatorManager.PlayTargetAttackActionAnimation(AttackType.LightAttack01, light_attack_01, true);
        }

    }

    public IEnumerator EndAttack01(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        movingToTarget = true;
        chargingAtk1 = false;
        //Test animation... I HAVE NO IDEA WHATS IM DOING. I try go back to reg movement here - alec 
        //characterAnimatorManager.PlayTargetActionAnimation("SomeIdleAnimation?", false, true, true, true);
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


    public void MoveToTarget()
    {
        var step = speed * Time.deltaTime; // calculate distance to move
        if(combatManager != null && combatManager.currentTarget != null)
            transform.position = Vector3.MoveTowards(transform.position, combatManager.currentTarget.transform.position, step);
    }

    
    public void MoveUpward()
    {
        float d = upwardSpeed*Time.deltaTime;
        transform.position = new Vector3(transform.position.x, transform.position.y + d, transform.position.z);
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
