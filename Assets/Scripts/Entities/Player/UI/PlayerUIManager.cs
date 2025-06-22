using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Unity.Netcode;

public class PlayerUIManager : MonoBehaviour
{
    public static PlayerUIManager instance;

    [HideInInspector] public PlayerUIHudManager playerUIHudManager;
    [HideInInspector] public PlayerUIPopUpManager playerUIPopUpManager;
    [HideInInspector] public PauseScript playerUIPauseMenu;

    [Header("UI Flags")]
    public bool menuWindowIsOpen = false;   //Inventory Screen/Equipmen Menu/Blacksmith menu, etc. //TODO: This needs to be toggled by Alec's pause system.
    public bool popUpWindowIsOpen = false;  //Item pick up, dialogue pop up, etc. //TODO: This needs to be toggled by Alec's dialogue system.

    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        playerUIHudManager = GetComponentInChildren<PlayerUIHudManager>();
        playerUIPopUpManager = GetComponentInChildren<PlayerUIPopUpManager>();
        playerUIPauseMenu = GetComponentInChildren<PauseScript>();
    }
    
}
