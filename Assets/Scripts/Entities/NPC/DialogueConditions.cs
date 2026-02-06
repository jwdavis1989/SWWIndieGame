using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class DialogueConditions : MonoBehaviour
{
    public bool canSeeDialogue = false;
    public void AlwaysTrue()
    {
        canSeeDialogue = true;
    }
    public void CheckFirstInvention()
    {
        foreach(InventionData invention in InventionManager.instance.inventionDatabase.inventions) { 
            if (invention.hasObtained)
            {
                canSeeDialogue=true;
                return;
            }
        }
    }
    public void CheckFirstIdea()
    {
        canSeeDialogue = InventionManager.instance.CheckHasIdea(IdeaID.METAL_PLATING);
    }
    public void CheckHasNotOpenedInventMenu()
    {
        canSeeDialogue = !JournalManager.instance.journalFlags.ContainsKey(JournalManager.hasOpenedInventMenuKey);
    }
    public void CheckIsFirst(NPCDialogue dialogue)
    {
        canSeeDialogue = dialogue.timesSpokenToMe <= 1;
    }
    public void CheckNotFirstDialogue(NPCDialogue dialogue)
    {
        canSeeDialogue = dialogue.timesSpokenToMe > 1;
    }
}