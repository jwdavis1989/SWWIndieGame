using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName = "Dungeon/Dungeon Data")]
public class DungeonData : ScriptableObject
{
    [Header("Unique I.D. Case insensitive.")]
    public string dungeonId;
    public string dungeonName;
    public string dungeonLevelSelectSceneID;
    public List<DungeonNode> dungeonNodes;
    public DungeonNode GetDungeonLevelNodeByID(string levelId)
    {
        foreach(DungeonNode node in dungeonNodes)
        {
            if(node.nodeID == levelId)
                return node;
        }
        return null;
    }
}
