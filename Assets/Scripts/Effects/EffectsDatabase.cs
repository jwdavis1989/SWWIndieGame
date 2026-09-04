using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Character Effects/Effects Database")]
public class EffectsDatabase : ScriptableSingleton<EffectsDatabase>
{
    public List<TimedCharacterEffect> timedCharacterEffects = new List<TimedCharacterEffect>();


    private  Dictionary<string, TimedCharacterEffect> timedEffectLookup;
    public  void Initialize()
    {
        timedEffectLookup = new Dictionary<string, TimedCharacterEffect>();

        foreach (TimedCharacterEffect effect in timedCharacterEffects)
        {
            if (!timedEffectLookup.ContainsKey(effect.effectId.ToLower()))
            {
                timedEffectLookup.Add(effect.effectId.ToLower(), effect);
            }
            else
            {
                Debug.LogWarning($"Duplicate effectId: {effect.effectId}");
            }
        }
    }
    public  TimedCharacterEffect GetItemEffect(string effectId)
    {
        if (timedEffectLookup == null)
            Initialize();
        effectId = effectId.ToLower();//case insensitivity
        timedEffectLookup.TryGetValue(effectId, out var effect);
        return effect;
    }

}