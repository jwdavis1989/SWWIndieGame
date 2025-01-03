using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
[CreateAssetMenu(menuName ="A.I./States/Pursue Target")]
public class PursueTargetState : AIState
{
    public override AIState Tick(AICharacterManager aiCharacter)
    {
        //check performing action, if so do nothing until action is finshed
        if (aiCharacter.isPerformingAction)
            return this;

        //if no target, then go idle
        if (aiCharacter.aiCharacterCombatManager.currentTarget == null)
        {
            return SwitchState(aiCharacter, aiCharacter.idle);
        }

        //Make sure our navmesh is active. if not, then enable it
        if(!aiCharacter.navMeshAgent.enabled) 
            aiCharacter.navMeshAgent.enabled = true;
        //If we are in combat range of the target, switch to Combat Stance State
        //if(aiCharacter.atkRange)

        //if target is not reachable/far return home

        //Pursue the Target
        //option1
        //aiCharacter.navMeshAgent.SetDestination(aiCharacter.aiCharacterCombatManager.currentTarget.transform.position);
        //option2
        NavMeshPath path = new NavMeshPath();
        aiCharacter.navMeshAgent.CalculatePath(aiCharacter.aiCharacterCombatManager.currentTarget.transform.position, path);
        aiCharacter.navMeshAgent.SetPath(path);
        return this;
    }
}
