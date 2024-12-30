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
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.E))//continue
        {
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
        if( (Input.GetMouseButtonDown(0)  || Input.GetKeyDown(KeyCode.E))
            && playerManager.isLockedOn)
        {
            Debug.Log("Handling Interact");
            NPCDialogue dialogue = playerManager.playerCombatManager.currentTarget.GetComponent<NPCDialogue>();
            if (dialogue != null)
            {
                Debug.Log("Handling Interact Got Dialogue");
                lines = dialogue.lines;
                lineIndex = 0;
                StartDialgoue();
            }
        }
    }
    void StartDialgoue()
    {
        Debug.Log("Starting Dialogue");
        canvas.gameObject.SetActive(true);
        //eventSystem.gameObject.SetActive(true);
        lineIndex = 0;
        StartCoroutine(TypeLine());
    }
    IEnumerator TypeLine()
    {
        foreach(char c in lines[lineIndex].ToCharArray())
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }
    void NextLine()
    {
        if (lineIndex < lines.Length - 1)
        {
            lineIndex++;
            dialogueText.text = "";
            StartCoroutine(TypeLine());
        }
        else
        {
            //gameObject.SetActive(false);
            canvas.gameObject.SetActive(false);
            eventSystem.gameObject.SetActive(false);
        }
    }
}
