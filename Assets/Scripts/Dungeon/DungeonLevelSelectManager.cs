using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.TerrainUtils;
using UnityEngine.UI;

public class DungeonLevelSelectManager : MonoBehaviour
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
    public Button startingNode;

    //scrolling window
    [Header("Scrolling window")]
    public ScrollRect scrollRect;
    public RectTransform content;

    //data
    private DungeonData dungeonData;
    private DungeonDatabase dungeonDatabase;

    //input
    [Header("Input")]
    public PlayerControls playerControls;
    public EventSystem eventSystem;
    public bool saveInput = false;
    public bool backInput = false;
    public bool floorInfoInput = false;

    // Start is called before the first frame update
    void Start()
    {
        dungeonDatabase = DungeonManager.GetDB();
        dungeonData = dungeonDatabase.GetDungeon(dungeonId);
        foreach (DungeonLevelNodeUI node in nodes)
        { // Show or Hide dungeon levels
            if (node.entrance)
            {
                node.Show();
                //select & scroll to entrance
                node.button.Select();
                ScrollTo(node.button.GetComponent<RectTransform>());
            }
            else 
                node.Hide(hiddenDungeonLevelSprite);
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
            //select & scroll to current
            if(DungeonManager.currentLevelId == node.dungeonLevelId)
            {
                node.button.Select();
                ScrollTo(node.button.GetComponent<RectTransform>());
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
            playerControls.DungeonLevelSelect.FloorInfo.performed += i => floorInfoInput = true;
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
        HandleFloorInfoInput();
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
                //scroll
                if (InputSwitchDetector.IsCurrentlyGamepad())
                {
                    RectTransform selectedRect = currentCursorObj.GetComponent<RectTransform>();
                    if (selectedRect != null)
                        ScrollTo(selectedRect);
                }
                //Handle tooltip
                DungeonLevelNodeUI ui = currentCursorObj.GetComponentInParent<DungeonLevelNodeUI>();
                if (ui != null)
                {
                    DungeonLevelData levelData = dungeonData.GetDungeonLevelNodeByID(ui.dungeonLevelId);
                    if (levelData != null)
                    {
                        DungeonNodeSaveData nodeSaveData = DungeonManager.GetDungeonNodeProgress(dungeonData.dungeonId, levelData.nodeID);
                        challengeTooltip.headerText.text = levelData.dungeonLevelName;
                        challengeTooltip.centerText.text = "";
                        foreach (DungeonChallengeData challengeData in levelData.dungeonChallenges)
                        {
                            if (nodeSaveData != null && nodeSaveData.challengesCompleted.Contains(challengeData.challengeId))
                                challengeTooltip.centerText.text += "[Completed] ";
                            challengeTooltip.centerText.text += challengeData.description + "\n";
                        }
                    }
                }
            }
            else // Nothing is selected. If on gamepad try to select something
            {
                if (startingNode != null && InputSwitchDetector.IsCurrentlyGamepad() && !IsWindowActive())
                {
                    startingNode.Select();
                }
            }
        }
    }
    void ScrollTo(RectTransform target)
    {
        Canvas.ForceUpdateCanvases();

        RectTransform content = scrollRect.content;
        RectTransform viewport = scrollRect.viewport;

        // Position of target relative to content
        Vector2 localPos = content.InverseTransformPoint(target.position);

        // Convert so top of content = 0
        float targetY = -localPos.y;

        // Center target in viewport (optional)
        float centeredY = targetY - (viewport.rect.height / 2f);

        // Clamp to valid scroll range
        float maxScroll = content.rect.height - viewport.rect.height;

        centeredY = Mathf.Clamp(centeredY, 0, maxScroll);

        content.anchoredPosition = new Vector2(
            content.anchoredPosition.x,
            centeredY
        );
    }
    bool IsWindowActive()
    {
        return backWindow.activeInHierarchy || saveWindow.activeInHierarchy;
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
    void HandleFloorInfoInput()
    {
        if (floorInfoInput)
        {
            floorInfoInput = false;
            if (!IsWindowActive())
            {
                challengeTooltip.gameObject.SetActive(!challengeTooltip.gameObject.activeInHierarchy);
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
        string currentLevel = DungeonManager.currentLevelId;
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
