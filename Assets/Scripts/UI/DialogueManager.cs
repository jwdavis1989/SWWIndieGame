using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.LowLevel;

public class DialogueManager : MonoBehaviour
{
    [Header("Speaker's lines")]
    public string[] lines;
    public float textSpeed;
    private int lineIndex = 0;
    [Header("TextMeshPro objects")]
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI speakerNameText;
    public TextMeshProUGUI bottomText;

    public Canvas canvas;
    public EventSystem eventSystem;

    PlayerManager playerManager;
    // Start is called before the first frame update
    void Start()
    {
        dialogueText.text = "";
        canvas.gameObject.SetActive(false);
        eventSystem.gameObject.SetActive(false);
        //StartDialgoue();
        playerManager = GameObject.Find("Player").GetComponent<PlayerManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (canvas.isActiveAndEnabled)
        {
            DialogueBoxClickToContinue();
        }
        else
        {
            HandleInteract();
        }
    }
    void DialogueBoxClickToContinue()
    {
        if (PlayerInputManager.instance.interactInput && playerManager.isLockedOn)
        {
            PlayerInputManager.instance.interactInput = false;
            if (dialogueText.text == lines[lineIndex])
            {//if line is finished go to next line
                NextLine();
            }
            else //Complete current line
            {
                StopAllCoroutines();
                dialogueText.text = lines[lineIndex];
            }
        }

    }
    void HandleInteract()
    {
        if(PlayerInputManager.instance.interactInput && playerManager.isLockedOn)
        {
            PlayerInputManager.instance.interactInput = false;
            //Debug.Log("Handling Interact");
            NPCDialogue dialogue = playerManager.playerCombatManager.currentTarget.GetComponent<NPCDialogue>();
            if (dialogue != null)
            {
                //Debug.Log("Handling Interact Got Dialogue");

                lines = dialogue.lines;
                speakerNameText.text = dialogue.speakerName;
                lineIndex = 0;
                StartDialgoue();
            }
        }
    }
    /** Reset dialogue box and begin dialogue */
    void StartDialgoue()
    {
        playerManager.isPerformingAction = true;
        playerManager.canMove = false;
        dialogueText.text = "";
        //Debug.Log("Starting Dialogue");
        canvas.gameObject.SetActive(true);
        //eventSystem.gameObject.SetActive(true);
        lineIndex = 0;
        StartCoroutine(TypeLine());
        if (lines.Length > 1)
        {
            bottomText.text = "Continue";
        }else bottomText.text = "Exit";
    }
    /** Slowly type a line */
    IEnumerator TypeLine()
    {
        bool richTextTag = false;// Rich text tags should be printed out instantly so they aren't seen by the player
        char lastLetter = '\0'; // Use \ to escape and print <
        foreach(char c in lines[lineIndex].ToCharArray())
        {
            dialogueText.text += c;
            if(c == '<' && lastLetter != '\\')
                richTextTag = true;
            if(richTextTag && c == '>')
                richTextTag= false;
            lastLetter = c;
            if(!richTextTag)
                yield return new WaitForSeconds(textSpeed);//wait to type the next letter
        }
    }
    /** Go to the next line or exit dialogue menu if finished */
    void NextLine()
    {
        if (lineIndex < lines.Length - 1)
        { // Go to next line
            lineIndex++;
            dialogueText.text = "";
            if (lineIndex < lines.Length - 1)
            {
                bottomText.text = "Continue";
            }
            else bottomText.text = "Exit";
            StartCoroutine(TypeLine());
        }
        else
        { // Finished
            // unlock player
            playerManager.isPerformingAction = false;
            playerManager.canMove = true;
            // turn off dialogue UI
            canvas.gameObject.SetActive(false);
            eventSystem.gameObject.SetActive(false);
            //gameObject.SetActive(false);
        }
    }
}
