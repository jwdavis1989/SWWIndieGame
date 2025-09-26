using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class RoseQuartz : InventoryItem
{
    public override void HandlePickup(GameObject player)
    {
        RevealEnemies();
        //player.GetComponent<Inventory>().items.Add(this);
        Destroy(gameObject);
    }
    public void RevealEnemies()
    {
        AICharacterManager[] minimapRevealers = FindObjectsOfType<AICharacterManager>();
        foreach (AICharacterManager aiCharacter in minimapRevealers)
        {
            aiCharacter.miniMapSprite.SetActive(true);
        }
    }
}
