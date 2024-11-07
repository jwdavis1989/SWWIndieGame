using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponManager : CharacterWeaponManager
{
    [Header("Description:List of Player's current wepaons\n\n")]
    public static PlayerWeaponManager instance;
    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
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
    }
}
