using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class AICharacterManager : CharacterManager
{

    [Header("Navmesh Agent")]
    public NavMeshAgent navMeshAgent;

    [HideInInspector] public AiCharacterCombatManager aiCharacterCombatManager;
    [HideInInspector] public AICharacterLocomotionManager aiCharacterLocomotionManager;
    [HideInInspector] public AICharacterStatsManager statsManager;

    [Header("Current State")]
    [SerializeField] AIState currentState;

    [Header("States")]
    public IdleState idleState;
    public PursueTargetState pursueTargetState;
    public CombatStanceState combatStanceState;
    public AttackState attackState;

    [Header("Determines which type of exp to drop on death")]
    public bool isHitByMainHand = false;
    public bool isHitByOffHand = false;

    protected override void Awake()
    {
        base.Awake();
        aiCharacterLocomotionManager = GetComponent<AICharacterLocomotionManager>();
        statsManager = GetComponent<AICharacterStatsManager>();
        aiCharacterCombatManager = GetComponent<AiCharacterCombatManager>();
        navMeshAgent = GetComponentInChildren<NavMeshAgent>();

        //Use a copy of the scriptable objects so the originals are not modified
        idleState = Instantiate(idleState);
        pursueTargetState = Instantiate(pursueTargetState);

        currentState = idleState;
    }

    protected override void Update() {
        base.Update();

        aiCharacterCombatManager.HandleActionRecovery(this);
    }
    protected override void FixedUpdate() {
        base.FixedUpdate();

        ProcessStateMachine();
    }

    protected override void LateUpdate() {
        base.LateUpdate();
    }

    public override IEnumerator ProcessDeathEvent(bool manuallySelectDeathAnimation = false) {
        characterStatsManager.currentHealth = 0;
        canMove = false;
        isDead = true;
        //Reset any Flags here that need to be reset
        //Todo: Add these later

        //If not grounded, play an aerial death animation
        if (!manuallySelectDeathAnimation) {
            //Could change this to choose a random death animation in the future if we wanted to.
            characterAnimatorManager.PlayTargetActionAnimation("Dead_01", true);
        }

        //Play Death SFX
        //characterSoundFXManager.audioSource.PlayOneShot(WorldSoundFXManager.instance.deathSFX);
        if (!isPlayer) {
            //If monster: Award players with Gold or items
            GetComponent<AICharacterStatsManager>().DoAllDrops(isHitByMainHand, isHitByOffHand);
        }
        
        yield return new WaitForSeconds(5);

    }
    
    public void ProcessStateMachine() {
        AIState nextState = currentState?.Tick(this);

        if (nextState != null) {
            currentState = nextState;
        }

        //The position/rotation should be reset only after the state machine has processed its tick
        navMeshAgent.transform.localPosition = Vector3.zero;
        navMeshAgent.transform.localRotation = Quaternion.identity;

        if (aiCharacterCombatManager.currentTarget != null) {
            aiCharacterCombatManager.targetsDirection = aiCharacterCombatManager.currentTarget.transform.position - transform.position;
            aiCharacterCombatManager.viewableAngle = WorldUtilityManager.instance.GetAngleOfTarget(transform, aiCharacterCombatManager.targetsDirection);
            aiCharacterCombatManager.distanceFromTarget = Vector3.Distance(transform.position, aiCharacterCombatManager.currentTarget.transform.position);
        }

        if (navMeshAgent.enabled) {
            Vector3 agentDestination = navMeshAgent.destination;
            float remainingDistance = Vector3.Distance(agentDestination, transform.position);

            if (remainingDistance > navMeshAgent.stoppingDistance) {
                isMoving = true;
            }
            else {
                isMoving = false;
            }
        }
        else {
            isMoving = false;
        }
    }


}
