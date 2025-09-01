using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoseQuartz : InventoryItem
{
    public override void HandlePickup(GameObject player)
    {
        RevealEnemies();
        //player.GetComponent<Inventory>().items.Add(this);
        Destroy(gameObject);
    }
    public void RevealEnemies()
    {
        MiniMapRevealCollider[] minimapRevealers = FindObjectsOfType<MiniMapRevealCollider>();
        foreach (MiniMapRevealCollider revealer in minimapRevealers)
        {
            //TODO Check if enemy or wall
            revealer.Reveal();
        }
    }
}
