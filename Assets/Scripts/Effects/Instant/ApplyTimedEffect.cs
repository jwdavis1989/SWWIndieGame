using System.Linq;
using UnityEngine;
[CreateAssetMenu(menuName = "Character Effects/Instant Effects/Apply Timed Effect")]
public class ApplyTimedEffect : InstantCharacterEffect
{
    public TimedCharacterEffect timedEffect;
    public override void ProcessEffect(CharacterManager character)
    {
        if (!timedEffect.stackable)
        { // check for duplicates
            foreach (ActiveCharacterEffect eff in character.characterEffectsManager.activeTimedEffects) {
                if (eff.effect.effectId.Equals(timedEffect.effectId)) {
                    // refresh duration
                    eff.remainingDuration = eff.effect.startingDuration;
                    // return without adding effect again
                    return;
                }
            }
        }
        /* Add timed effect */
        character.characterEffectsManager.activeTimedEffects.Add(timedEffect.ActiveEffect());
    }
}