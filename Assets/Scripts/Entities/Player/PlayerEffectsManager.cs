using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEffectsManager : CharacterEffectsManager
{
    [Header("Debug Delete Later")]
    [SerializeField] InstantCharacterEffect effectToTest;
    [SerializeField] bool ProcessEffect = false;

    private void Update() {
        if (ProcessEffect) {
            ProcessEffect = false;

            //Q: Why are we instantiating a copy of this?
            //A: So we never edit the original copy, for safety
            InstantCharacterEffect effect = Instantiate(effectToTest);
            ProcessInstantEffect(effect);
        }
    }
}
