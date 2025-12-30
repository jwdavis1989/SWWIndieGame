using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class InputSwitchDetector : MonoBehaviour
{
    public string currentDevice = GAMEPAD;
    public bool deviceChanged = false;
    public static InputSwitchDetector instance;
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
            // Any keyboard key
            if (Keyboard.current.anyKey.wasPressedThisFrame)
            {
                newDevice = KEYBOARD;
                //Debug.Log("A keyboard key was pressed!");
            }
        }
        else
        {
            if (Gamepad.current.buttonWest.wasPressedThisFrame || Gamepad.current.buttonEast.wasPressedThisFrame
                || Gamepad.current.buttonNorth.wasPressedThisFrame || Gamepad.current.buttonSouth.wasPressedThisFrame
                || Gamepad.current.leftTrigger.wasPressedThisFrame || Gamepad.current.rightTrigger.wasPressedThisFrame
                || Gamepad.current.leftShoulder.wasPressedThisFrame || Gamepad.current.rightShoulder.wasPressedThisFrame)
            {//TODO: just bind something in PlayerControls
                //Debug.Log("Gamepad updated while keyboard");
                newDevice = GAMEPAD;
            }
        }
        if (currentDevice != newDevice)
        {
            //device was changed
            deviceChanged = true;
            currentDevice = newDevice;
            //Debug.Log("Device changed to " + currentDevice);
        }
    }
}
