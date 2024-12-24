using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldAIManager : MonoBehaviour
{
    public static WorldAIManager instance;
    [Header("Enemies Prefabs")]
    [SerializeField] public GameObject[] enemies;
    [Header("Currently active enemies")]
    [SerializeField] public List<GameObject> spawnedEnemies;
    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void Update()
    {
        
    }
    private IEnumerator WaitForLoadThenSpawnChars()
    {
        while (!SceneManager.GetActiveScene().isLoaded)
        {
            yield return null;
        }
        SpawnAllChars();
    }
    private void SpawnAllChars()
    {
        foreach (GameObject enemy in enemies)
        {
            GameObject charInstance = Instantiate(enemy);
            spawnedEnemies.Add(charInstance);
        }
    }
    private void DespawnAllChars()
    {
        foreach (GameObject enemy in spawnedEnemies)
        {
            Destroy(enemy);
        }
    }
}
