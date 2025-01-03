using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldAIManager : MonoBehaviour
{
    public static WorldAIManager instance;

    [Header("AI Character Prefabs")]
    [SerializeField] GameObject[] aiCharacters;

    [Header("Currently active enemies")]
    [SerializeField] List<GameObject> spawnedAiCharacters;

    [Header("Debug")]
    [SerializeField] bool debugSpawnCharacters = false;
    [SerializeField] bool debugDespawnCharacters = false;

    public void Awake() {
        if (instance == null) {
            instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }

    private void Start() {
        StartCoroutine(WaitForSceneToLoadThenSpawnCharacters());
    }

    private void Update() {
        if (debugSpawnCharacters) {
            debugSpawnCharacters = false;
            SpawnAllCharacters();
        }

        if (debugDespawnCharacters) {
            debugDespawnCharacters = false;
            DespawnAllCharacters();
        }
    }

    private IEnumerator WaitForSceneToLoadThenSpawnCharacters() {
        while (!SceneManager.GetActiveScene().isLoaded) {
            yield return null;
        }

        SpawnAllCharacters();
    }

    private void SpawnAllCharacters()
    {
        foreach (GameObject aiCharacter in aiCharacters)
        {
            GameObject instantiatedCharacter = Instantiate(aiCharacter);
            spawnedAiCharacters.Add(instantiatedCharacter);
        }
    }

    private void DespawnAllCharacters() {
        foreach (GameObject aiCharacter in spawnedAiCharacters) {
            if (aiCharacter != null) {
                Destroy(aiCharacter);
            }
        }
    }

    private void DisableAllCharacters() {
        //Disable GameObjects if Disabled status is true

        //Can be used to disable characters that are far from player to save memory
        //Characters can be split into areas (e.g. Area_00, Area_01, etc.)
    }
}
