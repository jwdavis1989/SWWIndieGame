using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "A.I/State/Idle")]
public class IdleState : AIState
{
    public override AIState Tick(AICharacterManager ai)
    {
        if(ai.aiCombatManager.currentTarget != null)
        {
            ai.pursuingTarget = true;
            //TODO return PursueTargetState;
        }
        else
        {
            ai.aiCombatManager.FindATargetWitLineOSight(ai);
            return this;

        }
        return this;
    }
}
