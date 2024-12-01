using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GoldDropCollider : MonoBehaviour
{
    public int gold = 0;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerStatsManager>().gold += gold;
            Destroy(gameObject);
        }
    }
}
