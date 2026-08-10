using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character Effects/Timed Effects/Poison Effect")]
public class PoisonTimedEffect : TimedCharacterEffect
{
    public ApplyPoisonEffect applicator;
    public float interval = 1;
    public ActiveCharacterEffect ActiveEffect(float damage)
    {
        PoisonActiveEffect effect = new PoisonActiveEffect(this, startingDuration);
        effect.damageOnTick = damage;
        return effect;
    }
}
public class PoisonActiveEffect : ActiveCharacterEffect 
{
    public float damageOnTick;
    float timeSinceTick = 0;
    public PoisonTimedEffect poisonEffect;
    public PoisonActiveEffect(PoisonTimedEffect effect, float duration) : base(effect, duration)
    {
        poisonEffect = effect;
    }
    public override void TickEffect(CharacterManager character)
    {
        base.TickEffect(character);
        if (!finished)
        {
            timeSinceTick = timeSinceTick + Time.deltaTime;
            if (timeSinceTick > poisonEffect.interval)
            {
                CharacterStatsManager characterStatsManager = character.characterStatsManager;
                timeSinceTick -= poisonEffect.interval;
                characterStatsManager.currentHealth -= damageOnTick;
                if (character.isPlayer)
                {
                    PlayerUIManager.instance.playerUIHudManager.UpdateHealthBar(characterStatsManager.currentHealth, characterStatsManager.maxHealth);
                }
                Debug.Log("Tick Damage:" + damageOnTick);
            }
        }
    }
}

[CreateAssetMenu(menuName = "Character Effects/Instant Effects/Apply Poison Effect")]
public class ApplyPoisonEffect : ApplyTimedEffect
{
    public PoisonTimedEffect poisonEffect;
    public override void ProcessEffect(CharacterManager character)
    {
        CharacterManager target = character.characterCombatManager.currentTarget;
        float damage = 1f;
        if (character && character.characterCombatManager && character.characterCombatManager.currentWeaponBeingUsed)
        {
            damage = character.characterCombatManager.currentWeaponBeingUsed.stats.attack;
            damage /= (timedEffect.startingDuration + poisonEffect.interval);
        }
        //todo: check stackable
        /* Add timed effect */
        if (!timedEffect.stackable && target.characterEffectsManager.activeTimedEffects.Exists(
                (eff) => eff.effect.effectId == timedEffect.effectId))
        { // Non-Stackable so don't process
            return;
        }
        target.characterEffectsManager.activeTimedEffects.Add(poisonEffect.ActiveEffect(damage));
    }
}