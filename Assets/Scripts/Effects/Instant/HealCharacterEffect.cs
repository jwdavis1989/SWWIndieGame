using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character Effects/Instant Effects/Heal Character")]

public class HealCharacterEffect : ItemEffect
{
    public float damageHealed = 0f;

    [Header("Sound Effect")]
    public AudioClip healSFX;
    public GameObject healEffect;//could spawn a heal effect prefab?

    public override void ProcessEffect(CharacterManager character)
    {
        DungeonManager.healingItemUsed = true;
        //base.ProcessEffect(character);

        //Set new HP
        CharacterStatsManager characterStats = character.characterStatsManager;
        float newHp = Mathf.Min(characterStats.currentHealth + damageHealed, characterStats.maxHealth);
        characterStats.currentHealth = newHp;

        //Play damage sound FX
        PlayHealSFX(character);
        //Play Damage VFX
        PlayHealVFX(character);
    }
    private void PlayHealVFX(CharacterManager character)
    {
        if (healEffect != null)
        {
            Instantiate(healEffect);
        }
    }

    private void PlayHealSFX(CharacterManager damagedCharacter)
    {
        if (healSFX != null)
        {
            damagedCharacter.characterSoundFXManager.PlayAdvancedSoundFX(healSFX);
        }
    }
}
