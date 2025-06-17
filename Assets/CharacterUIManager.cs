using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterUIManager : MonoBehaviour
{
    [Header("UI")]
    public bool hasFloatingHPBar = true;
    public GameObject characterHPBarObject;
    public UICharacterHPBar characterHPBar;
    [HideInInspector] CharacterManager character;

    public void Start()
    {
        character = GetComponent<CharacterManager>();
        characterHPBar = GetComponentInChildren<UICharacterHPBar>();
        characterHPBarObject = characterHPBar.gameObject;
        characterHPBar.gameObject.SetActive(false);
    }

    //Might only exist if we add multiplayer
    // public void OnHPChange(float oldValue, float newValue)
    // {
    //     characterHPBar.oldHealthValue = oldValue;
    //     characterHPBar.SetStat(newValue);
    // }

    public void ActivateHealthBar()
    {
        if (hasFloatingHPBar && !character.isDead)
        {
            characterHPBarObject.SetActive(true);
            characterHPBar.enabled = true;
            characterHPBar.hideBarTextTimer = characterHPBar.defaultTimeBeforeBarTextHides;

            //Check to see if we should enable, and enable name
            characterHPBar.ActivateHPBarName();
        }
    }

    public void Update()
    {
        //TODO: Make this only happen when the bar timer should
        // if (!character.isDead && characterHPBar.willDisplayCharacterNameOnDamage
        //     && character.characterStatsManager.currentHealth != character.characterStatsManager.maxHealth)
        // {
        //     characterHPBar.hideBarTextTimer = characterHPBar.defaultTimeBeforeBarTextHides;
        //     characterHPBar.gameObject.SetActive(true);
        // }
    }

}
