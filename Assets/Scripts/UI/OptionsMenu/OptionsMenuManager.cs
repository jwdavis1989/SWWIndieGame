using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OptionsMenuManager : MonoBehaviour
{
    public static OptionsMenuManager instance;
    [HideInInspector] public PlayerSettings playerSettings;
    [Header("Buttons, knobs, and switches")]
    public Toggle invertedToggle;
    public Slider musicVolumeSlider;

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
        //todo: shouldnt be necessary? 
        invertedToggle.isOn = PlayerSettingsManager.instance.playerSettings.inverted;
        //make sure changed is reset
        isChanged = false;
        invertChanged = false;
        musicVolumeChanged = false;
        saveWindow.SetActive(false);
        //activate correct inputs
        if(playerControls != null)
        {
            playerControls.OptionsMenu.Enable();
            PauseScript.instance.playerControls.PauseMenu.Disable();
            PauseScript.instance.playerControls.UI.Disable();
            PlayerInputManager.instance.SafeDisable();//Shouldnt be necessary?
        }
    }
    private void OnDisable()
    {
        isChanged = false;
        SwitchOffOptionMenuControls();
    }
    // Start is called before the first frame update
    void Start()
    {
        if(musicVolumeSlider != null)
        {
            WorldMusicController.instance.GetComponent<AudioSource>().volume = musicVolumeSlider.value;
            musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChange);
        }
        if (playerControls == null)
        {
            playerControls = new PlayerControls();
            playerControls.OptionsMenu.SwitchMenuLeft.performed += i => menuLeftInput = true;
            playerControls.OptionsMenu.SwitchMenuRight.performed += i => menuRightInput = true;
            playerControls.OptionsMenu.ExitMenu.performed += i => exitPauseMenuInput = true;
            playerControls.OptionsMenu.SaveSettings.performed += i => saveSettingInput = true;
            playerControls.Enable();
        }
    }
    // Update is called once per frame
    void Update()
    {
        CheckControlsChanged();
        HandleGamePadSelected();
        HandleSwitchMenuInput();
        HandleExitPauseMenuInput();
        HandleSaveSettingsInput();
    }
    /***********************************************************************************************
     ********************************  I N P U T   H A N D L E R S  ********************************
     ***********************************************************************************************/
    private void CheckControlsChanged()
    {
        //Debug.Log("PauseScript.CheckControlsChanged");
        InputSwitchDetector inputSwitchDetector = InputSwitchDetector.instance;
        inputSwitchDetector.CheckControlsChanged();
        if (inputSwitchDetector.deviceChanged)
        {
            //Debug.Log("PauseScript.CheckControlsChanged Device Changed!" + inputSwitchDetector.currentDevice);
            inputSwitchDetector.deviceChanged = false;
            if (InputSwitchDetector.IsCurrentlyGamepad())
            {
                //Show controller UI
                //foreach (GameObject gamepadeUI in gamepadTooltips)
                //    gamepadeUI.SetActive(true);
                //foreach (GameObject gamepadeUI in keyboardMouseTooltips)
                //    gamepadeUI.SetActive(false);
            }
            else //Keyboard
            {
                //Hide Controller UI
                //foreach (GameObject gamepadeUI in gamepadTooltips)
                //    gamepadeUI.SetActive(false);
                //foreach (GameObject gamepadeUI in keyboardMouseTooltips)
                //    gamepadeUI.SetActive(true);
                //enable buttons
                //EnableAllNavigation();
            }
        }
    }
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
            Debug.Log("HandleExitPauseMenuInput");
            exitPauseMenuInput = false;
            saveWindowAction = "UNPAUSE";
            if (isChanged)
            {
                Debug.Log("HandleExitPauseMenuInput save window");
                saveWindow.SetActive(true);
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
            Debug.Log("saveSettingInput");
            saveSettingInput = false;
            saveWindowAction = "UNPAUSE";
            if (isChanged)
            {
                Debug.Log("HandleSaveSettingsInput isChanged");
                saveWindow.SetActive(true);
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
                saveWindow.SetActive(true);
            }
            else
            {
                CompleteSaveWindowAction();
            }
        }
    }
    // Save Window: No
    public void CompleteSaveWindowAction()
    {
        saveWindow.SetActive(false);
        SwitchOffOptionMenuControls();
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
        if (musicVolumeChanged)
        {
            SaveMusicVolume(musicVolume);
        }
        if (invertChanged)
        {
            SaveInvert(inverted);
        }
        PlayerSettingsManager.instance.Save();
        CompleteSaveWindowAction();
    }
    public void LoadOptions()
    {
        playerSettings = PlayerSettingsManager.instance.playerSettings;
        Debug.Log("Loaded inverted as " + playerSettings.inverted);
        invertedToggle.isOn = playerSettings.inverted;
        musicVolumeSlider.value = playerSettings.musicVolume;
        WorldMusicController.instance.GetComponent<AudioSource>().volume = playerSettings.musicVolume;
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
        Debug.Log("Saving inverted as " + playerSettings.inverted);
    }
    float musicVolume = 0;
    bool musicVolumeChanged = false;
    public void OnMusicVolumeChange(float newValue)
    {
        if (musicVolume != newValue) {
            musicVolume = newValue;
            isChanged = true;
            musicVolumeChanged = true;
        } 
    }
    void SaveMusicVolume(float newValue)
    {
        WorldMusicController.instance.GetComponent<AudioSource>().volume = newValue;
        playerSettings.musicVolume = newValue;
        PlayerSettingsManager.instance.playerSettings = playerSettings;
    }
    void SwitchOffOptionMenuControls()
    {
        if (playerControls != null)
        {
            playerControls.OptionsMenu.Disable();
            PauseScript.instance.playerControls.PauseMenu.Enable();
            PauseScript.instance.playerControls.UI.Enable();
        }
    }
    //public bool recentlySave = false;
    //public IEnumerator SaveTimer()
    //{
    //    recentlySave = true;
    //    yield return new WaitForSeconds(1.5f);
    //    recentlySave = false;
    //}

}
