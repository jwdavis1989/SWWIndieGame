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

    [Header("Pop-ups")]
    [SerializeField] GameObject noCharacterSlotsPopUp;
    [SerializeField] Button noCharacterSlotsOkayButton;


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
}
