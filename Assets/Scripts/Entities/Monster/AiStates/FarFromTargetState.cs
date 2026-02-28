using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName ="A.I./States/Far From Target")]

public class FarFromTargetState : AIState
{
    public override AIState Tick(AICharacterManager aiCharacter)
    {
        //Check if we're performing an action. If so, then do nothing until the action is finshed
        if (aiCharacter.isPerformingAction) {
            return this;
        }

        //If we have no target, then return to the Idle State
        if (aiCharacter.aiCharacterCombatManager.currentTarget == null) {
            //Reset Animation Speed to Idle Speed
            aiCharacter.animator.speed = aiCharacter.aiCharacterCombatManager.AIIdleAnimationSpeedModifier;

            return SwitchState(aiCharacter, aiCharacter.idleState);
        }

        //Make sure our navmesh is active. if not, then enable it
        if(aiCharacter.navMeshAgent && !aiCharacter.navMeshAgent.enabled) {
            aiCharacter.navMeshAgent.enabled = true;
        }

        //If our target is outside of our field of view, pivot to face them
        if (aiCharacter.aiCharacterCombatManager.viewableAngle < aiCharacter.aiCharacterCombatManager.minimumDetectionAngle 
         || aiCharacter.aiCharacterCombatManager.viewableAngle > aiCharacter.aiCharacterCombatManager.maximumDetectionAngle) {
            aiCharacter.aiCharacterCombatManager.PivotTowardsTarget(aiCharacter);
        }

        aiCharacter.aiCharacterLocomotionManager.RotateTowardsAgent(aiCharacter);

    if (aiCharacter.navMeshAgent)
        {
            //Dynamically begin sprinting if the target gets out of your range or not
            if (aiCharacter.aiCharacterCombatManager.canRun)
            {
                //Set Animation Speed to AI's Running Speed
                aiCharacter.animator.speed = aiCharacter.aiCharacterCombatManager.AIRunningSpeedModifier;

                aiCharacter.BeginRunningAtTarget();
            }

            //Option 02 - Only use for melee enemies, will use a different approach for ranged enemies
            if (aiCharacter.aiCharacterCombatManager.distanceFromTarget <= aiCharacter.navMeshAgent.stoppingDistance) {

                //Reset AI's animation speed to their attack speed modifier
                aiCharacter.animator.speed = aiCharacter.aiCharacterCombatManager.AIAttackSpeedModifier;

                return SwitchState(aiCharacter, aiCharacter.combatStanceState);
            }

            NavMeshPath path = new NavMeshPath();
            aiCharacter.navMeshAgent.CalculatePath(aiCharacter.aiCharacterCombatManager.currentTarget.transform.position, path);
            aiCharacter.navMeshAgent.SetPath(path);
        }

        return this;
    }
}
