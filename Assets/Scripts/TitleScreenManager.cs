using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreenManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // public void StartNetworkAsHost() {

    // }

    public void StartNewGame() {
        StartCoroutine(WorldSaveGameManager.instance.LoadNewGame());

        //Hide mouse cursor for KB&M players
        Cursor.visible = false;
    }
}
