using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldMusicController : MonoBehaviour
{
    public static WorldMusicController instance;
    private AudioSource audioSource;

    [Header("Music")]
    public AudioClip titleScreenTheme01;
    public AudioClip dungeonTheme01;
    public AudioClip overworldTheme01;

    // Start is called before the first frame update
    void Start() {
        audioSource = GetComponent<AudioSource>();
        DontDestroyOnLoad(gameObject);
        PlayAdvancedMusic(titleScreenTheme01, 0.25f); 
    }

    private void OnEnable() {
        SceneManager.activeSceneChanged += OnSceneChange;
    }
    private void Awake() {
        if (instance == null) {
            instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }

    private void OnDestroy() {
        //If we destroy this object, we unsubcribe from this event
        //This is to do with subscribing and might require research
        SceneManager.activeSceneChanged -= OnSceneChange;
    }

    public void PlayAdvancedMusic(AudioClip soundFX, float volume = 1f, float pitch = 1f, bool loop = true, bool randomizePitch = false, float pitchRandomRange = 0.1f, bool canOverlap = false) {
        //Avoids duplicate sounds playing over each other, unless canOverlap is true
        if (audioSource != null && (canOverlap || audioSource.clip != soundFX)) {
            //Modify AudioSource by Parameters
            audioSource.clip = soundFX;
            audioSource.loop = loop;
            audioSource.volume = volume;
            audioSource.pitch = pitch;

            //Set Randomized Pitch if setting enabled
            if (randomizePitch) {
                audioSource.pitch += Random.Range(-pitchRandomRange, pitchRandomRange);
            }

            //Play the music now that its settings have been set
            audioSource.Play();
        }
    }

    private void OnSceneChange(Scene oldScene, Scene newScene) {
        switch (newScene.name)
        {
            case "TitleScreen":
                PlayAdvancedMusic(titleScreenTheme01, 0.25f); 
                return;
            case "JerryDev":
                PlayAdvancedMusic(dungeonTheme01, 0.5f); 
                return;
            case "AlecDev":
                PlayAdvancedMusic(overworldTheme01); 
                return;  
            case "JacobDev":
                return;  
            default: 
                return;
        }
    }
}
