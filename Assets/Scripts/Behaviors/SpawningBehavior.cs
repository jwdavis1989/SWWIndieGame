using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class SpawningBehavior : MonoBehaviour
{
    [Header("SpawningBehavior can automate spawning or be provide spawn api to another script\n\n" +
        "Prefab to spawn")]
    public GameObject spawnPrefab;
    [Header("Maximum currently existing spawn")]
    public int max = 1;
    [Header("Randomized radius to spawn to")]
    public float distance = 0f;
    public List<GameObject> spawnList;
    [Header("Continuously spawn automatically")]
    public bool auto = false;
    [Header("Time to wait between spawns")]
    public float spawnInterval = 1;
    private float spawnTime = 0;
    public void Update()
    {
        if (!auto) 
            return;
        else
            spawnTime += Time.deltaTime;

        if (spawnTime >= spawnInterval)
        {
            spawnTime = 0;
            spawnList.RemoveAll(item => item == null);
            if (spawnList.Count < max)
            {
                Vector3 spawnPos = transform.position;
                if (distance > 0)
                {
                    //calculate randomized positon
                    float x = spawnPos.x + Random.Range(0, distance) * (Random.Range(0, 10) >= 5 ? -1 : 1);
                    float z = spawnPos.z + Random.Range(0, distance) * (Random.Range(0, 10) >= 5 ? -1 : 1);
                    Vector3 direction = transform.forward;
                    RaycastHit hitInfo = new RaycastHit();
                    if (Physics.Raycast(transform.position, direction, out hitInfo, distance))
                    {
                        spawnPos = hitInfo.point;
                    }
                    else
                    {
                        spawnPos.x = x;
                        spawnPos.z = z;
                    }
                }
                Spawn(spawnPos, transform.rotation);
            }
        }
    }
    /** Spawns at location, checks max */
    public GameObject Spawn(Vector3 location, Quaternion rotation)
    {
        if (spawnList.Count < max)
        {
            spawnList.Add(Instantiate(spawnPrefab, location, rotation));
            return spawnList[spawnList.Count - 1];
        }
        return null;
    }
    /** Spawns as child of transform, checks max */
    public GameObject Spawn(Transform transform)
    {
        if (spawnList.Count < max)
        {
            spawnList.Add(Instantiate(spawnPrefab, transform));
            return spawnList[spawnList.Count - 1];
        }
        return null;
    }
    /** Spawns at origin, checks max */
    public GameObject Spawn()
    {
        if (spawnList.Count < max)
        {
            spawnList.Add(Instantiate(spawnPrefab));
            return spawnList[spawnList.Count - 1];
        }
        return null;
    }
}
