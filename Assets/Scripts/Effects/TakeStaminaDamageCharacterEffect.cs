using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character Effects/Instant Effects/Take Stamina Damage")]
public class TakeStaminaDamageCharacterEffect : InstantCharacterEffect
{
    public float staminaDamage;

    public override void ProcessEffect(CharacterManager character)
    {
        CalculateStaminaDamage(character);
    }

    private void CalculateStaminaDamage(CharacterManager character) {
        //Compared the base stamina damage against other player effects/modifiers
        //Change the value before subtracting/adding it
        //Play sound FX or VFX during 
        Debug.Log("Character is taking: " + staminaDamage + " Stamina Damage.");
        character.characterStatsManager.currentStamina -= staminaDamage;
    }
}
