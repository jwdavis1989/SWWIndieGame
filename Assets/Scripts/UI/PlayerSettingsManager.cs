using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSettingsManager : MonoBehaviour
{
    [Header("Player Settings Manager stores the settings chosen in the Options Menu")]
    PlayerSettings playerSettings = new PlayerSettings();
    public PlayerPrefs playerPrefs;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
[Serializable] public class PlayerSettings 
{
    public float volume;
    public float brightness;
    public bool inverted;
    public bool gamepad;//otherwise KB&M
}