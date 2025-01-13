using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class InventionManager : MonoBehaviour
{
    public InventionScript[] allInventions;
    private bool [] ideaObtainedFlags = new bool[(int)IdeaType.MAX - 1];
    //TODO - Handle saving and loading of inventions
    private PlayerManager player;
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
        player = GameObject.Find("Player").GetComponent<PlayerManager>();
        CheckForSavedIdeas();
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
    public void CheckForSavedIdeas()
    {
        for (int i = 0; i < ideaObtainedFlags.Length; i++)
        {
            ideaObtainedFlags[i] = false;
            //Load from save data - TODO save place will prolly change... add save slot to name?
            string saveFileName = Application.dataPath + "/" + player.playerStatsManager.characterName + (IdeaType)i + ".png";
            if (File.Exists(saveFileName))
            {
                //Debug.Log("File exist for " + (IdeaType)i);//astest
                ideaObtainedFlags[i] = true;
            }
            //else Debug.Log("File dont exist " + saveFileName);//astest
        }

    }
}
