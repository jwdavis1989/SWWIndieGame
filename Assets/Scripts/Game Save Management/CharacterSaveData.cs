using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
//Since we want to reference this data for every save file, this script is not 
//a monobehavior and is instead serializable.
public class CharacterSaveData
{
    [Header("Character Name")]
    public string characterName;

    [Header("Time Played")]
    public float secondsPlayed;

    //Q: Why not Vector3?
    //A: We can only save data from "Basic" variables (e.g. Int, Float, Bool, String, etc.)
    [Header("World Coordinates")]
    public float xPosition;
    public float yPosition;
    public float zPosition;
}
