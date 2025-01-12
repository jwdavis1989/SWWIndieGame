using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class InventionManager : MonoBehaviour
{
    public InventionScript[] allInventions;
    private bool [] ideaObtainedFlags = new bool[(int)IdeaType.MAX - 1];
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
    private void Start()
    {
        for (int i = 0; i < ideaObtainedFlags.Length; i++)
        {
            ideaObtainedFlags[i] = false;
            //todo load from save data
        }
    }

    //INVENTION
    /** returns true if the player has aquired the upgrade */
    public bool CheckHasUpgrade(InventionType inventType)
    {
        return allInventions[(int)inventType].hasObtained;
    }
    public void SetHasUpgrade(InventionType inventType)
    {
        allInventions[(int)inventType].hasObtained = true;
    }

    //IDEA
    /** returns true if the player has photograped the idea */
    public bool CheckHasIdea(IdeaType ideaType)
    {
        return ideaObtainedFlags.Length > (int)ideaType && ideaObtainedFlags[(int)ideaType];
    }
    public void SetHasIdea(IdeaType type)
    {
        ideaObtainedFlags[(int)type] = true;
    }
}
