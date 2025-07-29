using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingableDoorInteractable : Interactable
{
    [Header("Usage: Place on an empty then add a door or two to rotate." +
        "\nDoor must already rotate on hinge\n")]
    public bool isClosed = true;
    //private bool hasUnlocked = false;
    [Header("Swings to negative angle")]
    public GameObject leftDoor;
    [Header("Swings to positive angle")]
    public GameObject rightDoor;
    private float currentDoorOpenTimer = 0f;
    private float maximumDoorOpenTimer = 1.1f;
    public float doorOpenAngle = 90f;
    [Header("TODO")]
    public bool needsKey = false;

    protected override void Start()
    {
        base.Start();
        if (isClosed)
            interactableText = "Open";
        else interactableText = "Close";
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
            StartCoroutine(OpenDoorOverTime());
        }

    }

    IEnumerator OpenDoorOverTime()
    {
        while (currentDoorOpenTimer < maximumDoorOpenTimer)
        {
            currentDoorOpenTimer += Time.deltaTime;
            var doorYOffset = (doorOpenAngle / maximumDoorOpenTimer) * Time.deltaTime;
            if(!isClosed)
                doorYOffset = -doorYOffset;
            //TODO Allow closing of door by negating 
            //Move Left Door
            if (leftDoor != null)
            {
                leftDoor.transform.Rotate(new Vector3(0.0f, -doorYOffset, 0f));
            }

            //Move Right Door
            if(rightDoor != null)
                rightDoor.transform.Rotate(new Vector3(0.0f, doorYOffset, 0f));

            yield return null;
        }
        isClosed = !isClosed;
        if (isClosed)
            interactableText = "Open";
        else interactableText = "Close";
        currentDoorOpenTimer = 0;
        interactableCollider.enabled = true;
    }

}
