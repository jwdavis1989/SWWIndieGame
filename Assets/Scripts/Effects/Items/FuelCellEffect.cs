using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Item Effects/Fuel Cell Effect")]

public class FuelCellEffect : ItemEffect
{
    [Header("Sound Effect")]
    public AudioClip healSFX;

    public override void ProcessEffect(CharacterManager character)
    {
        //base.ProcessEffect(character);
        //Set new fuel
        CharacterStatsManager characterStats = character.characterStatsManager;
        characterStats.currentFuel = characterStats.maxFuel;
        //Play sound FX
        PlayHealSFX(character);
    }

    private void PlayHealSFX(CharacterManager damagedCharacter)
    {
        if (healSFX != null)
        {
            damagedCharacter.characterSoundFXManager.PlayAdvancedSoundFX(healSFX);
        }
    }
}
