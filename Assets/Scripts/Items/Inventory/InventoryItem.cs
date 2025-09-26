using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InventoryItem : MonoBehaviour
{
    public string itemName;
    public string description;
    public int quantity = 0;
    public Sprite icon;
    public GameObject dropableItem;

    public virtual void HandlePickup(GameObject player)
    {
        player.GetComponent<Inventory>().items.Add(this);
        Destroy(gameObject);
    }
}
