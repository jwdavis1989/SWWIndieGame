using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    //What are Interactable? (Anything you can interact with)
    //Levers
    //Items
    //Fog Walls
    //Elevators

    public string interactableText; //Text Prompt given to player
    [SerializeField] protected Collider interactableCollider; //Collider that checks for player interaction

    protected virtual void Awake()
    {
        //Check if it's null, in some cases you may want to manually assign a collider as a child object (depending on the interactable)
        if (interactableCollider == null)
        {
            interactableCollider = GetComponent<Collider>();
        }
    }

    protected virtual void Start()
    {

    }

    public virtual void Interact(PlayerManager player)
    {
        //Debug.Log("You have interacted.");

        //Only true for one interaction objects, such as opening a locked chest.
        interactableCollider.enabled = false;

        player.playerInteractionManager.RemoveInteractionFromList(this);
        PlayerUIManager.instance.playerUIPopUpManager.CloseAllPopUpWindows();

    }

    public virtual void OnTriggerEnter(Collider other)
    {
        PlayerManager player = other.GetComponent<PlayerManager>();

        if (player != null)
        {
            //Pass the interaction to the player
            player.playerInteractionManager.AddInteractionToList(this);
        }
    }

    public virtual void OnTriggerExit(Collider other)
    {
        PlayerManager player = other.GetComponent<PlayerManager>();

        if (player != null)
        {
            //Remove the interaction from the player
            player.playerInteractionManager.RemoveInteractionFromList(this);

            PlayerUIManager.instance.playerUIPopUpManager.CloseAllPopUpWindows();
        }
    }

}
