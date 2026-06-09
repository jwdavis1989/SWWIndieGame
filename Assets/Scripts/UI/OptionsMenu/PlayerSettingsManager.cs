using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Audio;

public class PlayerSettingsManager : MonoBehaviour
{
    [Header("PlayerSettingsManager stores and handles save/load of player settings")]
    public AudioMixer mixer;
    public static PlayerSettingsManager instance;
    private string filename = "playerSettings.json";
    public PlayerSettings playerSettings = new PlayerSettings();
    private string filePath;
    public void Awake()
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
    public void Start()
    {
        filePath = Path.Combine(Application.persistentDataPath, filename);
        LoadPlayerSettings();
    }
    // Player Settings Manager provides api for save & load of player settings and stores loaded player settings
    //    = Path.Combine(
    //    Application.persistentDataPath,
    //    "settings.json"
    //);
    public  PlayerPrefs playerPrefs;
    // Start is called before the first frame update
    public void LoadPlayerSettings()
    {
        if (File.Exists(filePath))
        {
            //Debug.Log("Loading Player Settings");
            string json = File.ReadAllText(filePath);
            playerSettings = JsonUtility.FromJson<PlayerSettings>(json);
        }
        else
        {
            //Debug.Log("Creating Player Settings");
            // First run – create default settings
            playerSettings = new PlayerSettings();
            playerSettings.inverted = false;
            playerSettings.mainVolume = 1f;
            playerSettings.musicVolume = 1f;
            playerSettings.effectsVolume = 1f;
            SavePlayerSettings();
        }
        PlayerCamera.instance.isCameraInverted = playerSettings.inverted;
        SetMainVolumeLogarithmic(playerSettings.mainVolume);
        SetMusicVolumeLogarithmic(playerSettings.musicVolume);
        SetSFXVolumeLogarithmic(playerSettings.effectsVolume);
    }
    public void SavePlayerSettings()
    {

        string json = JsonUtility.ToJson(playerSettings, true);
        File.WriteAllText(filePath, json);
        //Debug.Log("Settings Saved to " + filePath);//astest
    }
    public void SetMainVolumeLogarithmic(float sliderValue)
    {
        if(sliderValue > 1f) // cap volume in case of bad save data
            sliderValue = 1f;
        // Convert linear slider (0–1) to logarithmic dB scale
        float adjusted = Math.Max(sliderValue * sliderValue, 0.0001f);
        float volumeDb = Mathf.Log10(adjusted) * 20;
        mixer.SetFloat("MainVolume", volumeDb);
    }
    public void SetSFXVolumeLogarithmic(float sliderValue)
    {
        if (sliderValue > 1f) // cap volume in case of bad save data
            sliderValue = 1f;
        // Convert linear slider (0–1) to logarithmic dB scale
        float adjusted = Math.Max(sliderValue * sliderValue, 0.0001f);
        float volumeDb = Mathf.Log10(adjusted) * 20;
        mixer.SetFloat("SFXVolume", volumeDb);
    }
    public void SetMusicVolumeLogarithmic(float sliderValue)
    {
        if (sliderValue > 1f) // cap volume in case of bad save data
            sliderValue = 1f;
        // Convert linear slider (0–1) to logarithmic dB scale
        float adjusted = Math.Max(sliderValue * sliderValue, 0.0001f);
        float volumeDb = Mathf.Log10(adjusted) * 20;
        mixer.SetFloat("MusicVolume", volumeDb);
    }
    public static float GetSensitivity()
    {
        float sensitivity = instance.playerSettings.mouseSensitivity;
        if (sensitivity < 0.1f) 
            return 0.1f;
        return sensitivity;
    }
    public static void SetSensitivity(float val) { instance.playerSettings.mouseSensitivity = val; }
}
[Serializable] public class PlayerSettings
{
    //controls
    public bool gamepad;//otherwise KB&M
    public bool inverted = true;//default to wrong
    public float mouseSensitivity = 1.0f;
    //volume
    public float mainVolume;
    public float musicVolume;
    public float effectsVolume;
    //vfx
    public float brightness;
}