using System.Linq;
using UnityEngine;
[CreateAssetMenu(menuName = "Character Effects/Instant Effects/Apply Timed Effect")]
public class ApplyTimedEffect : InstantCharacterEffect
{
    public TimedCharacterEffect timedEffect;
    public override void ProcessEffect(CharacterManager character)
    {
        //todo: check stackable
        /* Add timed effect */
        if (!timedEffect.stackable && character.characterEffectsManager.activeTimedEffects.Exists(
                (eff) => eff.effect.effectId == timedEffect.effectId))
        {
            // Non-Stackable so don't process
                return;
        }
        character.characterEffectsManager.activeTimedEffects.Add(timedEffect.ActiveEffect());
    }
}