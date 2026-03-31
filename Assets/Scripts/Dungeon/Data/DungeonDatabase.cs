using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dungeon/Dungeon Database")]
public class DungeonDatabase : ScriptableObject
{
    [Header("DungeonDatabase holds and allows easy access to information of any dungeon\n" +
        "Do NOT modify in game. Static data only")]
    public List<DungeonData> dungeons = new List<DungeonData>();
    private Dictionary<string, DungeonData> dungeonsLookup = null;
    void Initialize()
    {
        dungeonsLookup = new Dictionary<string, DungeonData>();
        foreach (DungeonData dungeon in dungeons)
        {
            if (!dungeonsLookup.ContainsKey(dungeon.dungeonId.ToLower()))
            {
                dungeonsLookup.Add(dungeon.dungeonId.ToLower(), dungeon);
            }
            else
            {
                Debug.LogWarning($"Duplicate dungeonId: {dungeon.dungeonId}");
            }
        }
    }
    public DungeonData GetDungeon( string dungeonId)
    {
        if(dungeonsLookup == null)
            Initialize();
        return dungeonsLookup[dungeonId];
    }
}
