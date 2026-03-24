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
        Load();
    }
    // Player Settings Manager provides api for save & load of player settings and stores loaded player settings
    //    = Path.Combine(
    //    Application.persistentDataPath,
    //    "settings.json"
    //);
    public  PlayerPrefs playerPrefs;
    // Start is called before the first frame update
    public  void Load()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            playerSettings = JsonUtility.FromJson<PlayerSettings>(json);
            PlayerCamera.instance.isCameraInverted = playerSettings.inverted;
            mixer.SetFloat("MusicVolume", playerSettings.musicVolume);
            mixer.SetFloat("SFXVolume", playerSettings.musicVolume);
        }
        else
        {
            // First run – create default settings
            playerSettings = new PlayerSettings();
            Save();
        }
    }
    public void Save()
    {
        string json = JsonUtility.ToJson(playerSettings, true);
        File.WriteAllText(filePath, json);
        //Debug.Log("Settings Saved to " + filePath);//astest
    }
}
[Serializable] public class PlayerSettings
{
    public bool inverted = true;//default to wrong
    public float mainVolume;
    public float musicVolume;
    public float effectsVolume;
    public float brightness;
    public bool gamepad;//otherwise KB&M
}