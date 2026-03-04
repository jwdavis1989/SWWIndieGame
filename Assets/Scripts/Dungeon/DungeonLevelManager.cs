using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonLevelManager : MonoBehaviour
{
    [Header("DungeonLevelManager handle the level select of a particular dungeon\n" +
        "It should exist in the scene that holds that UI")]
    public string dungeonId;
    DungeonDatabase dungeonDatabase;
    // Start is called before the first frame update
    void Start()
    {
        dungeonDatabase = DungeonManager.GetDB();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
