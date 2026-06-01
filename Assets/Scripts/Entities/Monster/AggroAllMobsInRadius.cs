using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AggroAllMobsInRadius : MonoBehaviour
{
    [Header("How close the Player will trigger aggro")]
    public float playerCollisionRadius = 3f;

    [Header("How far away enemies will aggro once triggered")]
    public float enemyAggroRadius = 20f;

    [Header("Layer of aggroing enemies")]
    public LayerMask targetEnemyLayer;
    private BoxCollider playerTriggerCollider;

    public PlayerManager playerThatTriggeredAggro;
    
    void Awake()
    {
        playerTriggerCollider = GetComponent<BoxCollider>();
        playerTriggerCollider.isTrigger = true;
        playerTriggerCollider.size = new Vector3(playerCollisionRadius * 2, playerCollisionRadius * 2, playerCollisionRadius * 2);

    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the zone is the Player
        if (other.CompareTag("Player"))
        {
            AggroAllMobs(other);
            playerThatTriggeredAggro = other.GetComponent<PlayerManager>();
        }
    }

    private void AggroAllMobs(Collider playerCollider)
    {
        Collider[] enemyColliders = Physics.OverlapSphere(transform.position, enemyAggroRadius, targetEnemyLayer);
        List<CharacterManager> aggrodCharacterManagers = new List<CharacterManager>();

        //Add all unique characters collided with to our list of aggro'ing characters
        foreach (var hitCollider in enemyColliders)
        {
            CharacterManager character = hitCollider.GetComponentInParent<CharacterManager>();

            //Only add a character if they both exist and aren't already in our List of aggro'ing characters
            if (character != null && !aggrodCharacterManagers.Contains(character))
            {
                aggrodCharacterManagers.Add(character);
                Debug.Log("Aggro'ing: " + aggrodCharacterManagers);
            }
        }

        //Set each aggro'ing character's lock on to the player, causing their state machine to swap to their pursueTargetState
        foreach (var aggrodCharacter in aggrodCharacterManagers)
        {
            if (!aggrodCharacter.isPlayer)
            {
                aggrodCharacter.characterCombatManager.currentTarget = playerCollider.GetComponent<CharacterManager>();
            }
        }

        //Deactivate Collider for optimization
        playerTriggerCollider.enabled = false;

    }

    // Visualizes the Aggro Radius in the Editor for easier balancing
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, enemyAggroRadius);
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector3(playerCollisionRadius * 2, playerCollisionRadius * 2, playerCollisionRadius * 2));
    }


}
