using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character Effects/Instant Effects/Poison On Hit Effect")]
public class PoisonTraitOnHitEffect : OnHitEffect
{
    public PoisonTimedEffect poisonEffect;

    public override void ProcessEffect(CharacterManager target)
    {

        //Debug.Log("" + poisonEffect.effectId + " hitDamage:" + hitDamage);
        //calculate damage per tick
        float numberOfTicks = timedEffect.startingDuration / poisonEffect.interval;
        float damagePerTick = hitDamage / numberOfTicks;
        //TakeHealthDamageCharacterEffect takeDamageEffect = Instantiate(WorldCharacterEffectsManager.instance.takeHealthDamageEffect);
        //takeDamageEffect.physicalDamage = damagePerTick;
        if (!timedEffect.stackable)
        { // check for duplicates
            foreach (ActiveCharacterEffect eff in target.characterEffectsManager.activeTimedEffects)
            {
                if (eff.effect.effectId.Equals(timedEffect.effectId))
                {
                    // refresh duration
                    eff.remainingDuration = eff.effect.startingDuration;
                    // return without adding effect again
                    return;
                }
            }
        }
        /* Add timed effect */
        target.characterEffectsManager.activeTimedEffects.Add(poisonEffect.ActiveEffect(damagePerTick));
    }
}
