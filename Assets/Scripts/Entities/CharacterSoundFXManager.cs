using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSoundFXManager : MonoBehaviour
{
    private CharacterManager characterManager;
    private AudioSource audioSource;
    private int footStepSFXCount;


    protected virtual void Awake() {
        characterManager = GetComponent<CharacterManager>();
        audioSource = GetComponent<AudioSource>();
        footStepSFXCount = WorldSoundFXManager.instance.walkFootStepSFX.Length;
    }

    public void PlayRollSoundFX() {
        audioSource.PlayOneShot(WorldSoundFXManager.instance.rollSFX);
    }

    public void PlayJumpJetBurstFX() {
        audioSource.PlayOneShot(WorldSoundFXManager.instance.rollSFX);
    }

     public void PlayFootStepSFX(){
        if(characterManager.isGrounded && !characterManager.isPerformingAction){
            if(characterManager.isSprinting){
                audioSource.PlayOneShot(WorldSoundFXManager.instance.walkFootStepSFX[Random.Range(0 , footStepSFXCount)]);
            }
           audioSource.PlayOneShot(WorldSoundFXManager.instance.walkFootStepSFX[Random.Range(0 , footStepSFXCount)]);
        }
    }
}
