using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ActiveCharacterEffect
{
    [Header("ActiveCharacterEffect is a simple object which tracks the life of an effect\n"
        +"Note: Added to save file. Not sure if necessary")]
    public TimedCharacterEffect effect;
    public float remainingDuration;
    [HideInInspector] public bool started = false;
    public bool finished = false;
    public ActiveCharacterEffect(TimedCharacterEffect effect, float duration)
    {
        this.effect = effect;
        remainingDuration = duration; 
    }
    public virtual void TickEffect(CharacterManager character)
    {
        if (!started)
        { // Start effect
            started = true;
            effect.OnEffectStart(character);
            //Debug.Log("Effect start:" + effect.effectId);
        }
        // Decrement duration
        remainingDuration -= Time.deltaTime;
        //tick
        effect.OnEffectTick(character);
        if (remainingDuration <= 0 && !finished)
        { // Complete effect
            finished = true;
            effect.OnEffectFinish(character);
            Debug.Log("Effect end:" + effect.effectId);
        }
    }
}
