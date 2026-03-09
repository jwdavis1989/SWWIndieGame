using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonLevelManager : MonoBehaviour
{
    [Header("DungeonLevelManager handle the level select of a particular dungeon\n" +
        "It should exist in the scene that holds that UI")]
    public string dungeonId;
    public List<DungeonLevelNodeUI> nodes;
    private DungeonData dungeonData;
    private DungeonDatabase dungeonDatabase;
    // Start is called before the first frame update
    void Start()
    {
        dungeonDatabase = DungeonManager.GetDB();
        dungeonData = dungeonDatabase.GetDungeon(dungeonId);
        foreach (DungeonLevelNodeUI node in nodes)
        {
            if (node.entrance)
                node.Show();
            else node.Hide();
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
    }

    // Update is called once per frame
    void Update()
    {
        
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
