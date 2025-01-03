using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIState : ScriptableObject
{
    public virtual AIState Tick(AICharacterManager aiCharacter) {
        Debug.Log("Tick");
        //Do some logic to find the player

        //If we have found the player, return the pursue target state instead

        //If we have not found the player, continue to return the idle state
        
        return this;
    }

    protected virtual AIState SwitchState(AICharacterManager aiCharacter, AIState newState) {
        ResetStateFlags(aiCharacter);
        return newState;
    }

    protected virtual void ResetStateFlags(AICharacterManager aiCharacter) {
        //Reset any state flags here, so when you return to the state, they are blank once again
    }


}
