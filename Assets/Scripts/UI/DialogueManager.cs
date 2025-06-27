using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.LowLevel;

public class DialogueManager : MonoBehaviour
{
    [Header("Speaker's lines")]
    //public string[] lines;
    public NPCDialogue currentDialogue;
    public float textSpeed;
    private int lineIndex = 0;
    [Header("TextMeshPro objects")]
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI speakerNameText;
    public TextMeshProUGUI bottomText;
    [SerializeField] bool dialogueContinueInput = false;//(A),[LMB]
    PlayerControls playerControls;

    public GameObject dialogueBox;
    public EventSystem eventSystem;

    PlayerManager player;
    public static DialogueManager instance;
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
    // Start is called before the first frame update
    void Start()
    {
        dialogueText.text = "";
        dialogueBox.gameObject.SetActive(false);
        eventSystem.gameObject.SetActive(false);
        //StartDialgoue();
        player = GameObject.Find("Player").GetComponent<PlayerManager>();
        if (playerControls == null)
        {
            playerControls = new PlayerControls();
            playerControls.UI.DialogueContinue.performed += i => dialogueContinueInput = true;
            playerControls.Enable();
        }
    }

    // Update is called once per frame
    void Update()
    {
        HandleDialogueContineuButton();
    }
    public static bool IsInDialogue()
    {
        return instance != null && instance.dialogueBox.activeSelf;
    }
    public void DialogueBoxContinue()
    {
        if (dialogueText.text == currentDialogue.lines2[lineIndex].line)
        {//if line is finished go to next line
            NextLine();
        }
        else //Complete current line
        {
            StopAllCoroutines();
            dialogueText.text = currentDialogue.lines2[lineIndex].line;
        }
    }
    public void PlayDialogue(NPCDialogue dialogue)
    {
        //Debug.Log("Handling Interact");
        if (dialogue != null){
            //Debug.Log("Handling Interact Got Dialogue");
            player.isPerformingAction = true;
            player.canMove = false;
            player.canRotate = false;
            player.isMoving = false;
            playerControls.PlayerActions.Disable();
            //lines = dialogue.lines;
            //lines2 = dialogue.lines2;
            currentDialogue = dialogue;
            speakerNameText.text = dialogue.speakerName;
            lineIndex = 0;
            StartDialgoue();
        }
        //}
    }
    /** Reset dialogue box and begin dialogue */
    void StartDialgoue()
    {
        dialogueText.text = "";
        //Debug.Log("Starting Dialogue");
        dialogueBox.gameObject.SetActive(true);
        //eventSystem.gameObject.SetActive(true);
        lineIndex = 0;
        StartCoroutine(TypeLine());
        if (currentDialogue.lines2.Length > 1)
        {
            bottomText.text = "Continue";
        }
        else bottomText.text = "Exit";
    }
    /** Slowly type a line */
    IEnumerator TypeLine()
    {
        bool richTextTag = false;// Rich text tags should be printed out instantly so they aren't seen by the player
        char lastLetter = '\0'; // Use \ to escape and print <
        foreach (char c in currentDialogue.lines2[lineIndex].line.ToCharArray())
        {
            dialogueText.text += c;
            if (c == '<' && lastLetter != '\\')
                richTextTag = true;
            if (richTextTag && c == '>')
                richTextTag = false;
            lastLetter = c;
            if (!richTextTag)
                yield return new WaitForSeconds(textSpeed);//wait to type the next letter
        }
    }
    /** Go to the next line or exit dialogue menu if finished */
    void NextLine()
    {
        if (lineIndex < currentDialogue.lines2.Length - 1)
        { // there is a next line and we can go to it
            // Go to next line
            lineIndex++;
            if (!CheckLineCondition())
            {
                NextLine();
                return;
            }
            dialogueText.text = "";
            if (lineIndex < currentDialogue.lines2.Length - 1)
            {
                bottomText.text = "Continue";
            }
            else bottomText.text = "Exit";
            StartCoroutine(TypeLine());
        }
        else
        { // Finished
            //Set bool so the Interactable system understands a Pop-Up window has closed
            PlayerUIManager.instance.popUpWindowIsOpen = false;

            // unlock player
            player.isPerformingAction = false;
            player.canMove = true;
            player.canRotate = true;
            playerControls.PlayerActions.Enable();
            // turn off dialogue UI
            dialogueBox.gameObject.SetActive(false);
            eventSystem.gameObject.SetActive(false);
            //gameObject.SetActive(false);
        }
    }
    public NPCDialogue HandleLocatingDialogueTargets()
    {
        //declarations
        List<NPCDialogue> availableTargets = new List<NPCDialogue>();
        NPCDialogue nearestTarget = null;
        float lockOnRadius = 12f;
        float minimumViewableAngle = -50f;
        float maximumViewableAngle = 50f;

        //Will be used to determine the target closest to us
        float shortestDistance = Mathf.Infinity;

        //Uses a Character Layermask to improve efficiency
        Collider[] colliders = Physics.OverlapSphere(player.transform.position, lockOnRadius, WorldUtilityManager.instance.GetCharacterLayers());

        for (int i = 0; i < colliders.Length; i++)
        {
            NPCDialogue lockOnTarget = colliders[i].GetComponent<NPCDialogue>();

            if (lockOnTarget != null)
            {
                //Check if they are within our Field of View
                Vector3 lockOnTargetsDirection = lockOnTarget.transform.position - player.transform.position;
                float distanceFromTarget = Vector3.Distance(player.transform.position, lockOnTarget.transform.position);
                float viewableAngle = Vector3.Angle(lockOnTargetsDirection, PlayerCamera.instance.cameraObject.transform.forward);

                
                //If the target is outside of the field of view or blocked by environment, check the next potential target
                if (viewableAngle > minimumViewableAngle && viewableAngle < maximumViewableAngle)
                {
                    RaycastHit hit;

                    //Check Line of sight by environment layers
                    if (Physics.Linecast(player.playerCombatManager.LockOnTransform.position, lockOnTarget.transform.position, out hit, WorldUtilityManager.instance.GetEnvironmentLayers()))
                    {
                        //We hit something in the environment, blocking line of sight
                        continue;
                    }
                    else
                    {
                        //Add the target to the available targets list since it's within Line of Sight
                        availableTargets.Add(lockOnTarget);
                    }
                }

            }
        }

        //Sort through potential targets to see which one we lock onto first
        for (int i = 0; i < availableTargets.Count; i++)
        {
            if (availableTargets[i] != null)
            {
                float distanceFromTarget = Vector3.Distance(player.transform.position, availableTargets[i].transform.position);

                if (distanceFromTarget < shortestDistance)
                {
                    shortestDistance = distanceFromTarget;
                    nearestTarget = availableTargets[i];
                }
            }
        }
        return nearestTarget;
    }
    //Interact Button during dialogue box
    void HandleDialogueContineuButton()
    {
        //if they press the button during a dialogue
        if (dialogueContinueInput)// [LMB], [E], (X)
        {
            dialogueContinueInput = false;
            if (IsInDialogue())
            {
                DialogueBoxContinue();
            }
        }
    }

    //returns true there is no condition or condition is true
    bool CheckLineCondition()
    {
        string key = currentDialogue.lines2[lineIndex].conditionKey;
        Debug.Log("CheckLineCondition key:" + key);
        if (key == null || key.Trim() == "")
            return true;
        Debug.Log("CheckLineCondition ContainsKey:" + JournalManager.instance.journalFlags.ContainsKey(key));
        return JournalManager.instance.journalFlags.ContainsKey(key) && JournalManager.instance.journalFlags[key];
        //DialogueConditions dialogueConditions = currentDialogue.gameObject.GetComponent<DialogueConditions>();
        //if (dialogueConditions == null)
        //    return true;
        //if (currentDialogue.lines2[lineIndex+1] == null || currentDialogue.lines2[lineIndex+1].condition == null)
        //    return true;
        //if (!currentDialogue.lines2[lineIndex + 1].condition.Yield().GetEnumerator().MoveNext())
        //    return true;
        //currentDialogue.lines2[lineIndex + 1].condition.Invoke();
        //return dialogueConditions.canSeeDialogue;
    }
}
