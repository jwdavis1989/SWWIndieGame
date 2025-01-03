using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "A.I./States/Idle")]

public class IdleState : AIState
{
    public override AIState Tick(AICharacterManager aiCharacter) {
        //Case: Target Aquired
        if(aiCharacter.aiCharacterCombatManager.currentTarget != null) {
            Debug.Log("State Info: Target Aquired.");

            //Changes state to the pursue target state
            return SwitchState(aiCharacter, aiCharacter.pursueTarget);
        }
        //Case: Target still being searched for
        else {
            aiCharacter.aiCharacterCombatManager.FindATargetWithInLineOSight(aiCharacter);
            Debug.Log("State Info: Searching for a Target.");

            //Continue searching for a new target
            return this;

        }
        
        //return this;
    }

    
}
