using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "A.I./States/Far From Target")]

public class FarFromTargetState : AIState
{
    [Header("Engagement Distance")]
    [SerializeField] public float maximumEngagementDistance = 5f;    //Distance at which monster switches back to pursue target state

    public override AIState Tick(AICharacterManager aiCharacter)
    {
        //Check if we're performing an action. If so, then do nothing until the action is finshed
        if (aiCharacter.isPerformingAction)
        {
            return this;
        }
        AiCharacterCombatManager aiCharacterCombatManager = aiCharacter.aiCharacterCombatManager;
        //If we have no target, then return to the Idle State
        if (aiCharacterCombatManager.currentTarget == null)
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
            //Dynamically begin sprinting if the target gets out of your range or not
            if (aiCharacterCombatManager.canRun)
            {
                //Set Animation Speed to AI's Running Speed
                aiCharacterCombatManager.SetSprintingSpeed(aiCharacter);

                aiCharacter.BeginRunningAtTarget();
            }

            //Old method, where creature immediately attacks once they get close while running
            // if (aiCharacterCombatManager.distanceFromTarget <= aiCharacter.navMeshAgent.stoppingDistance)
            // {

            //     //Reset AI's animation speed to their attack speed modifier
            //     aiCharacterCombatManager.SetAttackSpeed(aiCharacter);

            //     return SwitchState(aiCharacter, aiCharacter.combatStanceState);
            // }

            //If inside combat engagement range, switch to pursue target state
            if (aiCharacterCombatManager.distanceFromTarget < maximumEngagementDistance)
            {

                //Set Animation Speed to AI's Movement Speed
                aiCharacterCombatManager.SetBasicSpeed(aiCharacter);
                return SwitchState(aiCharacter, aiCharacter.pursueTargetState);
            }

            NavMeshPath path = new NavMeshPath();
            aiCharacter.navMeshAgent.CalculatePath(aiCharacterCombatManager.currentTarget.transform.position, path);
            aiCharacter.navMeshAgent.SetPath(path);
        }

        return this;
    }
}
