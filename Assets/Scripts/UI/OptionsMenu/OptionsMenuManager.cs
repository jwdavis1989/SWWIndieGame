using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenuManager : MonoBehaviour
{
    public static OptionsMenuManager instance;
    [HideInInspector] public PlayerSettings playerSettings;
    [Header("Buttons, knobs, and switches")]
    public Toggle invertedToggle;
    public Slider mainVolumeSlider;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            LoadOptions();
        }
        else
            Destroy(this);
    }
    private void OnEnable()
    {
        invertedToggle.isOn = PlayerSettingsManager.playerSettings.inverted;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public void LoadOptions()
    {
        playerSettings = PlayerSettingsManager.playerSettings;
        Debug.Log("Loaded inverted as " + playerSettings.inverted);
        invertedToggle.isOn = playerSettings.inverted;
    }

    public void OnInvertChange(bool newValue)
    {
        playerSettings.inverted = newValue;
        PlayerCamera.instance.isCameraInverted = newValue;
        PlayerSettingsManager.playerSettings = playerSettings;
        Debug.Log("Saving inverted as " + playerSettings.inverted);
        PlayerSettingsManager.Save();
    }

}
