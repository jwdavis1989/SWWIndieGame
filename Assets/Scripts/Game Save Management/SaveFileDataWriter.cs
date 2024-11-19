using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Linq.Expressions;

public class SaveFileDataWriter
{
    public string saveDataDirectoryPath = "";
    public string saveFileName = "";

    //Before we create a new save file, we must check to see if one of this character slot already exists (Max 10 character slots)
    public bool CheckToSeeIfFileExists() {
        return File.Exists(Path.Combine(saveDataDirectoryPath, saveFileName));
    }

    //Delete character save file
    public void DeleteSaveFile() {
        File.Delete(Path.Combine(saveDataDirectoryPath, saveFileName));
    }

    //Create a Save File upon starting a new game
    public void CreateNewCharacterSaveFile(CharacterSaveData characterData) {
        //Make a path to save the file (A location on the machine)
        string savePath = Path.Combine(saveDataDirectoryPath, saveFileName);

        try {
            //Create the directory the file will be written to, if it doesn't already exist
            Directory.CreateDirectory(Path.GetDirectoryName(savePath));
            Debug.Log("Creating save file, at save path: " + savePath);

            //Serialize the C# Game Data Object into JSON
            string dataToStore = JsonUtility.ToJson(characterData, true);

            //Write file to our system now that it's in JSON
            using (FileStream stream = new FileStream(savePath, FileMode.Create)) {
                using (StreamWriter fileWriter = new StreamWriter(stream)) {
                    fileWriter.Write(dataToStore);
                }
            }
        }
        catch (Exception exception) {
            Debug.LogError("Error while trying to save character data, game not saved: \n" + savePath + "\n" + exception);
        }
    }

    public CharacterSaveData LoadSaveFile() {
        CharacterSaveData characterData = null;

        //Make a patht o load the file
        string loadPath = Path.Combine(saveDataDirectoryPath, saveFileName);

        if (File.Exists(loadPath)) {
            try {
                string dataToLoad = "";

                using (FileStream stream = new FileStream(loadPath, FileMode.Open)) {
                    using (StreamReader reader = new StreamReader(stream)) {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                //Deserialize the data from JSON back to Unity
                characterData = JsonUtility.FromJson<CharacterSaveData>(dataToLoad);
            }
            catch(Exception exception) {
                Debug.Log("File is blank!\n" + exception);
            }
        }

        return characterData;
    }
}
