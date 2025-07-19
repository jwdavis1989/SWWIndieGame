using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorSwing : MonoBehaviour
{
    public float swingForce = 100f; // Adjust force for desired swing speed

    public Rigidbody doorRigidbody;

    void Start()
    {
        //doorRigidbody = GetComponent<Rigidbody>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Check for player interaction
        {
            //doorRigidbody.AddForce(transform.right * swingForce); // Apply force to open
            doorRigidbody.AddTorque(transform.right * swingForce); // Apply force to open
        }
    }
}
