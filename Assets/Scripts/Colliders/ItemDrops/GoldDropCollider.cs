using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GoldDropCollider : MonoBehaviour
{
    public int gold = 0;
    public AudioClip goldClip;//TODO use array?
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerStatsManager>().gold += gold;
            if(goldClip != null)
            {
                other.GetComponent<PlayerManager>().characterSoundFXManager.PlayAdvancedSoundFX(goldClip, 1, 1f, true, 0.1f);
            }
            Destroy(gameObject.GetComponentInParent<Rigidbody>().gameObject);
        }
    }
}
