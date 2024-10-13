using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleScreenManager : MonoBehaviour
{
    [Header("Menus")]
    [SerializeField] GameObject titleScreenMainMenu;
    [SerializeField] GameObject titleScreenLoadMenu;

    [Header("Buttons")]
    [SerializeField] Button loadMenuReturnButton;
    [SerializeField] Button mainMenuReturnButton;

    // public void StartNetworkAsHost() {

    // }

    public void StartNewGame() {
        WorldSaveGameManager.instance.CreateNewGame();
        StartCoroutine(WorldSaveGameManager.instance.LoadWorldScene());

        //Hide mouse cursor for KB&M players
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
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
}
