using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "A.I./States/Idle")]

public class IdleState : AIState
{
    public override AIState Tick(AICharacterManager aiCharacter) {
        //Case: Target Aquired
        if(aiCharacter.aiCharacterCombatManager.currentTarget != null) {
            //Turn on the enemy's Minimap Triangle if it's not already visible
            if (aiCharacter.miniMapSprite != null && !aiCharacter.isDead) {
                aiCharacter.miniMapSprite.SetActive(true);
            }

            //Changes state to the pursue target state
            return SwitchState(aiCharacter, aiCharacter.pursueTargetState);
        }
        //Case: Target still being searched for
        else {
            //Continue searching for a new target
            aiCharacter.aiCharacterCombatManager.FindATargetWithInLineOSight(aiCharacter);
            
            return this;

        }
        
        //return this;
    }

    
}
