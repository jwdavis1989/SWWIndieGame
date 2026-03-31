using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class PickupCollider : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //Debug.Log("PickUpCollider");
            gameObject.GetComponentInParent<PickupableItem>().HandlePickup(other.gameObject);
        }
    }
}
