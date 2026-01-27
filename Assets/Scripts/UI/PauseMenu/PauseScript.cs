using NUnit.Framework.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseScript : MonoBehaviour
{
    public bool gamePaused = false;
    
    [SerializeField] GameObject pauseMenu;
    //[SerializeField] GameObject mainPauseMenu;//old
    [Header("Menus")]
    [SerializeField] GameObject weaponMenu;
    [SerializeField] GameObject weaponMenuSideBar;
    [SerializeField] GameObject inventoryMenu;
    [SerializeField] GameObject inventMenu;
    [SerializeField] GameObject optionsMenu;
    [SerializeField] GameObject exitMenu;
    [SerializeField] GameObject topPanelButtons;
    public GameObject playerHud;
    MenuTab lastMenuTab = MenuTab.Weapons;
    [Header("Controls")]
    [SerializeField] bool pauseInput = false;
    [SerializeField] bool menuLeftInput = false;
    [SerializeField] bool menuRightInput = false;
    [SerializeField] bool exitPauseMenuInput = false;
    PlayerControls playerControls;
    public EventSystem mainPauseMenuEvents;
    public GameObject mainMenuButton;
    [Header("Controls Help")]
    PlayerInput playerInput;
    //public Image bottomTooltip;
    public GameObject bottomTooltipPauseMenuGamepad;
    public GameObject bottomTooltipWeaponMenuGamepad;
    //public List<GameObject> gamepadControlsUI;
    GameObject keyboardControlsUI;
    [Header("Debug")]
    public bool debugMode = false;
    [SerializeField] GameObject DebugSaveGameButton;
    [SerializeField] GameObject DebugAddItemButton;

    [Header("Pause is a singleton")]
    public static PauseScript instance;
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
    public void Start()
    {
        DontDestroyOnLoad(gameObject);
        if (debugMode) return;//ASTEST
        DisableAllMenus();
        //if (playerInput == null)
        //{
        //    playerInput = PlayerInputManager.instance.gameObject.GetComponent<PlayerInput>();
        //    playerInput.onControlsChanged += OnControlsChanged;
        //    //playerInput.actions["PauseButton"].performed += i => pauseInput = true;
        //    //playerInput.actions["SwitchMenuLeft"].performed += i => menuLeftInput = true;
        //    //playerInput.actions["SwitchMenuRight"].performed += i => menuRightInput = true;
        //    playerInput.enabled = true;
        //}
        if (playerControls == null)
        {
            playerControls = new PlayerControls();
            playerControls.PauseMenu.PauseButton.performed += i => pauseInput = true;
            playerControls.PauseMenu.SwitchMenuLeft.performed += i => menuLeftInput = true;
            playerControls.PauseMenu.SwitchMenuRight.performed += i => menuRightInput = true;
            playerControls.PauseMenu.ExitMenu.performed += i => exitPauseMenuInput = true;
            playerControls.Enable();
        }
        WorldMusicController.instance.GetComponent<AudioSource>().volume = PlayerSettingsManager.instance.playerSettings.musicVolume;
    }
    void Update()
    {
        HandlePauseInput();
        HandleSwitchMenuInput();
        HandleExitPauseMenuInput();
    }
    WaitForEndOfFrame frameEnd = new WaitForEndOfFrame();
    IEnumerator WaitToEndOfFrameThenContinue()
    {
        yield return frameEnd; //wait for end of frame to avoid both paused/unpaused input triggering
        Unpause();
    }
    public void WeaponMenuClick()
    {
        DisableAllMenus();
        if (weaponMenu != null)
            weaponMenu.SetActive(true);
        if (weaponMenuSideBar != null)
            weaponMenuSideBar.SetActive(true);
        lastMenuTab = MenuTab.Weapons;
        SetWeaponMenuTooltip();
    }
    public void InventoryMenuClick()
    {
        DisableAllMenus();
        if (inventoryMenu != null)
            inventoryMenu.SetActive(true);
        lastMenuTab = MenuTab.Inventory;
        SetMainPauseMenuTooltip();
        bottomTooltipPauseMenuGamepad.SetActive(true);
    }
    public void InventMenuClick()
    {
        DisableAllMenus();
        if (inventMenu != null)
        {
            inventMenu.SetActive(true);
            InventionUIManager.instance.OpenInventionMenu();
        }
        lastMenuTab = MenuTab.Invent;
        SetMainPauseMenuTooltip();
    }
    void DisableAllMenus()
    {
        mainPauseMenuEvents.SetSelectedGameObject(null);
        if (weaponMenu != null) weaponMenu.SetActive(false);
        if (weaponMenuSideBar != null) weaponMenuSideBar.SetActive(false);
        if (inventMenu != null) inventMenu.SetActive(false);
        if (exitMenu != null) exitMenu.SetActive(false);
        if (inventoryMenu != null) inventoryMenu.SetActive(false);
        if (optionsMenu != null) optionsMenu.SetActive(false);
    }
    public void OptionsMenuClick()
    {
        DisableAllMenus();
        if (optionsMenu!= null) optionsMenu.SetActive(true);
        lastMenuTab = MenuTab.Options;
        SetMainPauseMenuTooltip();
    }
    public void ExitMenuClick()
    {
        DisableAllMenus();
        if (exitMenu != null)
            exitMenu.SetActive(true);
        if(mainMenuButton != null)
            mainPauseMenuEvents.SetSelectedGameObject(mainMenuButton);
        mainMenuButton.GetComponent<Button>().Select();
        lastMenuTab = MenuTab.ExitGame;
        SetMainPauseMenuTooltip();
    }
    void SetWeaponMenuTooltip()
    {
        bottomTooltipPauseMenuGamepad.SetActive(false);
        if (InputSwitchDetector.IsCurrentlyGamepad())
        {
            bottomTooltipWeaponMenuGamepad.SetActive(true);
        }
    }
    void SetMainPauseMenuTooltip()
    {
        bottomTooltipWeaponMenuGamepad.SetActive(false); 
        if (InputSwitchDetector.IsCurrentlyGamepad())
        {
            bottomTooltipPauseMenuGamepad.SetActive(true);
        }
    }
    public void MainMenuClick()
    {
        //GameObject.Find("Player").transform.position = new Vector3(0,0,0);
        Unpause();
        //Destroy(GameObject.Find("DontDestroyOnLoad")); //Not a real object
        Destroy(GameObject.Find("Player"));
        Destroy(GameObject.Find("Player Camera"));
        Destroy(GameObject.Find("Player Input Manager"));
        //Destroy(GameObject.Find("Player UI Manager")); //For some reason destroying this causes issues on title screen
        Destroy(GameObject.Find("TinkerComponentManager"));
        Destroy(GameObject.Find("WeaponController"));
        Destroy(GameObject.Find("MiniMap Camera"));
        Destroy(GameObject.Find("JournalManager"));
        Destroy(GameObject.Find("IdeaCameraController"));
        Destroy(GameObject.Find("WorldMusicManager"));
        Destroy(GameObject.Find("WorldSaveGameManager"));
        //Scene scne = GameObject.Find("Player").scene;
        //GameObject ddol = GameObject.Find("DontDestroyOnLoad");
        //foreach(GameObject obj in scne.GetRootGameObjects())
        //{
        //    Destroy(obj);
        //}
        //GameObject.Find("DontDestroyOnLoad").transform.DetachChildren();
        SceneManager.LoadScene(0);
    }
    public void ExitGameClicked()
    {
        Application.Quit();
    }
    public void DebugSaveGameCLick()
    {
        WorldSaveGameManager.instance.saveGame = true;
    }
    public void DebugRandomComponentClick()
    {
        GameObject playerObj = GameObject.Find("Player");
        if (playerObj != null)
        {
            TinkerComponentManager.instance.DropRandomItem(playerObj.transform, 5.0f);
        }
    }
    public void PauseUnpause()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0) //dont pause on title screen
            return;
        if (gamePaused)
            Unpause();
        else
            Pause();
        
    }
    void Pause()
    {
        //PlayerInputManager.instance.enabled = false;
        playerControls.PlayerActions.Disable();
        playerControls.UI.Enable();
        Time.timeScale = 0;
        gamePaused = true;
        pauseMenu.SetActive(true);
        //mainPauseMenu.SetActive(true);
        //if (debugMode)
        //{
        //    DebugSaveGameButton.SetActive(true);
        //    DebugAddItemButton.SetActive(true);
        //}
        //else
        //{
        //    DebugSaveGameButton.SetActive(false);
        //    DebugAddItemButton.SetActive(false);
        //}

        //Disable Controls
        PlayerInputManager.instance.playerControls.Disable();

        //Set bool so the Interactable system understands a Menu window has opened
        PlayerUIManager.instance.menuWindowIsOpen = true;
        playerHud.SetActive(false);
        switch (lastMenuTab)
        {//go to last menu the player had open
            case MenuTab.Weapons:
                WeaponMenuClick();
                break;
            case MenuTab.Inventory:
                InventoryMenuClick();
                break;
            case MenuTab.Invent:
                InventMenuClick();
                break;
            case MenuTab.Options:
                OptionsMenuClick();
                break;
            default:
                WeaponMenuClick();
                break;
        } 
    }
    void Unpause()
    {
        playerControls.PlayerActions.Enable();
        //playerControls.UI.Disable();//Currently causes game to be unpauseable
        //PlayerInputManager.instance.enabled = true;
        Time.timeScale = 1;
        gamePaused = false;
        weaponMenu.SetActive(false);
        weaponMenuSideBar.SetActive(false);
        //mainPauseMenu.SetActive(true);
        pauseMenu.SetActive(false);
        inventMenu.SetActive(false);
        mainPauseMenuEvents.SetSelectedGameObject(mainPauseMenuEvents.firstSelectedGameObject);

        //Re-enable Controls
        PlayerInputManager.instance.playerControls.Enable();

        //Set bool so the Interactable system understands a Menu window has closed
        PlayerUIManager.instance.menuWindowIsOpen = false;
        playerHud.SetActive(true);
    }
    public void HandlePauseInput()
    {
        if (pauseInput) // [Esc], (Start/Menu)
        {
            //Debug.Log("PAUSE INPUT");
            pauseInput = false;
            PauseUnpause();
        }
    }
    public void HandleExitPauseMenuInput()
    {
        if (exitPauseMenuInput) // [Esc], (Start/Menu)
        {
            //Debug.Log("exitPauseMenuInput INPUT");
            exitPauseMenuInput = false;
            if (gamePaused)
                StartCoroutine(WaitToEndOfFrameThenContinue());
        }
    }
    void HandleSwitchMenuInput()
    {
        //GameObject newBtnSelected;
        if (menuLeftInput)
        {
            menuLeftInput = false;
            if (!gamePaused) return;
            switch (lastMenuTab)
            {
                case MenuTab.ExitGame:
                    lastMenuTab = MenuTab.Options;
                    OptionsMenuClick();
                    break;
                case MenuTab.Options:
                    lastMenuTab = MenuTab.Invent;
                    InventMenuClick();
                    break;
                case MenuTab.Invent:
                    lastMenuTab = MenuTab.Inventory;
                    InventoryMenuClick();
                    break;
                case MenuTab.Inventory:
                    lastMenuTab = MenuTab.Weapons;
                    WeaponMenuClick();
                    break;
            }
        }
        else if (menuRightInput)
        {
            menuRightInput = false;
            if (!gamePaused) return;
            switch (lastMenuTab)
            {
                case MenuTab.Weapons:
                    lastMenuTab = MenuTab.Inventory; 
                    InventoryMenuClick();
                    break;
                case MenuTab.Inventory:
                    lastMenuTab = MenuTab.Invent;
                    InventMenuClick();
                    break;
                case MenuTab.Invent:
                    lastMenuTab = MenuTab.Options;
                    OptionsMenuClick();
                    break;
                case MenuTab.Options:
                    lastMenuTab = MenuTab.ExitGame;
                    ExitMenuClick();
                    break;
            }
        }
        //if (gamePaused)
        //{
        //    newBtnSelected = topPanelButtons.transform.GetChild(1 + (int)lastMenuTab).gameObject;
        //    newBtnSelected.GetComponent<Button>().Select();
        //}
    }
    public enum MenuTab
    {
        Weapons,
        Inventory,
        Invent,
        Options,
        ExitGame
    }
}
