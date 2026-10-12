using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAudioAfterDelayInSeconds : MonoBehaviour
{
    public AudioSource audioSource;
    public float delayInSeconds = 2.0f;

    void Awake()
    {
        if (!audioSource) {
            audioSource = GetComponent<AudioSource>();
        }
        audioSource.PlayDelayed(delayInSeconds);
    }
}
