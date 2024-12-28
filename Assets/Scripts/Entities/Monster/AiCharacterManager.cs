using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class AICharacterManager : CharacterManager
{

    [Header("Navmesh Agent")]
    public NavMeshAgent navMeshAgent;

    [HideInInspector] public AICombatManager aiCombatManager;
    [Header("Current State")]
    [SerializeField] AIState currentState;
    [Header("States")]
    [SerializeField] public IdleState idle;
    [SerializeField] public PursueTargetState pursueTarget;
    //[Header("Aggro")]
    //public bool isAggro = false;
    //public float aggroRange = 5.0f;
    public AggroCollider aggroCollider;
    public AtkRangeCollider atkCollider;
    [Header("Attack")]
    public float atkRange = 2.0f;
    bool chargingAtk1 = false;
    public float atk1ChargeTime = 2.0f;
    [Header("Movement")]
    public bool pursuingTarget = false;
    public float speed = 2.0f;
    public float turnSpeed = 1.0f;
    bool isFlying = true;
    public float upwardSpeed = 1.0f;
    [Header("Determines which type of exp to drop on death")]
    public bool isHitByMainHand = false;
    public bool isHitByOffHand = false;
    public AIStatsManager statsManager;
    protected override void Awake()
    {
        base.Awake();
        statsManager = GetComponent<AIStatsManager>();
        aiCombatManager = GetComponent<AICombatManager>();
        navMeshAgent = GetComponentInChildren<NavMeshAgent>();
        // use a copy of the scriptable objects os the originals are not modified...

        //old
        //if(aggroCollider) aggroCollider.SetRange(aggroRange);
        //if(atkCollider) atkCollider.SetRange(atkRange);
    }
    protected override void LateUpdate()
    {
        base.LateUpdate();
        if (!isDead && isLockedOn)
        {
            if(canRotate)
                TurnTowardsTarget();
            if (canMove && pursuingTarget)
            {
                MoveToTarget();
                if(isFlying)
                    MoveUpward();
            }
        }
    }

    public void AggroPlayer(GameObject player)
    {
        if (aiCombatManager != null)
        {
            isLockedOn = true;
            aiCombatManager.LockOnTransform = player.transform;
            aiCombatManager.SetTarget(player.GetComponent<CharacterManager>());
            pursuingTarget = true;
        }
        else Debug.Log("Combat manager is null");
    }
    public void ChargeAttack01()
    {
        if (!chargingAtk1)
        {
            chargingAtk1 = true;
            pursuingTarget = false;
            StartCoroutine(BeginAttack01(atk1ChargeTime));
            //Test animation... I HAVE NO IDEA WHATS IM DOING. I thikn atk ani should start here - alec 
            //string light_attack_01 = "Main_Hand_Light_Attack_01";
            //characterAnimatorManager.PlayTargetAttackActionAnimation(AttackType.LightAttack01, light_attack_01, true);
        }

    }
    private bool performingAtk1 = false;
    public float atk1Duration = 2.0f;

    public IEnumerator BeginAttack01(float delayTime)
    {
        if (!performingAtk1)
        {
            yield return new WaitForSeconds(delayTime);
            performingAtk1 = true;
            pursuingTarget = true;
            chargingAtk1 = false;
            StartCoroutine(EndAttack01(atk1Duration));
        }
        //Test animation... I HAVE NO IDEA WHATS IM DOING. I try go back to reg movement here - alec 
        //characterAnimatorManager.PlayTargetActionAnimation("SomeIdleAnimation?", false, true, true, true);
    }
    public IEnumerator EndAttack01(float delayTime)
    {
        if (performingAtk1)
        {
            yield return new WaitForSeconds(delayTime);
            performingAtk1 = false;
            ChargeAttack01();
        }
        //Test animation... I HAVE NO IDEA WHATS IM DOING. I try go back to reg movement here - alec 
        //characterAnimatorManager.PlayTargetActionAnimation("SomeIdleAnimation?", false, true, true, true);
    }
    public void TurnTowardsTarget()
    {
        //This rotates this gameObject
        Vector3 rotationDirection = aiCombatManager.currentTarget.characterCombatManager.LockOnTransform.position - transform.position;
        rotationDirection.Normalize();
        rotationDirection.y = 0;

        Quaternion targetRotation = Quaternion.LookRotation(rotationDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed);

        //This rotates the pivot object
        //We don't set rotationDirection.y = 0 because this is the up/down rotation
        rotationDirection = aiCombatManager.currentTarget.characterCombatManager.LockOnTransform.position - transform.position;
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
        if(aiCombatManager != null && aiCombatManager.currentTarget != null)
            transform.position = Vector3.MoveTowards(transform.position, aiCombatManager.currentTarget.transform.position, step);
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
            GetComponent<AIStatsManager>().DoAllDrops(isHitByMainHand, isHitByOffHand);
        }
        
        yield return new WaitForSeconds(5);

    }
    
    public void ProcessStateMachine()
    {
        AIState nextState = currentState?.Tick(this);
        if (nextState != null)
        {
            currentState = nextState;
        }
    }
}
