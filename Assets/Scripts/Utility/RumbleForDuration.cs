using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RumbleForDuration : MonoBehaviour
{
    public float leftMotorSpeedLowFrequency = 0.25f;
    public float rightMotorSpeedHighFrequency = 0.25f;
    public float rumbleDurationInSeconds = 1f;
    private void OnEnable()
    {
        StartCoroutine(RumbleRoutine());
    }

    private IEnumerator RumbleRoutine()
    {
        if (Gamepad.current != null)
        {
            Gamepad.current.SetMotorSpeeds(leftMotorSpeedLowFrequency, rightMotorSpeedHighFrequency);

        yield return new WaitForSeconds(rumbleDurationInSeconds);

        Gamepad.current.SetMotorSpeeds(0f, 0f); 
        }
        else
        {
            Debug.Log("WARNING: No active Gamepad found for RumbleForDuration.cs");
        }
    }
    
    void OnDisable()
    {
        if (Gamepad.current != null)
        {
            Gamepad.current.SetMotorSpeeds(0f, 0f);
        }
    }

    private void OnDestroy()
    {
        if (Gamepad.current != null)
        {
            Gamepad.current.SetMotorSpeeds(0f, 0f);
        }
    }

}
