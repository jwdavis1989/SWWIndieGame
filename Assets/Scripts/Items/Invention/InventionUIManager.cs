using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventionUIManager : MonoBehaviour
{
    public static InventionUIManager instance;
    [Header("UI")]
    [Header("Ideas used for current invention")]
    public GameObject firstIdea;
    public GameObject secondIdea;
    public GameObject thirdIdea;
    public GameObject inventButton;
    public GameObject outputText;
    public GameObject ownedIdeasGrid;
    public GameObject gridElementPrefab;
    [Header("Currently selected idea. 1st, 2nd, or 3rd")]
    private int activeIdea = 1;
    //helpful references
    //private PlayerManager player;


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
    //private void Start()
    //{
    //    player = GameObject.Find("Player").GetComponent<PlayerManager>();
    //}
    public void OpenInventionMenu()
    {
        LoadIdeasToScreen();
        firstIdea.SetActive(false);
        secondIdea.SetActive(false);
        thirdIdea.SetActive(false);
    }

    /**
     * Clear component list and reload it with current values
     */
    private int currentIdeasPage = 0;
    [Header("Total ideas able to display per row when selecting an idea")]
    public int ideasPerRow = 6;
    void LoadIdeasToScreen()
    {
        foreach (Transform child in ownedIdeasGrid.transform)
        {
            Destroy(child.gameObject);
        }
        int displayedCount = 0;
        int maxDisplayed = 12;
        int ideasToSkip = currentIdeasPage * ideasPerRow;
        //basic components
        int ideaIndex = -1;
        int totalIdeaCount = 0;
        foreach (bool ideaFlag in InventionManager.instance.ideaObtainedFlags)
        {
            ideaIndex++;
            if (ideaFlag)
            {
                totalIdeaCount++;
                if (ideasToSkip > 0)
                {
                    ideasToSkip--;
                    continue;
                }
                if (++displayedCount > maxDisplayed) break;
                Object gridElement = Instantiate(gridElementPrefab, ownedIdeasGrid.transform);
                GridElementController gridScript = gridElement.GetComponent<GridElementController>();
                gridScript.topText.text = "" + (IdeaType)ideaIndex;
                gridScript.bottomText.text = "";
                gridScript.cornerButton.gameObject.SetActive(false);
                //load image
                byte[] bytes = InventionManager.instance.ideaImages[ideaIndex];
                Texture2D texture = new Texture2D(0, 0);
                texture.LoadImage(bytes);
                gridScript.mainButtonForeground.GetComponent<RawImage>().texture = texture;

                //add behavior to button
                Debug.Log("ADDING BEHAVIOUR TO  BUTTON");//astest
                gridScript.mainButton.onClick.AddListener(() => {
                    Debug.Log("IN BEHAVIOUR "+ activeIdea);//astest
                    if (activeIdea == 1)
                    {
                        activeIdea++;
                        firstIdea.SetActive(true);
                        firstIdea.GetComponent<GridElementController>().mainButtonForeground.GetComponent<RawImage>().texture = gridScript.mainButtonForeground.GetComponent<RawImage>().texture;
                        firstIdea.GetComponent<GridElementController>().bottomText.text = "" + (IdeaType)ideaIndex;
                    }
                    else if (activeIdea == 2)
                    {
                        activeIdea++;
                        secondIdea.SetActive(true);
                        secondIdea.GetComponent<GridElementController>().mainButtonForeground.GetComponent<RawImage>().texture = gridScript.mainButtonForeground.GetComponent<RawImage>().texture;
                        secondIdea.GetComponent<GridElementController>().bottomText.text = "" + (IdeaType)ideaIndex;
                    }
                    else
                    {
                        thirdIdea.SetActive(true);
                        thirdIdea.GetComponent<GridElementController>().mainButtonForeground.GetComponent<RawImage>().texture = gridScript.mainButtonForeground.GetComponent<RawImage>().texture;
                        thirdIdea.GetComponent<GridElementController>().bottomText.text = "" + (IdeaType)ideaIndex;
                    }
                });
                // cant use component. disable the button
                //else gridScript.mainButton.interactable = false;
            }
        }
        int numOfPage = totalIdeaCount / ideasPerRow;

        //TODO scrolling
        //if (numOfPage < 2)
        //{
        //    cmpntScroll.gameObject.SetActive(false);
        //}
        //else
        //{
        //    cmpntScroll.gameObject.SetActive(true);
        //    cmpntScroll.numberOfSteps = numOfPage;
        //    cmpntScroll.size = 1.0f / numOfPage;
        //    cmpntCurrentStep = Mathf.Round(cmpntScroll.value * numOfPage);
        //}
    }
}
