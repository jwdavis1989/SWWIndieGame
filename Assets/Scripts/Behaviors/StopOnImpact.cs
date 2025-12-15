using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopOnImpact : MonoBehaviour
{
    public LayerMask groundLayers;
    void OnCollisionStay(Collision collision)
    {
        //TODO Layer Mask
        if (((1 << collision.gameObject.layer) & groundLayers) != 0)
        //if (collision.gameObject.layer == 0)
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
        }
    }
}
