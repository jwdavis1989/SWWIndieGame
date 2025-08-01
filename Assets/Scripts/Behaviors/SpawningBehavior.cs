using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawningBehavior : MonoBehaviour
{
    public GameObject spawnPrefab;
    [Header("Time to wait before spawn. Resets automatically \n" +
            "Combine with SelfDestructBehavior for a single spawn\n" +
            "Can also set to 0 and call Spawn() in another script")]
    public float spawnInterval= 1;
    private float spawnTime = 0;
    //public void Update()
    //{
    //    spawnTime += Time.deltaTime;
    //    if(spawnTime >= spawnInterval)
    //    {
    //        spawnTime = 0;
    //        Spawn(transform);
    //    }
    //}
    public GameObject Spawn(Vector3 location, Quaternion rotation)
    {
        return Instantiate(spawnPrefab, location, rotation);
    }
    public GameObject Spawn(Transform transform)
    {
        return Instantiate(spawnPrefab, transform);
    }
    public GameObject Spawn()
    {
        return Instantiate(spawnPrefab);
    }
}
