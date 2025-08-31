using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LostAdventurerJournal : InventoryItem
{
    public override void HandlePickup(GameObject player)
    {
        RevealObjects();
        player.GetComponent<Inventory>().items.Add(this);
        Destroy(gameObject);
    }
    public void RevealObjects()
    {

    }
}
