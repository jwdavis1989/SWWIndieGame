using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonLevelManager : MonoBehaviour
{
    [Header("DungeonLevelManager handle the level select of a particular dungeon\n" +
        "It should exist in the scene that holds that UI")]
    public string dungeonId;
    DungeonDatabase dungeonDatabase;
    public List<DungeonLevelNodeUI> nodes;
    // Start is called before the first frame update
    void Start()
    {
        dungeonDatabase = DungeonManager.GetDB();
        DungeonData dungeonData = dungeonDatabase.GetDungeon(dungeonId);
        foreach (DungeonLevelNodeUI node in nodes)
        {
            if (node.entrance)
                node.Show();
            else node.Hide();
            //DungeonNode dungeonNode = dungeonData.GetDungeonLevelNodeByID(node.dungeonLevelId);
            DungeonNodeSaveData dungeonNodeSaveData = DungeonManager.GetDungeonNodeProgress(dungeonId, node.dungeonLevelId);
            if (dungeonNodeSaveData != null)
            {
                if (dungeonNodeSaveData.completed)
                {
                    node.completed = true;
                    node.Show();
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
