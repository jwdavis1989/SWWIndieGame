using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.TextCore.Text;

public class AICharacterManager : CharacterManager
{

    [Header("Character Name")]
    public string characterName = "";

    [Header("Navmesh Agent")]
    public NavMeshAgent navMeshAgent;
    [Header("Navmesh movement speed used")]
    public bool navMeshMovement = false;

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

    [Header("Default Flank Direction")]
    private bool flankRight = true;

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

        if (hasSecondaryAnimator)
        {
            instantiatedSecondaryAnimatorActor = Instantiate(secondaryAnimatorActorPrefab, transform.position, transform.rotation);
            secondaryAnimatorActorSyncScript = instantiatedSecondaryAnimatorActor.GetComponent<AiSyncLocationConstantly>();
            secondaryAnimatorActorSyncScript.InitializeAiSync(gameObject, this);
            characterWeaponManager.mainHandWeaponAnchor = instantiatedSecondaryAnimatorActor.GetComponentInChildren<MainHandWeaponAnchor>().gameObject;
            secondaryAnimator = instantiatedSecondaryAnimatorActor.GetComponent<Animator>();
        }

        //Initialize AIActivationBeacon
        CreateActivationBeacon();

        //Character should begin deactivated until they enter player render distance
        DeactivateCharacter();
    }

    protected override void Update()
    {
        base.Update();

        if (hasSecondaryAnimator && secondaryAnimatorActorSyncScript != null && secondaryAnimatorActorSyncScript.hasBeenInitialized)
        {
            secondaryAnimator?.SetBool("isGrounded", isGrounded);
            secondaryAnimator?.SetBool("isChargingAttack", isChargingAttack);
            secondaryAnimator?.SetBool("isChargingSpell", isChargingSpellAttack);
            secondaryAnimator?.SetBool("isAiming", isAiming);
            secondaryAnimator?.SetBool("isMoving", isMoving);
            secondaryAnimator?.SetBool("isBlocking", isBlocking);
        }

        aiCharacterCombatManager.HandleActionRecovery(this);
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        ProcessStateMachine();
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();
    }

    protected override void OnEnable()
    {
        base.OnEnable();

    }

    //WARNING: Can't be overriden normally in Unity. If bugs involving OnDestroy effects failing, check here.
    private void OnDestroy()
    {
        if (activationBeacon != null)
        {
            Destroy(activationBeacon);
        }

        if (instantiatedSecondaryAnimatorActor != null)
        {
            Destroy(instantiatedSecondaryAnimatorActor);
        }
    }

    public void ResetNavMeshAgentPosition()
    {
        if (navMeshAgent)
        {
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

    public void ProcessStateMachine()
    {
        AIState nextState = currentState?.Tick(this);

        if (nextState != null)
        {
            currentState = nextState;
        }

        //The position/rotation should be reset only after the state machine has processed its tick
        if (navMeshAgent)
        {
            navMeshAgent.transform.localPosition = Vector3.zero;
            navMeshAgent.transform.localRotation = Quaternion.identity;
        }

        if (aiCharacterCombatManager.currentTarget != null)
        {
            aiCharacterCombatManager.targetsDirection = aiCharacterCombatManager.currentTarget.transform.position - transform.position;
            aiCharacterCombatManager.viewableAngle = WorldUtilityManager.instance.GetAngleOfTarget(transform, aiCharacterCombatManager.targetsDirection);
            aiCharacterCombatManager.distanceFromTarget = Vector3.Distance(transform.position, aiCharacterCombatManager.currentTarget.transform.position);
        }

        if (navMeshAgent && navMeshAgent.enabled)
        {
            Vector3 agentDestination = navMeshAgent.destination;
            float remainingDistance = Vector3.Distance(agentDestination, transform.position);

            if (remainingDistance > navMeshAgent.stoppingDistance)
            {
                isMoving = true;
            }
            else
            {
                isMoving = false;
            }
        }
        else
        {
            isMoving = false;
        }
    }

    public void BeginRunningAtTarget()
    {
        characterAnimatorManager.UpdateAnimatorMovementParameters(0, 1, false);
    }

    public void BeginFlankingTarget()
    {
        // characterAnimatorManager.UpdateAnimatorMovementParameters(0.5f, 0, false);

        //1. Play the walking animation
        characterAnimatorManager.UpdateAnimatorMovementParameters(0.5f, 0, false);
        
        //2. Safeguard check for targets
        if (aiCharacterCombatManager.currentTarget == null || navMeshAgent == null) {
            return; 
        }
        
        Transform targetTransform = aiCharacterCombatManager.currentTarget.transform;
        
        //3. Randomize the direction if the agent has reached its destination or has no path
        if (!navMeshAgent.hasPath || navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            flankRight = Random.value > 0.5f;
        }
        
        //4. Determine flank direction based on the randomized choice
        //Positive right vector moves right, negative right vector moves left
        Vector3 sideDirection = flankRight ? targetTransform.right : -targetTransform.right;
        
        //Combine side direction with a slight pull toward the back of the player
        Vector3 flankDirection = (sideDirection - targetTransform.forward).normalized;// Determine how far away from the player the flanking path should orbit (e.g., 2 meters)
        float flankRadius = 2f;
        Vector3 targetFlankPosition = targetTransform.position + (flankDirection * flankRadius);
        
        //5. Sample the NavMesh to find the closest valid walkable point
        if (NavMesh.SamplePosition(targetFlankPosition, out NavMeshHit hit, 2f, NavMesh.AllAreas))
        {
            //6. Tell the agent to move to the flanking point
            navMeshAgent.SetDestination(hit.position);
        }
    }

    public void BeginFlankingTargetFast()
    {
        //1. Play the walking animation
        characterAnimatorManager.UpdateAnimatorMovementParameters(1, 0, false);
        
        //2. Safeguard check for targets
        if (aiCharacterCombatManager.currentTarget == null || navMeshAgent == null) {
            return; 
        }
        
        Transform targetTransform = aiCharacterCombatManager.currentTarget.transform;
        
        //3. Randomize the direction if the agent has reached its destination or has no path
        if (!navMeshAgent.hasPath || navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            flankRight = Random.value > 0.5f;
        }
        
        //4. Determine flank direction based on the randomized choice
        //Positive right vector moves right, negative right vector moves left
        Vector3 sideDirection = flankRight ? targetTransform.right : -targetTransform.right;
        
        //Combine side direction with a slight pull toward the back of the player
        Vector3 flankDirection = (sideDirection - targetTransform.forward).normalized;// Determine how far away from the player the flanking path should orbit (e.g., 2 meters)
        float flankRadius = 2f;
        Vector3 targetFlankPosition = targetTransform.position + (flankDirection * flankRadius);
        
        //5. Sample the NavMesh to find the closest valid walkable point
        if (NavMesh.SamplePosition(targetFlankPosition, out NavMeshHit hit, 2f, NavMesh.AllAreas))
        {
            //6. Tell the agent to move to the flanking point
            navMeshAgent.SetDestination(hit.position);
        }
    }

    public void BeginFlankingAndRunningAtTarget()
    {
        characterAnimatorManager.UpdateAnimatorMovementParameters(1, 1, false);
    }

    public void BeginFlankingAndWalkingAtTarget()
    {
        characterAnimatorManager.UpdateAnimatorMovementParameters(0.5f, 0.5f, false);
    }

    public void ActivateCharacter()
    {
        gameObject.SetActive(true);

        //Enable Renderers to save on memory
        // characterModel.SetActive(true);
        // animator.enabled = true;
        // navMeshAgent.enabled = true;

        //Re-enable secondary animators for non-humanoid creatures
        if (hasSecondaryAnimator)
        {
            instantiatedSecondaryAnimatorActor.SetActive(true);
        }


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

        //Disable secondary animators for non-humanoid creatures
        if (hasSecondaryAnimator)
        {
            instantiatedSecondaryAnimatorActor.SetActive(false);
        }
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
    public override void ApplyDamage(float damage, CharacterManager characterCausingDamage = null, bool isMainHand = false, string damageColor = "white")
    {
        base.ApplyDamage(damage, characterCausingDamage, isMainHand);
        if (characterCausingDamage != null)
        {
            if (isMainHand)
            {
                isHitByMainHand = true;
            }
            else
            {
                isHitByOffHand = true;
                DungeonManager.offHandUsed = true;
            }
            //Aggro the monster if they aren't already
            if (characterCausingDamage.isPlayer && characterCombatManager.currentTarget == null)
            {
                characterCombatManager.AggroPlayer(characterCausingDamage.gameObject);
            }
        }
        if (characterUIManager != null)
        {
            characterUIManager.TriggerGlitchTextEffect();
            characterUIManager.TriggerDamagePopUp(damage, damageColor);
        }
    }
    public override void ApplyOnHitEffects(CharacterManager target, float hitDamage = 1, bool isMainHand = false)
    {
        if (characterWeaponManager != null)
        {
            WeaponScript weapon = isMainHand ? characterWeaponManager.GetMainHand() : characterWeaponManager.GetOffHand();
            if (weapon != null)
            {
                weapon.ApplyWeaponOnHitEffects(target, hitDamage);
            }
        }
        else if (aiCharacterCombatManager.onHitEffect != null)
        {
            target.characterEffectsManager.ProcessInstantEffect(aiCharacterCombatManager.onHitEffect.Instantiate(hitDamage));
        }
        //else do on hit effects from enemies without weapons?
    }
}
