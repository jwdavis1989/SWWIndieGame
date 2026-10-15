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
    public Animator animator;

    [Header("Lock & Key")]
    public bool needsKey = false;
    public string key_id = "chest_key";
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

        if (needsKey) { // needing a key
            if (Open(player)) {
                SuccessfullyOpen(player);
            } else // Can't open
                SetColliderEnabled(true);
        } else {
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
        if(animator != null)
            animator?.SetBool("isOpened", true);
        StartCoroutine(OpenDoorOverTime());
        HandleLootTable();
    }
    // Still used by small chest. Not used by big chest. May depreciate when small chest 
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
        ItemDetails itemDetails = lootTable.GetRandomItem();
        //contents.Add(
        ItemDropManager.instance.DropItemById(itemDetails.itemId, transform);
        //);
        Debug.Log("HandleLootTable:GetRandomItem:" + itemDetails.itemId);

    }
    public override void OnTriggerEnter(Collider other)
    {
        PlayerManager player = other.GetComponent<PlayerManager>();
        if (player != null) {
            //Pass the interaction to the player
            player.playerInteractionManager.AddInteractionToList(this);
            if (needsKey)
            {
                if(CanOpen(player))
                    interactableText = "Unlock";
                else
                    interactableText = "Locked";
            }
        }
    }
    bool CanOpen(PlayerManager player)
    {
        return CheckKeyOrLockpick(player, false);
    }
    bool Open(PlayerManager player)
    {
        return CheckKeyOrLockpick(player, true);
    }
    bool CheckKeyOrLockpick(PlayerManager player, bool willOpen = false)
    {
        if (player.GetComponent<Inventory>().CheckOwnedQty(key_id) > 0)
        { // player has the key 
            if (willOpen)
                player.GetComponent<Inventory>().GetItem(key_id).itemQty--;
            return true;
        }
        // Check for lockpick
        WeaponScript mainHandWeapon = player.characterWeaponManager.GetMainHand();
        WeaponScript offHandWeapon = player.characterWeaponManager.GetOffHand();
        if (DungeonManager.lockpicksUsed < 2 )
        {
            if(mainHandWeapon.HasTrait("lockpick") && mainHandWeapon.lockpickUsed == false)
            {
                if (willOpen)
                {
                    mainHandWeapon.lockpickUsed = true;
                    DungeonManager.lockpicksUsed++;
                }
                return true;
            }
            if(offHandWeapon.HasTrait("lockpick") && offHandWeapon.lockpickUsed == false)
            {
                if (willOpen)
                {
                    offHandWeapon.lockpickUsed = true;
                    DungeonManager.lockpicksUsed++;
                }
                return true;
            }
        }
        return false;
    }
}
