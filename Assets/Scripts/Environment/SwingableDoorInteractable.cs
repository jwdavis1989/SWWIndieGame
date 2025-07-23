using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingableDoorInteractable : Interactable
{
    private bool hasUnlocked = false;
    public GameObject leftDoor;
    public GameObject rightDoor;
    public bool isLocked = true;
    private float currentDoorOpenTimer = 0f;
    private float maximumDoorOpenTimer = 3f;
    public float doorOpenAngle = 90f;
    public bool needsKey = false;

    protected override void Start()
    {
        base.Start();
        interactableText = "Open Door";
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
            if (!hasUnlocked)
            {
                isLocked = false;

                hasUnlocked = true;
                StartCoroutine(OpenDoorOverTime());
            }
        }

    }

    IEnumerator OpenDoorOverTime()
    {
        while (currentDoorOpenTimer < maximumDoorOpenTimer)
        {
            currentDoorOpenTimer += Time.deltaTime;
            var doorYOffset = (doorOpenAngle / maximumDoorOpenTimer) * Time.deltaTime;

            //Move Left Door
            leftDoor.transform.Rotate(new Vector3(0.0f, -doorYOffset, 0f));

            //Move Right Door
            rightDoor.transform.Rotate(new Vector3(0.0f, doorYOffset, 0f));

            yield return null;
        }

    }

}
