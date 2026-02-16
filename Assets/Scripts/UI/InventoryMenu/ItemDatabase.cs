using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Items/Item Database")]

public class ItemDatabase : ScriptableObject
{
    [Header("Lookup table for static item data (sprite, desc, etc.)")]
    public List<ItemDetails> items;
    public List<ItemEffect> itemEffects;

    private Dictionary<string, ItemDetails> itemLookup;
    public Dictionary<string, ItemEffect> itemEffectsLookup;

    public void Initialize()
    {
        itemLookup = new Dictionary<string, ItemDetails>();

        foreach (ItemDetails item in items)
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
        foreach (ItemEffect itemEffect in itemEffects)
        {
            if (!itemEffectsLookup.ContainsKey(itemEffect.itemId))
            {
                itemEffectsLookup.Add(itemEffect.itemId, itemEffect);
            }
            else
            {
                Debug.LogWarning($"Duplicate itemEffect.itemId: {itemEffect.itemId}");
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
    public ItemEffect GetItemEffect(string itemId)
    {
        if (itemEffectsLookup == null)
            Initialize();

        itemEffectsLookup.TryGetValue(itemId, out var item);
        return item;
    }
}
