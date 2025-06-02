using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
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
        activeIdea = 1;
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

            //add behavior to button
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
        int oldActiveIdea = activeIdea;
        GridElementController usedIdeaPanel;
        if (activeIdea == 1)
        {
            usedIdeaTypes[0] = ideaType;
            activeIdea++;
            usedIdeaPanel = firstIdea.GetComponent<GridElementController>();
        }
        else if (activeIdea == 2)
        {
            usedIdeaTypes[1] = ideaType;
            activeIdea++;
            usedIdeaPanel = secondIdea.GetComponent<GridElementController>();
        }
        else
        {
            usedIdeaTypes[2] = ideaType;
            usedIdeaPanel = thirdIdea.GetComponent<GridElementController>();
        }
        usedIdeaPanel.gameObject.gameObject.SetActive(true);
        //set picture
        usedIdeaPanel.GetComponent<GridElementController>().mainButtonForeground.GetComponent<RawImage>().texture = gridScript.mainButtonForeground.GetComponent<RawImage>().texture;
        usedIdeaPanel.GetComponent<GridElementController>().topText.text = GetIdeaString(ideaType);
        /** USED IDEA BUTTON BEHAVIOUR  */
        usedIdeaPanel.mainButton.onClick.AddListener(() => {
            Debug.Log("Used Idea Clicked");
            //usedIdeaPanel.gameObject.SetActive(false);
            ////reenable this idea
            //gridScript.mainButton.interactable = true;
            //activeIdea = oldActiveIdea;
        });
        //disable this idea
        gridScript.mainButton.interactable = false;
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
