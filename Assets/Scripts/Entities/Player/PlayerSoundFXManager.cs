using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundFXManager : CharacterSoundFXManager
{
    public AudioSource sprintBoosterAudioSource;

    protected override void Start()
    {
        base.Start();

        sprintBoosterAudioSource = GameObject.Find("BoosterSFXManager").GetComponent<AudioSource>();

        //TODO: Initialize all of the audio emitter components for the booster system to future proof booster audio system.
    }

    public override void PlaySprintBoostSoundFX()
    {
        //FUTURE NOTE: This sound effect is actually being managed by enabling audio emitters on Back Boosters skeleton child, which are hardcoded values. 
        //Could be upgrade later to be more dynamic.


        //AudioClip rollSFX = WorldSoundFXManager.instance.ChooseRandomSFXFromArray(WorldSoundFXManager.instance.rollSFX);
        // WorldSoundFXManager.instance.PlayAdvancedSoundFX(sprintBoosterAudioSource, rollSFX, 0.6f, 0.9f, true, 0.1f, false);
        // WorldSoundFXManager.instance.PlayAdvancedSoundFX(sprintBoosterAudioSource, rollSFX, 0.6f, 0.5f, true, 0.1f, false);
        //WorldSoundFXManager.instance.PlayAdvancedSoundFX(sprintBoosterAudioSource, WorldSoundFXManager.instance.jumpJetAirSFX, 1f, 0.25f, true, 0.1f, true);

        //WorldSoundFXManager.instance.ActivateAdvancedSoundFXComponent(sprintBoosterAudioSource, WorldSoundFXManager.instance.jumpJetAirSFX, 1f, 0.25f, true, 0.1f);
    }

    public override void PlayJumpJetBurstFX()
    {
        //FUTURE NOTE: This sound effect is actually being managed by enabling audio emitters on Each Side Boosters skeleton child, which are hardcoded values. 
        //Could be upgrade later to be more dynamic.
        if (characterManager.isPlayer)
        {
            //WorldSoundFXManager.instance.PlayAdvancedSoundFX(sprintBoosterAudioSource, WorldSoundFXManager.instance.jumpJetAirSFX, 1.25f, 1.2f, true);
        }
    }

    public override void PlayAirDashSoundFX()
    {
        //FUTURE NOTE: This sound effect is actually being managed by enabling audio emitters on the Air Dash Boosters skeleton child, which are hardcoded values. 
        //Could be upgrade later to be more dynamic.


        // AudioClip rollSFX = WorldSoundFXManager.instance.ChooseRandomSFXFromArray(WorldSoundFXManager.instance.rollSFX);
        // WorldSoundFXManager.instance.PlayAdvancedSoundFX(sprintBoosterAudioSource, rollSFX, 1.5f, 0.9f, true, 0.1f);
        // WorldSoundFXManager.instance.PlayAdvancedSoundFX(sprintBoosterAudioSource, rollSFX, 1.5f, 0.5f, true, 0.1f);
        // Debug.Log("PlayerAir");
    }

    public void StopSprintBoosterAudioClip()
    {
        if (sprintBoosterAudioSource.enabled == true)
        {   
            WorldSoundFXManager.instance.DisableAdvancedSoundFXComponent(sprintBoosterAudioSource);
        }
    }

}
