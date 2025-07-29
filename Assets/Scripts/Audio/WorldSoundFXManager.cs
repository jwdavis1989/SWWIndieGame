using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSoundFXManager : MonoBehaviour
{
    public static WorldSoundFXManager instance;

    [Header("Melee Weapon Swing Sounds")]
    public AudioClip[] slashingWeaponSwingSFX;
    public AudioClip[] heavySlashingWeaponSwingSFX;
    public AudioClip[] bludgeoningWeaponSwingSFX;
    public AudioClip[] scytheWeaponSwingSFX;
    public AudioClip[] piercingWeaponSwingSFX;

    [Header("Damage Sounds")]
    public AudioClip[] slashingImpactSFX;
    public AudioClip[] bludgeoningImpactSFX;
    public AudioClip[] piercingImpactSFX;
    public AudioClip[] gunImpactSFX;
    public AudioClip[] explosionImpactSFX;
    public AudioClip[] fireImpactSFX;
    public AudioClip[] iceImpactSFX;
    public AudioClip[] windImpactSFX;
    public AudioClip[] lightningImpactSFX;
    public AudioClip[] lightImpactSFX;
    public AudioClip[] earthImpactSFX;
    public AudioClip[] blockedImpactSFX;
    public AudioClip[] guardBrokenSFX;
    public AudioClip[] perfectGuardFlourishSFX;

    [Header("Action Sounds")]
    public AudioClip[] rollSFX;
    public AudioClip jumpJetAirSFX;
    public AudioClip[] walkFootStepSFX;
    public AudioClip[] runFootStepSFX;
    public AudioClip backPedalSFX;

    [Header("Ambience")]
    public AudioClip caveAmbience;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public AudioClip ChooseRandomSFXFromArray(AudioClip[] arrayOfAudioClips)
    {
        if (arrayOfAudioClips.Length > 0)
        {
            return arrayOfAudioClips[Random.Range(0, arrayOfAudioClips.Length)];
        }
        else
        {
            return null;
        }
    }

    public void PlayAdvancedSoundFX(AudioSource audioSource, AudioClip soundFX, float volume = 1f, float pitch = 1f, bool randomizePitch = true, float pitchRandomRange = 0.1f, bool canOverlap = false)
    {
        if (canOverlap || audioSource.clip != soundFX)
        {
            audioSource.clip = soundFX;
            audioSource.PlayOneShot(soundFX, volume);

            //Reset pitch from last time called
            audioSource.pitch = pitch;

            if (randomizePitch)
            {
                audioSource.pitch += Random.Range(-pitchRandomRange, pitchRandomRange);
            }

            //StartCoroutine(StopSoundAfterDelay(audioSource));
        }
    }

    public void ActivateAdvancedSoundFXComponent(AudioSource audioSource, AudioClip soundFX, float volume = 1f, float pitch = 1f, bool randomizePitch = true, float pitchRandomRange = 0.1f)
    {
        audioSource.enabled = true;
        audioSource.clip = soundFX;
        audioSource.volume = volume;

        //Reset pitch from last time called
        audioSource.pitch = pitch;

        if (randomizePitch)
        {
            audioSource.pitch += Random.Range(-pitchRandomRange, pitchRandomRange);
        }
    }

    public void DisableAdvancedSoundFXComponent(AudioSource audioSource)
    {
        audioSource.enabled = false;
    }

    IEnumerator StopSoundAfterDelay(AudioSource audioSource)
    {
        yield return new WaitForSeconds(audioSource.clip.length);
        audioSource.Stop();
    }

}
