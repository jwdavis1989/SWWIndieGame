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
        audioSource.loop = true;
        audioSource.Play();
    }
}
