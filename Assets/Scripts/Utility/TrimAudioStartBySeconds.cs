using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrimAudioStartBySeconds : MonoBehaviour
{
    private AudioSource audioSource;
    public float TrimBeginningInSeconds = 0f;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Awake()
    {
        audioSource.time = TrimBeginningInSeconds;
    }
}
