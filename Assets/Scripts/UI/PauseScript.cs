using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PauseScript : MonoBehaviour
{
    bool gamePaused = false;
    public bool debugMode = false;
    [SerializeField] GameObject canvas;
    [SerializeField] GameObject mainPauseMenu;
    [SerializeField] GameObject upgradeMenu;
    [SerializeField] GameObject DebugSaveGameButton;
    [SerializeField] GameObject DebugAddItemButton;
    public EventSystem mainPauseMenuEvents;

    public void Start()
    {
        DontDestroyOnLoad(gameObject);
        upgradeMenu.SetActive(false);
        mainPauseMenu.SetActive(true);
        canvas.SetActive(false);
    }
    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.JoystickButton7)
            ) && gamePaused == false)
        {
            Pause();
        }
        else if ((Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.JoystickButton7)) && gamePaused == true)
        {
            Unpause();
            mainPauseMenuEvents.SetSelectedGameObject(mainPauseMenuEvents.firstSelectedGameObject);
        }
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
        Destroy(GameObject.Find("Player"));
        Destroy(GameObject.Find("DontDestroyOnLoad"));
        SceneManager.LoadSceneAsync(0);
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
    void Pause()
    {
        Time.timeScale = 0;
        gamePaused = true;
        canvas.SetActive(true);
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
        canvas.SetActive(false);
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
