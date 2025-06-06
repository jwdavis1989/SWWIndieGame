using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PauseScript : MonoBehaviour
{
    public bool gamePaused = false;
    public bool debugMode = false;
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject mainPauseMenu;
    [SerializeField] GameObject upgradeMenu;
    [SerializeField] GameObject DebugSaveGameButton;
    [SerializeField] GameObject DebugAddItemButton;
    public EventSystem mainPauseMenuEvents;

    public void Start()
    {
        DontDestroyOnLoad(gameObject);
        upgradeMenu.SetActive(false);
        mainPauseMenu.SetActive(false);
        pauseMenu.SetActive(false);
    }
    void Update()
    {
        //if ((Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.JoystickButton7)
        //    ) && gamePaused == false && SceneManager.GetActiveScene().buildIndex != 0)
        //{
        //    Pause();
        //}
        //else if ((Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.JoystickButton7)) && gamePaused == true)
        //{
        //    Unpause();
            
        //}
        if (gamePaused && mainPauseMenuEvents.currentSelectedGameObject == null)
        {   // Handle for lost cursor
            mainPauseMenuEvents.SetSelectedGameObject(mainPauseMenuEvents.firstSelectedGameObject);
        }
    }

    public void ContinueClick()
    {
        Unpause();
    }
    public void UpgradeMenuClick()
    {
        mainPauseMenu.SetActive(false);
        upgradeMenu.SetActive(true);
    }
    public void UpgradeMenuBackClick()
    {
        upgradeMenu.SetActive(false);
        mainPauseMenu.SetActive(true);
    }
    public void MainMenuClick()
    {
        //GameObject.Find("Player").transform.position = new Vector3(0,0,0);
        Destroy(GameObject.Find("Player"));
        Destroy(GameObject.Find("Player Camera"));
        Destroy(GameObject.Find("Player Input Manager"));
        Destroy(GameObject.Find("Player UI Manager"));
        Destroy(GameObject.Find("TinkerComponentManager"));
        Destroy(GameObject.Find("WeaponController"));
        Destroy(GameObject.Find("UpgradeMenuManager"));
        Destroy(GameObject.Find("DontDestroyOnLoad"));
        Destroy(GameObject.Find("MiniMap Camera"));
        //GameObject.Find("DontDestroyOnLoad").transform.DetachChildren();
        SceneManager.LoadScene(0);
        Unpause();
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
        Time.timeScale = 0;
        gamePaused = true;
        pauseMenu.SetActive(true);
        mainPauseMenu.SetActive(true);
        if (debugMode)
        {
            DebugSaveGameButton.SetActive(true);
            DebugAddItemButton.SetActive(true);
        }
        else
        {
            DebugSaveGameButton.SetActive(false);
            DebugAddItemButton.SetActive(false);
        }
    }
    void Unpause()
    {
        Time.timeScale = 1;
        gamePaused = false;
        upgradeMenu.SetActive(false);
        mainPauseMenu.SetActive(true);
        pauseMenu.SetActive(false);
        mainPauseMenuEvents.SetSelectedGameObject(mainPauseMenuEvents.firstSelectedGameObject);
    }
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
}
