using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    //list of items
    //public List<InventoryItem> items;
    public Dictionary<string, InventoryItem> items = new Dictionary<string, InventoryItem>();
    public InventionManager inventionManager; //Reference to tinker components
    public CharacterWeaponManager weapons;//Reference to weapons list

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
}
