using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Items/Item Details")]

public class ItemDetails : ScriptableObject
{
    [Header("InventoryItemDetails contains unchanging info about items. " +
        "\nI.e. Information that does not change such as the description. " +
        "\nThis is so that it can be kept as a single copy and looked up by name. ")]
    public string itemId;
    public string itemName;
    public int cost = 1;
    public Sprite icon;
    [TextArea] public string description;
}
