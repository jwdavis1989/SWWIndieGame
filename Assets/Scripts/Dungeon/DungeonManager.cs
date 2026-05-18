using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DungeonManager : MonoBehaviour
{
    [Header("DungeonManager allows access to the dungeon database in game\n" +
        "It also handles saving and loading of player dungeon progress.")]
    public DungeonDatabase dungeonDatabase;
    [HideInInspector] public List<DungeonSaveData> savedDungeons = new List<DungeonSaveData>();
    [HideInInspector] public static string currentDungeonId = null;
    [HideInInspector] public static string currentLevelId = null;

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
        //Debug.Log("EnterDungeonLevel,"+dungeonId+","+dungeonLevelId+".");
        DungeonData dungeonData = instance.dungeonDatabase.GetDungeon(dungeonId);
        //Debug.Log("EnterDungeonLevel," + dungeonData.dungeonName + ".");
        DungeonLevelData dungeonNode = dungeonData.GetDungeonLevelNodeByID(dungeonLevelId);
        if(dungeonNode != null)
        {
            currentDungeonId = dungeonData.dungeonId;
            currentLevelId = dungeonLevelId;
            TeleportData.yRotation = dungeonNode.startYRotation;
            TeleportData.playerManager.TeleportPlayerToSceneAndCoordinates(0, dungeonNode.startX, dungeonNode.startY, dungeonNode.startZ, dungeonNode.levelSceneId);
            foreach(DungeonChallengeData challenge in dungeonNode.dungeonChallenges)
            {
                challenge.Initialize();
            }
        }
        else
            Debug.Log("EnterDungeonLevel dungeonNode null");
    }
    public static void CompleteCurrentDungeonLevel(float completeTime = 0, string unlockedFloorId = null)
    {
        elapsedTime = completeTime;
        if(instance.savedDungeons == null)
            instance.savedDungeons = new List<DungeonSaveData> ();
        DungeonData dungeonData = instance.dungeonDatabase.GetDungeon(currentDungeonId);
        DungeonLevelData dungeonNode = dungeonData.GetDungeonLevelNodeByID (currentLevelId);
        DungeonSaveData dungeonSaveData = instance.savedDungeons.Find((savedDungeon) => savedDungeon.dungeonId.Equals(currentDungeonId));
        if (dungeonSaveData == null)
        { // No dungeon data yet saved
            dungeonSaveData = new DungeonSaveData ();
            dungeonSaveData.dungeonId = currentDungeonId;
            instance.savedDungeons.Add(dungeonSaveData);
        }
        DungeonNodeSaveData nodeSaveData = GetDungeonNodeProgress(currentDungeonId, currentLevelId);
        if (nodeSaveData == null)
        { // current level not yet saved
            nodeSaveData = new DungeonNodeSaveData();
            nodeSaveData.nodeID = currentLevelId;
            dungeonSaveData.savedNodes.Add(nodeSaveData);
        }
        // mark level as complete
        nodeSaveData.completed = true;
        foreach (DungeonChallengeData challenge in dungeonNode.dungeonChallenges)
        {
            if (!challenge.IsFailed())
            {
                nodeSaveData.challengesCompleted.Add(challenge.challengeId);
            }
        }
        // unlock connecting level
        foreach (DungeonLevelData connectedNode in dungeonNode.connections)
        {
            if(unlockedFloorId == null || unlockedFloorId.Trim().Length == 0 || connectedNode.nodeID.Equals(unlockedFloorId))
            {
                nodeSaveData = new DungeonNodeSaveData();
                nodeSaveData.nodeID = connectedNode.nodeID;
                nodeSaveData.unlocked = true;
                dungeonSaveData.savedNodes.Add(nodeSaveData);
            }
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
    public static bool healingItemUsed = false;
}
