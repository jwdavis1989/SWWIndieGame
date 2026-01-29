using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

public class ConsumableItem : InventoryItem
{
    public InstantCharacterEffect effect;
    public override void HandlePickup(GameObject player)
    { 
        //base.HandlePickup(player);
        player.GetComponent<Inventory>().items.Add(this);
        HoverOverHead(player);
        StartCoroutine(HideAfterDelay());
    }
    IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(DESTROY_DELAY);
        gameObject.SetActive(false);
        //if(dropableItem != null)
        //{
        //    Destroy(dropableItem);
        //}
    }
    public void Consume(GameObject player)
    {
        player.GetComponent<PlayerEffectsManager>().ProcessInstantEffect(effect);
        Destroy(gameObject);
    }

}
