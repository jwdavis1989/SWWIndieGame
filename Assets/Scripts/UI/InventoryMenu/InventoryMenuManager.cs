using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryMenuManager : MonoBehaviour
{
    [Header("Tooltip")]
    public TooltipUI tooltip;
    [Header("Grid window where items appear")]
    public GameObject inventoryWindow;
    [Header("Prefab for inventory item UI Element")]
    public GameObject itemUIPrefab;
    [Header("ScriptableObjects containing statistical item information and sprites")]
    public List<InventoryItemDetails> inventoryItemDetails;

    [Header("Reference to inventory stored on player")]
    public Inventory playerInventory;
    //public List<InventoryItem> inventoryContents = new List<InventoryItem>();
    [Header("Input")]
    public List<GameObject> gamepadTooltips = new List<GameObject>();
    public List<GameObject> keyboardMouseTooltips = new List<GameObject>();

    public static InventoryMenuManager instance;
    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void OnEnable()
    {
        if(playerInventory == null)
        {
            playerInventory = GameObject.Find("Player").GetComponent<Inventory>();
        }
        LoadItemsToWindow(null);
    }

    public void OnDisable()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckControlsChanged();
    }
    // Load Items to screen
    int itemsPerRow = 11;
    int curItemRow = 0;
    void LoadItemsToWindow(List<string> filters)
    {
        foreach (Transform child in inventoryWindow.transform)
        {
            Destroy(child.gameObject);
        }
        int displayedCount = 0;
        int maxDisplayed = 32;
        int itemsToSkip = curItemRow * itemsPerRow;
        int index = 0;
        if(filters != null)
        {
            //TODO
        }
        foreach (KeyValuePair<string, InventoryItem> itemKVP in playerInventory.items)
        {
            InventoryItem item = itemKVP.Value;
            InventoryItemDetails itemDetails = GetItemDetails(itemKVP.Key);
            if (item == null || itemDetails == null) continue;
            if (item.quantity > 0)
            {
                if (itemsToSkip > 0)
                {
                    itemsToSkip--;
                    continue;
                }
                index++;
                if (++displayedCount > maxDisplayed) break;
                GameObject itemGridElement = Instantiate(itemUIPrefab, inventoryWindow.transform);
                InventoryItemUI itemUI = itemGridElement.GetComponent<InventoryItemUI>();
                itemUI.mainButtonForeground.GetComponent<Image>().sprite = itemDetails.icon;
                itemUI.itemName = itemDetails.name;
                //Add tooltip on hover event
                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerEnter;
                entry.callback.AddListener((eventData) =>
                {
                    SetTooltipToItem(itemUI.itemName);
                    itemUI.mainButton.Select();
                });
                itemUI.mainButton.GetComponent<EventTrigger>().triggers.Add(entry);
            }
        }
    }
    GameObject currentCursorObj = null;
    public EventSystem eventSystem;
    public void HandleGamepadSelectedObject()
    {
        if (currentCursorObj != eventSystem.currentSelectedGameObject)
        {
            currentCursorObj = eventSystem.currentSelectedGameObject;
            if (currentCursorObj != null)
            {
                InventoryItemUI ui = currentCursorObj.GetComponentInParent<InventoryItemUI>();
                if (ui != null)
                { // currently on a tinker component
                    SetTooltipToItem(ui.itemName);
                }
            }
        }
    }
    void SetTooltipToItem(string itemName)
    {
        tooltip.headerText.text = itemName;
        tooltip.centerText.text = GetItemDetails(itemName).description;
    }
    private void CheckControlsChanged()
    {
        //Debug.Log("PauseScript.CheckControlsChanged");
        InputSwitchDetector inputSwitchDetector = InputSwitchDetector.instance;
        inputSwitchDetector.CheckControlsChanged();
        if (inputSwitchDetector.deviceChanged)
        {
            //Debug.Log("PauseScript.CheckControlsChanged Device Changed!" + inputSwitchDetector.currentDevice);
            inputSwitchDetector.deviceChanged = false;
            if (InputSwitchDetector.IsCurrentlyGamepad())
            {
                //Show controller UI
                foreach (GameObject gamepadeUI in gamepadTooltips)
                    gamepadeUI.SetActive(true);
                foreach (GameObject gamepadeUI in keyboardMouseTooltips)
                    gamepadeUI.SetActive(false);
            }
            else //Keyboard
            {
                //Hide Controller UI
                foreach (GameObject gamepadeUI in gamepadTooltips)
                    gamepadeUI.SetActive(false);
                foreach (GameObject gamepadeUI in keyboardMouseTooltips)
                    gamepadeUI.SetActive(true);
                //enable buttons
                //EnableAllNavigation();
            }
        }
    }
    InventoryItemDetails GetItemDetails(string itemName)
    {
        foreach (InventoryItemDetails details in inventoryItemDetails)
            if (details.itemName == itemName)
                return details;
        return null;
    }

    //private void ToggleItemNavigation(bool enable)
    //{
    //    bool selected = false;
    //    foreach (Transform t in inventoryWindow.transform)
    //    {
    //        // toggle navigation
    //        Button button = t.gameObject.GetComponent<InventoryItemUI>().mainButton;
    //        button.interactable = enable;
    //        Navigation nav = button.navigation;
    //        nav.mode = enable ? Navigation.Mode.Automatic : Navigation.Mode.None;
    //        button.navigation = nav;
    //        if (enable && !selected && InputSwitchDetector.IsCurrentlyGamepad())
    //        { // Select first
    //            selected = true;
    //            button.Select();
    //        }
    //    }
    //}

    // Item types? Consumable, Component, Weapon?
    //const string CONSUMABLES_FILTER = "CONSUMEABLES";
}
