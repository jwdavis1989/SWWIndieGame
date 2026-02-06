using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

public class UsableItem : InventoryItem
{
    public InstantCharacterEffect effect;
    public bool consumable = false;
    public override void HandlePickup(GameObject player)
    { 
        base.HandlePickup(player);
    }
    public override void Use(GameObject player)
    {
        player.GetComponent<PlayerEffectsManager>().ProcessInstantEffect(effect);
        if (consumable)
        {
            Debug.Log("Using " + itemId);//astest
            quantity--;
            if (quantity <= 0)
            {
                player.GetComponent<Inventory>().items.Remove(itemId);
                Destroy(gameObject);
            }
        }
    }

}
