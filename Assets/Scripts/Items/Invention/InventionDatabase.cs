using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Items/Invention Database")]
public class InventionDatabase : ScriptableObject
{
    [Header("Lookup table for static invention data (sprite, desc, etc.)")]
    public List<InventionData> inventions;

    private Dictionary<string, InventionData> inventionLookup;

    public void Initialize()
    {
        inventionLookup = new Dictionary<string, InventionData>();

        foreach (var invention in inventions)
        {
            if (!inventionLookup.ContainsKey(invention.inventionId))
            {
                inventionLookup.Add(invention.inventionId, invention);
            }
            else
            {
                Debug.LogWarning($"Duplicate itemId: {invention.inventionId}");
            }
        }
    }

    public InventionData GetInvention(string inventionId)
    {
        if (inventionLookup == null)
            Initialize();

        inventionLookup.TryGetValue(inventionId, out var invention);
        return invention;
    }
    public void SetHasObtained(string inventionId)
    {
        foreach (var invention in inventions)
        {
            if (invention.inventionId == inventionId)
            {
                invention.hasObtained = true;
                GetInvention(inventionId).hasObtained = true;
            }
        }
    }
    public List<string> GetSaveData()
    {
        List<string> rv = new List<string>();
        foreach (var invention in inventions)
        {
            if (invention.hasObtained)
                rv.Add(invention.inventionId);
        }
        return rv;
    }
}
