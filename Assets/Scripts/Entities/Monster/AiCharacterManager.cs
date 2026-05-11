using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class AICharacterManager : CharacterManager
{

    [Header("Character Name")]
    public string characterName = "";

    [Header("Navmesh Agent")]
    public NavMeshAgent navMeshAgent;

    [HideInInspector] public AiCharacterCombatManager aiCharacterCombatManager;
    [HideInInspector] public AICharacterLocomotionManager aiCharacterLocomotionManager;
    [HideInInspector] public AICharacterStatsManager statsManager;
    [HideInInspector] public AiCharacterSoundFXManager aiCharacterSoundFXManager;

    [Header("Current State")]
    [SerializeField] AIState currentState;

    [Header("States")]
    public IdleState idleState;
    public PursueTargetState pursueTargetState;
    public CombatStanceState combatStanceState;
    public AttackState attackState;
    public FarFromTargetState farFromTargetState;

    [Header("Activation Beacon")]
    protected AIActivationBeacon activationBeacon;

    [Header("Determines which type of exp to drop on death")]
    public bool isHitByMainHand = false;
    public bool isHitByOffHand = false;

    protected override void Awake()
    {
        base.Awake();
        isPlayer = false;
        aiCharacterLocomotionManager = GetComponent<AICharacterLocomotionManager>();
        statsManager = GetComponent<AICharacterStatsManager>();
        aiCharacterCombatManager = GetComponent<AiCharacterCombatManager>();
        aiCharacterSoundFXManager = GetComponent<AiCharacterSoundFXManager>();
        navMeshAgent = GetComponentInChildren<NavMeshAgent>();
        ResetNavMeshAgentPosition();


        //Use a copy of the scriptable objects so the originals are not modified
        idleState = Instantiate(idleState);
        pursueTargetState = Instantiate(pursueTargetState);

        currentState = idleState;
    }

    protected override void Start()
    {
        base.Start();

        //Initialize UI manager to avoid race condition
        characterUIManager.initializeUIManager();

        //Initialize AIActivationBeacon
        CreateActivationBeacon();

        //Character should begin deactivated until they enter player render distance
        DeactivateCharacter();
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

    protected override void OnEnable()
    {
        base.OnEnable();

    }

    //WARNING: Can't be overriden normally in Unity. If bugs involving OnDestroy effects failing, check here.
    private void OnDestroy() {
        if (activationBeacon != null)
        {
            Destroy(activationBeacon);
        }
    }

    public void ResetNavMeshAgentPosition()
    {
        if (navMeshAgent) {
            navMeshAgent.enabled = false;
            navMeshAgent.Warp(transform.position);
            navMeshAgent.enabled = true;
        }
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

        if (!isPlayer)
        {
            characterUIManager.characterHPBar.enabled = false;
        }

        yield return new WaitForSeconds(deathExplosionVFXDelay);

        //Play Death SFX
        //characterSoundFXManager.audioSource.PlayOneShot(WorldSoundFXManager.instance.deathSFX);
        if (!isPlayer)
        {
            //If monster: Award players with Gold or items
            GetComponent<AICharacterStatsManager>().DoAllDrops(isHitByMainHand, isHitByOffHand);

            //Explode!
            characterEffectsManager.PlayDeathVFX();

            //Disable or Despawn Character
            Destroy(this.gameObject);
        }
    }
    
    public void ProcessStateMachine() {
        AIState nextState = currentState?.Tick(this);

        if (nextState != null) {
            currentState = nextState;
        }

        //The position/rotation should be reset only after the state machine has processed its tick
        if (navMeshAgent)
        {
            navMeshAgent.transform.localPosition = Vector3.zero;
            navMeshAgent.transform.localRotation = Quaternion.identity;
        }

        if (aiCharacterCombatManager.currentTarget != null) {
            aiCharacterCombatManager.targetsDirection = aiCharacterCombatManager.currentTarget.transform.position - transform.position;
            aiCharacterCombatManager.viewableAngle = WorldUtilityManager.instance.GetAngleOfTarget(transform, aiCharacterCombatManager.targetsDirection);
            aiCharacterCombatManager.distanceFromTarget = Vector3.Distance(transform.position, aiCharacterCombatManager.currentTarget.transform.position);
        }

        if (navMeshAgent && navMeshAgent.enabled) {
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

    public void BeginRunningAtTarget()
    {
        characterAnimatorManager.UpdateAnimatorMovementParameters(0, 1, false);
    }

    public void ActivateCharacter()
    {
        gameObject.SetActive(true);

        //Enable Renderers to save on memory
        // characterModel.SetActive(true);
        // animator.enabled = true;
        // navMeshAgent.enabled = true;


        aiCharacterCombatManager.isPlayerInRenderRange = true;
    }

    public void DeactivateCharacter()
    {
        //Disable Renderers to save on memory
        // characterModel.SetActive(false);
        // animator.enabled = false;
        // navMeshAgent.enabled = false;

        if (activationBeacon != null)
        {
            activationBeacon.transform.position = transform.position;
            activationBeacon.gameObject.SetActive(true);
        }

        aiCharacterCombatManager.isPlayerInRenderRange = false;
        aiCharacterCombatManager.SetTarget(null);

        //Disable enemy to save on memory
        gameObject.SetActive(false);
    }

    public void CreateActivationBeacon()
    {
        if (activationBeacon == null)
        {
            GameObject activationBeaconObject = Instantiate(WorldAIManager.instance.activationBeaconGameObject);
            activationBeaconObject.transform.position = transform.position;

            activationBeacon = activationBeaconObject.GetComponent<AIActivationBeacon>();
            activationBeacon.SetOwnerOfBeacon(this);
        }
        else
        {
            activationBeacon.transform.position = transform.position;
            activationBeacon.gameObject.SetActive(true);
        }
    }

}
