using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character Effects/Instant Effects/Heal Character")]

public class HealCharacterEffect : ItemEffect
{
    public float damageHealed = 0f;

    [Header("Sound Effect")]
    public bool willPlayHealSFX = true;
    public AudioClip healSFX;
    public GameObject healEffect;//could spawn a heal effect prefab?

    public override void ProcessEffect(CharacterManager character)
    {
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

    }

    private void PlayHealSFX(CharacterManager damagedCharacter)
    {
        AudioClip impactSFX;
        impactSFX = WorldSoundFXManager.instance.ChooseRandomSFXFromArray(WorldSoundFXManager.instance.rollSFX);
        damagedCharacter.characterSoundFXManager.PlayAdvancedSoundFX(impactSFX, 1, 1f, true, 0.1f);
    }
}
