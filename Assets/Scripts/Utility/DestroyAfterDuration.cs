using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterDuration : MonoBehaviour
{
    [SerializeField] float timeUntilDestroyed = 5f;

    private void Awake()
    {
        Destroy(gameObject, timeUntilDestroyed);
    }
    
}
