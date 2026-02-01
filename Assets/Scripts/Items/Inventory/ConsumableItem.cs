using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

public class ConsumableItem : InventoryItem
{
    public InstantCharacterEffect effect;
    public override void HandlePickup(GameObject player)
    { 
        base.HandlePickup(player);
    }
    public void Consume(GameObject player)
    {
        player.GetComponent<PlayerEffectsManager>().ProcessInstantEffect(effect);
        quantity--;
        if(quantity <= 0)
        {
            player.GetComponent<Inventory>().items.Remove(itemName);
            Destroy(gameObject);
        }
    }

}
