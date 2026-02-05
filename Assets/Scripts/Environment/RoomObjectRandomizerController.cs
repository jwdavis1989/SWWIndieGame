using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomObjectRandomizerController : MonoBehaviour
{
    [Header("Warning, ensure both arrays are the same length")]
    public GameObject[] potentialObjects;
    public int[] potentialObjectSpawnChanceInInt;

    void Awake()
    {
        if (potentialObjects.Length == potentialObjects.Length)
        {
            for (int i=0; i<potentialObjects.Length;i++)
            {
                if (Random.Range(0, 101) > potentialObjectSpawnChanceInInt[i])
                {
                    potentialObjects[i].SetActive(false);
                }
            }
        }
        else
        {
            Debug.Log("ERROR: RoomObjectRandomizerController - Array length mismatch.");
        }
    }
}
