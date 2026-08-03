using UnityEngine;
[CreateAssetMenu(menuName = "Character Effects/Instant Effects/Apply Timed Effect")]
public class ApplyTimedEffect : InstantCharacterEffect
{
    public TimedCharacterEffect timedEffect;
    public override void ProcessEffect(CharacterManager character)
    {
        //todo: check stackable
        /* Add timed effect */
        character.characterEffectsManager.activeTimedEffects.Add(timedEffect.ActiveEffect());
    }
}