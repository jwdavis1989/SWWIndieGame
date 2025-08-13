using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSoundFXManager : MonoBehaviour
{
    protected CharacterManager characterManager;
    public AudioSource audioSource;
    private int footStepSFXCount;
    private int runFootStepSFXCount;
    public float walkFootStepPitch = 1f;
    public float runFootStepPitch = 1.2f;
    private float currentFootStepPitch;
    private float lastFootStep;

    [SerializeField] protected AudioClip[] takeDamageGrunts;


    //protected virtual void Awake() {
    //I moved to Start because I kept getting an error with this loading before the WorldSoundFXManager - Alec 11/2/24
    protected virtual void Start()
    {
        characterManager = GetComponent<CharacterManager>();
        audioSource = GetComponent<AudioSource>();
        footStepSFXCount = WorldSoundFXManager.instance.walkFootStepSFX.Length;
        runFootStepSFXCount = WorldSoundFXManager.instance.runFootStepSFX.Length;
        currentFootStepPitch = walkFootStepPitch;
    }

    private void Update()
    {
        HandleFootStepSFX();
    }

    public void PlayAdvancedSoundFX(AudioClip soundFX, float volume = 1f, float pitch = 1f, bool randomizePitch = true, float pitchRandomRange = 0.1f, bool canOverlap = false)
    {
        if (canOverlap || audioSource.clip != soundFX)
        {
            audioSource.PlayOneShot(soundFX, volume);

            //Reset pitch from last time called
            audioSource.pitch = pitch;

            if (randomizePitch)
            {
                audioSource.pitch += Random.Range(-pitchRandomRange, pitchRandomRange);
            }

        }
    }

    public void PlayRollSoundFX()
    {
        AudioClip rollSFX = WorldSoundFXManager.instance.ChooseRandomSFXFromArray(WorldSoundFXManager.instance.rollSFX);
        PlayAdvancedSoundFX(rollSFX, 0.3f, 1f, true, 0.1f);
    }

    public virtual void PlayAirDashSoundFX()
    {
        //FUTURE NOTE: This sound effect is actually being managed by enabling audio emitters on the Air Dash Boosters skeleton child, which are hardcoded values. 
        //Could be upgrade later to be more dynamic.

        // AudioClip rollSFX = WorldSoundFXManager.instance.ChooseRandomSFXFromArray(WorldSoundFXManager.instance.rollSFX);
        // PlayAdvancedSoundFX(rollSFX, 1.5f, 0.9f, true, 0.1f);
        // PlayAdvancedSoundFX(rollSFX, 1.5f, 0.5f, true, 0.1f);
    }

    public virtual void PlaySprintBoostSoundFX()
    {

        //FUTURE NOTE: This sound effect is actually being managed by enabling audio emitters on the Sprint Dash Boosters skeleton child, which are hardcoded values. 
        //Could be upgrade later to be more dynamic.

        // AudioClip rollSFX = WorldSoundFXManager.instance.ChooseRandomSFXFromArray(WorldSoundFXManager.instance.rollSFX);
        // PlayAdvancedSoundFX(rollSFX, 0.6f, 0.9f, true, 0.1f, false);
        // PlayAdvancedSoundFX(rollSFX, 0.6f, 0.5f, true, 0.1f, false);
        // PlayAdvancedSoundFX(WorldSoundFXManager.instance.jumpJetAirSFX, 0.5f, 0.25f, true, 0.1f, false);
    }

    public virtual void PlayJumpJetBurstFX()
    {
        if (characterManager.isPlayer)
        {
            PlayAdvancedSoundFX(WorldSoundFXManager.instance.jumpJetAirSFX, 1.25f, 1.2f, true);
        }
    }

    public void PlayLandingSFX()
    {
        PlayAdvancedSoundFX(WorldSoundFXManager.instance.runFootStepSFX[Random.Range(0, runFootStepSFXCount)], 0.2f, 1f, true, 0.1f, true);
    }

    public void PlayBackPedalSFX()
    {
        //audioSource.PlayOneShot(WorldSoundFXManager.instance.backPedalSFX);
        PlayAdvancedSoundFX(WorldSoundFXManager.instance.runFootStepSFX[Random.Range(0, runFootStepSFXCount)], 0.1f, 1f, true, 0.1f, true);
    }

    public void PlayJumpFootStepSFX()
    {
        PlayAdvancedSoundFX(WorldSoundFXManager.instance.runFootStepSFX[Random.Range(0, runFootStepSFXCount)], 0.2f, 1.1f, true, 0.1f, false);
    }

    public void PlayFootStepSFX()
    {
        if (characterManager.isGrounded && !characterManager.isPerformingAction)
        {
            UpdateFootStepPitch();
            PlayAdvancedSoundFX(WorldSoundFXManager.instance.runFootStepSFX[Random.Range(0, runFootStepSFXCount)], 0.1f, currentFootStepPitch, true, 0.2f, true);
        }
    }

    public void UpdateFootStepPitch()
    {
        if (characterManager.isSprinting)
        {
            currentFootStepPitch = runFootStepPitch;
        }
        else
        {    
            currentFootStepPitch = walkFootStepPitch;
        }
    }

    public void HandleFootStepSFX()
    {
        var footStep = characterManager.animator.GetFloat("FootStepCurve");

        if ((lastFootStep > 0 && footStep < 0) || (lastFootStep < 0 && footStep > 0))
        {
            PlayFootStepSFX();
        }

        lastFootStep = footStep;
    }

    public virtual void PlayTakeDamageGrunts()
    {
        PlayAdvancedSoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(takeDamageGrunts));
    }

    public virtual void PlayBlockSoundFX()
    {
        PlayAdvancedSoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(WorldSoundFXManager.instance.blockedImpactSFX));
    }

    public virtual void PlayGuardBrokenSoundFX()
    {
        PlayAdvancedSoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(WorldSoundFXManager.instance.guardBrokenSFX), 1f, 1.75f);
    }

    public virtual void PlayPerfectGuardSFX()
    {
        PlayAdvancedSoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(WorldSoundFXManager.instance.perfectGuardFlourishSFX), 1f, 1f);
    }

}
