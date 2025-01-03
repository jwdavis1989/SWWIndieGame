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
    [HideInInspector] public AIStatsManager statsManager;

    [Header("Current State")]
    [SerializeField] AIState currentState;

    [Header("States")]
    [SerializeField] public IdleState idle;
    [SerializeField] public PursueTargetState pursueTarget;

    [Header("Determines which type of exp to drop on death")]
    public bool isHitByMainHand = false;
    public bool isHitByOffHand = false;

    protected override void Awake() {
        base.Awake();
        statsManager = GetComponent<AIStatsManager>();
        aiCharacterCombatManager = GetComponent<AiCharacterCombatManager>();
        navMeshAgent = GetComponentInChildren<NavMeshAgent>();
        // video note: use a copy of the scriptable objects so the originals are not modified...
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
            GetComponent<AIStatsManager>().DoAllDrops(isHitByMainHand, isHitByOffHand);
        }
        
        yield return new WaitForSeconds(5);

    }
    
    public void ProcessStateMachine() {
        AIState nextState = currentState?.Tick(this);

        if (nextState != null) {
            currentState = nextState;
        }

        if (navMeshAgent.enabled) {
            Vector3 agentDestination = navMeshAgent.destination;
            float remainingDistance = Vector3.Distance(agentDestination, transform.position);

            if (remainingDistance > navMeshAgent.stoppingDistance) {
                //aiCharacterNetworkManager.isMoving = true;
            }
            else {
                //aiCharacterNetworkManager.isMoving = false;
            }
        }
        else {
            //aiCharacterNetworkManager.isMoving = false;
        }
    }


}
