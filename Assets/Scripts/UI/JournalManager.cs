using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class JournalManager : MonoBehaviour
{
    public static JournalManager instance;
    public Dictionary<string, bool> journalFlags = new Dictionary<string, bool>();
    // Start is called before the first frame update
    void Start()
    {
        journalFlags[hasNotOpenedInventMenuKey] = true;
        //TODO! LOAD/SAVE
    }

    // Update is called once per frame
    void Update()
    {
        
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
    public static string hasNotOpenedInventMenuKey = "hasNotOpenedInventMenuKey";
    public static string hasInventedSomethingKey = "hasInventedSomethingKey";
    public static string hasHalfInventionIdea = "hasHalfInventionIdea";
}
