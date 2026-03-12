using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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
        {
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
        CloseBackWindow();
        CloseSaveWindow();
        if (playerControls == null)
        {
            playerControls = new PlayerControls();
            playerControls.DungeonLevelSelect.SaveGame.performed += i => saveInput = true;
            playerControls.DungeonLevelSelect.Back.performed += i => backInput = true;
            playerControls.Enable();
        }
    }

    // Update is called once per frame
    void Update()
    {
        HandleBackInput();
        HandleSaveGameInput();
    }
    void HandleSaveGameInput()
    {
        if (saveInput)
        {
            saveInput = false;
            if(saveWindow != null)
                saveWindow.SetActive(true);
        }
    }
    public void CloseSaveWindow()
    {
        if (saveWindow != null)
            saveWindow.SetActive(false);
    }
    public void CloseBackWindow()
    {
        if (backWindow != null)
            backWindow.SetActive(false);
    }
    void HandleBackInput()
    {
        if (backInput)
        {
            backInput = false;
            if(backWindow != null) 
                backWindow.SetActive(true);
        }
    }
    public void OnSaveGameClick()
    {
        WorldSaveGameManager.instance.SaveGame();
    }
    public void OnExitClick()
    {
        //DungeonManager
        TeleportData.playerManager.TeleportPlayerToSceneAndCoordinates(-1, dungeonData.exitX, dungeonData.exitY, dungeonData.exitZ, dungeonData.exitSceneID);
    }
}
