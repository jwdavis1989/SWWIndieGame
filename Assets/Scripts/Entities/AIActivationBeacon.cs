using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIActivationBeacon : MonoBehaviour
{
    [SerializeField] AICharacterManager beaconOwner;

    public void SetOwnerOfBeacon(AICharacterManager newOwner)
    {
        beaconOwner = newOwner;
    }

    public void ReactivateAICharacter()
    {
        if (beaconOwner == null)
        {
            return;
        }

        beaconOwner.ActivateCharacter();
    }

    private void OnTriggerEnter(Collider other)
    {
        BeaconDetector detector = other.GetComponent<BeaconDetector>();

        if (detector == null)
        {
            return;
        }

        ReactivateAICharacter();
    }

}
