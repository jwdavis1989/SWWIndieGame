using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;

public class NPCDialogue : Interactable
{
    [Header("Speaker's name")]
    public string speakerName = "Default Name";
    [Header("Speaker's lines")]
    public string[] lines;
    public Line[] lines2;
    public int timesSpokenToMe = 0;
    //public bool firstTime = true;
    public override void Interact(PlayerManager player)
    {
        //Debug.Log("You have interacted.");

        //Only true for one interaction objects, such as opening a locked chest.
        interactableCollider.enabled = false;
        player.playerInteractionManager.RemoveInteractionFromList(this);
        PlayerUIManager.instance.playerUIPopUpManager.CloseAllPopUpWindows();
        DialogueManager.instance.PlayDialogue(this);
        //Set bool so the Interactable system understands a Pop-Up window has opened
        PlayerUIManager.instance.popUpWindowIsOpen = true;
    }
}
[Serializable]
public class Line
{
    public string line;
    //public UnityEngine.Events.UnityEvent condition;
    public string conditionKey;
}
