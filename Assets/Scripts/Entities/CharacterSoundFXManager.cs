using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSoundFXManager : MonoBehaviour
{
    private CharacterManager characterManager;
    public AudioSource audioSource;
    private int footStepSFXCount;
    private int runFootStepSFXCount;

    [SerializeField] protected AudioClip[] takeDamageGrunts;


    //protected virtual void Awake() {
    //I moved to Start because I kept getting an error with this loading before the WorldSoundFXManager - Alec 11/2/24
    protected virtual void Start() { 
        characterManager = GetComponent<CharacterManager>();
        audioSource = GetComponent<AudioSource>();
        footStepSFXCount = WorldSoundFXManager.instance.walkFootStepSFX.Length;
        runFootStepSFXCount = WorldSoundFXManager.instance.runFootStepSFX.Length;
    }

    public void PlayAdvancedSoundFX(AudioClip soundFX, float volume = 1f, float pitch = 1f, bool randomizePitch = true, float pitchRandomRange = 0.1f, bool canOverlap = false) {
        if (canOverlap || audioSource.clip != soundFX) {
            audioSource.PlayOneShot(soundFX, volume);

            //Reset pitch from last time called
            audioSource.pitch = pitch;

            if (randomizePitch) {
                audioSource.pitch += Random.Range(-pitchRandomRange, pitchRandomRange);
            }

        }
    }

    public void PlayRollSoundFX() {
        AudioClip rollSFX = WorldSoundFXManager.instance.ChooseRandomSFXFromArray(WorldSoundFXManager.instance.rollSFX);
        PlayAdvancedSoundFX(rollSFX, 1, 1f, true, 0.1f);
    }

    public void PlayAirDashSoundFX() {
        AudioClip rollSFX = WorldSoundFXManager.instance.ChooseRandomSFXFromArray(WorldSoundFXManager.instance.rollSFX);
        PlayAdvancedSoundFX(rollSFX, 1.5f, 0.9f, true, 0.1f);
        PlayAdvancedSoundFX(rollSFX, 1.5f, 0.5f, true, 0.1f);
    }

    public void PlaySprintBoostSoundFX() {
        AudioClip rollSFX = WorldSoundFXManager.instance.ChooseRandomSFXFromArray(WorldSoundFXManager.instance.rollSFX);
        PlayAdvancedSoundFX(rollSFX, 0.6f, 0.9f, true, 0.1f, false);
        PlayAdvancedSoundFX(rollSFX, 0.6f, 0.5f, true, 0.1f, false);
        PlayAdvancedSoundFX(WorldSoundFXManager.instance.jumpJetAirSFX, 0.5f, 0.25f, true, 0.1f, false);
    }

    public void PlayJumpJetBurstFX() {
        if (characterManager.isPlayer) {
            PlayAdvancedSoundFX(WorldSoundFXManager.instance.jumpJetAirSFX, 1.25f, 1.2f, true);
        }
    }

    public void PlayLandingSFX() {
        PlayAdvancedSoundFX(WorldSoundFXManager.instance.runFootStepSFX[Random.Range(0 , runFootStepSFXCount)], 1f, 1f, true, 0.1f, true);
    }

    public void PlayBackPedalSFX() {
        //audioSource.PlayOneShot(WorldSoundFXManager.instance.backPedalSFX);
        PlayAdvancedSoundFX(WorldSoundFXManager.instance.runFootStepSFX[Random.Range(0 , runFootStepSFXCount)], 0.8f, 1f, true, 0.1f, true);
    }

    public void PlayJumpFootStepSFX(){
        PlayAdvancedSoundFX(WorldSoundFXManager.instance.runFootStepSFX[Random.Range(0 , runFootStepSFXCount)], 0.8f, 1.1f, true, 0.1f, false);
    }

    public void PlayFootStepSFX(){
        if(characterManager.isGrounded && !characterManager.isPerformingAction){
            //PlayAdvancedSoundFX(WorldSoundFXManager.instance.walkFootStepSFX[Random.Range(0 , footStepSFXCount)], 1f, 1f, true, 0.1f, true);
            PlayAdvancedSoundFX(WorldSoundFXManager.instance.runFootStepSFX[Random.Range(0 , runFootStepSFXCount)], 0.4f, 1.2f, true, 0.2f, true);
        }
    }

    public void PlayRunFootStepSFX(){
        if(characterManager.isGrounded && !characterManager.isPerformingAction){
            PlayAdvancedSoundFX(WorldSoundFXManager.instance.runFootStepSFX[Random.Range(0 , runFootStepSFXCount)], 1f, 1f, true, 0.1f, true);
        }
    }

    public virtual void PlayTakeDamageGrunts() {
        PlayAdvancedSoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(takeDamageGrunts));
    }

}
