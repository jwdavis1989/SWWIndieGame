using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Items/Inventory Item Details")]

public class InventoryItemDetails : ScriptableObject
{
    [Header("InventoryItemDetails contains unchanging info about items. " +
        "\nI.e. Information that does not change such as the description. " +
        "\nThis is so that it can be kept as a single copy and looked up by name. ")]
    public string itemName;
    public string description;
    public Sprite icon;
}
