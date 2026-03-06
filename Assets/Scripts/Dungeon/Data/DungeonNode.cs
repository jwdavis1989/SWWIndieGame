using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName = "Dungeon/Dungeon Node")]
public class DungeonNode : ScriptableObject
{
    [Header("node id, unique to a particular dungeon.\n" +
        "I'm inclinded to use b1_n1, b1_n2, b2_n1, etc")]
    public string nodeID = "b1_n1";
    //public Scene scene; // or string sceneName
    public string levelSceneId = null;
    public int sceneId; // TODO LOAD BY STRING
    public float startX = 0f;
    public float startY = 0f;
    public float startZ = 0f;
    public List<DungeonNode> connections; // nodes you can travel to
}