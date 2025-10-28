using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrimAudioStartBySeconds : MonoBehaviour
{
    public float TrimBeginningInSeconds = 0f;

    void OnEnable()
    {
        GetComponent<AudioSource>().time = TrimBeginningInSeconds;
    }
}
