using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "A.I./States/HornHound Pursue Target")]

public class HornHoundPursueTargetState : PursueTargetState
{
    public override AIState Tick(AICharacterManager aiCharacter)
    {
        //Check if we're performing an action. If so, then do nothing until the action is finshed
        if (aiCharacter.isPerformingAction)
        {
            return this;
        }
        AiCharacterCombatManager aiCharacterCombatManager = aiCharacter.aiCharacterCombatManager;
        //If we have no target, then return to the Idle State
        if (aiCharacter.aiCharacterCombatManager.currentTarget == null)
        {
            //Reset Animation Speed to Idle Speed
            aiCharacterCombatManager.SetIdleSpeed(aiCharacter);

            return SwitchState(aiCharacter, aiCharacter.idleState);
        }

        //Make sure our navmesh is active. if not, then enable it
        if (aiCharacter.navMeshAgent && !aiCharacter.navMeshAgent.enabled)
        {
            aiCharacter.navMeshAgent.enabled = true;
        }

        //If our target is outside of our field of view, pivot to face them
        if (aiCharacterCombatManager.viewableAngle < aiCharacterCombatManager.minimumDetectionAngle
         || aiCharacterCombatManager.viewableAngle > aiCharacterCombatManager.maximumDetectionAngle)
        {
            aiCharacterCombatManager.PivotTowardsTarget(aiCharacter);
        }

        aiCharacter.aiCharacterLocomotionManager.RotateTowardsAgent(aiCharacter);

        //If Target is no longer present, return to the Idle State
        if (aiCharacterCombatManager.currentTarget == null) {
            //Reset Animation Speed to Idle Speed
            aiCharacterCombatManager.SetIdleSpeed(aiCharacter);
            return SwitchState(aiCharacter, aiCharacter.idleState);
        }

        if (aiCharacter.navMeshAgent)
        {
            if (aiCharacterCombatManager.CheckTargetFarRangeThreshold() && aiCharacter.farFromTargetState != null)
            {
                Debug.Log("New State: FarFromTarget");
                return SwitchState(aiCharacter, aiCharacter.farFromTargetState);
            }

            //Only use for melee enemies, will use a different approach for ranged enemies
            if (aiCharacter.aiCharacterCombatManager.distanceFromTarget <= aiCharacter.navMeshAgent.stoppingDistance)
            {
                //Calculate if the HornHound is behind its target
                // Vector3 directionToAI = (aiCharacter.transform.position - aiCharacter.aiCharacterCombatManager.currentTarget.transform.position).normalized;
                // float dotProduct = Vector3.Dot(aiCharacter.aiCharacterCombatManager.currentTarget.transform.forward, directionToAI);

                // if (dotProduct < 0)
                // {
                    //Reset AI's animation speed to their attack speed modifier
                    aiCharacterCombatManager.SetAttackSpeed(aiCharacter);

                    
                    Debug.Log("New State: Combat");
                    return SwitchState(aiCharacter, aiCharacter.combatStanceState);
                // }
                // else
                // {
                    
                //     Debug.Log("New State: Flanking");
                //     aiCharacter.BeginFlankingAndRunningAtTarget();
                // }
            }
            // else
            // {
            //     Debug.Log("New State: Flanking");
            //     aiCharacter.BeginFlankingAndRunningAtTarget();
            // }

            //if target is not reachable/far return home

            //Pursue the Target
            //Option 1: Better performance, Asynchronous, might not always work
            //aiCharacter.navMeshAgent.SetDestination(aiCharacter.aiCharacterCombatManager.currentTarget.transform.position);

            //Option 2: Worse Performance, guaranteed to work, tutorial cites ~60 characters using it simultaneously with no noticible performance drop
            NavMeshPath path = new NavMeshPath();
            aiCharacter.navMeshAgent.CalculatePath(aiCharacterCombatManager.currentTarget.transform.position, path);
            aiCharacter.navMeshAgent.SetPath(path);
        }

        return this;
    }
}
