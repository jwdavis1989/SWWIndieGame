using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DungeonLevelManager : MonoBehaviour
{
    [Header("DungeonLevelManager handle the level select of a particular dungeon\n" +
        "It should exist in the scene that holds that UI")]
    public string dungeonId;
    [Header("Sprite for a dungeon level that is not available yet")]
    public Sprite hiddenDungeonLevelSprite;
    public List<DungeonLevelNodeUI> nodes;
    public GameObject saveWindow;
    public GameObject backWindow;
    public List<GameObject> keyboardMouseTooltips;
    public List<GameObject> gamepadTooltips;
    public TooltipUI challengeTooltip;

    //data
    private DungeonData dungeonData;
    private DungeonDatabase dungeonDatabase;

    //input
    public PlayerControls playerControls;
    public EventSystem eventSystem;
    public bool saveInput = false;
    public bool backInput = false;


    // Start is called before the first frame update
    void Start()
    {
        dungeonDatabase = DungeonManager.GetDB();
        dungeonData = dungeonDatabase.GetDungeon(dungeonId);
        foreach (DungeonLevelNodeUI node in nodes)
        { // Show or Hide dungeon levels
            if (node.entrance)
                node.Show();
            else node.Hide(hiddenDungeonLevelSprite);
            DungeonLevelData dungeonNode = dungeonData.GetDungeonLevelNodeByID(node.dungeonLevelId);
            DungeonNodeSaveData dungeonNodeSaveData = DungeonManager.GetDungeonNodeProgress(dungeonId, node.dungeonLevelId);
            if (dungeonNodeSaveData != null)
            {
                if (dungeonNodeSaveData.completed)
                {
                    //node.completed = true;
                    node.Show();
                }
                else if (dungeonNodeSaveData.unlocked)
                    node.Show();
            }
        }
        // Close open windows
        CloseBackWindow();
        CloseSaveWindow();
        // Set up input
        if (playerControls == null)
        {
            playerControls = new PlayerControls();
            playerControls.DungeonLevelSelect.SaveGame.performed += i => saveInput = true;
            playerControls.DungeonLevelSelect.Back.performed += i => backInput = true;
            playerControls.Enable();
            PlayerInputManager.instance.SafeDisable(true, true);
        }
        // Show relevant tooltips
        if (InputSwitchDetector.IsCurrentlyGamepad())
        {
            foreach (GameObject gamepadeUI in gamepadTooltips)
                gamepadeUI.SetActive(true);
            foreach (GameObject kbmUI in keyboardMouseTooltips)
                kbmUI.SetActive(false);
        }
        else
        {
            foreach (GameObject gamepadeUI in gamepadTooltips)
                gamepadeUI.SetActive(false);
            foreach (GameObject kbmUI in keyboardMouseTooltips)
                kbmUI.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        HandleBackInput();
        HandleSaveGameInput();
        CheckControlsChanged();
        HandleGamepadSelectedObject();
    }
    GameObject currentCursorObj;
    string selectedLevelId;
    public void HandleGamepadSelectedObject()
    {

        if (currentCursorObj != eventSystem.currentSelectedGameObject)
        { // Need to change active tooltip window
            //Debug.Log("TooltipActive & New cursor obj");
            currentCursorObj = eventSystem.currentSelectedGameObject;
            if (currentCursorObj != null)
            {
                DungeonLevelNodeUI ui = currentCursorObj.GetComponentInParent<DungeonLevelNodeUI>();
                if (ui != null)
                {
                    DungeonLevelData levelData = dungeonData.GetDungeonLevelNodeByID(ui.dungeonLevelId);
                    if (levelData != null)
                    {
                        challengeTooltip.headerText.text = "Level " + levelData.nodeID;
                        challengeTooltip.centerText.text = "";
                        foreach (DungeonChallengeData challengeData in levelData.dungeonChallenges)
                        {
                            challengeTooltip.centerText.text += challengeData.description + "\n";
                        }
                    }
                }
            }
            else // Nothing is selected. If on gamepad try to select something
            {
                
            }
        }
    }
    void HandleSaveGameInput()
    {
        if (saveInput)
        {
            saveInput = false;
            if(saveWindow != null && !backWindow.activeInHierarchy)
            {
                saveWindow.SetActive(true);
                DisableLevelNavigation();
            }
        }
    }
    public void CloseSaveWindow()
    {
        if (saveWindow != null)
            saveWindow.SetActive(false);
        EnableLevelNavigation();
    }
    public void CloseBackWindow()
    {
        if (backWindow != null)
            backWindow.SetActive(false);
        EnableLevelNavigation();
    }
    void HandleBackInput()
    {
        if (backInput)
        {
            backInput = false;
            if(backWindow != null && !(saveWindow.activeInHierarchy || backWindow.activeInHierarchy))
            {
                backWindow.SetActive(true);
                DisableLevelNavigation();
            }
            else if (saveWindow.activeInHierarchy)
            {
                CloseSaveWindow();
            }
            else if (backWindow.activeInHierarchy)
            {
                CloseBackWindow();
            }

        }
    }
    public void OnSaveGameClick()
    {
        WorldSaveGameManager.instance.SaveGame();
        CloseSaveWindow();
    }
    public void OnExitClick()
    {
        DisableLevelNavigation();
        TeleportData.yRotation = dungeonData.exitYRotation;
        TeleportData.playerManager.TeleportPlayerToSceneAndCoordinates(-1, dungeonData.exitX, dungeonData.exitY, dungeonData.exitZ, dungeonData.exitSceneID);
    }
    void DisableLevelNavigation()
    {
        foreach(DungeonLevelNodeUI lvlUI in nodes)
        {
            Navigation nav = lvlUI.button.navigation;
            nav.mode = Navigation.Mode.None;
            lvlUI.button.navigation = nav;
        }
    }
    void EnableLevelNavigation()
    {
        string currentLevel = DungeonManager.instance.currentLevelId;
        foreach (DungeonLevelNodeUI lvlUI in nodes)
        {
            Navigation nav = lvlUI.button.navigation;
            nav.mode = Navigation.Mode.Automatic;
            lvlUI.button.navigation = nav;
            if (lvlUI.dungeonLevelId == currentLevel)
                lvlUI.button.Select();
        }
    }
    private void CheckControlsChanged()
    {
        InputSwitchDetector inputSwitchDetector = InputSwitchDetector.instance;
        inputSwitchDetector.CheckControlsChanged();
        if (inputSwitchDetector.deviceChanged)
        {
            inputSwitchDetector.deviceChanged = false;
            if (InputSwitchDetector.IsCurrentlyGamepad())
            {
                //Show controller UI
                foreach (GameObject gamepadeUI in gamepadTooltips)
                    gamepadeUI.SetActive(true);
                foreach (GameObject kbmUI in keyboardMouseTooltips)
                    kbmUI.SetActive(false);
            }
            else //Keyboard
            {
                //Hide Controller UI
                foreach (GameObject gamepadeUI in gamepadTooltips)
                    gamepadeUI.SetActive(false);
                foreach (GameObject kbmUI in keyboardMouseTooltips)
                    kbmUI.SetActive(true);
            }
        }
    }
}
