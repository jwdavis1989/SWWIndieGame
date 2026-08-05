using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveCharacterEffect
{
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
        }
        // Decrement duration
        remainingDuration -= Time.deltaTime;
        if (remainingDuration <= 0 && !finished)
        { // Complete effect
            finished = true;
            effect.OnEffectFinish(character);
        }
    }
}
