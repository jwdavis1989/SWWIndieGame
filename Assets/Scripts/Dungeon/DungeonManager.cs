using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonManager : MonoBehaviour
{
    [Header("DungeonManager allows access to the dungeon database in game\n" +
        "It also handles saving and loading of player dungeon progress.")]
    public DungeonDatabase dungeonDatabase;
    [HideInInspector] public List<DungeonSaveData> savedDungeons = new List<DungeonSaveData>();
    [HideInInspector] public string currentDungeonId = null;
    [HideInInspector] public string currentLevelId = null;

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
    public static DungeonNodeSaveData GetDungeonNodeProgress(string dungeonId, string nodeId)
    {
        foreach (DungeonSaveData savedDungeon in instance.savedDungeons){
            if (savedDungeon.dungeonId.ToLower().Equals(dungeonId.ToLower())){
                if(savedDungeon.savedNodes == null)
                    return null;
                foreach (DungeonNodeSaveData savedNode in savedDungeon.savedNodes){
                    if (savedNode.nodeID.ToLower().Equals(nodeId.ToLower()))
                        return savedNode;
                }
                //Debug.Log("No saved node for d:" + dungeonId + " n:" + nodeId +".");
                return null;
            }
        }
        //Debug.Log("No saved dungeon for d:" + dungeonId + ".");
        return null;
    }
    public static List<DungeonSaveData> SaveDungeons()
    {
        return instance.savedDungeons;
    }
    public static void LoadDungeons(List<DungeonSaveData> dungeons)
    {
        instance.savedDungeons = dungeons;
    }
    public static void EnterDungeonLevel(string dungeonId, string dungeonLevelId)
    {
        Debug.Log("EnterDungeonLevel,"+dungeonId+","+dungeonLevelId+".");
        DungeonData dungeonData = instance.dungeonDatabase.GetDungeon(dungeonId);
        Debug.Log("EnterDungeonLevel," + dungeonData.dungeonName + ".");
        DungeonLevelData dungeonNode = dungeonData.GetDungeonLevelNodeByID(dungeonLevelId);
        if(dungeonNode != null)
        {
            instance.currentDungeonId = dungeonData.dungeonId;
            instance.currentLevelId = dungeonLevelId;
            TeleportData.yRotation = dungeonNode.startYRotation;
            TeleportData.playerManager.TeleportPlayerToSceneAndCoordinates(0, dungeonNode.startX, dungeonNode.startY, dungeonNode.startZ, dungeonNode.levelSceneId);
        }
        else
            Debug.Log("EnterDungeonLevel dungeonNode null");
    }
    public static void CompleteCurrentDungeonLevel()
    {
        if(instance.savedDungeons == null)
            instance.savedDungeons = new List<DungeonSaveData> ();
        DungeonData dungeonData = instance.dungeonDatabase.GetDungeon(instance.currentDungeonId);
        DungeonLevelData dungeonNode = dungeonData.GetDungeonLevelNodeByID (instance.currentLevelId);
        DungeonSaveData dungeonSaveData = instance.savedDungeons.Find((savedDungeon) => savedDungeon.dungeonId.Equals(instance.currentDungeonId));
        if (dungeonSaveData == null)
        { // No dungeon data yet saved
            dungeonSaveData = new DungeonSaveData ();
            dungeonSaveData.dungeonId = instance.currentDungeonId;
            instance.savedDungeons.Add(dungeonSaveData);
        }
        DungeonNodeSaveData nodeSaveData = GetDungeonNodeProgress(instance.currentDungeonId, instance.currentLevelId);
        if (nodeSaveData == null)
        { // current level not yet saved
            nodeSaveData = new DungeonNodeSaveData();
            nodeSaveData.nodeID = instance.currentLevelId;
            dungeonSaveData.savedNodes.Add(nodeSaveData);
        }
        nodeSaveData.completed = true;
        foreach (DungeonLevelData connectedNode in dungeonNode.connections)
        {
            nodeSaveData = new DungeonNodeSaveData();
            nodeSaveData.nodeID = connectedNode.nodeID;
            nodeSaveData.unlocked = true;
            dungeonSaveData.savedNodes.Add(nodeSaveData);
        }
    }
    //challenges
    public static float elapsedTime = 0;
    public static float GetElapsedTime()
    {
        return elapsedTime;
    }
    public static bool mainHandUsed = false;
    public static bool offHandUsed = false;
}
