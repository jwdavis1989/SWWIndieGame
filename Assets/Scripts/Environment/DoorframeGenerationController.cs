using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorframeGenerationController : MonoBehaviour
{
    [Header("Warning, ensure both arrays are the same length")]
    public DynamicDoorframeController[] potentialDoorframes;
    public bool[] whichWallsAreDoorframes;

    void Awake()
    {
        if (potentialDoorframes.Length == whichWallsAreDoorframes.Length)
        {
            for (int i=0; i<potentialDoorframes.Length;i++)
            {
                if (whichWallsAreDoorframes[i])
                {
                    potentialDoorframes[i].CreateDoorframe();
                }
            }
        }
        else
        {
            Debug.Log("ERROR: HandleDoorwayGeneration - Array length mismatch.");
        }
    }


}
