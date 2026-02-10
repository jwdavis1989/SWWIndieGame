using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InventoryItem : MonoBehaviour
{
    public string itemId;
    public int quantity = 0;
    public DateTime aquireTime;

    public virtual void HandlePickup(GameObject player)
    {
        HoverOverHead(player);
        AddToInventory(player.GetComponent<Inventory>());
    }
    public const float HOVER_HEIGHT = 2.25f;
    public void HoverOverHead(GameObject player)
    {
        //hover above head
        transform.SetParent(player.transform);
        Vector3 pos = player.transform.position;
        pos = new Vector3(pos.x, pos.y + HOVER_HEIGHT, pos.z);
        transform.position = pos;
        //deactivate movement
        if(gameObject.GetComponentInChildren<ItemDropFloatCollider>() != null)
            gameObject.GetComponentInChildren<ItemDropFloatCollider>().enabled = false;
        if(gameObject.GetComponent<BoxCollider>() != null)
            gameObject.GetComponent<BoxCollider>().enabled = false;
        Rigidbody rb = GetComponent<Rigidbody>();
        if(rb != null)
        {
            rb.velocity = new Vector3(0, 0, 0);
            rb.useGravity = false;
        }
    }
    public void AddToInventory(Inventory inventory)
    {
        // make sure quantity is at least 1 when picking up
        quantity = (quantity > 0) ? quantity : 1;

        if (inventory.items.ContainsKey(itemId))
        {   // update inventory quantity
            inventory.items[itemId].quantity += quantity;
            StartCoroutine(DestroyAfterDelay());
        }
        else
        {   // add item to inventory
            aquireTime = DateTime.UtcNow;
            inventory.items.Add(itemId, this);
            StartCoroutine(HideAfterDelay());
        }
    }
    public const float DESTROY_DELAY = 2.0f;
    IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(DESTROY_DELAY);
        Destroy(gameObject);
    }
    IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(DESTROY_DELAY);
        foreach(Transform obj in transform)
        {
            obj.gameObject.SetActive(false);
        }
    }
    public virtual void Use(GameObject player) { }

    // Possibly useful when added to save/load
    //public string itemType;
    //public void LoadItem(InventoryItem item, Inventory inventory)
    //{
    //    inventory.items.Add
    //}
}
