using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UndergroundAmbienceController : MonoBehaviour
{
    private AudioSource audioSource;

    void Awake() {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = WorldSoundFXManager.instance.caveAmbience;
        audioSource.loop = true;
        audioSource.Play();
    }
}
