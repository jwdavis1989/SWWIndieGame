using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ExplosionVFXManager : MonoBehaviour
{
    private ParticleSystem particleSystem;
    private float particleDuration;

    // Start is called before the first frame update
    void Awake()
    {
        //Get the Particle System
        particleSystem = GetComponent<ParticleSystem>();

        //If Particle System not found on object, check its children
        if (particleSystem == null)
        {
            particleSystem = GetComponentInChildren<ParticleSystem>();
        }

        //Set the timer to the Particle System's Lifespan
        particleDuration = particleSystem.main.duration;

    }

    // Update is called once per frame
    void Update()
    {
        if (particleDuration > 0)
        {
            particleDuration -= Time.deltaTime;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

}
