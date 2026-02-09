using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Items/Item Database")]

public class ItemDatabase : ScriptableObject
{
    [Header("Lookup table for static item data (sprite, desc, etc.)")]
    public List<ItemDetails> items;

    private Dictionary<string, ItemDetails> itemLookup;

    public void Initialize()
    {
        itemLookup = new Dictionary<string, ItemDetails>();

        foreach (var item in items)
        {
            if (!itemLookup.ContainsKey(item.itemId))
            {
                itemLookup.Add(item.itemId, item);
            }
            else
            {
                Debug.LogWarning($"Duplicate itemId: {item.itemId}");
            }
        }
    }

    public ItemDetails GetItem(string itemId)
    {
        if (itemLookup == null)
            Initialize();

        itemLookup.TryGetValue(itemId, out var item);
        return item;
    }
}
