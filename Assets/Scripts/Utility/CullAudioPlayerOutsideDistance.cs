using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CullAudioPlayerOutsideDistance : MonoBehaviour
{
    [Header("Settings")]
    public float cullDistance = 20f;
    public float checkIntervalInSeconds = 1f; 

    private AudioSource[] audioSources;
    private Transform playerTransform;

    void Start()
    {
        //Stores an array of the character's AudioSources
        audioSources = GetComponentsInChildren<AudioSource>();
        
        //Checks based on main Camera for easy search
        if (Camera.main != null)
            playerTransform = Camera.main.transform;

        InvokeRepeating(nameof(CheckDistance), 0f, checkIntervalInSeconds);
    }

    void CheckDistance()
    {
        if (playerTransform == null) {
            return;
        }
        
        //Faster to check by squaring rather than needing to use the Square Root operation used by Vector3.Distance
        float distanceSquared = (transform.position - playerTransform.position).sqrMagnitude;
        float thresholdSquared = cullDistance * cullDistance;

        //Enable or disable all sources based on distance
        bool isCurrentAudioSourceCloseEnough = distanceSquared < thresholdSquared;
        
        foreach (var source in audioSources)
        {
            if (source.enabled != isCurrentAudioSourceCloseEnough)
            {
                source.enabled = isCurrentAudioSourceCloseEnough;
            }
        }
    }
}
