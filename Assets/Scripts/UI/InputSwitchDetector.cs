using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class InputSwitchDetector : MonoBehaviour
{
    private void OnEnable()
    {
        InputUser.onChange += OnInputUserChange;
    }
    private void OnDisable()
    {
        InputUser.onChange -= OnInputUserChange;
    }

    private void OnInputUserChange(InputUser user, InputUserChange change, InputDevice device)
    {
        if (change == InputUserChange.ControlSchemeChanged)
        {
            Debug.Log("Switched to: " + user.controlScheme.Value.name);
            // Update UI here (swap KB/M prompts to Controller prompts, etc.)
        }
    }
}
