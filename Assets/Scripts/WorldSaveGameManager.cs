using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldSaveGameManager : MonoBehaviour
{

    //Create Singleton Instance
    public static WorldSaveGameManager instance;
    [SerializeField] PlayerManager player;

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
    // public CharacterSaveData characterSlot02;
    // public CharacterSaveData characterSlot03;
    // public CharacterSaveData characterSlot04;
    // public CharacterSaveData characterSlot05;
    // public CharacterSaveData characterSlot06;
    // public CharacterSaveData characterSlot07;
    // public CharacterSaveData characterSlot08;
    // public CharacterSaveData characterSlot09;
    // public CharacterSaveData characterSlot10;


    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
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

    private void DecideCharacterFileNameBasedOnCharacterSlotBeingUsed() {
        switch (currentCharacterSlotBeingUsed) {
            case CharacterSlot.CharacterSlot_01:
                fileName = "CharacterSlot_01";
                break;
            case CharacterSlot.CharacterSlot_02:
                fileName = "CharacterSlot_02";
                break;
            case CharacterSlot.CharacterSlot_03:
                fileName = "CharacterSlot_03";
                break;
            case CharacterSlot.CharacterSlot_04:
                fileName = "CharacterSlot_04";
                break;
            case CharacterSlot.CharacterSlot_05:
                fileName = "CharacterSlot_05";
                break;
            case CharacterSlot.CharacterSlot_06:
                fileName = "CharacterSlot_06";
                break;
            case CharacterSlot.CharacterSlot_07:
                fileName = "CharacterSlot_07";
                break;
            case CharacterSlot.CharacterSlot_08:
                fileName = "CharacterSlot_08";
                break;
            case CharacterSlot.CharacterSlot_09:
                fileName = "CharacterSlot_09";
                break;
            case CharacterSlot.CharacterSlot_10:
                fileName = "CharacterSlot_10";
                break;
        }
    }
    
    public void CreateNewGame() {
        //Create new file, with a file name depending on which slot we are using
        DecideCharacterFileNameBasedOnCharacterSlotBeingUsed();

        currentCharacterData = new CharacterSaveData();
    }

    public void LoadGame() {
        //Load a previous file, with a file name depending on which slot we are using
        DecideCharacterFileNameBasedOnCharacterSlotBeingUsed();

        saveFileDataWriter = new SaveFileDataWriter();

        //Generally works on multiple machine types (Application.persistentDataPath)
        saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath;
        saveFileDataWriter.saveFileName = fileName;
        currentCharacterData = saveFileDataWriter.LoadSaveFile();

        StartCoroutine(LoadWorldScene());
    }

    public void SaveGame() {
        //Save the current file under a file name depending on which slot we are using
        DecideCharacterFileNameBasedOnCharacterSlotBeingUsed();

        saveFileDataWriter = new SaveFileDataWriter();
        //Generally works on multiple machine types (Application.persistentDataPath)
        saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath;
        saveFileDataWriter.saveFileName = fileName;

        //Pass the Player's Info, from game, to their save file
        player.SaveGameDataToCurrentCharacterData(ref currentCharacterData);

        //Write that info onto a JSON file, saved to this machine
        saveFileDataWriter.CreateNewCharacterSaveFile(currentCharacterData);
    }

    public IEnumerator LoadWorldScene() {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(worldSceneIndex);

        yield return null;
    }

    public int GetWorldSceneIndex() {
        return worldSceneIndex;
    }


}
