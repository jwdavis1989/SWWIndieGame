using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenuManager : MonoBehaviour
{
    public static OptionsMenuManager instance;
    [HideInInspector] public PlayerSettings playerSettings;
    [Header("Buttons, knobs, and switches")]
    public Toggle invertedToggle;
    public Slider musicVolumeSlider;
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
        invertedToggle.isOn = PlayerSettingsManager.instance.playerSettings.inverted;
    }
    // Start is called before the first frame update
    void Start()
    {
        if(musicVolumeSlider != null)
        {
            WorldMusicController.instance.GetComponent<AudioSource>().volume = musicVolumeSlider.value;
            musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChange);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public void LoadOptions()
    {
        playerSettings = PlayerSettingsManager.instance.playerSettings;
        Debug.Log("Loaded inverted as " + playerSettings.inverted);
        invertedToggle.isOn = playerSettings.inverted;
        musicVolumeSlider.value = playerSettings.musicVolume;
        WorldMusicController.instance.GetComponent<AudioSource>().volume = playerSettings.musicVolume;
    }

    public void OnInvertChange(bool newValue)
    {
        playerSettings.inverted = newValue;
        PlayerCamera.instance.isCameraInverted = newValue;
        PlayerSettingsManager.instance.playerSettings = playerSettings;
        Debug.Log("Saving inverted as " + playerSettings.inverted);
        PlayerSettingsManager.instance.Save();
    }
    public void OnMusicVolumeChange(float newValue)
    {
        WorldMusicController.instance.GetComponent<AudioSource>().volume = newValue;
        if (!recentlySave)
        {
            playerSettings.musicVolume = newValue;
            PlayerSettingsManager.instance.playerSettings = playerSettings;
            PlayerSettingsManager.instance.Save();
            SaveTimer();
        }
    }
    public bool recentlySave = false;
    public IEnumerator SaveTimer()
    {
        recentlySave = true;
        yield return new WaitForSeconds(1.5f);
        recentlySave = false;
    }

}
