using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BeaconDetector : MonoBehaviour
{
    //Probably don't need, consider removing.
    public PlayerManager player;

    private void OnTriggerEnter(Collider other) {
        
    }

    private void OnTriggerExit(Collider other) {
        if (player == null)
        {
            player = GetComponentInParent<PlayerManager>();
        }

        //Double check to make sure a camera exists before continuing
        if (player == null)
        {
            return;
        }

        AICharacterManager aICharacter = other.GetComponent<AICharacterManager>();

        if (aICharacter != null)
        {
            aICharacter.DeactivateCharacter();
        }

        

    }

}
