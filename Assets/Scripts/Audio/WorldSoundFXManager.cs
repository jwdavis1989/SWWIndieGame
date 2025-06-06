using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSoundFXManager : MonoBehaviour
{
    public static WorldSoundFXManager instance;

    [Header("Melee Weapon Swing Sounds")]
    public AudioClip[] slashingWeaponSwingSFX;
    public AudioClip[] heavySlashingWeaponSwingSFX;
    public AudioClip[] bludgeoningWeaponSwingSFX;
    public AudioClip[] scytheWeaponSwingSFX;
    public AudioClip[] piercingWeaponSwingSFX;
    
    [Header("Damage Sounds")]
    public AudioClip[] slashingImpactSFX;
    public AudioClip[] bludgeoningImpactSFX;
    public AudioClip[] piercingImpactSFX;
    public AudioClip[] gunImpactSFX;
    public AudioClip[] explosionImpactSFX;
    public AudioClip[] fireImpactSFX;
    public AudioClip[] iceImpactSFX;
    public AudioClip[] windImpactSFX;
    public AudioClip[] lightningImpactSFX;
    public AudioClip[] lightImpactSFX;
    public AudioClip[] earthImpactSFX;

    [Header("Action Sounds")]
    public AudioClip[] rollSFX;
    public AudioClip jumpJetAirSFX;
    public AudioClip[] walkFootStepSFX;
    public AudioClip[] runFootStepSFX;
    public AudioClip backPedalSFX;

    [Header("Ambience")]
    public AudioClip caveAmbience;

    private void Awake() {
        if (instance == null) {
            instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }

    private void Start() {
        DontDestroyOnLoad(gameObject);
    }

    public AudioClip ChooseRandomSFXFromArray(AudioClip[] arrayOfAudioClips) {
        if (arrayOfAudioClips.Length > 0) {
            return arrayOfAudioClips[Random.Range(0, arrayOfAudioClips.Length)];
        }
        else {
            return null;
        }
    }
    
}
