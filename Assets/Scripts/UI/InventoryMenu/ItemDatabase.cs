using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Items/Item Database")]

public class ItemDatabase : ScriptableObject
{
    [Header("Lookup table for static item data (sprite, desc, etc.)\n" +
        "Case insensitive.")]
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
        itemEffectsLookup = new Dictionary<string, ItemEffect>();
        weaponDetailsLookup = new Dictionary<string, WeaponData>();
        componentDetailsLookup = new Dictionary<string, TinkerComponentData>();

        foreach (ItemDetails item in items)
        {
            if (!itemLookup.ContainsKey(item.itemId.ToLower()))
            {
                itemLookup.Add(item.itemId.ToLower(), item);
            }
            else
            {
                Debug.LogWarning($"Duplicate itemId: {item.itemId}");
            }
        }
        foreach (ItemEffect itemEffect in itemEffects)
        {
            if (!itemEffectsLookup.ContainsKey(itemEffect.itemId.ToLower()))
            {
                itemEffectsLookup.Add(itemEffect.itemId.ToLower(), itemEffect);
            }
            else
            {
                Debug.LogWarning($"Duplicate itemEffect.itemId: {itemEffect.itemId}");
            }
        }
        foreach (WeaponData weaponData in weaponDetails)
        {
            if (!weaponDetailsLookup.ContainsKey(weaponData.itemId.ToLower()))
            {
                weaponDetailsLookup.Add(weaponData.itemId.ToLower(), weaponData);
            }
            else
            {
                Debug.LogWarning($"Duplicate weaponData.itemId: {weaponData.itemId}");
            }
        }
        foreach (TinkerComponentData componentData in componentDetails)
        {
            if (!componentDetailsLookup.ContainsKey(componentData.itemId.ToLower()))
            {
                componentDetailsLookup.Add(componentData.itemId.ToLower(), componentData);
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
        itemId = itemId.ToLower();//case insensitivity
        itemLookup.TryGetValue(itemId, out var item);
        return item;
    }
    public ItemEffect GetItemEffect(string itemId)
    {
        if (itemEffectsLookup == null)
            Initialize();
        itemId = itemId.ToLower();//case insensitivity
        itemEffectsLookup.TryGetValue(itemId, out var item);
        return item;
    }
    public WeaponData GetWeaponData(string itemId)
    {
        if (itemEffectsLookup == null)
            Initialize();
        itemId = itemId.ToLower();//case insensitivity
        weaponDetailsLookup.TryGetValue(itemId, out var data);
        return data;
    }
    public TinkerComponentData GetTinkerComponentData(string itemId)
    {
        if (itemEffectsLookup == null)
            Initialize();
        itemId = itemId.ToLower();//case insensitivity
        componentDetailsLookup.TryGetValue(itemId, out var data);
        return data;
    }
}
