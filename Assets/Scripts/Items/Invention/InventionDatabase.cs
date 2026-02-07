using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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
    private bool isRuntimeInstance;
//    private void OnEnable()
//    {
//#if UNITY_EDITOR
//        isRuntimeInstance = EditorApplication.isPlaying;
//#endif
//    }

//    private void OnValidate()
//    {
//#if UNITY_EDITOR
//        if (EditorApplication.isPlaying)
//        {
//            Debug.LogError(
//                $"{name} was modified during Play Mode!",
//                this
//            );
//        }
//#endif
//    }
}