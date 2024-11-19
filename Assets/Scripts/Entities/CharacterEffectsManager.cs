using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterEffectsManager : MonoBehaviour
{
    //Process Instant Effects (e.g. Take Damage, Healing)

    //Process Timed Effects (Poison, Builds-up)

    //Process Static Effects (Adding/Removing Buffs/Debuffs from equipped items)

    CharacterManager character;

    protected virtual void Awake() {
        character = GetComponent<CharacterManager>();
    }

    public virtual void ProcessInstantEffect(InstantCharacterEffect effect) {
        //Take in an effect
        //Process it
        effect.ProcessEffect(character);
    }
}
