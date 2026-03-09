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
    public SceneAsset exitScene;
#endif
    //TODO Check to see if [HideInInspector] causes this not to work after export
    [HideInInspector] public string dungeonLevelSelectSceneID;
    [HideInInspector] public string exitSceneID;
    public float exitX = 0, exitY = 0, exitZ = 0;

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
        if(exitScene != null) 
            exitSceneID = exitScene.name;
    }
#endif
}
