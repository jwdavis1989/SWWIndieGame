using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DungeonLevelNodeUI : MonoBehaviour
{
    public string dungeonLevelId;
    public Button button;
    public Image dungeonLevelImage;
    public Sprite dungeonLevelSprite;

    public bool showHiddenSprite = true;
    public bool locked = false;

    public DungeonLevelManager dungeonLevelManager;

    //TODO - Onhover, onclick, show/hide, lock/unlock
    void Start()
    {
        if(dungeonLevelManager == null)
            dungeonLevelManager = GetComponentInParent<DungeonLevelManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void DungeonLevelOnClick()
    {
        DungeonManager.EnterDungeonLevel(dungeonLevelManager.dungeonId, dungeonLevelId);//
    }
}
