using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.UI;

public class InventionManager : MonoBehaviour
{
    public InventionScript[] allInventions;
    private bool [] ideaObtainedFlags = new bool[(int)IdeaType.IDEAS_SIZE - 1];
    public byte[][] ideaImages = new byte[(int)IdeaType.IDEAS_SIZE - 1][];
    //TODO - Handle saving and loading of inventions
    private PlayerManager player;
    public static InventionManager instance;
    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerManager>();
        StartCoroutine(CheckForSavedIdeas());
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
    public byte[] GetIdeaPicture(IdeaType ideaType)
    {
        return ideaImages[(int)ideaType];
    }
    public void SetIdeaPicture(byte[] ideaPicture, IdeaType idea)
    {
        ideaImages[(int)idea ]= ideaPicture;
    }
    /** returns true if the player has photograped the idea */
    public bool CheckHasIdea(IdeaType ideaType)
    {
        return ideaObtainedFlags.Length > (int)ideaType && ideaObtainedFlags[(int)ideaType];
    }
    public void SetHasIdea(IdeaType type)
    {
        ideaObtainedFlags[(int)type] = true;
    }
    public IEnumerator CheckForSavedIdeas()
    {
        for (int i = 0; i < ideaObtainedFlags.Length; i++)
        {
            ideaObtainedFlags[i] = false;
            //Load from save data
            string saveFileName = Application.persistentDataPath + "/" + player.playerStatsManager.characterName + WorldSaveGameManager.instance.currentCharacterSlotBeingUsed + (IdeaType)i + ".png";
            if (File.Exists(saveFileName))
            {
                //Debug.Log("File exist for " + (IdeaType)i);//astest
                ideaObtainedFlags[i] = true;
                byte[] bytes = System.IO.File.ReadAllBytes(saveFileName);
                ideaImages[i] = bytes;
            }
            //else Debug.Log("File dont exist " + saveFileName);//astest
        }
        yield return null;

    }
    public void SaveIdeas()
    {
        //save
        for (int i = 0; i < ideaImages.Length; i++)
        {
            if(ideaImages[i] != null)
            {
                string saveFileName = Application.persistentDataPath + "/" + player.playerStatsManager.characterName + WorldSaveGameManager.instance.currentCharacterSlotBeingUsed + (IdeaType)i + ".png";
                //save file for idea
                System.IO.File.WriteAllBytes(saveFileName, ideaImages[i]);
            }
            
        }
    }
}
