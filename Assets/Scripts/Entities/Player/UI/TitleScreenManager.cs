using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleScreenManager : MonoBehaviour
{
    public static TitleScreenManager instance;
    [Header("Menus")]
    [SerializeField] GameObject titleScreenMainMenu;
    [SerializeField] GameObject titleScreenLoadMenu;

    [Header("Buttons")]
    [SerializeField] Button loadMenuReturnButton;
    [SerializeField] Button mainMenuReturnButton;
    [SerializeField] Button mainMenuNewGameButton;
    [SerializeField] Button deleteCharacterPopUpConfirmButton;

    [Header("Pop-ups")]
    [SerializeField] GameObject noCharacterSlotsPopUp;
    [SerializeField] Button noCharacterSlotsOkayButton;
    [SerializeField] GameObject deleteCharacterSlotPopUp;

    [Header("Character Slots")]
    public CharacterSlot currentSelectedSlot = CharacterSlot.NO_SLOT;

    [Header("Title Screen Inputs")]
    [SerializeField] bool deleteCharacterSlot = false;

    // public void StartNetworkAsHost() {

    // }

    private void Awake() {
        if (instance == null) {
            instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }
    public void StartNewGame() {
        WorldSaveGameManager.instance.AttemptToCreateNewGame();
    }

    public void OpenLoadGameMenu() {

        //Close the main menu
        titleScreenMainMenu.SetActive(false);

        //Open the Load menu
        titleScreenLoadMenu.SetActive(true);

        //Select the Return Button First
        loadMenuReturnButton.Select();
    }

    public void CloseLoadGameMenu() {

        //Close the Load menu
        titleScreenLoadMenu.SetActive(false);

        //Open the main menu
        titleScreenMainMenu.SetActive(true);

        //Select the Return Button First
        mainMenuReturnButton.Select();
    }

    public void DisplayNoFreeCharacterSlotsPopUp() {
        noCharacterSlotsPopUp.SetActive(true);
        noCharacterSlotsOkayButton.Select();
    }

    public void CloseNoFreeCharacterSlotsPopUp() {
        noCharacterSlotsPopUp.SetActive(false);
        mainMenuNewGameButton.Select();
    }

    //Character Slots

    public void SelectCharacterSlot(CharacterSlot characterSlot) {
        currentSelectedSlot = characterSlot;
    }

    public void SelectNoSlot() {
        currentSelectedSlot = CharacterSlot.NO_SLOT;
    }

    public void AttemptToDeleteCharacterSlot() {
        if (currentSelectedSlot != CharacterSlot.NO_SLOT) {
            deleteCharacterSlotPopUp.SetActive(true);
            deleteCharacterPopUpConfirmButton.Select();
        }
    }

    public void DeleteCharacterSlot() {
        deleteCharacterSlotPopUp.SetActive(false);
        WorldSaveGameManager.instance.DeleteGame(currentSelectedSlot);

        //Handle this thing like a router and turn that bitch off then on to update the display
        titleScreenLoadMenu.SetActive(false);
        titleScreenLoadMenu.SetActive(true);
        
        loadMenuReturnButton.Select();
    }

    public void CloseDeleteCharacterPopUp() {
        deleteCharacterSlotPopUp.SetActive(false);
        loadMenuReturnButton.Select();
    }
    
}