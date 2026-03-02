using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    //list of items
    //public List<InventoryItem> items;
    public Dictionary<string, InventoryItem> items = new Dictionary<string, InventoryItem>();
    public InventionManager inventionManager; //Reference to tinker components
    public CharacterWeaponManager weapons;//Reference to weapons list
    public List<WeaponSalvageComponent> weaponSalvageComponents;

    //quickslots, simply storing the item name
    public const int TOTAL_QUICKSLOTS = 4;
    public string[] quickSlotItems = new string[TOTAL_QUICKSLOTS];

    public InventoryItem GetItem(string itemId)
    {
        return items[itemId];
    }
    public string GetQuickSlotItemId(int quickslot)
    {
        return quickSlotItems[quickslot];
    }
    /** @returns owned quantiy of an item */
    public int CheckOwnedQty(string itemId)
    {
        if (items.ContainsKey(itemId))
            return items[itemId].quantity;
        return 0;
    }
    /** Attempts to use an item */
    public void UseItem(string itemId)
    {
        ItemEffect itemEffect = ItemDropManager.instance.itemDatabase.GetItemEffect(itemId);
        ItemDetails itemDetails = ItemDropManager.instance.itemDatabase.GetItem(itemId);
        if (itemEffect != null)
        {
            GetComponent<PlayerEffectsManager>().ProcessInstantEffect(itemEffect);
            if (itemDetails.itemType.ToLower().Equals("consumable"))
                items[itemId].quantity--;
        }
    }
    /** Returns owned tinker component items */
    public Dictionary<string,InventoryItem> GetTinkerComponents()
    {
        //filter items
        int i = 0;
        return items.Where((kvp) =>
        {
            ItemDetails itemDetails = ItemDropManager.instance.itemDatabase.GetItem(kvp.Key);
            Debug.Log("i=" + i++);
            if(itemDetails == null) 
                return false; // No details for this item. Skip it
            return itemDetails.itemType.ToLower().Equals("component");
        })
        .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }
    public void LoadInventory(List<InventoryItem> savedItems)
    {
        items = new Dictionary<string, InventoryItem> ();
        foreach (InventoryItem item in savedItems)
        {
            if (!items.ContainsKey(item.itemId.ToLower()))
            {
                items.Add(item.itemId.ToLower(), item);
            }
            else
            {
                Debug.LogWarning($"Duplicate itemId: {item.itemId}");
            }
        }
    }
    public List<InventoryItem> SaveItems()
    {

        return items.Values.ToList();
    }
}
[Serializable]
public class InventoryItem
{
    public string itemId;
    public int quantity;
}