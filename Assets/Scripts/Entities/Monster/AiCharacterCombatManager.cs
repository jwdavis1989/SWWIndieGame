using UnityEngine;

public class AiCharacterCombatManager : CharacterCombatManager
{
    [Header("Aggro")]
    public bool isAggro = false;
    public float aggroRange = 5.0f;
    public float minimumDetectionAngle = -35f;
    public float maximumDetectionAngle = 35f;

    [Header("Target Information")]
    public float distanceFromTarget;
    public float viewableAngle;
    public Vector3 targetsDirection;

    [Header("Action Recovery")]
    public float actionRecoveryTimer = 0f;

    [Header("Attack Rotation Speed")]
    public float attackRotationSpeed = 25f;

    public void FindATargetWithInLineOSight(AICharacterManager aiCharacter) {
        if(currentTarget != null) {
            return;
        }

        Collider[] colliders = Physics.OverlapSphere(aiCharacter.transform.position, aggroRange, WorldUtilityManager.instance.GetCharacterLayers());

        for(int i = 0; i < colliders.Length; i++) {
            CharacterManager targetCharacter = colliders[i].transform.GetComponent<CharacterManager>();

            //Check if not targeting targetCharacter, self, or a dead targetCharacter
            if(targetCharacter == null || targetCharacter == aiCharacter || targetCharacter.isDead) {
                continue;
            }

            //Can I attack this character based on their Faction? If so, make them my target
            if(WorldUtilityManager.instance.CanIAttackThisTarget(aiCharacter.faction, targetCharacter.faction)) {

                //If a potential target is found, it has to be in front of us
                Vector3 targetsDirection = targetCharacter.transform.position - aiCharacter.transform.position;
                float angleOfPotentialTarget = Vector3.Angle(targetsDirection, aiCharacter.transform.forward);

                if(angleOfPotentialTarget > minimumDetectionAngle && angleOfPotentialTarget < maximumDetectionAngle) {

                    //Check if the environment blocks sight to the target
                    if(Physics.Linecast(aiCharacter.characterCombatManager.LockOnTransform.position, 
                     targetCharacter.characterCombatManager.LockOnTransform.position, 
                     WorldUtilityManager.instance.GetEnvironmentLayers())) {
                        //blocked
                        Debug.DrawLine(aiCharacter.characterCombatManager.LockOnTransform.position, targetCharacter.characterCombatManager.LockOnTransform.position);
                    }
                    else {
                        targetsDirection = targetCharacter.transform.position - transform.position;
                        viewableAngle = WorldUtilityManager.instance.GetAngleOfTarget(transform, targetsDirection);
                        aiCharacter.characterCombatManager.SetTarget(targetCharacter);
                        PivotTowardsTarget(aiCharacter);
                    }
                }
            }
        }
    }

    public void AggroPlayer(GameObject player) {
        character.isLockedOn = true;
        LockOnTransform = player.transform;
        SetTarget(player.GetComponent<CharacterManager>());
    }

    public void PivotTowardsTarget(AICharacterManager aiCharacter) {
        //Play a Pivot Animation depending on the Viewable Angle of Target
        if (aiCharacter.isPerformingAction) {
            return;
        }

        //Note: Commented out version is for having specific angled animations (e.g. Turn_Right_45) like in the tutorial episode 37
        //if (viewableAngle >= 20 && viewableAngle <= 60) {
        //     aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("AI_Main_Turn_Right_01", true);
        // }
        
        //Note: Commented out version is for having specific angled animations (e.g. Turn_Right_45) like in the tutorial episode 37
        // else if (viewableAngle <= -20 && viewableAngle >= -60) {
        //     aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("AI_Main_Turn_Left_01", true);
        // }
    }

    public void HandleActionRecovery(AICharacterManager aiCharacter) {
        if (actionRecoveryTimer > 0 && !aiCharacter.isPerformingAction) {
            actionRecoveryTimer -= Time.deltaTime;
        }
    }

    public void RotateTowardsAgent(AICharacterManager aiCharacter) {
        if (aiCharacter.isMoving) {
            aiCharacter.transform.rotation = aiCharacter.navMeshAgent.transform.rotation;
        }
    }

    public void RotateTowardsTargetWhileAttacking(AICharacterManager aiCharacter) {
        if (currentTarget == null) {
            return;
        }

        //1. Check if we can rotate
        if (!aiCharacter.canRotate) {
            return;
        }

        if (!aiCharacter.isPerformingAction) {
            return;
        }

        //2. Rotate towards the target at a specific rotation speed during specified frames
        Vector3 targetDirection = currentTarget.transform.position - aiCharacter.transform.position;
        targetDirection.y = 0;
        targetDirection.Normalize();

        if (targetDirection == Vector3.zero) {
            targetDirection = aiCharacter.transform.forward;
        }

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

        aiCharacter.transform.rotation = Quaternion.Slerp(aiCharacter.transform.rotation, targetRotation, attackRotationSpeed * Time.deltaTime);
    }

}