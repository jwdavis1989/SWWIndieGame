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
    //public List<DungeonNode> dungeonNodes;
}
