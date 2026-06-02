using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiCharacterSoundFXManager : CharacterSoundFXManager
{
    public AudioClip aggroSFX;
    public float aggroSFXVolume = 1f;
    public float aggroSFXPitch = 1f;

    public void PlayAggroSFX()
    {
        PlayAdvancedSoundFX(aggroSFX,aggroSFXVolume, aggroSFXPitch);
    }
    
}
