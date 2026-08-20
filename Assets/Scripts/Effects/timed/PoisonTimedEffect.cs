using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character Effects/Timed Effects/Poison Effect")]
public class PoisonTimedEffect : TimedCharacterEffect
{
    public float interval = 1;
    public PoisonActiveEffect ActiveEffect(float damage)
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
            while (timeSinceTick >= poisonEffect.interval)
            {
                //Debug.Log("PoisonActiveEffect tick:" + effect.effectId);
                timeSinceTick -= poisonEffect.interval;
                character.ApplyDamage(damageOnTick, null, false, "green");
                //Debug.Log("Tick Damage:" + damageOnTick);
            }
        }
    }
}
