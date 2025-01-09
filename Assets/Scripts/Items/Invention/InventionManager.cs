using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventionManager : MonoBehaviour
{
    public InventionScript[] allInventions;
    public IdeaScript allIdeas;
    //TODO - Handle saving and loading of inventions
    public static InventionManager instance;
    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }


    }

    /** returns true if the player has aquired the upgrade */
    public bool CheckHasUpgrade(InventionType type)
    {
        foreach (var item in allInventions) 
            if (item.type == type && item.hasObtained) return true;
        return false;
    }
    /** returns true if the player has aquired the upgrade assuming allInventions is ordered */
    public bool CheckHasUpgradeOrdered(InventionType type)
    {
        return allInventions[(int)type].hasObtained;
    }
}
