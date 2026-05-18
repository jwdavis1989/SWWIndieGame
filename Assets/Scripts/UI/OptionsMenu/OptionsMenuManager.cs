using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class OptionsMenuManager : MonoBehaviour
{
    public static OptionsMenuManager instance;
    [HideInInspector] public PlayerSettings playerSettings; 
    public AudioMixer mixer;
    
    [Header("Buttons, knobs, and switches")]
    public Toggle invertedToggle;
    public Slider mainVolumeSlider;
    public Slider effectsVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider mouseSensitivitySlider;

    [Header("Save window")]
    public GameObject saveWindow;
    public bool isChanged;
    public string saveWindowAction = "";

    [Header("Input - Must clone generic pause input for save window")]
    public EventSystem eventSystem;
    PlayerControls playerControls;
    [SerializeField] bool menuLeftInput = false;
    [SerializeField] bool menuRightInput = false;
    [SerializeField] bool exitPauseMenuInput = false;
    [SerializeField] bool saveSettingInput = false;
    [SerializeField] Vector2 scrollInput = new Vector2();
    //tooltips
    public List<GameObject> keyboardMouseTooltips;
    public List<GameObject> gamepadTooltips;

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
        if (playerControls == null)
        {
            playerControls = new PlayerControls();
            playerControls.OptionsMenu.SwitchMenuLeft.performed += i => menuLeftInput = true;
            playerControls.OptionsMenu.SwitchMenuRight.performed += i => menuRightInput = true;
            playerControls.OptionsMenu.ExitMenu.performed += i => exitPauseMenuInput = true;
            playerControls.OptionsMenu.SaveSettings.performed += i => saveSettingInput = true;
            playerControls.OptionsMenu.Scroll.performed += i => scrollInput = i.ReadValue<Vector2>();
            playerControls.Enable();
        }
        //todo: shouldnt be necessary? 
        invertedToggle.isOn = PlayerSettingsManager.instance.playerSettings.inverted;
        //make sure changed is reset
        isChanged = false;
        invertChanged = false;
        musicVolumeChanged = false;
        effectsVolumeChanged = false;
        saveWindow.SetActive(false);
        ToggleNavigation(true);
        //activate correct inputs
        if (playerControls != null)
        {
            playerControls.OptionsMenu.Enable();
            PauseScript.instance.playerControls.PauseMenu.Disable();
            PauseScript.instance.playerControls.UI.PauseButton.Disable();
        }
        // load tooltips
        if (InputSwitchDetector.IsCurrentlyGamepad())
        {
            foreach (GameObject gamepadeUI in gamepadTooltips)
                gamepadeUI.SetActive(true);
            foreach (GameObject kbmUI in keyboardMouseTooltips)
                kbmUI.SetActive(false);
        }
        else
        {
            foreach (GameObject gamepadeUI in gamepadTooltips)
                gamepadeUI.SetActive(false);
            foreach (GameObject kbmUI in keyboardMouseTooltips)
                kbmUI.SetActive(true);
        }
    }
    private void OnDisable()
    {
        isChanged = false;
        foreach (GameObject gamepadeUI in gamepadTooltips)
            gamepadeUI.SetActive(false);
        foreach (GameObject kbmUI in keyboardMouseTooltips)
            kbmUI.SetActive(false);
        SwapFromOptionMenuControls();
    }
    // Start is called before the first frame update
    void Start()
    {
        if (mainVolumeSlider != null)
            mainVolumeSlider.onValueChanged.AddListener(OnMainVolumeChange);
        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChange);
        }
        if (effectsVolumeSlider != null)
        {
            effectsVolumeSlider.onValueChanged.AddListener(OnEffectsVolumeChange);
        }
        if(mouseSensitivitySlider != null)
        {
            mouseSensitivitySlider.onValueChanged.AddListener(OnMouseSensitivityChanged);
        }
    }
    // Update is called once per frame
    void Update()
    {
        CheckControlsChanged();
        HandleGamePadSelected();
        HandleSwitchMenuInput();
        HandleSaveSettingsInput();
        HandleScrollInput();
    }
    private void LateUpdate()
    {
        HandleExitPauseMenuInput();
    }
    /***********************************************************************************************
     ********************************  I N P U T   H A N D L E R S  ********************************
     ***********************************************************************************************/
    void HandleScrollInput()
    {
        float scrollY = scrollInput.y;
        float scrollX = scrollInput.x;
        if(scrollX != 0)
        {

        }
        Debug.Log("ScrollX="+scrollX + " scrollY="+scrollY);
    }
    // Handles swapping between gamepad/keyboard
    private void CheckControlsChanged()
    {
        InputSwitchDetector inputSwitchDetector = InputSwitchDetector.instance;
        inputSwitchDetector.CheckControlsChanged();
        if (inputSwitchDetector.deviceChanged)
        {
            inputSwitchDetector.deviceChanged = false;
            if (InputSwitchDetector.IsCurrentlyGamepad())
            {
                //Show controller UI
                foreach (GameObject gamepadeUI in gamepadTooltips)
                    gamepadeUI.SetActive(true);
                foreach (GameObject kbmUI in keyboardMouseTooltips)
                    kbmUI.SetActive(false);
            }
            else //Keyboard
            {
                //Hide Controller UI
                foreach (GameObject gamepadeUI in gamepadTooltips)
                    gamepadeUI.SetActive(false);
                foreach (GameObject kbmUI in keyboardMouseTooltips)
                    kbmUI.SetActive(true);
            }
        }
    }
    // handles navigation updates
    void HandleGamePadSelected()
    {
        //TODO
        if (eventSystem.currentSelectedGameObject == null && InputSwitchDetector.IsCurrentlyGamepad())
        { //Handle Lost gamepad Cursor
            if (invertedToggle != null)
            {
                invertedToggle.Select();
            }
        }
    }
    public void HandleExitPauseMenuInput()
    {
        if (exitPauseMenuInput)
        {
            //Debug.Log("exitPauseMenuInput - options menu isChanged=" + isChanged);
            exitPauseMenuInput = false;
            saveWindowAction = "UNPAUSE";
            if (isChanged)
            {
                ActivateSaveWindow();
            }
            else
            {
                CompleteSaveWindowAction();
            }
        }
    }
    public void HandleSaveSettingsInput()
    {
        if (saveSettingInput)
        {
            saveSettingInput = false;
            saveWindowAction = "UNPAUSE";
            if (isChanged)
            {
                ActivateSaveWindow();
            }
        }
    }
    void HandleSwitchMenuInput()
    {
        bool switched = false;
        if (menuLeftInput)
        {
            menuLeftInput = false;
            saveWindowAction = "LEFT";
            switched = true;
        }
        if (menuRightInput)
        {
            menuRightInput = false;
            saveWindowAction = "RIGHT";
            switched = true;
        }
        if (switched)
        {
            if (isChanged)
            {
                ActivateSaveWindow();
            }
            else
            {
                CompleteSaveWindowAction();
            }
        }
    }
    // Save Window Exit
    public void CompleteSaveWindowAction()
    {
        //Debug.Log("CompleteSaveWindowAction - saveWindowAction=" + saveWindowAction);
        saveWindow.SetActive(false);
        SwapFromOptionMenuControls();
        switch (saveWindowAction)
        {
            case "UNPAUSE":
                PauseScript.instance.exitPauseMenuInput = true;
                PauseScript.instance.HandleExitPauseMenuInput(); 
                break;
            case "LEFT":
                PauseScript.instance.menuLeftInput = true;
                PauseScript.instance.HandleSwitchMenuInput(); 
                break;
            case "RIGHT":
                PauseScript.instance.menuRightInput = true;
                PauseScript.instance.HandleSwitchMenuInput();
                break;
            case "MENUCLICK":
                PauseScript.instance.GoToLastMenu();
                break;
            case "NONE":
                break;
            default:
                Debug.Log("Unrecognized saveWindowAction:" + saveWindowAction); break;
        }
    }
    // Save Window: Yes
    public void SaveThenCompleteSaveWindowAction()
    {
        if (mainVolumeChanged)
        {
            SaveMainVolume(mainVolume);
        }
        if (musicVolumeChanged)
        {
            SaveMusicVolume(musicVolume);
        }
        if (effectsVolumeChanged)
        {
            SaveEffectsVolume(effectsVolume);
        }
        if (invertChanged)
        {
            SaveInvert(inverted);
        }
        PlayerSettingsManager.instance.SavePlayerSettings();
        CompleteSaveWindowAction();
    }
    public void LoadOptions()
    {
        playerSettings = PlayerSettingsManager.instance.playerSettings;
        //Debug.Log("Loaded inverted as " + playerSettings.inverted);
        invertedToggle.isOn = playerSettings.inverted;
        mainVolumeSlider.value = playerSettings.mainVolume;
        musicVolumeSlider.value = playerSettings.musicVolume;
        effectsVolumeSlider.value = playerSettings.effectsVolume;
        if(mouseSensitivitySlider != null)
            mouseSensitivitySlider.value = playerSettings.mouseSensitivity;
        mixer.SetFloat("MainVolume", playerSettings.mainVolume);
        mixer.SetFloat("MusicVolume", playerSettings.musicVolume);
        mixer.SetFloat("SFXVolume", playerSettings.musicVolume);
    }

    bool invertChanged = false;
    bool inverted = false;
    public void OnInvertChange(bool newValue)
    {
        inverted = newValue;
        isChanged = true;
        invertChanged = true;
    }
    void SaveInvert(bool newValue)
    {
        playerSettings.inverted = newValue;
        PlayerCamera.instance.isCameraInverted = newValue;
        PlayerSettingsManager.instance.playerSettings = playerSettings;
        //Debug.Log("Saving inverted as " + playerSettings.inverted);
    }
    float mainVolume = 0;
    bool mainVolumeChanged = false;
    public void OnMainVolumeChange(float newValue)
    {
        if (mainVolume != newValue)
        {
            Debug.Log("OnMainVolumeChange:" + newValue);
            mainVolume = newValue;
            isChanged = true;
            mainVolumeChanged = true;
        }
    }
    float musicVolume = 0;
    bool musicVolumeChanged = false;
    public void OnMusicVolumeChange(float newValue)
    {
        if (musicVolume != newValue) {
            Debug.Log("OnMusicVolumeChange:" + newValue);
            musicVolume = newValue;
            isChanged = true;
            musicVolumeChanged = true;
        } 
    }
    float effectsVolume = 0;
    bool effectsVolumeChanged = false;
    public void OnEffectsVolumeChange(float newValue)
    {
        if (effectsVolume != newValue)
        {
            effectsVolume = newValue;
            isChanged = true;
            effectsVolumeChanged = true;
        }
    }
    float mouseSensitivity = 0;
    bool mouseSensitivityChanged = false;
    public void OnMouseSensitivityChanged(float newValue)
    {
        if(mouseSensitivity != newValue)
        {
            mouseSensitivity = newValue;
            isChanged = true;
            mouseSensitivityChanged = true;
        }
    }
    void SaveMainVolume(float newValue)
    {
        playerSettings.mainVolume = newValue;
        PlayerSettingsManager.instance.SetMainVolumeLogarithmic(playerSettings.mainVolume);
        PlayerSettingsManager.instance.playerSettings = playerSettings;
    }
    void SaveMusicVolume(float newValue)
    {
        playerSettings.musicVolume = newValue;
        PlayerSettingsManager.instance.SetMusicVolumeLogarithmic(playerSettings.musicVolume);
        PlayerSettingsManager.instance.playerSettings = playerSettings;
    }
    //.GetComponent<PlayerManager>().characterSoundFXManager.
    void SaveEffectsVolume(float newValue)
    {
        playerSettings.effectsVolume = newValue;
        PlayerSettingsManager.instance.SetSFXVolumeLogarithmic(playerSettings.effectsVolume);
        PlayerSettingsManager.instance.playerSettings = playerSettings;
    }
    void SwapFromOptionMenuControls()
    {
        DisableOptionsControls();
        PauseScript.instance.playerControls.PauseMenu.Enable();
        //PauseScript.instance.playerControls.UI.Enable();
        PauseScript.instance.playerControls.UI.PauseButton.Enable();
    }
    public void ActivateSaveWindow()
    {
        saveWindow.SetActive(true);
        ToggleNavigation(false);
    }
    void ToggleNavigation(bool enable)
    {
        Navigation nav = invertedToggle.navigation;
        nav.mode = enable ? Navigation.Mode.Automatic : Navigation.Mode.None;
        invertedToggle.navigation = nav;

        nav = mainVolumeSlider.navigation;
        nav.mode = enable ? Navigation.Mode.Automatic : Navigation.Mode.None;
        mainVolumeSlider.navigation = nav;

        nav = effectsVolumeSlider.navigation;
        nav.mode = enable ? Navigation.Mode.Automatic : Navigation.Mode.None;
        effectsVolumeSlider.navigation = nav;

        nav = musicVolumeSlider.navigation;
        nav.mode = enable ? Navigation.Mode.Automatic : Navigation.Mode.None;
        musicVolumeSlider.navigation = nav;
    }
    void DisableOptionsControls()
    {
        menuLeftInput = false;
        menuRightInput = false;
        exitPauseMenuInput = false;
        menuRightInput = false;
        if (playerControls != null)
        {
            playerControls.OptionsMenu.Disable();
        }
    }
    public void OnOptionsScroll(Vector2 value)
    {
        Debug.Log("OnOptionsScroll:" + value);
    }
    public void OnVerticalScroll(float value)
    {
        Debug.Log("OnVerticalScroll:" + value);
    }
}
