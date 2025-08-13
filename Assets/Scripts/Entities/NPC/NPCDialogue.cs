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
    //public string[] lines;
    public Line[] lines;
    public int timesSpokenToMe = 0;
    //public bool firstTime = true;
    public override void Interact(PlayerManager player)
    {
        timesSpokenToMe++;
        //Debug.Log("You have interacted.");

        //Only true for one interaction objects, such as opening a locked chest.
        interactableCollider.enabled = false;
        player.playerInteractionManager.RemoveInteractionFromList(this);
        PlayerUIManager.instance.playerUIPopUpManager.CloseAllPopUpWindows();
        DialogueManager.instance.PlayDialogue(this);
        //Set bool so the Interactable system understands a Pop-Up window has opened
        PlayerUIManager.instance.popUpWindowIsOpen = true;
    }
    public void SetColliderEnabled(bool enabled)
    {
        interactableCollider.enabled = enabled;
    }
}
[Serializable]
public class Line
{
    public string line;
    [Header("Journal key to check before speaking line")]
    public string conditionKey;
    [Header("Condition Event that can be used with DialogueCondition")]
    public UnityEngine.Events.UnityEvent conditionEvent;
}
