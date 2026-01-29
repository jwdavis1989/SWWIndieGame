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
        HoverOverHead(player);
        StartCoroutine(DestroyAfterDelay());
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
        //gameObject.GetComponent<Rigidbody>().enabled = false;
        Rigidbody rb = GetComponent<Rigidbody>();
        if(rb != null)
        {
            rb.velocity = new Vector3(0, 0, 0);
            rb.useGravity = false;
        }
    }
    public const float DESTROY_DELAY = 2.0f;
    protected IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(DESTROY_DELAY);
        Destroy(gameObject);
    }
}
