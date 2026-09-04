using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedCharacterEffect : ScriptableObject
{
    [Header("TimedCharacterEffect is a Data which stores static data & effect ")]
    public string effectId;  
    public bool stackable = false;
    public float startingDuration;
    public virtual ActiveCharacterEffect ActiveEffect()
    {
        ActiveCharacterEffect effect = new ActiveCharacterEffect(this, startingDuration);
        return effect;
    }
    public virtual void OnEffectStart(CharacterManager character)
    { 
        // virtual method
    }
    public virtual void OnEffectTick(CharacterManager character)
    {
        // virtual method
    }
    public virtual void OnEffectFinish(CharacterManager character)
    {
        // virtual method
    }
}
