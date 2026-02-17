using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiCharacterSoundFXManager : CharacterSoundFXManager
{
    public AudioClip aggroSFX;

    public void PlayAggroSFX()
    {
        PlayAdvancedSoundFX(aggroSFX);
    }
    
}
