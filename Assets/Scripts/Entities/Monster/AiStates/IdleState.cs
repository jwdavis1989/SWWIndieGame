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

            //Activate monster health bar
            aiCharacter.characterUIManager.ActivateHealthBar();

            aiCharacter.aiCharacterSoundFXManager.PlayAggroSFX();

            //Set Animation Speed to AI's Movement Speed
            aiCharacter.animator.speed = aiCharacter.aiCharacterCombatManager.AIMovementSpeedModifier;

            //Changes state to the pursue target state
            return SwitchState(aiCharacter, aiCharacter.pursueTargetState);
        }
        //Case: Target still being searched for
        else {
            //Reset NavMeshAgent to ensure accurate positioning
            aiCharacter.ResetNavMeshAgentPosition();

            //Continue searching for a new target
            aiCharacter.aiCharacterCombatManager.FindATargetWithInLineOSight(aiCharacter);
            
            return this;

        }
        
        //return this;
    }

    
}
