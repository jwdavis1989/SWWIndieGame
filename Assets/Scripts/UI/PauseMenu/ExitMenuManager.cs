using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitMenuScript : MonoBehaviour
{
    GameObject currentCursorObj = null; // mostly used for gamepad
    [Header("Tooltips and any elements that are activated/deactivated when switching inputs")]
    public List<GameObject> gamepadTooltips = new List<GameObject>();
    public List<GameObject> keyboardMouseTooltips = new List<GameObject>();
    ExitMenuScript instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
            Destroy(this);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        CheckControlsChanged();
    }
    private void OnEnable()
    {
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
        foreach (GameObject gamepadeUI in gamepadTooltips)
            gamepadeUI.SetActive(false);
        foreach (GameObject kbmUI in keyboardMouseTooltips)
            kbmUI.SetActive(false);
    }
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
                foreach (GameObject gamepadeUI in keyboardMouseTooltips)
                    gamepadeUI.SetActive(false);
            }
            else //Keyboard
            {
                //Hide Controller UI
                foreach (GameObject gamepadeUI in gamepadTooltips)
                    gamepadeUI.SetActive(false);
                foreach (GameObject gamepadeUI in keyboardMouseTooltips)
                    gamepadeUI.SetActive(true);
            }
        }
    }
}
