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
    public bool [] ideaObtainedFlags = new bool[(int)IdeaType.IDEAS_SIZE - 1];
    public byte[][] ideaImages = new byte[(int)IdeaType.IDEAS_SIZE - 1][];

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
                //Debug.Log("File exist for " + (IdeaType)i);//astest
                ideaObtainedFlags[i] = true;
                byte[] bytes = System.IO.File.ReadAllBytes(saveFileName);
                ideaImages[i] = bytes;
            }
            //else Debug.Log("File dont exist " + saveFileName);//astest
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
    //private int currentIdeasPage = 0;
    //[Header("Total ideas able to display per row when selecting an idea")]
    //public int ideasPerRow = 6;
    //void LoadIdeasToScreen()
    //{
    //    Debug.Log("LoadIdeasToScreen called " + ideaObtainedFlags.Length);//astest
    //    foreach (Transform child in ownedIdeasGrid.transform)
    //    {
    //        Destroy(child.gameObject);
    //    }
    //    int displayedCount = 0;
    //    int maxDisplayed = 12;
    //    int ideasToSkip = currentIdeasPage * ideasPerRow;
    //    //basic components
    //    int ideaIndex = -1;
    //    int totalIdeaCount = 0;
    //    foreach (bool ideaFlag in ideaObtainedFlags)
    //    {
    //        ideaIndex++;
    //        Debug.Log("" + (IdeaType)ideaIndex + " is "+ ideaFlag);//astest
    //        if (!ideaFlag) 
    //            continue;
    //        else 
    //        {
    //            totalIdeaCount++;
    //            if (ideasToSkip > 0)
    //            {
    //                ideasToSkip--;
    //                continue;
    //            }
    //            if (++displayedCount > maxDisplayed) break;
    //            Object gridElement = Instantiate(gridElementPrefab, ownedIdeasGrid.transform);
    //            GridElementController gridScript = gridElement.GetComponent<GridElementController>();
    //            gridScript.topText.text = ""+(IdeaType)ideaIndex;
    //            gridScript.bottomText.text = "";
    //            gridScript.cornerButton.gameObject.SetActive(false);
    //            //load image
    //            byte[] bytes = ideaImages[ideaIndex];
    //            Texture2D texture = new Texture2D(0, 0);
    //            texture.LoadImage(bytes);
    //            gridScript.mainButtonForeground.GetComponent<RawImage>().texture = texture;

    //            //add behavior to button
    //            gridScript.mainButton.onClick.AddListener(() => {
    //                GridElementController usedIdeaPanel;
    //                if (activeIdea == 1){
    //                    usedIdeaPanel = firstIdea.GetComponent<GridElementController>();
    //                }else if (activeIdea == 2){
    //                    usedIdeaPanel = secondIdea.GetComponent<GridElementController>();
    //                }else{
    //                    usedIdeaPanel = thirdIdea.GetComponent<GridElementController>();
    //                }
    //                usedIdeaPanel.gameObject.SetActive(true);
    //                usedIdeaPanel.mainButtonForeground.GetComponent<RawImage>().texture = texture;
    //                usedIdeaPanel.bottomText.text = "" + (IdeaType)ideaIndex;
    //            });
    //            // cant use component. disable the button
    //            //else gridScript.mainButton.interactable = false;
    //        }
    //    }
    //    int numOfPage = totalIdeaCount / ideasPerRow;

    //    //TODO scrolling
    //    //if (numOfPage < 2)
    //    //{
    //    //    cmpntScroll.gameObject.SetActive(false);
    //    //}
    //    //else
    //    //{
    //    //    cmpntScroll.gameObject.SetActive(true);
    //    //    cmpntScroll.numberOfSteps = numOfPage;
    //    //    cmpntScroll.size = 1.0f / numOfPage;
    //    //    cmpntCurrentStep = Mathf.Round(cmpntScroll.value * numOfPage);
    //    //}
    //}
}
