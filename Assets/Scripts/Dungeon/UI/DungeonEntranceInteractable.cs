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
        interactableText = "Enter " + dungeon_id;
        dungeonDatabase = DungeonManager.GetDB();
        DungeonData dungeonData = dungeonDatabase.GetDungeon(dungeon_id);
        interactableText = "Enter " + dungeonData.dungeonName;
    }
    public override void Interact(PlayerManager player)
    {
        base.Interact(player);
        TeleportData.playerManager.TeleportPlayerToSceneAndCoordinates(8);
    }
}
