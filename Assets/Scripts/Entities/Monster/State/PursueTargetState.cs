using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
[CreateAssetMenu(menuName ="A.I/States/Pursue Target")]
public class PursueTargetState : AIState
{
    public override AIState Tick(AICharacterManager ai)
    {
        //check performing action, if so do nothing until action is finshed
        if (ai.isPerformingAction)
            return this;

        //if no target go idle
        if (ai.aiCombatManager.currentTarget == null)
        {
            return SwitchState(ai, ai.idle);
        }

        //make sure our navmesh is active. if not enable it - ?
        if(!ai.navMeshAgent.enabled) 
            ai.navMeshAgent.enabled = true;
        //if in combat range of target switch to combat stance state
        //if(ai.atkRange)

        //if target is not reachable/far return home

        //pursue the target
        //option1
        //ai.navMeshAgent.SetDestination(ai.aiCombatManager.currentTarget.transform.position);
        //option2
        NavMeshPath path = new NavMeshPath();
        ai.navMeshAgent.CalculatePath(ai.aiCombatManager.currentTarget.transform.position, path);
        ai.navMeshAgent.SetPath(path);
        return this;
    }
}
