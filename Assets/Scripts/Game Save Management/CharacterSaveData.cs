using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TinkerComponentManager;

[System.Serializable]
//Since we want to reference this data for every save file, this script is not 
//a monobehavior and is instead serializable.
public class CharacterSaveData
{
    [Header("Scene Index")]
    public int sceneIndex = 1;

    [Header("Character Name")]
    public string characterName = "Character Name";

    [Header("Time Played")]
    public float secondsPlayed;

    [Header("Weapons")]
    public WeaponsArray weapons;
    public int indexOfEquippedWeapon = 0;
    public int indexOfEquippedSpecialWeapon = 0;
    [Header("Components")]
    public ComponentsArray ownedComponents;
    public ComponentsArray ownedWpnComponents;

    //Q: Why not Vector3?
    //A: We can only save data from "Basic" variables (e.g. Int, Float, Bool, String, etc.)
    [Header("World Coordinates")]
    public float xPosition;
    public float yPosition;
    public float zPosition;

    [Header("Attributes")]
    public int fortitude = 10;
    public int endurance = 10;

    [Header("Resources")]
    public float currentHealth = 100;
    public float currentStamina = 100;

    public Dictionary<string, bool> journalFlags = new Dictionary<string, bool>();
    public IdeaStats[] ideas = new IdeaStats[(int)IdeaType.IDEAS_SIZE];

    public CharacterSaveData()
    {
        this.weapons = new WeaponsArray();
        this.weapons.weaponStats = new WeaponStats[0];
        this.ownedComponents = new ComponentsArray();
        this.ownedComponents.components = new TinkerComponentStats[0];
        this.ownedWpnComponents = new ComponentsArray();
        this.ownedWpnComponents.components = new TinkerComponentStats[0];
    }

}
