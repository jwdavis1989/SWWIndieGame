using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseScript : MonoBehaviour
{
    bool gamePaused = false;
    public bool debugMode = false;
    [SerializeField] GameObject canvas;
    [SerializeField] GameObject mainPauseMenu;
    [SerializeField] GameObject upgradeMenu;
    [SerializeField] GameObject DebugSaveGameButton;

    public void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && gamePaused == false)
        {
            Pause();
        }
        else if ((Input.GetKeyDown(KeyCode.Escape) && gamePaused == true))
        {
            Unpause();
        }
    }

    public void Home()
    {
        SceneManager.LoadScene(0);
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
        SceneManager.LoadSceneAsync(0);
        Unpause();
    }
    public void DebugSaveGameCLick()
    {
        WorldSaveGameManager.instance.saveGame = true;
    }
    void Pause()
    {
        Time.timeScale = 0;
        gamePaused = true;
        canvas.SetActive(true);
        mainPauseMenu.SetActive(true);
        if (debugMode) DebugSaveGameButton.SetActive(true); else DebugSaveGameButton.SetActive(false);
    }
    void Unpause()
    {
        Time.timeScale = 1;
        gamePaused = false;
        mainPauseMenu.SetActive(true);
        upgradeMenu.SetActive(false);
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
