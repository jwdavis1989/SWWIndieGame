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
        foreach(InventionScript invention in InventionManager.instance.allInventions)
        {
            if (invention.hasObtained)
            {
                canSeeDialogue=true;
                return;
            }
        }
    }
    public void CheckFirstIdea()
    {
        canSeeDialogue = InventionManager.instance.CheckHasIdea(IdeaType.MetalPlating);
    }
}