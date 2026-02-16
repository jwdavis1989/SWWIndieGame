using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Items/Item Details")]

public class ItemDetails : ScriptableObject
{
    [Header("InventoryItemDetails contains unchanging info about items. " +
        "\nI.e. Information that does not change such as the description. " +
        "\nThis is so that it can be kept as a single copy and looked up by name. ")]
    [Header("Unique I.D.")]
    public string itemId;
    [Header("Display name")]
    public string itemName;
    [Header("Valid: component, usable, consumable, weapon, dungeon")]
    public string itemType;
    public int cost = 1;
    public Sprite icon;
    [TextArea] public string description;
}
