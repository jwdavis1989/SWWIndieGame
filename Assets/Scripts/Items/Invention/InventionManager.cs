using Palmmedia.ReportGenerator.Core.Common;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;

public class InventionManager : MonoBehaviour
{
    //singleton
    public static InventionManager instance;

    [Header("All possible inventions")]
    public InventionScript[] allInventions;
    [Header("All current idea info")]
    //MAJOR TODO Make IdeaStats object with image, flag & name...
    public bool [] ideaObtainedFlags = new bool[(int)IdeaType.IDEAS_SIZE];
    public byte[][] ideaImages = new byte[(int)IdeaType.IDEAS_SIZE][];

    //helpful references
    private PlayerManager player;

    
    //TODO - Handle saving and loading of inventions
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
        ideaObtainedFlags = new bool[(int)IdeaType.IDEAS_SIZE];
        ideaImages = new byte[(int)IdeaType.IDEAS_SIZE][];
        player = GameObject.Find("Player").GetComponent<PlayerManager>();
        StartCoroutine(CheckForSavedIdeas());
        DontDestroyOnLoad(gameObject);
    }

    //INVENTION
    /** returns true if the player has aquired the upgrade */
    public bool CheckHasUpgrade(InventionType inventType)
    {
        return allInventions[(int)inventType].hasObtained;
    }
    /** flag that this invention type has been aquired */
    public void SetHasUpgrade(InventionType inventType)
    {
        allInventions[(int)inventType].hasObtained = true;
    }

    /** returns image for idea type */
    public byte[] GetIdeaPicture(IdeaType ideaType)
    {
        return ideaImages[(int)ideaType];
    }
    public void SetIdeaPicture(byte[] ideaPicture, IdeaType idea)
    {
        ideaImages[(int)idea]= ideaPicture;
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
    /** loads idea images from current save slot */
    public IEnumerator CheckForSavedIdeas()
    {
        for (int i = 0; i < ideaObtainedFlags.Length; i++)
        {
            ideaObtainedFlags[i] = false;
            //Load from save data
            string saveFileName = Application.persistentDataPath + "/" + player.playerStatsManager.characterName + WorldSaveGameManager.instance.currentCharacterSlotBeingUsed + (IdeaType)i + ".png";
            if (File.Exists(saveFileName))
            {
                ideaObtainedFlags[i] = true;
                byte[] bytes = System.IO.File.ReadAllBytes(saveFileName);
                ideaImages[i] = bytes;
            }
            //else Debug.Log("File dont exist " + saveFileName);
        }
        yield return null;

    }
    /** saves idea images to current save slot */
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
    /**
     * Clear component list and reload it with current values
     */
    public InventionScript CheckForInvention(IdeaType idea1, IdeaType idea2, IdeaType idea3)
    {
        //used for returning an invention with only 2 matches
        bool halfFound = false;
        InventionScript halfAnswer = null;
        foreach (InventionScript inventionScript in allInventions)
        {
            if (CheckHasUpgrade(inventionScript.type))
            {
                continue; // skip already invented
            }
            int ideaMatches = 0;
            foreach (IdeaType neededIdea in inventionScript.neededIdeas)
            {
                if (idea1 == neededIdea) ideaMatches++;
                else if (idea2 == neededIdea) ideaMatches++;
                else if (idea3 == neededIdea) ideaMatches++;
            }
            if (ideaMatches == 2)
            {
                //half invention found
                halfFound = true;
                halfAnswer = inventionScript;
            }
            else if (ideaMatches == 3)
            {
                //new invention found
                return inventionScript;
            }
        }
        if(halfFound) 
            return halfAnswer;
        return null;
    }

    public void HandleNewInvention(InventionScript newInvention)
    {
        foreach (InventionScript invention in allInventions)
        {
            if (invention.type == newInvention.type)
            {
                Debug.Log("Invented "+ invention.type);//astest
                invention.hasObtained = true;
                HandleNewInventionType(newInvention.type);
                return;
            }
        }
        Debug.Log("Unhandled Invention");
    }
    /** Handles immediate effects of new invention types */
    public void HandleNewInventionType(InventionType newInventionType)
    {
        switch (newInventionType)
        {
             case InventionType.QuickChargeCapacitory :
                 break;
             case InventionType.PredictiveNeuralLink :
                //No immediate effects. Check using InventionManager.instance.CheckHasUpgrade(InventionType.PredictiveNeuralLink);
                break;
             case InventionType.IcarausBoosters :
                 break;
             case InventionType.TreasureScanner :
                //No immediate effects. Check using InventionManager.instance.CheckHasUpgrade(InventionType.TreasureScanner);
                break;
             case InventionType.GolemEndoplating :
                player.characterStatsManager.fortitude += 1;
                player.characterStatsManager.SetNewMaxHealthValue();
                break;
             case InventionType.Alternator :
                player.characterStatsManager.endurance += 1;
                player.characterStatsManager.SetNewMaxStaminaValue();
                break;
             case InventionType.RollerJoints :
                //No immediate effects. Check using InventionManager.instance.CheckHasUpgrade(InventionType.RollerJoints);
                break;
             case InventionType.EnemyRadar :
                 break;
            case InventionType.DaedalusNanoMaterials :
                break;
            default:
                Debug.Log("Unhandled Invent Type");
                break;
        }
    }
}
