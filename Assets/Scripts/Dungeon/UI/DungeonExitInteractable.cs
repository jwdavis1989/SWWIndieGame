using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonExitInteractable : Interactable
{
    public string dungeon_id = "dungeon_id";
    public string key_id = "none";

    [Header("Used for floors with multiple connecting floors. \n" +
        "If blank all connecting floors will be unlocked")]
    public string connectedFloorId = "";

    DungeonData dungeonData;
    private float elapsedTime = 0f;
    protected override void Awake()
    {
        base.Awake();
        dungeonData = DungeonManager.GetDB().GetDungeon(dungeon_id);
        interactableText = "Exit level";
    }
    public void Update()
    {
        elapsedTime += Time.deltaTime;
    }
    public override void Interact(PlayerManager player)
    {
        base.Interact(player);
        if (player.GetComponent<Inventory>().CheckOwnedQty(key_id) > 0)
        {
            player.GetComponent<Inventory>().GetItem(key_id).quantity--;
            Open();
        }
        else
            SetColliderEnabled(true);
    }
    public override void OnTriggerEnter(Collider other)
    {
        PlayerManager player = other.GetComponent<PlayerManager>();

        if (player != null)
        {
            //Pass the interaction to the player
            player.playerInteractionManager.AddInteractionToList(this);
            if (player.GetComponent<Inventory>().CheckOwnedQty(key_id) > 0)
            {
                interactableText = "Exit dungeon";
            }
            else
            {
                interactableText = "Locked";
            }
        }
    }
    public void SetColliderEnabled(bool enabled)
    {
        interactableCollider.enabled = enabled;
    }
    public void Open()
    {
        //DungeonData dungeonData = DungeonManager.GetDB().GetDungeon(dungeon_id);
        string levelSelectScene = dungeonData.dungeonLevelSelectSceneID;
        DungeonManager.CompleteCurrentDungeonLevel(elapsedTime, connectedFloorId);
        //DungeonManager
        PlayerInputManager.instance.SafeDisable(true, true);
        TeleportData.yRotation = dungeonData.exitYRotation;
        TeleportData.playerManager.TeleportPlayerToSceneAndCoordinates(0, 0, 0, 0, levelSelectScene, false);
    }
}
