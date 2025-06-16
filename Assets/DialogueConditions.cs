using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueConditions : MonoBehaviour
{
    public bool canSeeDialogue = false;

    public void CheckFirstInvention()
    {
        canSeeDialogue = InventionManager.instance.CheckHasUpgrade(InventionType.QuickChargeCapacitory);
    }
    public void CheckFirstIdean()
    {
        canSeeDialogue = InventionManager.instance.CheckHasIdea(IdeaType.Golem);
    }
}