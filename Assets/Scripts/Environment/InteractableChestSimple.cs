using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableChestSimple : Interactable
{
    public bool isClosed = true;
    //private bool hasUnlocked = false;
    [Header("Door object to rotate")]
    public GameObject door;
    private float currentDoorOpenTimer = 0f;
    private float maximumDoorOpenTimer = 0.5f;
    public float doorOpenAngle = 90f;
    [Header("Minimap Icon")]
    public GameObject minimapIcon;
    [Header("Item's dropped")]
    public List<GameObject> contents = new List<GameObject>();

    [Header("Sound")]
    public AudioClip chestSound;//TODO use array?

    [Header("Lock & Key")]
    public bool needsKey = false;
    public string key_id = "brass_key";
    protected override void Start()
    {
        base.Start();
        if (isClosed)
            interactableText = "Open";
        //else interactableText = "Close";
        SetColliderEnabled(true);
    }
    public override void Interact(PlayerManager player)
    {
        base.Interact(player);

        if (needsKey)
        { // needing a key
            if (player.GetComponent<Inventory>().CheckOwnedQty(key_id) > 0)
            {
                player.GetComponent<Inventory>().GetItem(key_id).quantity--;
                SuccessfullyOpen(player);
            }
            else
                SetColliderEnabled(true);
        }
        else
        {
            SuccessfullyOpen(player);
        }

    }
    void SuccessfullyOpen(PlayerManager player)
    {
        //play sound
        if (chestSound != null)
            player.characterSoundFXManager.PlayAdvancedSoundFX(chestSound, 1, 1f, true, 0.05f);
        //remove minimap icon
        if (minimapIcon != null)
            Destroy(minimapIcon);
        //disable interactable
        SetColliderEnabled(false);
        //open animation
        StartCoroutine(OpenDoorOverTime());
        HandleLootTable();
    }

    IEnumerator OpenDoorOverTime()
    {
        while (currentDoorOpenTimer < maximumDoorOpenTimer)
        {
            currentDoorOpenTimer += Time.deltaTime;
            var doorXOffset = (doorOpenAngle / maximumDoorOpenTimer) * Time.deltaTime;
            if (!isClosed)
                doorXOffset = -doorXOffset;
            //Move Door
            if (door != null)
            {
                door.transform.Rotate(new Vector3(-doorXOffset, 0f, 0f));
            }
            yield return null;
        }
        //activate items
        foreach (GameObject item in contents)
        {
            item.SetActive(true);
        }

        //reset values - Unecessary?
        //isClosed = !isClosed;
        //if (isClosed)
        //    interactableText = "Open";
        //else interactableText = "Close";
        //currentDoorOpenTimer = 0;
    }
    public void SetColliderEnabled(bool enabled)
    {
        interactableCollider.enabled = enabled;
    }
    [SerializeField] LootTable lootTable;
    public void HandleLootTable()
    {
        if (lootTable == null) return;
        Debug.Log("HandleLootTable:GetRandomItem:" + lootTable.GetRandomItem().itemId);
    }
}
