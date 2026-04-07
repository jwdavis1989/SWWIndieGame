using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
[CreateAssetMenu(menuName = "Dungeon/Dungeon Level Data")]
public class DungeonLevelData : ScriptableObject
{
    [Header("node id, unique to a particular dungeon.\n" +
        "I'm inclinded to use b1_n1, b1_n2, b2_n1, etc")]
    public string nodeID = "b1_n1";
#if UNITY_EDITOR
    public SceneAsset dungeonLevelScene;
#endif
    public float startX = 0f;
    public float startY = 0f;
    public float startZ = 0f;
    public List<DungeonLevelData> connections; // nodes you can travel to
    [HideInInspector] public string levelSceneId;

#if UNITY_EDITOR
    void OnValidate()
    {
        if (dungeonLevelScene != null)
        { // set scene name
            levelSceneId = dungeonLevelScene.name;
        }
    }
#endif
}
