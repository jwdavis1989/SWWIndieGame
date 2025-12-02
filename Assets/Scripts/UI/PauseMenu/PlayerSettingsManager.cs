using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlayerSettingsManager
{
    // Player Settings Manager provides api for save & load of player settings and stores loaded player settings
    public static PlayerSettings playerSettings = new PlayerSettings();

    private static string filePath = Path.Combine(
        Application.persistentDataPath,
        "settings.json"
    );
    public static PlayerPrefs playerPrefs;
    // Start is called before the first frame update
    public static void Load()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            playerSettings = JsonUtility.FromJson<PlayerSettings>(json);
        }
        else
        {
            // First run – create default settings
            playerSettings = new PlayerSettings();
            Save();
        }
    }
    public static void Save()
    {
        string json = JsonUtility.ToJson(playerSettings, true);
        File.WriteAllText(filePath, json);
        Debug.Log("Settings Saved to " + filePath);//astest
    }
}
[Serializable] public class PlayerSettings
{
    public bool inverted = true;//default to wrong
    public float volume;
    public float brightness;
    public bool gamepad;//otherwise KB&M
}