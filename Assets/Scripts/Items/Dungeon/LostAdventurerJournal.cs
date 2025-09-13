using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LostAdventurerJournal : InventoryItem
{
    public override void HandlePickup(GameObject player)
    {
        RevealObjects();
        if(player.GetComponent<Inventory>() != null)
            player.GetComponent<Inventory>().items.Add(this);
        Destroy(gameObject);
    }
    public void RevealObjects()
    {
        MiniMapRevealCollider[] minimapRevealers = FindObjectsOfType<MiniMapRevealCollider>();
        foreach (MiniMapRevealCollider revealer in minimapRevealers)
        {
            if(revealer.journalReveal)
                revealer.Reveal();
        }
    }
}
