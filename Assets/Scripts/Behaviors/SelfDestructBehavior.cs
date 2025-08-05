using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestructBehavior : MonoBehaviour
{
    [Header("Object will cease to exist after this number of seconds." +
        "\nThis timer will be paused when the game is paused.")]
    public float secondsToLive = 1.0f;
    private float secondsAlive = 0.0f;
    // Update is called once per frame
    void Update()
    {
        secondsAlive += Time.deltaTime;
        if(secondsAlive > secondsToLive)
            Destroy(gameObject);
    }
}
