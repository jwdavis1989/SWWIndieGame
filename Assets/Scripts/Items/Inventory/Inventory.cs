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
    public Dictionary<string, InventoryItem> inventoryItems = new Dictionary<string, InventoryItem>();
    public InventionManager inventionManager; //Reference to tinker components
    public CharacterWeaponManager weapons;//Reference to weapons list
    public List<WeaponSalvageComponent> weaponSalvageComponents;
    public List<string> weaponTraits = new List<string>();

    //quickslots, simply storing the item name
    public const int TOTAL_QUICKSLOTS = 4;
    public string[] quickSlotItems = new string[TOTAL_QUICKSLOTS];

    public InventoryItem GetItem(string itemId)
    {
        return inventoryItems[itemId];
    }
    public string GetQuickSlotItemId(int quickslot)
    {
        return quickSlotItems[quickslot];
    }
    /** @returns owned quantiy of an item */
    public int CheckOwnedQty(string itemId)
    {
        if (inventoryItems.ContainsKey(itemId))
            return inventoryItems[itemId].quantity;
        return 0;
    }
    /** Attempts to use an item */
    public void UseItem(string itemId)
    {
        ItemEffect itemEffect = ItemDropManager.GetDB().GetItemEffect(itemId);
        if (itemEffect != null)
        {
            //Debug.Log("USING:" + itemId);
            ItemDetails itemDetails = ItemDropManager.GetDB().GetItem(itemId);
            GetComponent<PlayerEffectsManager>().ProcessInstantEffect(itemEffect);
            if (itemDetails.IsConsumable()) {
                //Debug.Log("CONSUME:" + itemId);
                inventoryItems[itemId].quantity--;
            }
            if(quickSlotItems.Contains(itemId) && CheckOwnedQty(itemId) == 0) {
                //Debug.Log("USED UP:" + itemId);
                for (int i = 3; i >= 0; i--) {
                    //Debug.Log("DOES:" + itemId + "=" + quickSlotItems[i]);
                    if (quickSlotItems[i] == itemId) {
                        Debug.Log("REMOVING:" + itemId);
                        quickSlotItems[i] = null;
                    }
                }
            }
        }
    }
    /** Returns owned tinker component items */
    public Dictionary<string,InventoryItem> GetTinkerComponents()
    {
        //filter items
        return inventoryItems.Where((kvp) =>
        {
            ItemDetails itemDetails = ItemDropManager.GetDB().GetItem(kvp.Key);
            if(itemDetails == null) 
                return false; // No details for this item. Skip it
            return itemDetails.itemType.ToLower().Equals("component");
        })
        .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }
    public void LoadInventory(List<InventoryItem> savedItems, List<WeaponSalvageComponent> savedWpnComponents)
    {
        inventoryItems = new Dictionary<string, InventoryItem> ();
        foreach (InventoryItem item in savedItems)
        {
            if (!inventoryItems.ContainsKey(item.itemId.ToLower()))
            {
                inventoryItems.Add(item.itemId.ToLower(), item);
            }
            else
            {
                Debug.LogWarning($"Duplicate itemId: {item.itemId}");
            }
        }
        weaponSalvageComponents = new List<WeaponSalvageComponent>();
        foreach (WeaponSalvageComponent wpnCpmnt in savedWpnComponents)
        {
            weaponSalvageComponents.Add(wpnCpmnt);
        }
    }
    public List<InventoryItem> SaveItems()
    {
        return inventoryItems.Values.ToList();
    }
    public List<WeaponSalvageComponent> SaveWeaponComponents()
    {
        return weaponSalvageComponents;
    }
        public ItemDetails GetItemDetails(string itemId){
        return ItemDropManager.GetDB().GetItem(itemId);
    }
    public Dictionary<string, InventoryItem> GetItemsSorted(string sortType)
    {
        if (string.Equals(sortType, "value", StringComparison.OrdinalIgnoreCase)) {
            return inventoryItems
                .OrderBy(entry =>
                {
                    ItemDetails itemDetails = GetItemDetails(entry.Value.itemId);
                    return itemDetails.cost;
                })
                .ThenBy(entry => entry.Value.itemId)
                .ToDictionary(entry => entry.Key, entry => entry.Value);
        } else if (string.Equals(sortType, "itemType", StringComparison.OrdinalIgnoreCase)) {
            return inventoryItems
                .OrderBy(entry =>
                {
                    ItemDetails itemDetails = GetItemDetails(entry.Value.itemId);
                    return itemDetails.itemType;
                })
                .ThenBy(entry => entry.Value.itemId)
                .ToDictionary(entry => entry.Key, entry => entry.Value);
        } else {
            Debug.LogWarning("Invalid sortType: " + sortType);
            return inventoryItems;
        }
    }
    public Dictionary<string, InventoryItem> GetItemsFilteredByType(string itemType)
    {
        if (string.IsNullOrWhiteSpace(itemType)) {
            Debug.LogWarning("GetItemsFilteredByType called with null/empty itemType.");
            return new Dictionary<string, InventoryItem>();
        }
        return inventoryItems
            .Where(entry =>
            {
                ItemDetails itemDetails = GetItemDetails(entry.Value.itemId);
                return itemDetails != null &&
                       string.Equals(itemDetails.itemType, itemType, StringComparison.OrdinalIgnoreCase);
            })
            .ToDictionary(entry => entry.Key, entry => entry.Value);
    }
    public Dictionary<string, InventoryItem> GetAllItems()
    {
        Dictionary<string, InventoryItem> allItems = new Dictionary<string, InventoryItem>();
        allItems.AddRange(inventoryItems);
        foreach (WeaponSalvageComponent weaponSalvageComponent in weaponSalvageComponents) {

        }
        return inventoryItems;
    }
}
[Serializable]
public class InventoryItem
{
    public string itemId;
    public int quantity = 1;
    [Header("Each of this item is unique. E.g. Weapons, salvage")]
    public bool uniqueItem = false;
}
