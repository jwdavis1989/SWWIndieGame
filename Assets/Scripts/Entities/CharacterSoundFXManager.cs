using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSoundFXManager : MonoBehaviour
{
    private CharacterManager characterManager;
    public AudioSource audioSource;
    private int footStepSFXCount;


    //protected virtual void Awake() {
    //I moved to Start because I kept getting an error with this loading before the WorldSoundFXManager - Alec 11/2/24
    protected virtual void Start() { 
        characterManager = GetComponent<CharacterManager>();
        audioSource = GetComponent<AudioSource>();
        footStepSFXCount = WorldSoundFXManager.instance.walkFootStepSFX.Length;
    }
    public void PlayRollSoundFX() {
        audioSource.PlayOneShot(WorldSoundFXManager.instance.rollSFX);
    }

    public void PlayJumpJetBurstFX() {
        audioSource.PlayOneShot(WorldSoundFXManager.instance.jumpJetAirSFX);
    }

    public void PlayBackPedalSFX() {
        audioSource.PlayOneShot(WorldSoundFXManager.instance.backPedalSFX);
    }

    public void PlayFootStepSFX(){
        if(characterManager.isGrounded && !characterManager.isPerformingAction){
            audioSource.PlayOneShot(WorldSoundFXManager.instance.walkFootStepSFX[Random.Range(0 , footStepSFXCount)]);
        }
    }
}
