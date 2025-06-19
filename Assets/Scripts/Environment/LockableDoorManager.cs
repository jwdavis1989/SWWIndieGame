using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockableDoorManager : MonoBehaviour
{
    private bool hasUnlocked = false;
    public GameObject lockedDoorLight;
    public GameObject leftDoor;
    public GameObject rightDoor;
    public bool isLocked = true;
    private float currentDoorOpenTimer = 0f;
    private float maximumDoorOpenTimer = 3f;
    public float doorOpenDistance = 1.5f;

    // Update is called once per frame
    void Update()
    {
        if (!isLocked && !hasUnlocked)
        {
            lockedDoorLight.SetActive(false);
            hasUnlocked = true;
            StartCoroutine(OpenDoorOverTime());
        }
    }
    
    IEnumerator OpenDoorOverTime ()
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
