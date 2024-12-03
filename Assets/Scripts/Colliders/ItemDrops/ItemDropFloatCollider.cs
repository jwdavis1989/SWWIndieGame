using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDropFloatCollider : MonoBehaviour
{
    public float speed = 1.0f;
    private GameObject player = null;

    private void Update()
    {
        if (player != null)
        {
            var step = speed * Time.deltaTime; // calculate distance to move
            speed *= 1.0f + (0.5f * Time.deltaTime);//accelerate
            transform.parent.position = Vector3.MoveTowards(transform.parent.position, player.transform.position, step);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (player == null && other.CompareTag("Player"))
        {
            player = other.gameObject;
        }
    }
}
