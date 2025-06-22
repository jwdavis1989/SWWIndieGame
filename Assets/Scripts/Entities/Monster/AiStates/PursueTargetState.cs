using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName ="A.I./States/Pursue Target")]

public class PursueTargetState : AIState
{
    public override AIState Tick(AICharacterManager aiCharacter)
    {
        //Check if we're performing an action. If so, then do nothing until the action is finshed
        if (aiCharacter.isPerformingAction) {
            return this;
        }

        //If we have no target, then return to the Idle State
        if (aiCharacter.aiCharacterCombatManager.currentTarget == null) {
            return SwitchState(aiCharacter, aiCharacter.idleState);
        }

        //Make sure our navmesh is active. if not, then enable it
        if(!aiCharacter.navMeshAgent.enabled) {
            aiCharacter.navMeshAgent.enabled = true;
        }

        //If our target is outside of our field of view, pivot to face them
        if (aiCharacter.aiCharacterCombatManager.viewableAngle < aiCharacter.aiCharacterCombatManager.minimumDetectionAngle 
         || aiCharacter.aiCharacterCombatManager.viewableAngle > aiCharacter.aiCharacterCombatManager.maximumDetectionAngle) {
            aiCharacter.aiCharacterCombatManager.PivotTowardsTarget(aiCharacter);
        }

        aiCharacter.aiCharacterLocomotionManager.RotateTowardsAgent(aiCharacter);

        //If we are in combat range of the target, switch to Combat Stance State
        //Option 01
        // if(aiCharacter.aiCharacterCombatManager.distanceFromTarget <= aiCharacter.combatStanceState.maximumEngagementDistance) {
        //     return SwitchState(aiCharacter, aiCharacter.combatStanceState);
        // }

        //Option 02 - Only use for melee enemies, will use a different approach for ranged enemies
        if(aiCharacter.aiCharacterCombatManager.distanceFromTarget <= aiCharacter.navMeshAgent.stoppingDistance) {
            return SwitchState(aiCharacter, aiCharacter.combatStanceState);
        }

        //if target is not reachable/far return home

        //Pursue the Target
        //Option 1: Better performance, Asynchronous, might not always work
        //aiCharacter.navMeshAgent.SetDestination(aiCharacter.aiCharacterCombatManager.currentTarget.transform.position);

        //Option 2: Worse Performance, guaranteed to work, tutorial cites ~60 characters using it simultaneously with no noticible performance drop
        NavMeshPath path = new NavMeshPath();
        aiCharacter.navMeshAgent.CalculatePath(aiCharacter.aiCharacterCombatManager.currentTarget.transform.position, path);
        aiCharacter.navMeshAgent.SetPath(path);

        return this;
    }
}
