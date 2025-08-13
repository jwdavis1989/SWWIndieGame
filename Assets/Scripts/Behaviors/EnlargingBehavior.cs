using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnlargingBehavior : MonoBehaviour
{
    public float startingScale = 1;
    public float maxScale = 1.5f;
    public float enlargeTime = 1f;

    private float enlargeTimer = 0;
    // Update is called once per frame
    void Update()
    {
        if (enlargeTimer <= enlargeTime)
        {
            enlargeTimer += Time.deltaTime;
            float newScale = startingScale + (enlargeTimer/enlargeTime) * (maxScale-startingScale);
            Vector3 newScaling = new Vector3(newScale, newScale, newScale);
            gameObject.transform.localScale = newScaling;
        }
    }
}
