using Palmmedia.ReportGenerator.Core.Common;
using System;
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
    public InventionDatabase inventionDatabase;
    //public InventionScript[] allInventions; // TODO REPLACE WITH SCRIPTABLE OBJECTS
    [Header("current idea info")]
    public List<IdeaStats> ideas = new List<IdeaStats>();
    //public IdeaDatabase ideaDatabase;

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
        //ideaObtainedFlags = new bool[(int)IdeaType.IDEAS_SIZE];
        //ideaImages = new byte[(int)IdeaType.IDEAS_SIZE][];
        ideas = new List<IdeaStats>();
        player = GameObject.Find("Player").GetComponent<PlayerManager>();
        //StartCoroutine(CheckForSavedIdeas());
        DontDestroyOnLoad(gameObject);
    }

    //INVENTION
    public List<string> SaveInventions()
    {
        //List<string> rv = new List<string>();
        //foreach (var invention in inventionDatabase.inventions)
        //{
        //    if(invention.hasObtained)
        //        rv.Add(invention.inventionId);
        //}
        return inventionDatabase.GetSaveData();
        //bool[] rv = new bool[inventionDatabase.inventions.Count];
        //for (int i = 0; i < inventionDatabase.inventions.Count; i++)
        //{
        //    rv[i] = inventionDatabase.inventions[i] != null && inventionDatabase.inventions[i].hasObtained;
        //}
        //return rv;
    }
    public void LoadInventions(List<string> inventions)
    {
        foreach (string invention in inventions)
        {
            inventionDatabase.SetHasObtained(invention);
        }
    }
    /** returns true if the player has aquired the upgrade */
    public bool CheckHasUpgrade(string inventId)
    {
        return inventionDatabase.GetInvention(inventId).hasObtained;
            //allInventions[(int)inventType].hasObtained;
    }
    /** flag that this invention type has been aquired */
    public void SetHasUpgrade(string inventType)
    {
        inventionDatabase.SetHasObtained(inventType);
        //allInventions[(int)inventType].hasObtained = true;
    }

    /** returns image for idea type */
    public byte[] GetIdeaPicture(string ideaID)
    {
        foreach (var idea in ideas)
        {
            if (idea.ideaID == ideaID)
            {
                return idea.image;
            }
        }
        return null;// ideas[(int)ideaType].image;
    }
    public void SetIdeaPicture(byte[] ideaPicture, string ideaID)
    {
        foreach(var idea in ideas)
        {
            if(idea.ideaID == ideaID)
            {
                idea.image = ideaPicture;
            }
        }
        //ideas[(int)idea].image = ideaPicture;
    }
    /** returns true if the player has photograped the idea */
    public bool CheckHasIdea(string ideaId)
    {
        foreach (IdeaStats idea in ideas)
        {
            if(idea.ideaID == ideaId)
                return true; 
        }
        return false;
        //return ideas.Length > (int)ideaType && ideas[(int)ideaType] != null && ideas[(int)ideaType].obtained;
    }
    public void SetHasIdea(IdeaScript ideaScript)
    {
        foreach(IdeaStats idea in ideas)
        {
            if(idea.ideaID == ideaScript.ideaId)
            {
                idea.obtained = true;
                return;
            }
        }
        IdeaStats ideaStats = new IdeaStats();
        ideaStats.ideaID = ideaScript.ideaId;
        ideaStats.obtained = true;
        ideas.Add(ideaStats);
        //ideas[(int)type] = new IdeaStats();
        //ideas[(int)type].obtained = true;
    }
    /**
     * Clear component list and reload it with current values
     */
    public InventionData CheckForInvention(string idea1, string idea2, string idea3)
    {
        //used for returning an invention with only 2 matches
        bool halfFound = false;
        InventionData halfAnswer = null;
        foreach (InventionData inventionScript in inventionDatabase.inventions)
        {
            if (CheckHasUpgrade(inventionScript.inventionId))
            {
                continue; // skip already invented
            }
            int ideaMatches = 0;
            foreach (string neededIdea in inventionScript.ideas)
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
        if (halfFound)
            return halfAnswer;
        return null;
    }

    public void HandleNewInvention(InventionData newInvention)
    {
        foreach (InventionData invention in inventionDatabase.inventions)
        {
            if (invention.inventionId == newInvention.inventionId)
            {
                Debug.Log("Invented " + invention.inventionId);//astest
                invention.hasObtained = true;
                invention.createTime = DateTime.UtcNow;
                HandleNewInventionType(newInvention.inventionId);
                return;
            }
        }
        Debug.Log("Unhandled Invention");
    }
    /** Handles immediate effects of new invention types */
    public void HandleNewInventionType(string newInventionId)
    {
        switch (newInventionId.ToLower())
        {
            case "quick_charge_capacitor":// InventionType.QuickChargeCapacitory:
                //No immediate effects. Check using InventionManager.instance.CheckHasUpgrade(InventionType.QuickChargeCapacitory);
                break;
            case "predictive_neural_link":
                //No immediate effects. Check using InventionManager.instance.CheckHasUpgrade(InventionID.PredictiveNeuralLink);
                break;
            case "icarus_boosters": // Icarus Boosters: No immediate effects. Check using InventionManager.instance.CheckHasUpgrade(InventionID.IcarusBoosters);
                break;
            case "treasure_scanner"://.TreasureScanner:: No immediate effects. Check using InventionManager.instance.CheckHasUpgrade(InventionID.TreasureScanner);
                break;
            case "golem_endoplating"://.GolemEndoplating:
                player.characterStatsManager.fortitude += 1;
                player.characterStatsManager.SetNewMaxHealthValue();
                break;
            case "alternator"://.Alternator:
                player.characterStatsManager.endurance += 1;
                player.characterStatsManager.SetNewMaxStaminaValue();
                break;
            case "roller_joints"://.RollerJoints:
                //No immediate effects. Check using InventionManager.instance.CheckHasUpgrade(InventionType.RollerJoints);
                break;
            case "enemy_radar"://.EnemyRadar:
                break;
            case "daedalus_nano_materials":// Daedalus Nano Materials: No immediate effects. Check using InventionManager.instance.CheckHasUpgrade(InventionID.DAEDALUS_NANO_MATERIALS);
                break;
            case "synthetic_diamond"://.SyntheticDiamond:
                ItemDropManager.DropComponent(TinkerComponentType.Diamond, player.transform);
                break;
            default:
                Debug.Log("Unhandled Invent Type");
                break;
        }
    }
}
[Serializable]
public class IdeaStats
{
    public bool obtained = false;
    public byte[] image = null;
    public string ideaID = "defaultID";
    public string ideaName = "DefaultName";
}

public static class InventionID
{
    public const string ROLLER_JOINT = "roller_joints";
    public const string PREDICTIVE_NEURALINK = "predictive_neural_link";
    public const string DAEDALUS_NANO_MATERIALS = "daedalus_nano_materials";
    public const string ICARUS_BOOSTERS = "icarus_boosters";
    public const string QUICKCHARGE_CAPACITORY = "quickcharge_capacitory";
    //public const string DAEDALUS_NANO_MATERIALS = "daedalus_nano_materials";
}
public static class IdeaID
{
    public const string METAL_PLATING = "metal_plating";
    //public const string PREDICTIVE_NEURALINK = "predictive_neural_link";
    //public const string DAEDALUS_NANO_MATERIALS = "daedalus_nano_materials";
    //public const string ICARUS_BOOSTERS = "icarus_boosters";
    //public const string QUICKCHARGE_CAPACITORY = "quickcharge_capacitory";
    //public const string DAEDALUS_NANO_MATERIALS = "daedalus_nano_materials";
}