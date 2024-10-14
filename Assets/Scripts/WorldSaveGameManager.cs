using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldSaveGameManager : MonoBehaviour
{

    //Create Singleton Instance
    public static WorldSaveGameManager instance;
    public PlayerManager player;

    [Header("Save/Load")]
    [SerializeField] bool saveGame;
    [SerializeField] bool loadGame;

    [Header("World Scene Index")]
    [SerializeField] int worldSceneIndex = 1;

    [Header("Save Data Writer")]
    private SaveFileDataWriter saveFileDataWriter;

    [Header("Current Character Data")]
    public CharacterSlot currentCharacterSlotBeingUsed;
    public CharacterSaveData currentCharacterData;
    private string fileName;

    //Might change these to an array later, trying to see why he didn't create them that way from the start
    [Header("Character Slots")]
    public CharacterSaveData characterSlot01;
    public CharacterSaveData characterSlot02;
    public CharacterSaveData characterSlot03;
    public CharacterSaveData characterSlot04;
    public CharacterSaveData characterSlot05;
    public CharacterSaveData characterSlot06;
    public CharacterSaveData characterSlot07;
    public CharacterSaveData characterSlot08;
    public CharacterSaveData characterSlot09;
    public CharacterSaveData characterSlot10;


    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        LoadAllCharacterProfiles();
    }

    // Update is called once per frame
    void Update()
    {
        if (saveGame) {
            saveGame = false;
            SaveGame();
        }

        if (loadGame) {
            loadGame = false;
            LoadGame();
        }
    }

    public void Awake() {
        if (instance == null) {
            instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }

    public string DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot characterSlot) {
        string tempFileName = "";
        switch (characterSlot) {
            case CharacterSlot.CharacterSlot_01:
                tempFileName = "CharacterSlot_01";
                break;
            case CharacterSlot.CharacterSlot_02:
                tempFileName = "CharacterSlot_02";
                break;
            case CharacterSlot.CharacterSlot_03:
                tempFileName = "CharacterSlot_03";
                break;
            case CharacterSlot.CharacterSlot_04:
                tempFileName = "CharacterSlot_04";
                break;
            case CharacterSlot.CharacterSlot_05:
                tempFileName = "CharacterSlot_05";
                break;
            case CharacterSlot.CharacterSlot_06:
                tempFileName = "CharacterSlot_06";
                break;
            case CharacterSlot.CharacterSlot_07:
                tempFileName = "CharacterSlot_07";
                break;
            case CharacterSlot.CharacterSlot_08:
                tempFileName = "CharacterSlot_08";
                break;
            case CharacterSlot.CharacterSlot_09:
                tempFileName = "CharacterSlot_09";
                break;
            case CharacterSlot.CharacterSlot_10:
                tempFileName = "CharacterSlot_10";
                break;
        }

        return tempFileName;
    }
    
    public void AttemptToCreateNewGame() {
        saveFileDataWriter = new SaveFileDataWriter();
        saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath;

        //Check if we can make a new slot (Check for pre-existing slot)
        //Slot 01
        saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_01);
        if (!saveFileDataWriter.CheckToSeeIfFileExists()) {
            //If Profile slot is not taken, we will use it
            currentCharacterSlotBeingUsed = CharacterSlot.CharacterSlot_01;
            currentCharacterData = new CharacterSaveData();
            StartCoroutine(LoadWorldScene());
            //Hide mouse cursor for KB&M players
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;
            return;
        }

        //Slot 02
        saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_02);
        if (!saveFileDataWriter.CheckToSeeIfFileExists()) {
            //If Profile slot is not taken, we will use it
            currentCharacterSlotBeingUsed = CharacterSlot.CharacterSlot_02;
            currentCharacterData = new CharacterSaveData();
            StartCoroutine(LoadWorldScene());
            //Hide mouse cursor for KB&M players
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;
            return;
        }

        //Slot 03
        saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_03);
        if (!saveFileDataWriter.CheckToSeeIfFileExists()) {
            //If Profile slot is not taken, we will use it
            currentCharacterSlotBeingUsed = CharacterSlot.CharacterSlot_03;
            currentCharacterData = new CharacterSaveData();
            StartCoroutine(LoadWorldScene());
            //Hide mouse cursor for KB&M players
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;
            return;
        }

        //Slot 04
        saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_04);
        if (!saveFileDataWriter.CheckToSeeIfFileExists()) {
            //If Profile slot is not taken, we will use it
            currentCharacterSlotBeingUsed = CharacterSlot.CharacterSlot_04;
            currentCharacterData = new CharacterSaveData();
            StartCoroutine(LoadWorldScene());
            //Hide mouse cursor for KB&M players
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;
            return;
        }

        //Slot 05
        saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_05);
        if (!saveFileDataWriter.CheckToSeeIfFileExists()) {
            //If Profile slot is not taken, we will use it
            currentCharacterSlotBeingUsed = CharacterSlot.CharacterSlot_05;
            currentCharacterData = new CharacterSaveData();
            StartCoroutine(LoadWorldScene());
            //Hide mouse cursor for KB&M players
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;
            return;
        }

        //Slot 06
        saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_06);
        if (!saveFileDataWriter.CheckToSeeIfFileExists()) {
            //If Profile slot is not taken, we will use it
            currentCharacterSlotBeingUsed = CharacterSlot.CharacterSlot_06;
            currentCharacterData = new CharacterSaveData();
            StartCoroutine(LoadWorldScene());
            //Hide mouse cursor for KB&M players
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;
            return;
        }

        //Slot 07
        saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_07);
        if (!saveFileDataWriter.CheckToSeeIfFileExists()) {
            //If Profile slot is not taken, we will use it
            currentCharacterSlotBeingUsed = CharacterSlot.CharacterSlot_07;
            currentCharacterData = new CharacterSaveData();
            StartCoroutine(LoadWorldScene());
            //Hide mouse cursor for KB&M players
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;
            return;
        }

        //Slot 08
        saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_08);
        if (!saveFileDataWriter.CheckToSeeIfFileExists()) {
            //If Profile slot is not taken, we will use it
            currentCharacterSlotBeingUsed = CharacterSlot.CharacterSlot_08;
            currentCharacterData = new CharacterSaveData();
            StartCoroutine(LoadWorldScene());
            //Hide mouse cursor for KB&M players
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;
            return;
        }

        //Slot 09
        saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_09);
        if (!saveFileDataWriter.CheckToSeeIfFileExists()) {
            //If Profile slot is not taken, we will use it
            currentCharacterSlotBeingUsed = CharacterSlot.CharacterSlot_09;
            currentCharacterData = new CharacterSaveData();
            StartCoroutine(LoadWorldScene());
            //Hide mouse cursor for KB&M players
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;
            return;
        }

        //Slot 10
        saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_10);
        if (!saveFileDataWriter.CheckToSeeIfFileExists()) {
            //If Profile slot is not taken, we will use it
            currentCharacterSlotBeingUsed = CharacterSlot.CharacterSlot_10;
            currentCharacterData = new CharacterSaveData();
            StartCoroutine(LoadWorldScene());
            //Hide mouse cursor for KB&M players
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;
            return;
        }

        //If there are no free slots, notify the player
        TitleScreenManager.instance.DisplayNoFreeCharacterSlotsPopUp();
    }

    public void LoadGame() {
        //Load a previous file, with a file name depending on which slot we are using
        fileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(currentCharacterSlotBeingUsed);

        saveFileDataWriter = new SaveFileDataWriter();

        //Generally works on multiple machine types (Application.persistentDataPath)
        saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath;
        saveFileDataWriter.saveFileName = fileName;
        currentCharacterData = saveFileDataWriter.LoadSaveFile();

        StartCoroutine(LoadWorldScene());
    }

    public void SaveGame() {
        //Save the current file under a file name depending on which slot we are using
        fileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(currentCharacterSlotBeingUsed);

        saveFileDataWriter = new SaveFileDataWriter();
        //Generally works on multiple machine types (Application.persistentDataPath)
        saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath;
        saveFileDataWriter.saveFileName = fileName;

        //Pass the Player's Info, from game, to their save file
        player.SaveGameDataToCurrentCharacterData(ref currentCharacterData);

        //Write that info onto a JSON file, saved to this machine
        saveFileDataWriter.CreateNewCharacterSaveFile(currentCharacterData);
    }

    public void DeleteGame(CharacterSlot characterSlot) {
        //Choose a file based on name
        saveFileDataWriter = new SaveFileDataWriter();
        saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath;
        saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(characterSlot);
        
        saveFileDataWriter.DeleteSaveFile();
    }

    //Load all character profiles on device when starting game
    private void LoadAllCharacterProfiles() {
        saveFileDataWriter = new SaveFileDataWriter();
        saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath;

        saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_01);
        characterSlot01 = saveFileDataWriter.LoadSaveFile();

        saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_02);
        characterSlot02 = saveFileDataWriter.LoadSaveFile();

        saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_03);
        characterSlot03 = saveFileDataWriter.LoadSaveFile();

        saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_04);
        characterSlot04 = saveFileDataWriter.LoadSaveFile();

        saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_05);
        characterSlot05 = saveFileDataWriter.LoadSaveFile();

        saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_06);
        characterSlot06 = saveFileDataWriter.LoadSaveFile();

        saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_07);
        characterSlot07 = saveFileDataWriter.LoadSaveFile();

        saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_08);
        characterSlot08 = saveFileDataWriter.LoadSaveFile();

        saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_09);
        characterSlot09 = saveFileDataWriter.LoadSaveFile();

        saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_10);
        characterSlot10 = saveFileDataWriter.LoadSaveFile();
    }

    public IEnumerator LoadWorldScene() {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(worldSceneIndex);

        //Give player object data from file
        player.LoadGameFromCurrentCharacterData(ref currentCharacterData);
        yield return null;
    }

    public int GetWorldSceneIndex() {
        return worldSceneIndex;
    }


}
