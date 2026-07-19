using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockableDoorInteractable : Interactable
{
    private bool hasUnlocked = false;
    public GameObject lockedDoorLight;
    public GameObject leftDoor;
    public GameObject rightDoor;
    public bool isLocked = true;
    private float currentDoorOpenTimer = 0f;
    private float maximumDoorOpenTimer = 3f;
    private float doorEndOfStopTimer = 0.5f;
    public float doorOpenDistance = 1.5f;
    public bool needsKey = false;
    public AudioSource audioSource;

    protected override void Start()
    {
        base.Start();
        interactableText = "Open Door";
        audioSource = GetComponent<AudioSource>();
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

                //Turn off Red Locked Light Bar
                lockedDoorLight.SetActive(false);

                hasUnlocked = true;
                StartCoroutine(OpenDoorOverTime());
                WorldSoundFXManager worldSoundFXManager = WorldSoundFXManager.instance;
                AudioClip unlockSFX = worldSoundFXManager.ChooseRandomSFXFromArray(worldSoundFXManager.ruinsDoorOpenSFX);
                worldSoundFXManager.PlayAdvancedSoundFX(audioSource, unlockSFX, 1f, unlockSFX.length/(maximumDoorOpenTimer + doorEndOfStopTimer), false);

                IdeaCameraController ideaCameraController = IdeaCameraController.instance;
                AudioClip unlockBeepSFX = worldSoundFXManager.ChooseRandomSFXFromArray(ideaCameraController.steveAudioClipAffirmative);
                worldSoundFXManager.PlayAdvancedSoundFX(PlayerCamera.instance.player.playerSoundFXManager.audioSource, unlockBeepSFX);
            }
        }

    }

    IEnumerator OpenDoorOverTime()
    {
        while (currentDoorOpenTimer < maximumDoorOpenTimer)
        {
            currentDoorOpenTimer += Time.deltaTime;
            var doorXOffset = (doorOpenDistance / maximumDoorOpenTimer) * Time.deltaTime;

            //Move Left Door
            leftDoor.transform.position = new Vector3(leftDoor.transform.position.x + doorXOffset, leftDoor.transform.position.y, leftDoor.transform.position.z);

            //Move Right Door
            rightDoor.transform.position = new Vector3(rightDoor.transform.position.x - doorXOffset, rightDoor.transform.position.y, rightDoor.transform.position.z);

            yield return null;
        }

    }

}
