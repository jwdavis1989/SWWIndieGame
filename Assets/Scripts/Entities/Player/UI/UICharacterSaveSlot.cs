using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UICharacterSaveSlot : MonoBehaviour
{
    SaveFileDataWriter saveFileDataWriter;

    [Header("Game Slot")]
    public CharacterSlot characterSlot;

    [Header("Character Info")]
    public TextMeshProUGUI characterName;
    public TextMeshProUGUI timePlayed;

    private void OnEnable()
    {
        LoadSaveSlots();
    }

    private void LoadSaveSlots() {
        saveFileDataWriter = new SaveFileDataWriter();
        saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath;

        //Save Slot 01
        switch (characterSlot) {
            case CharacterSlot.CharacterSlot_01:
                HandleLoadSaveSingularSlot(WorldSaveGameManager.instance.characterSlot01);
                break;
            case CharacterSlot.CharacterSlot_02:
                HandleLoadSaveSingularSlot(WorldSaveGameManager.instance.characterSlot02);
                break;
            case CharacterSlot.CharacterSlot_03:
                HandleLoadSaveSingularSlot(WorldSaveGameManager.instance.characterSlot03);
                break;
            case CharacterSlot.CharacterSlot_04:
                HandleLoadSaveSingularSlot(WorldSaveGameManager.instance.characterSlot04);
                break;
            case CharacterSlot.CharacterSlot_05:
                HandleLoadSaveSingularSlot(WorldSaveGameManager.instance.characterSlot05);
                break;
            case CharacterSlot.CharacterSlot_06:
                HandleLoadSaveSingularSlot(WorldSaveGameManager.instance.characterSlot06);
                break;
            case CharacterSlot.CharacterSlot_07:
                HandleLoadSaveSingularSlot(WorldSaveGameManager.instance.characterSlot07);
                break;
            case CharacterSlot.CharacterSlot_08:
                HandleLoadSaveSingularSlot(WorldSaveGameManager.instance.characterSlot08);
                break;
            case CharacterSlot.CharacterSlot_09:
                HandleLoadSaveSingularSlot(WorldSaveGameManager.instance.characterSlot09);
                break;
            case CharacterSlot.CharacterSlot_10:
                HandleLoadSaveSingularSlot(WorldSaveGameManager.instance.characterSlot10);
                break;
        }
    }

    private void HandleLoadSaveSingularSlot(CharacterSaveData slotCharacterSaveData) {
        saveFileDataWriter.saveFileName = WorldSaveGameManager.instance.DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(characterSlot);
        //If file exists, get the information from it
        if (saveFileDataWriter.CheckToSeeIfFileExists()) {
            characterName.text = slotCharacterSaveData.characterName;
        }
        //Hide if it doesn't exist
        else {
            gameObject.SetActive(false);
        }
    }

    public void LoadGameFromCharacterSlot() {
        WorldSaveGameManager.instance.currentCharacterSlotBeingUsed = characterSlot;
        WorldSaveGameManager.instance.LoadGame();
    }
}
