using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableChestSimple : Interactable
{
    public bool isClosed = true;
    //private bool hasUnlocked = false;
    [Header("Swings to negative angle")]
    public GameObject door;
    private float currentDoorOpenTimer = 0f;
    private float maximumDoorOpenTimer = 0.5f;
    public float doorOpenAngle = 90f;
    [Header("Items. Either set manually or let a separate script set")]
    public List<GameObject> contents = new List<GameObject>();

    [Header("Sound")]
    public AudioClip chestSound;//TODO use array?

    [Header("TODO")]
    public bool needsKey = false;
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
        {
            //TODO: In the future, add a version that handles needing a key!
        }
        else
        {
            if (chestSound != null)
            {
                player.characterSoundFXManager.PlayAdvancedSoundFX(chestSound, 1, 1f, true, 0.05f);
            }
            SetColliderEnabled(false);
            StartCoroutine(OpenDoorOverTime());
        }

    }

    IEnumerator OpenDoorOverTime()
    {
        while (currentDoorOpenTimer < maximumDoorOpenTimer)
        {
            currentDoorOpenTimer += Time.deltaTime;
            var doorXOffset = (doorOpenAngle / maximumDoorOpenTimer) * Time.deltaTime;
            if (!isClosed)
                doorXOffset = -doorXOffset;
            //TODO Allow closing of door by negating 
            //Move Left Door
            if (door != null)
            {
                door.transform.Rotate(new Vector3(-doorXOffset, 0f, 0f));
            }
            yield return null;
        }
        foreach (GameObject item in contents)
        {
            item.SetActive(true);
        }
        isClosed = !isClosed;
        if (isClosed)
            interactableText = "Open";
        else interactableText = "Close";
        currentDoorOpenTimer = 0;
    }
    public void SetColliderEnabled(bool enabled)
    {
        interactableCollider.enabled = enabled;
    }
}
