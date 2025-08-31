using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoseQuartz : InventoryItem
{
    public override void HandlePickup(GameObject player)
    {
        ReavealEnemies();
        player.GetComponent<Inventory>().items.Add(this);
        Destroy(gameObject);
    }
    public void ReavealEnemies()
    {

    }
}
