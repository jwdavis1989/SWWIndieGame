using System.Linq;
using UnityEngine;
[CreateAssetMenu(menuName = "Character Effects/Instant Effects/On Hit Effect")]
public class OnHitEffect : InstantCharacterEffect
{
    public TimedCharacterEffect timedEffect;
    protected float hitDamage;
    public OnHitEffect Instantiate(float hitDamage){
        OnHitEffect rv = Instantiate(this);
        rv.hitDamage = hitDamage; 
        return rv;
    } 
    public override void ProcessEffect(CharacterManager target)
    {
        if (!timedEffect.stackable)
        { // check for duplicates
            foreach (ActiveCharacterEffect eff in target.characterEffectsManager.activeTimedEffects) {
                if (eff.effect.effectId.Equals(timedEffect.effectId)) {
                    // refresh duration
                    eff.remainingDuration = eff.effect.startingDuration;
                    // return without adding effect again
                    return;
                }
            }
        }
        /* Add timed effect */
        target.characterEffectsManager.activeTimedEffects.Add(timedEffect.ActiveEffect());
    }
}