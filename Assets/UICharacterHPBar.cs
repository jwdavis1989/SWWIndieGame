using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//Performs identically to the UIStatBar, except this bar appears and disappears in World Space (Will always face the camera)
public class UICharacterHPBar : UIStatBar
{
    private CharacterManager character;
    private AICharacterManager aiCharacter;

    [SerializeField] public bool willDisplayCharacterNameOnDamage = true;
    [SerializeField] public float defaultTimeBeforeBarTextHides = 3f;
    [SerializeField] public float hideBarTextTimer = 0f;
    [SerializeField] float currentDamageTaken = 0f;
    [SerializeField] TextMeshProUGUI characterName;
    [SerializeField] TextMeshProUGUI characterDamage;
    [HideInInspector] public float oldHealthValue = 0f;

    protected override void Awake()
    {
        base.Awake();

        character = GetComponentInParent<CharacterManager>();

        //Check if it's an AI Character
        if (character != null)
        {
            aiCharacter = character as AICharacterManager;
        }
    }

    protected override void Start()
    {
        base.Start();

        //gameObject.SetActive(false);
    }

    public override void SetStat(float newValue)
    {
        //Call this in case Max Health changes from effects/buffs
        slider.maxValue = character.characterStatsManager.maxHealth;

        //TODO: Run secondary bar logic (Yellow bar that appears behind HP when damaged)

        //Total the damage taken whilst the bar is active
        currentDamageTaken = Mathf.RoundToInt(currentDamageTaken + (oldHealthValue - newValue));

        //Healing Received
        if (currentDamageTaken < 0)
        {
            currentDamageTaken = Mathf.Abs(currentDamageTaken);
            characterDamage.text = "+ " + currentDamageTaken.ToString();
        }
        //Damage Received
        else if (currentDamageTaken > 0)
        {
            characterDamage.text = "- " + currentDamageTaken.ToString();
        }
        //No Health Change within last window
        else
        {
            characterDamage.text = "";
        }

        //Check if their health has changed within the last window to update display timer
        if (oldHealthValue - newValue != 0)
        {
            hideBarTextTimer = defaultTimeBeforeBarTextHides;
        }

        slider.value = newValue;
        oldHealthValue = newValue;
    }

    public void ActivateHPBarName()
    {
        if (willDisplayCharacterNameOnDamage)
        {
            characterName.enabled = true;
            if (aiCharacter != null)
            {
                characterName.text = aiCharacter.characterName;
            }
        }
    }

    private void Update()
    {
        //Face the camera
        if (Camera.main != null)
        {   
            transform.LookAt(transform.position + Camera.main.transform.forward);
        }

        //Handle Text Slowly Disappearing if they haven't been damaged in a while
        if (hideBarTextTimer > 0)
        {
            hideBarTextTimer -= Time.deltaTime;
        }
        else
        {
            characterDamage.text = "";
            currentDamageTaken = 0;
        }

        //Update Health Bar
        if (character.characterUIManager.hasFloatingHPBar)
        {
            SetStat(character.characterStatsManager.currentHealth);
        }

    }

    private void OnEnable()
    {
        gameObject.SetActive(true);
        oldHealthValue = character.characterStatsManager.currentHealth;
    }
    private void OnDisable()
    {
        currentDamageTaken = 0;
        gameObject.SetActive(false);
    }

}
