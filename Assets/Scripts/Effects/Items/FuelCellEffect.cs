using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Item Effects/Fuel Cell Effect")]

public class FuelCellEffect : ItemEffect
{
    //public float amoutHealed = 0f;

    [Header("Sound Effect")]
    public AudioClip healSFX;
    //public GameObject healEffect;//could spawn a heal effect prefab?

    public override void ProcessEffect(CharacterManager character)
    {
        DungeonManager.healingItemUsed = true;
        //base.ProcessEffect(character);

        //Set new fuel
        CharacterStatsManager characterStats = character.characterStatsManager;
        //float newHp = Mathf.Min(characterStats.currentHealth + damageHealed, characterStats.maxHealth);
        //characterStats.currentHealth = newHp;

        //Play damage sound FX
        PlayHealSFX(character);
        //Play Damage VFX
        PlayHealVFX(character);
    }
    private void PlayHealVFX(CharacterManager character)
    {

    }

    private void PlayHealSFX(CharacterManager damagedCharacter)
    {
        if (healSFX != null)
        {
            damagedCharacter.characterSoundFXManager.PlayAdvancedSoundFX(healSFX);
        }
    }
}
