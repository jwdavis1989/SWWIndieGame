using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class InputSwitchDetector : MonoBehaviour
{
    public string currentDevice = GAMEPAD;
    public bool deviceChanged = false; // set to true when device is changed. Reset to false within context. E.g. WeaponMenu
    public static InputSwitchDetector instance;
    [SerializeField] bool anyGamepadInput = false;
    [SerializeField] bool anyKeyboardOrMouse = false;
    PlayerControls playerControls;
    //constants
    public const string GAMEPAD = "GAMEPAD";
    public const string KEYBOARD = "KEYBOARD";
    public void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogError("Extra InputSwitchDetector created");
            Destroy(gameObject);
        }
    }
    public void Start()
    {
        if (playerControls == null)
        {
            playerControls = new PlayerControls();
            playerControls.DeviceDetection.AnyGamepad.performed += i => anyGamepadInput = true;
            playerControls.DeviceDetection.AnyKeyboardOrMouse.performed += i => anyKeyboardOrMouse = true;
            playerControls.Enable();
        }
    }
    public void CheckControlsChanged()
    {
        //Debug.Log("InputSwitchDetector.CheckControlsChanged");
        //check for device change
        string newDevice = currentDevice;
        if (currentDevice == GAMEPAD)
        {
            // Mouse click
            if (Mouse.current.leftButton.wasPressedThisFrame || Mouse.current.rightButton.wasPressedThisFrame)
            {
                newDevice = KEYBOARD;
                //Debug.Log("Left mouse clicked");
            }
            // Any keyboard key - TODO: see if binding any key in playerControls is more responsive
            if (Keyboard.current.anyKey.wasPressedThisFrame || anyKeyboardOrMouse)
            {
                newDevice = KEYBOARD;
                //Debug.Log("A keyboard key was pressed!");
            }
        }
        else
        {
            if(anyGamepadInput)
            {
                //Debug.Log("Gamepad updated while keyboard");
                anyGamepadInput = false;
                newDevice = GAMEPAD;
            }
        }
        if (currentDevice != newDevice)
        {
            //Debug.Log("Device changed to " + currentDevice + " " + count++);
            deviceChanged = true;
            currentDevice = newDevice;
        }
        anyKeyboardOrMouse = false;
        anyGamepadInput = false;
    }
    public static int count = 0;
    public static bool IsCurrentlyGamepad()
    {
        return instance.currentDevice == GAMEPAD;
    }
}
