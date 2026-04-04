using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonEntranceInteractable : Interactable
{
    public string dungeon_id = "dungeon_id";
    public bool isInterior = false;
    public bool isDungeonLevelExit = false;
    public bool needsKey = false;
    public string key_id = "none";
    protected override void Awake()
    {
        base.Awake();
        interactableText = "Enter " + dungeon_id;
        DungeonData dungeonData = DungeonManager.GetDB().GetDungeon(dungeon_id);
        interactableText = "Enter " + dungeonData.dungeonName;
        if (isDungeonLevelExit)
        {
            interactableText = "Exit level";
        }
        else if (isInterior)
        {
            interactableText = "Return to level select";
        }
        else
        {
            interactableText = "Enter " + dungeonData.dungeonName;
        }
    }
    public override void Interact(PlayerManager player)
    {
        base.Interact(player);

        if (needsKey)
        { // needing a key
            if (player.GetComponent<Inventory>().CheckOwnedQty(key_id) > 0)
            {
                player.GetComponent<Inventory>().GetItem(key_id).quantity--;
                Open();
            }
            else
                SetColliderEnabled(true);
        }
        else
        {
            Open();
        } 
    }
    public override void OnTriggerEnter(Collider other)
    {
        PlayerManager player = other.GetComponent<PlayerManager>();

        if (player != null)
        {
            //Pass the interaction to the player
            player.playerInteractionManager.AddInteractionToList(this);
            if (needsKey)
                interactableText = "Locked";
        }
    }
    public void SetColliderEnabled(bool enabled)
    {
        interactableCollider.enabled = enabled;
    }
    public void Open()
    {
        DungeonData dungeonData = DungeonManager.GetDB().GetDungeon(dungeon_id);
        string levelSelectScene = dungeonData.dungeonLevelSelectSceneID;
        if (isDungeonLevelExit)
        {
            DungeonManager.CompleteCurrentDungeonLevel();
        }
        //DungeonManager
        PlayerInputManager.instance.SafeDisable(true, true);
        Time.timeScale = 0;
        TeleportData.playerManager.TeleportPlayerToSceneAndCoordinates(0,0,0,0, levelSelectScene, false);
    }
}
