using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventionUIManager : MonoBehaviour
{
    public static InventionUIManager instance;
    [Header("UI")]
    [Header("Ideas used for current invention")]
    public GridElementController firstIdea;
    public GridElementController secondIdea;
    public GridElementController thirdIdea;
    private IdeaType[] usedIdeaTypes = new IdeaType[3];
    public GameObject inventButton;
    public GameObject outputText;
    public GameObject ownedIdeasGrid;
    public GameObject gridElementPrefab;
    public Texture questionMark;
    public GameObject returnButton;
    public EventSystem eventSystem;
    //helpful references
    private PlayerManager player;

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
    }
    public void Update()
    {
        if (eventSystem.currentSelectedGameObject == null)
        {   // Handle for lost cursor
            if (ownedIdeasGrid != null){
                Transform[] allChildren = ownedIdeasGrid.transform.GetComponentsInChildren<Transform>();
                if(allChildren.Length > 0)
                    eventSystem.SetSelectedGameObject(allChildren[0].gameObject);
                else if (returnButton != null) 
                    eventSystem.SetSelectedGameObject(returnButton.gameObject);
            }
        }
    }
    public void OpenInventionMenu()
    {
        outputText.GetComponent<TextMeshProUGUI>().text = "???";
        LoadIdeasToScreen();
        firstIdea.gameObject.SetActive(false);
        secondIdea.gameObject.SetActive(false);
        thirdIdea.gameObject.SetActive(false);
    }

    /**
     * Clear idea list and reload it with current values
     */
    [Header("Total ideas able to display per row when selecting an idea")]
    public int ideasPerRow = 6;
    private int currentIdeasPage = 0;
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
        int totalIdeaCount = 0;
        //loop through all possible ideas
        int ideaIndex = -1;
        foreach (bool ideaFlag in InventionManager.instance.ideaObtainedFlags)
        {
            ideaIndex++;

            if (!ideaFlag) 
                continue;//Skip ideas not obtained for now - could show Question Mark or hint

            //used for scrolling
            totalIdeaCount++;
            if (ideasToSkip > 0)
            {
                ideasToSkip--;
                continue;
            }
            if (++displayedCount > maxDisplayed) break;

            //display an idea
            Object gridElement = Instantiate(gridElementPrefab, ownedIdeasGrid.transform);
            GridElementController gridScript = gridElement.GetComponent<GridElementController>();
            IdeaType ideaType = (IdeaType)ideaIndex;
            string ideaName = GetIdeaString(ideaType);
            gridScript.topText.text = ideaName;
            gridScript.bottomText.text = "";
            gridScript.cornerButton.gameObject.SetActive(false);

            //load image
            byte[] bytes = InventionManager.instance.ideaImages[ideaIndex];
            Texture2D texture = new Texture2D(0, 0);
            texture.LoadImage(bytes);
            gridScript.mainButtonForeground.GetComponent<RawImage>().texture = texture;

            //add owned IDEA BUTTON BEHAVIOUR  
            gridScript.mainButton.onClick.AddListener(()=>OwnedIdeaOnclick(ideaType, gridScript));
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
    /** OBTAINED IDEA BUTTON BEHAVIOUR  
     @param ideaType   Idea to show on button
     @param gridScript The script for this particular button on the UI
     */
    private void OwnedIdeaOnclick(IdeaType ideaType, GridElementController gridScript)
    {
        int activeIdea;
        GridElementController usedIdeaPanel;
        if (thirdIdea.isActiveAndEnabled)
            return;
        else if (secondIdea.isActiveAndEnabled)
        {
            activeIdea = 3;
            usedIdeaTypes[2] = ideaType;
            usedIdeaPanel = thirdIdea;
        }
        else if (firstIdea.isActiveAndEnabled)
        {
            activeIdea = 2;
            usedIdeaTypes[1] = ideaType;
            usedIdeaPanel = secondIdea;
        }
        else
        {
            activeIdea = 1;
            usedIdeaTypes[0] = ideaType;
            usedIdeaPanel = firstIdea;
        }
        usedIdeaPanel.gameObject.gameObject.SetActive(true);
        //set picture
        usedIdeaPanel.mainButtonForeground.GetComponent<RawImage>().texture = gridScript.mainButtonForeground.GetComponent<RawImage>().texture;
        usedIdeaPanel.topText.text = GetIdeaString(ideaType);
        usedIdeaPanel.mainButton.onClick.RemoveAllListeners();
        /** USED IDEA BUTTON BEHAVIOUR  */
        usedIdeaPanel.mainButton.onClick.AddListener(() => 
        {
            UsedIdeaClick(activeIdea, usedIdeaPanel, gridScript);
        });
    }
    bool proccessingUsedIdeaClick = false;
    void UsedIdeaClick(int firstSecondOrThird, GridElementController usedIdeaBtn, GridElementController ownedIdeaBtn)
    {
        if (proccessingUsedIdeaClick)
            return;
        else
            proccessingUsedIdeaClick = true;

        if (firstSecondOrThird == 3)
        {
            thirdIdea.gameObject.SetActive(false);
        }
        else if (firstSecondOrThird == 2)
        {
            if (thirdIdea.isActiveAndEnabled)
            {
                secondIdea.topText.text = "" + thirdIdea.topText.text;
                secondIdea.mainButtonForeground.GetComponent<RawImage>().texture = thirdIdea.mainButtonForeground.GetComponent<RawImage>().texture;
                usedIdeaTypes[1] = usedIdeaTypes[2];
                thirdIdea.gameObject.SetActive(false);
            }
            else
            {
                secondIdea.gameObject.SetActive(false);
            }
        }
        else
        {
            if (secondIdea.isActiveAndEnabled)
            {
                usedIdeaTypes[0] = usedIdeaTypes[1];
                firstIdea.topText.text = "" + secondIdea.topText.text;
                firstIdea.mainButtonForeground.GetComponent<RawImage>().texture = secondIdea.mainButtonForeground.GetComponent<RawImage>().texture;
                if (thirdIdea.isActiveAndEnabled)
                {
                    usedIdeaTypes[1] = usedIdeaTypes[2];
                    secondIdea.topText.text = "" + thirdIdea.topText.text;
                    secondIdea.mainButtonForeground.GetComponent<RawImage>().texture = thirdIdea.mainButtonForeground.GetComponent<RawImage>().texture;
                    thirdIdea.gameObject.SetActive(false);
                }
                else
                {
                    usedIdeaTypes[0] = usedIdeaTypes[1];
                    firstIdea.topText.text = ""+secondIdea.topText.text;
                    firstIdea.mainButtonForeground.GetComponent<RawImage>().texture = secondIdea.mainButtonForeground.GetComponent<RawImage>().texture;
                    secondIdea.gameObject.SetActive(false);
                }
            }
            else
            {
                firstIdea.gameObject.SetActive(false);
            }
        }
        proccessingUsedIdeaClick = false;
    }
    private void MoveButton(GridElementController from, GridElementController to)
    {
        to.mainButtonForeground.GetComponent<RawImage>().texture = from.mainButtonForeground.GetComponent<RawImage>().texture;
        to.topText.text = ""+from.topText.text;
        to.mainButton.onClick = from.mainButton.onClick;
    }
    public void OnInventClick()
    {
        InventionScript possibleInvention = InventionManager.instance.CheckForInvention(usedIdeaTypes[0], usedIdeaTypes[1], usedIdeaTypes[2]);
        if (possibleInvention != null)
        {
            int ideaMatches = 0;
            foreach (IdeaType neededIdea in possibleInvention.neededIdeas)
            {
                if (usedIdeaTypes[0] == neededIdea) ideaMatches++;
                else if (usedIdeaTypes[1] == neededIdea) ideaMatches++;
                else if (usedIdeaTypes[2] == neededIdea) ideaMatches++;
            }
            if (ideaMatches == 3)
            {
                //something is invented
                outputText.GetComponent<TextMeshProUGUI>().text = "Invented " + possibleInvention.type + "!";
                possibleInvention.hasObtained = true;
                if(possibleInvention.type == InventionType.GolemEndoplating)
                {
                    player.characterStatsManager.maxHealth += 10;
                }
            }
            else if (ideaMatches == 2)
            {
                outputText.GetComponent<TextMeshProUGUI>().text = "Hmmm... There's something here";
                //half idea
                int usedIdeaUnmatched = 0;
                int neededIdeaUnmatched = 0;
                for (int i = 0; i < 3; i++)
                {
                    bool match = false;
                    foreach (IdeaType neededIdea in possibleInvention.neededIdeas)
                    {
                        if (neededIdea == usedIdeaTypes[i]) match = true;
                    }
                    if (!match)
                    {
                        //found unmatched used
                        usedIdeaUnmatched = i;

                    }
                }
                for (int i = 0; i < 3; i++)
                {
                    bool match = false;
                    foreach (IdeaType usedIdea in usedIdeaTypes)
                    {
                        if (usedIdea == possibleInvention.neededIdeas[i]) match = true;
                    }
                    if (!match)
                    {
                        //found unmatched needed
                        neededIdeaUnmatched = i;

                    }
                }
                //Show the partial name for the half invented idea
                string needIdeaName = GetIdeaString(possibleInvention.neededIdeas[neededIdeaUnmatched]);
                string displayName = "";
                for (int i = 0; i < needIdeaName.Length-1; i++)
                {
                    if(i <= 0 || needIdeaName[i] == ' ')
                        displayName += needIdeaName[i];
                    else
                        displayName += '_';
                }

                if (usedIdeaUnmatched == 0)
                {
                    firstIdea.GetComponent<GridElementController>().mainButtonForeground.GetComponent<RawImage>().texture = questionMark;
                    firstIdea.GetComponent<GridElementController>().topText.text = displayName;
                }else if(usedIdeaUnmatched == 1)
                {
                    secondIdea.GetComponent<GridElementController>().mainButtonForeground.GetComponent<RawImage>().texture = questionMark;
                    secondIdea.GetComponent<GridElementController>().topText.text = displayName;
                }
                else
                {
                    thirdIdea.GetComponent<GridElementController>().mainButtonForeground.GetComponent<RawImage>().texture = questionMark;
                    thirdIdea.GetComponent<GridElementController>().topText.text = displayName;
                }
            }
        }
    }
    public void OnReturnClick()
    {
        PauseScript.instance.InventMenuBackClick();
    }
    public string GetIdeaString(IdeaType type)
    {
        string name = "" + type;
        string formatted = "";
        foreach (char letter in name)
        {
            if (char.IsUpper(letter))
            {
                formatted += " " + letter;
            }
            else
            {
                formatted += letter;
            }
        }
        formatted = formatted.Substring(1);
        return formatted;
    }
}
