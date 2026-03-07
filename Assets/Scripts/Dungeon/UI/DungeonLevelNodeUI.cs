using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Composites;
using UnityEngine.UI;

public class DungeonLevelNodeUI : MonoBehaviour
{
    public string dungeonLevelId;
    public Button button;
    public Image dungeonLevelImage;
    public Sprite dungeonLevelSprite;

    public bool showHiddenSprite = true;
    public bool entrance = false;
    public bool completed = false;

    public DungeonLevelManager dungeonLevelManager;

    //TODO - Onhover, onclick, show/hide, lock/unlock
    void Start()
    {
        if(dungeonLevelManager == null)
            dungeonLevelManager = GetComponentInParent<DungeonLevelManager>();
    }

    public void Show()
    {
        button.interactable = true;
    }
    public void Hide()
    {
        button.interactable = false;
    }
    public void DungeonLevelOnClick()
    {
        DungeonManager.EnterDungeonLevel(dungeonLevelManager.dungeonId, dungeonLevelId);//
    }
}
