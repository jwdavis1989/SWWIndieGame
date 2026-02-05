using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RumbleUntilDisabled : MonoBehaviour
{
    public float leftMotorSpeedLowFrequency = 0.25f;
    public float rightMotorSpeedHighFrequency = 0.25f;
    
    private void OnEnable()
    {
        if (Gamepad.current != null)
        {
            Gamepad.current.SetMotorSpeeds(leftMotorSpeedLowFrequency, rightMotorSpeedHighFrequency);
        }
    }

    private void OnDisable()
    {
        if (Gamepad.current != null)
        {
            Gamepad.current.SetMotorSpeeds(0f, 0f);
        }
    }
}
