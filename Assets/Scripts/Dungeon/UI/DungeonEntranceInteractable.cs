using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonEntranceInteractable : Interactable
{
    public string dungeon_id = "dungeon_name";
    DungeonDatabase dungeonDatabase;
    protected override void Awake()
    {
        base.Awake();
        interactableText = "";
        //dungeonDatabase = DungeonManager.instance.dungeonDatabase;
    }
    public override void Interact(PlayerManager player)
    {
        base.Interact(player);
    }
}
