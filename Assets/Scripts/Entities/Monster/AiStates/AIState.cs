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

    protected virtual AIState SwitchState(AICharacterManager ai, AIState newState) {
        ResetStateFlags(ai);
        return newState;
    }

    protected virtual void ResetStateFlags(AICharacterManager ai) {
        //todo
    }


}
