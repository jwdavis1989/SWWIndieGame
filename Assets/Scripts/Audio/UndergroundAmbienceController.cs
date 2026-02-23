using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UndergroundAmbienceController : MonoBehaviour
{
    private AudioSource audioSource;
    public float audioVolume = 0.5f;


    void Awake() {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = WorldSoundFXManager.instance.caveAmbience;
        audioSource.volume = audioVolume;
        //TODO IDK ABOUT THIS
        audioSource.volume = audioVolume * PlayerSettingsManager.instance.playerSettings.effectsVolume;
        audioSource.loop = true;
        audioSource.Play();
    }
}
