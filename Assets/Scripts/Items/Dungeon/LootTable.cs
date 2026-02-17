using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Loot/Loot Table")]
public class LootTable : ScriptableObject
{
    public List<LootEntry> entries;

    public ItemDetails GetRandomItem()
    {
        int totalWeight = 0;

        foreach (var entry in entries)
            totalWeight += entry.weight;

        int random = Random.Range(0, totalWeight);
        int current = 0;

        foreach (var entry in entries)
        {
            current += entry.weight;

            if (random < current)
                return entry.item;
        }

        return null;
    }
}
[System.Serializable]
public class LootEntry
{
    public ItemDetails item;   // Your ScriptableObject item
    public int weight;      // Relative drop chance
}