using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonManager : MonoBehaviour
{
    [Header("DungeonManager allows access to the dungeon database in game\n" +
        "It also handles saving and loading of player dungeon progress.")]
    public List<DungeonSaveData> savedDungeons = new List<DungeonSaveData>();

    public DungeonDatabase dungeonDatabase;

    //singleton behavior
    public static DungeonDatabase GetDB()
    {
        return instance.dungeonDatabase;
    }
    public static DungeonManager instance;
    public void Awake()
    {
        if (instance == null){
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }
    public DungeonNodeSaveData CheckDungeonNodeProgress(string dungeonId, string nodeId)
    {
        foreach (DungeonSaveData savedDungeon in savedDungeons){
            if (savedDungeon.dungeonId.ToLower().Equals(dungeonId.ToLower())){
                foreach (DungeonNodeSaveData savedNode in savedDungeon.savedNodes){
                    if (savedNode.nodeID.ToLower().Equals(nodeId.ToLower()))
                        return savedNode;
                }
                Debug.Log("No saved node for d:" + dungeonId + " n:" + nodeId +".");
                return null;
            }
        }
        Debug.Log("No saved dungeon for d:" + dungeonId + ".");
        return null;
    }
    public List<DungeonSaveData> SaveDungeons()
    {
        return savedDungeons;
    }
    public void LoadDungeons(List<DungeonSaveData> dungeons)
    {
        savedDungeons = dungeons;
    }
}
