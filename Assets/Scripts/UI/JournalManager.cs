using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class JournalManager : MonoBehaviour
{
    public static JournalManager instance;
    public Dictionary<string, bool> journalFlags = new Dictionary<string, bool>();
    public string dateFormat = "YYY-MM-DD-HH-MM";
    // Start is called before the first frame update
    void Start()
    {
        journalFlags[hasNotOpenedInventMenuKey] = true;
    }

    public bool CheckJournalFlag(string key)
    {
        return journalFlags[key];
    }
    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }








    //KEYS
    public const string hasNotOpenedInventMenuKey = "hasNotOpenedInventMenuKey";
    public const string hasOpenedInventMenuKey = "hasOpenedInventMenuKey";
    public const string hasInventedSomethingKey = "hasInventedSomethingKey";
    public const string hasHalfInventionIdea = "hasHalfInventionIdea";
    public const string hasObtainedTier2RareWeapon = "hasObtainedTier2RareWeapon";
}
