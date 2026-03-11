using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Composites;
using UnityEngine.UI;

public class DungeonLevelNodeUI : MonoBehaviour
{
    [Header("Case insensitve I.D. for this level")]
    public string dungeonLevelId;
    [Header("Holds OnClick & button highlight")]
    public Button button;

    [Header("Marks level as the dungeon entrance. (Always unlocked)")]
    public bool entrance = false;

    //currently unused
    public Image dungeonLevelImage;
    public Sprite dungeonLevelSprite;
    public bool showHiddenSprite = true;

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
        PlayerInputManager.instance.SafeEnable();
        Time.timeScale = 1;
        DungeonManager.EnterDungeonLevel(dungeonLevelManager.dungeonId, dungeonLevelId);//
    }
}
