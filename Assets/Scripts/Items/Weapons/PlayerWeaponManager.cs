using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponManager : CharacterWeaponManager
{
    [Header("Materials database based on weapon's highest element")]
    public Material[] elementalMaterialsArray; 
    
    [Header("Description:List of Player's current wepaons\n\n")]
    public static PlayerWeaponManager instance;
    new public void Awake()
    {
        if (instance == null)
        {
            instance = this;
            WorldUtilityManager.StaticObjects.Add(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        //Avoids destroying this object when changing scenes
        DontDestroyOnLoad(gameObject);
        WorldUtilityManager.StaticObjects.Add(gameObject);
    }
    private void OnDestroy()
    {
        instance = null; // For main menu button
    }
}
