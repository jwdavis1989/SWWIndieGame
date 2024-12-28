using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIState : ScriptableObject
{
    public virtual AIState Tick(AICharacterManager enemy)
    {
        Debug.Log("Tick");
        return this;
    }
    protected virtual AIState SwitchState(AICharacterManager ai, AIState newState)
    {
        ResetStateFlags(ai);
        return newState;
    }
    protected virtual void ResetStateFlags(AICharacterManager ai)
    {
        //todo
    }
}
