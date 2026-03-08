using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "Dungeon/Dungeon Data")]
public class DungeonData : ScriptableObject
{
    [Header("Unique I.D. Case insensitive.")]
    public string dungeonId;
    public string dungeonName;
#if UNITY_EDITOR
    public SceneAsset levelSelectScene;
#endif
    [HideInInspector] public string dungeonLevelSelectSceneID;
    public List<DungeonLevelData> dungeonNodes;
    public DungeonLevelData GetDungeonLevelNodeByID(string levelId)
    {
        foreach(DungeonLevelData node in dungeonNodes)
        {
            if(node.nodeID == levelId)
                return node;
        }
        return null;
    }
#if UNITY_EDITOR
    void OnValidate()
    {
        if (levelSelectScene != null)
        { // set scene name
            dungeonLevelSelectSceneID = levelSelectScene.name;
        }
    }
#endif
}
