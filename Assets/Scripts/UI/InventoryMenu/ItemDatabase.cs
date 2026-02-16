using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Items/Item Database")]

public class ItemDatabase : ScriptableObject
{
    [Header("Lookup table for static item data (sprite, desc, etc.)")]
    public List<ItemDetails> items;
    public List<ItemEffect> itemEffects;
    public List<WeaponData> weaponDetails;
    public List<TinkerComponentData> componentDetails;

    private Dictionary<string, ItemDetails> itemLookup;
    public Dictionary<string, ItemEffect> itemEffectsLookup;
    private Dictionary<string, WeaponData> weaponDetailsLookup;
    public Dictionary<string, TinkerComponentData> componentDetailsLookup;

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
        foreach (WeaponData weaponData in weaponDetails)
        {
            if (!weaponDetailsLookup.ContainsKey(weaponData.itemId))
            {
                weaponDetailsLookup.Add(weaponData.itemId, weaponData);
            }
            else
            {
                Debug.LogWarning($"Duplicate weaponData.itemId: {weaponData.itemId}");
            }
        }
        foreach (TinkerComponentData componentData in componentDetails)
        {
            if (!componentDetailsLookup.ContainsKey(componentData.itemId))
            {
                componentDetailsLookup.Add(componentData.itemId, componentData);
            }
            else
            {
                Debug.LogWarning($"Duplicate componentData.itemId: {componentData.itemId}");
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
    public WeaponData GetWeaponData(string itemId)
    {
        if (itemEffectsLookup == null)
            Initialize();

        weaponDetailsLookup.TryGetValue(itemId, out var data);
        return data;
    }
    public TinkerComponentData GetTinkerComponentData(string itemId)
    {
        if (itemEffectsLookup == null)
            Initialize();

        componentDetailsLookup.TryGetValue(itemId, out var data);
        return data;
    }
}
