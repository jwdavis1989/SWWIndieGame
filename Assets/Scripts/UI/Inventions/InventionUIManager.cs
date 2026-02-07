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
    [Header("--------------------------------------------------------------------------------\n" +
        "   UI Elements  " +
        "\n--------------------------------------------------------------------------------")]
    [Header("Selected ideas displayed for current invention")]
    public IdeaPanel firstIdea;
    public IdeaPanel secondIdea;
    public IdeaPanel thirdIdea;
    //private IdeaType[] usedIdeaTypes = new IdeaType[3];
    private string[] usedIdeas = new string[3];
    [Header("Displayed Ideas")]
    public GameObject ownedIdeasGrid;
    [Header("(TODO) Displayed Inventions")]
    public GameObject allInventionsGrid;
    public InventionDatabase inventionDatabase;
    [Header("Input")]
    public GameObject inventButton;
    public EventSystem eventSystem;
    GameObject currentCursorObj = null; // mostly used for gamepad
    [Header("Tooltips and any elements that are activated/deactivated when switching inputs")]
    public List<GameObject> gamepadTooltips = new List<GameObject>();
    public List<GameObject> keyboardMouseTooltips = new List<GameObject>();
    [Header("Output")]
    public GameObject outputText;
    [Header("Other assets")]
    public Sprite questionMarkSpr;
    public GameObject ideaUIPrefab;
    public GameObject inventionUIPrefab;
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
                if (allChildren.Length > 0)
                {
                    eventSystem.SetSelectedGameObject(allChildren[0].gameObject);
                }
            }
        }
        CheckControlsChanged();
    }
    public void OnEnable()
    {
        // load tooltips
        if (InputSwitchDetector.IsCurrentlyGamepad())
        {
            foreach (GameObject gamepadeUI in gamepadTooltips)
                gamepadeUI.SetActive(true);
            foreach (GameObject kbmUI in keyboardMouseTooltips)
                kbmUI.SetActive(false);
        }
        else
        {
            foreach (GameObject gamepadeUI in gamepadTooltips)
                gamepadeUI.SetActive(false);
            foreach (GameObject kbmUI in keyboardMouseTooltips)
                kbmUI.SetActive(true);
        }
    }
    public void OnDisable()
    {
        //if (playerControls != null)
        //{
        //    playerControls.InventMenu.Disable();
            //hide bottom tooltips
            foreach (GameObject gamepadeUI in gamepadTooltips)
                gamepadeUI.SetActive(false);
            foreach (GameObject gamepadeUI in keyboardMouseTooltips)
                gamepadeUI.SetActive(false);
        //}
    }
    public void OpenInventionMenu()
    {
        JournalManager.instance.journalFlags[JournalManager.hasNotOpenedInventMenuKey] = false;
        JournalManager.instance.journalFlags[JournalManager.hasOpenedInventMenuKey] = true;
        outputText.GetComponent<TextMeshProUGUI>().text = "???";
        LoadIdeasToScreen();
        LoadInventionsToScreen();
        firstIdea.gameObject.SetActive(false);
        secondIdea.gameObject.SetActive(false);
        thirdIdea.gameObject.SetActive(false);
    }
    /**
     * Clear idea list and reload it with current values
     */
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
        foreach (IdeaStats idea in InventionManager.instance.ideas)
        {
            ideaIndex++;

            if (idea == null || !idea.obtained) 
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
            Object gridElement = Instantiate(ideaUIPrefab, ownedIdeasGrid.transform);
            IdeaPanel ideaPanel = gridElement.GetComponent<IdeaPanel>();
            ideaPanel.index = ideaIndex;
            ideaPanel.topText.text = idea.ideaName;

            //load image
            byte[] bytes = InventionManager.instance.ideas[ideaIndex].image;
            Texture2D texture = new Texture2D(0, 0);
            texture.LoadImage(bytes);
            Debug.Log("Setting idea pic");
            ideaPanel.image.texture = texture;

            //add owned IDEA BUTTON BEHAVIOUR  
            ideaPanel.mainButton.onClick.AddListener(()=>OwnedIdeaOnclick(idea.ideaID, ideaPanel));//TODO FIX THIS
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
    private void CheckControlsChanged()
    {
        //Debug.Log("PauseScript.CheckControlsChanged");
        InputSwitchDetector inputSwitchDetector = InputSwitchDetector.instance;
        inputSwitchDetector.CheckControlsChanged();
        if (inputSwitchDetector.deviceChanged)
        {
            //Debug.Log("PauseScript.CheckControlsChanged Device Changed!" + inputSwitchDetector.currentDevice);
            inputSwitchDetector.deviceChanged = false;
            if (InputSwitchDetector.IsCurrentlyGamepad())
            {
                //Show controller UI
                foreach (GameObject gamepadeUI in gamepadTooltips)
                    gamepadeUI.SetActive(true);
                foreach (GameObject gamepadeUI in keyboardMouseTooltips)
                    gamepadeUI.SetActive(false);
            }
            else //Keyboard
            {
                //Hide Controller UI
                foreach (GameObject gamepadeUI in gamepadTooltips)
                    gamepadeUI.SetActive(false);
                foreach (GameObject gamepadeUI in keyboardMouseTooltips)
                    gamepadeUI.SetActive(true);
                //enable buttons
                //EnableAllNavigation();
            }
        }
    }
    //************************** I D E A   S C R O L L **************************
    /**
     * This section controls the Owned Ideas scroll bar
     */

    [Header("--------------------------------------------------------------------------------\n" +
    "   Settings  " +
    "\n--------------------------------------------------------------------------------")]
    [Header("Idea Box Scrollbar")]
    public Scrollbar ideaScroll; //Unity scrollbar object
    [Header("Total ideas able to display per row i.e. Number of columns")]
    public int ideasPerRow = 6;
    public float ideaCurrentStep = 0; //current scrollbar val
    public float ideaLastStep = 0; //last scrollbar val
    private int currentIdeasPage = 0; //current row scrolled to
    public void IdeaScroll(float value)
    {
        int count = 0;//count total unique ideas owned
        foreach (IdeaStats idea in InventionManager.instance.ideas)
        {
            if (idea != null && idea.obtained) count++;
        }
        int numOfPage = count / ideasPerRow;
        if (numOfPage < 2)
        {
            ideaScroll.gameObject.SetActive(false);
        }
        else
        {
            ideaScroll.gameObject.SetActive(true);
            ideaScroll.numberOfSteps = numOfPage;
            ideaScroll.size = 1.0f / numOfPage;
            ideaCurrentStep = Mathf.Round(ideaScroll.value * numOfPage);
        }
        if (ideaCurrentStep == ideaLastStep)
            return; // no change
        if (ideaCurrentStep > ideaLastStep)
        {
            currentIdeasPage++;
        }
        else
        {
            currentIdeasPage--;
        }
        if (currentIdeasPage > numOfPage)
        {// past the end go to beg
            currentIdeasPage = 0;
        }
        else if (currentIdeasPage < 0)
        {//past beggining go to end
            currentIdeasPage = numOfPage;
        }
        ideaLastStep = ideaCurrentStep;
        LoadIdeasToScreen();
    }
    /** OBTAINED IDEA BUTTON BEHAVIOUR  
     @param ideaType   Idea to show on button
     @param gridScript The script for this particular button on the UI
     */
    private void OwnedIdeaOnclick(string ideaType, IdeaPanel gridScript)
    {
        int activeIdea;
        IdeaPanel usedIdeaPanel;
        if (thirdIdea.isActiveAndEnabled) 
            return; // Can only use 3 ideas
        else if (secondIdea.isActiveAndEnabled)
        { // place in 3rd spot
            activeIdea = 3;
            usedIdeas[2] = ideaType;
            usedIdeaPanel = thirdIdea;
        }
        else if (firstIdea.isActiveAndEnabled)
        { // place in 2nd spot
            activeIdea = 2;
            usedIdeas[1] = ideaType;
            usedIdeaPanel = secondIdea;
        }
        else
        { // place in 1st spot
            activeIdea = 1;
            usedIdeas[0] = ideaType;
            usedIdeaPanel = firstIdea;
        }
        usedIdeaPanel.gameObject.gameObject.SetActive(true);
        //set picture
        usedIdeaPanel.image.texture = gridScript.image.texture;
        usedIdeaPanel.topText.text = ideaType;// TODO FIX THIS GetIdeaString(ideaType);
        usedIdeaPanel.mainButton.onClick.RemoveAllListeners();
        /** USED IDEA BUTTON BEHAVIOUR  */
        usedIdeaPanel.mainButton.onClick.AddListener(() => 
        {
            UsedIdeaClick(activeIdea, usedIdeaPanel, gridScript);
        });
    }
    [Header("Total ideas able to display per row i.e. Number of columns")]
    public int inventionsPerRow = 6;
    private int currentInventionPage = 0;
    /**
     * Clear Invention list and reload it with current values
     */
    void LoadInventionsToScreen()
    {
        foreach (Transform child in allInventionsGrid.transform)
        {
            Destroy(child.gameObject);
        }
        int displayedCount = 0;
        int maxDisplayed = 16;
        int inventionsToSkip = currentInventionPage * inventionsPerRow;
        //basic components
        int totalInventionCount = 0;
        //loop through all possible ideas
        foreach (InventionData inventionData in inventionDatabase.inventions)
        {
            // used for scrolling
            totalInventionCount++;
            if (inventionsToSkip > 0)
            {
                inventionsToSkip--;
                continue;
            }
            if (++displayedCount > maxDisplayed) break;

            
            if (InventionManager.instance.obtainedInventions.Contains(inventionData.inventionId))
            { // display an invention
                if (++displayedCount > maxDisplayed) break;
                Object gridElement = Instantiate(inventionUIPrefab, allInventionsGrid.transform);
                InventionPanel gridScript = gridElement.GetComponent<InventionPanel>();
                gridScript.topText.text = inventionData.inventionName;
                if (inventionData.icon != null)
                    gridScript.image.sprite = inventionData.icon;
                else
                    gridScript.image.sprite = questionMarkSpr;
            }
            //else // Display hint?
        }
        int numOfPage = totalInventionCount / ideasPerRow;
    }
    bool proccessingUsedIdeaClick = false;
    void UsedIdeaClick(int firstSecondOrThird, IdeaPanel usedIdeaBtn, IdeaPanel ownedIdeaBtn)
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
                secondIdea.image.texture = thirdIdea.image.texture;
                usedIdeas[1] = usedIdeas[2];
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
                usedIdeas[0] = usedIdeas[1];
                firstIdea.topText.text = "" + secondIdea.topText.text;
                firstIdea.image.texture = secondIdea.image.texture;
                if (thirdIdea.isActiveAndEnabled)
                {
                    usedIdeas[1] = usedIdeas[2];
                    secondIdea.topText.text = "" + thirdIdea.topText.text;
                    secondIdea.image.texture = thirdIdea.image.texture;
                    thirdIdea.gameObject.SetActive(false);
                }
                else
                {
                    usedIdeas[0] = usedIdeas[1];
                    firstIdea.topText.text = ""+secondIdea.topText.text;
                    firstIdea.image.texture = secondIdea.image.texture;
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
    //private void MoveButton(GridElementController from, GridElementController to)
    //{
    //    to.mainButtonForeground.GetComponent<RawImage>().texture = from.mainButtonForeground.GetComponent<RawImage>().texture;
    //    to.topText.text = ""+from.topText.text;
    //    to.mainButton.onClick = from.mainButton.onClick;
    //}
    public void OnInventClick()
    {
        InventionData possibleInvention = InventionManager.instance.CheckForInvention(usedIdeas[0], usedIdeas[1], usedIdeas[2]);
        if (possibleInvention != null)
        {
            int ideaMatches = 0;
            foreach (string neededIdea in possibleInvention.ideas)
            {
                if (usedIdeas[0] == neededIdea) ideaMatches++;
                else if (usedIdeas[1] == neededIdea) ideaMatches++;
                else if (usedIdeas[2] == neededIdea) ideaMatches++;
            }
            if (ideaMatches == 3) // something is invented
            {
                JournalManager.instance.journalFlags[JournalManager.hasHalfInventionIdea] = false;//set to false so that dialogue doesn't play
                JournalManager.instance.journalFlags[JournalManager.hasInventedSomethingKey] = true;
                InventionManager.instance.HandleNewInvention(possibleInvention);
                outputText.GetComponent<TextMeshProUGUI>().text = "Invented " + possibleInvention.ToString() + "!";
                firstIdea.gameObject.SetActive(false);
                secondIdea.gameObject.SetActive(false);
                thirdIdea.gameObject.SetActive(false);
                LoadInventionsToScreen();
            }
            else if (ideaMatches == 2) // almost invention
            {
                JournalManager.instance.journalFlags[JournalManager.hasHalfInventionIdea] = true;
                outputText.GetComponent<TextMeshProUGUI>().text = "Hmmm... There's something here";
                int usedIdeaUnmatched = 0;
                int neededIdeaUnmatched = 0;
                for (int i = 0; i < 3; i++)
                {
                    bool match = false;
                    foreach (string neededIdea in possibleInvention.ideas)
                    {
                        if (neededIdea == usedIdeas[i]) match = true;
                    }
                    if (!match)
                    {
                        //found used idea to be removed
                        usedIdeaUnmatched = i;

                    }
                }
                for (int i = 0; i < 3; i++)
                {
                    bool match = false;
                    foreach (string usedIdea in usedIdeas)
                    {
                        if (usedIdea == possibleInvention.ideas[i]) match = true;
                    }
                    if (!match)
                    {
                        //found needed idea to hint at
                        neededIdeaUnmatched = i;

                    }
                }
                //Show the partial name for the half invented idea
                string needIdeaName = GetIdeaString(possibleInvention.neededIdeas[neededIdeaUnmatched]);
                string displayName = "";
                int displayedLetters = InventionManager.instance.CheckHasUpgrade(InventionID.PREDICTIVE_NEURALINK)
                    ? needIdeaName.Length/4 : 1;
                for (int i = 0; i < needIdeaName.Length; i++)
                {
                    if(i < displayedLetters || needIdeaName[i] == ' ')
                        displayName += needIdeaName[i];
                    else
                        displayName += '_';
                }

                if (usedIdeaUnmatched == 0)
                {
                    firstIdea.image.texture = questionMarkSpr.texture;
                    firstIdea.topText.text = displayName;
                }else if(usedIdeaUnmatched == 1)
                {
                    secondIdea.image.texture = questionMarkSpr.texture;
                    secondIdea.topText.text = displayName;
                }
                else
                {
                    thirdIdea.image.texture = questionMarkSpr.texture;
                    thirdIdea.topText.text = displayName;
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
