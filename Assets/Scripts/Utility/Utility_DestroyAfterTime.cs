using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Might be redundant, but keeping for backwards compatability.
public class Utility_DestroyAfterTime : MonoBehaviour
{
    [SerializeField] float timeUntilDestroyed = 5f;

    private void Awake()
    {
        Destroy(gameObject, timeUntilDestroyed);
    }
}
