using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;

public class WorldAIManager : MonoBehaviour
{
    public static WorldAIManager instance;

    [Header("AI Character Prefabs")]
    [SerializeField] List<AICharacterSpawner> aiCharacterSpawners;

    [Header("Currently active enemies")]
    [SerializeField] List<GameObject> spawnedAiCharacters;

    [Header("Debug")]
    [SerializeField] bool debugDespawnCharacters = false;

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

    public void SpawnCharacter(AICharacterSpawner aiCharacterSpawner)
    {
        aiCharacterSpawners.Add(aiCharacterSpawner);
        aiCharacterSpawner.AttemptToSpawnCharacter();
    }

    private void DespawnAllCharacters()
    {
        foreach (GameObject aiCharacter in spawnedAiCharacters)
        {
            if (aiCharacter != null)
            {
                Destroy(aiCharacter);
            }
        }
    }

    private void DisableAllCharacters()
    {
        //Disable GameObjects if Disabled status is true

        //Can be used to disable characters that are far from player to save memory
        //Characters can be split into areas (e.g. Area_00, Area_01, etc.)
    }
    
    
}
