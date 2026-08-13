using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character Effects/Instant Effects/Apply Poison Effect")]
public class PoisonOnHitEffect : ApplyTimedEffect
{


        public PoisonTimedEffect poisonEffect;
    public override void ProcessEffect(CharacterManager character)
    {
        //calculate damage per tick
        float damagePerTick = 1f;
        if (character && character.characterCombatManager && character.characterCombatManager.currentWeaponBeingUsed)
        {
            float totalDamage = character.characterCombatManager.currentWeaponBeingUsed.stats.attack;
            float numberOfTicks = timedEffect.startingDuration / poisonEffect.interval;
            damagePerTick = totalDamage / numberOfTicks;
        }
        //TakeHealthDamageCharacterEffect takeDamageEffect = Instantiate(WorldCharacterEffectsManager.instance.takeHealthDamageEffect);
        //takeDamageEffect.physicalDamage = damagePerTick;

        if (!timedEffect.stackable)
        { // check for duplicates
            foreach (ActiveCharacterEffect eff in character.characterEffectsManager.activeTimedEffects)
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
        character.characterEffectsManager.activeTimedEffects.Add(poisonEffect.ActiveEffect(damagePerTick));
    }
}
