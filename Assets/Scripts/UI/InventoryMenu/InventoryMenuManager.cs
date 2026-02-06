using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryMenuManager : MonoBehaviour
{
    //
    [Header("Tooltip")]
    public TooltipUI tooltip;
    [Header("Grid window where items appear")]
    public GridLayoutGroup inventoryWindow;
    public InventoryItemUI[] quickslotItems = new InventoryItemUI[4];
    public TextMeshProUGUI[] quickslotText = new TextMeshProUGUI[4];
    [Header("Prefab for inventory item UI Element")]
    public GameObject itemUIPrefab;
    [Header("ScriptableObjects - Contain static item data")]
    public ItemDatabase itemDatabase;
    //public List<ItemDetails> inventoryItemDetails;

    [Header("Reference to inventory stored on player")]
    public Inventory playerInventory;

    [Header("Input")]
    public EventSystem eventSystem;
    PlayerControls playerControls;
    [HideInInspector][SerializeField] bool useButtonPerformed = false;
    [HideInInspector][SerializeField] bool quickSlotGamepad = false;
    [HideInInspector][SerializeField] bool quickSlot1 = false;
    [HideInInspector][SerializeField] bool quickSlot2 = false;
    [HideInInspector][SerializeField] bool quickSlot3 = false;
    [HideInInspector][SerializeField] bool quickSlot4 = false;
    GameObject currentCursorObj = null; // mostly used for gamepad
    [Header("Tooltips and any elements that are activated/deactivated when switching inputs")]
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
        if (playerControls != null)
            playerControls.InventoryMenu.Enable();
        LoadItemsToWindow();
        LoadQuickslots();
    }

    public void OnDisable()
    {
        if (playerControls != null)
        {
            useButtonPerformed = false;
            quickSlotGamepad = false;
            quickSlot1 = false;
            quickSlot2 = false;
            quickSlot3 = false;
            quickSlot4 = false;
            playerControls.InventoryMenu.Disable();
            //hide bottom tooltips
            foreach (GameObject gamepadeUI in gamepadTooltips)
                gamepadeUI.SetActive(false);
            foreach (GameObject gamepadeUI in keyboardMouseTooltips)
                gamepadeUI.SetActive(false);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (playerControls == null)
        {
            //Debug.Log("setting weapon menu controls...");
            playerControls = new PlayerControls();
            playerControls.InventoryMenu.UseButton.performed += i => useButtonPerformed = true;
            playerControls.InventoryMenu.QuickslotButtonGamepad.performed += i => quickSlotGamepad = true;
            playerControls.InventoryMenu.QuickslotButton1.performed += i => quickSlot1 = true;
            playerControls.InventoryMenu.QuickslotButton2.performed += i => quickSlot2 = true;
            playerControls.InventoryMenu.QuickslotButton3.performed += i => quickSlot3 = true;
            playerControls.InventoryMenu.QuickslotButton4.performed += i => quickSlot4 = true;
            playerControls.Enable();
        }
    }

    // Update is called once per frame
    void Update()
    {
        CheckControlsChanged();
        HandleGamePadSelected();
        HandleUseButtonInput();
        HandlequickSlotGamepadInput();
        HandlequickSlotKeyboardInput();
    }

    /***********************************************************************************************
     ********************************  I N P U T   H A N D L E R S  ********************************
     ***********************************************************************************************/
    GameObject currentlySelectedObj;
    void HandleGamePadSelected()
    {
        //TODO
        if (eventSystem.currentSelectedGameObject == null && InputSwitchDetector.IsCurrentlyGamepad())
        { //Handle Lost gamepad Cursor
            if (inventoryWindow.transform.childCount > 0)
            {
                inventoryWindow.transform.GetChild(0).GetComponentInChildren<Button>().Select();
            }
        }
        if (eventSystem.currentSelectedGameObject != null && eventSystem.currentSelectedGameObject != currentlySelectedObj)
        {
            InventoryItemUI itemUi = eventSystem.currentSelectedGameObject.GetComponentInParent<InventoryItemUI>();
            if (itemUi != null)
            {
                SetTooltipToItem(itemUi.itemId);
            }
        }
    }
    public void HandleUseButtonInput()
    {
        if (useButtonPerformed)
        {
            useButtonPerformed = false;
            if (eventSystem.currentSelectedGameObject != null)
            {
                if (inventoryWindow.transform.childCount > 0)
                {
                    string itemId = eventSystem.currentSelectedGameObject.GetComponentInParent<InventoryItemUI>().itemId;
                    UsableItem usableItem = playerInventory.GetItem(itemId).GetComponent<UsableItem>();
                    if (usableItem != null)
                    {
                        usableItem.Use(playerInventory.gameObject);
                        LoadItemsToWindow();
                        LoadQuickslots();
                    }
                }
            }
        }
    }
    public void HandlequickSlotGamepadInput()
    {
        if (quickSlotGamepad)
        {
            quickSlotGamepad = false;
            //TODO need to let dpad or something let you toggle active quickslot
            int quickslot = 1;
            SetQuickslotItem(quickslot);
        }
    }
    public void HandlequickSlotKeyboardInput()
    {
        if (quickSlot1)
        {
            quickSlot1 = false;
            SetQuickslotItem(0);
        }
        if (quickSlot2) 
        { 
            quickSlot2 = false;
            SetQuickslotItem(1);
        }
        if (quickSlot3)
        {
            quickSlot3 = false;
            SetQuickslotItem(2);
        }
        if (quickSlot4)
        {
            quickSlot4 = false;
            SetQuickslotItem(3);
        }
    }
    void SetQuickslotItem(int quickslotIndex)
    {
        InventoryItemUI itemUI = eventSystem.currentSelectedGameObject.GetComponentInParent<InventoryItemUI>();
        if (itemUI != null)
        {
            UsableItem usableItem = playerInventory.GetItem(itemUI.itemId).GetComponent<UsableItem>();
            if (usableItem != null)
            {
                playerInventory.quickSlotItems[quickslotIndex] = usableItem.itemId;
                LoadQuickslots();
            }
        }
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
                    SetTooltipToItem(ui.itemId);
                }
            }
        }
    }
    /***********************************************************************************************
     ******************************  O U T P U T   T O   S C R E E N  ******************************
     ***********************************************************************************************/
    // Load Items to screen
    int itemsPerRow = 11;
    int curItemRow = 0;
    void LoadItemsToWindow()
    {
        foreach (Transform child in inventoryWindow.transform)
        {
            Destroy(child.gameObject);
        }
        int displayedCount = 0;
        int maxDisplayed = 32;
        int itemsToSkip = curItemRow * itemsPerRow;
        int index = 0;
        //if(filters != null)
        //{
        //    //TODO
        //}
        foreach (KeyValuePair<string, InventoryItem> itemKVP in playerInventory.items)
        {
            InventoryItem item = itemKVP.Value;
            ItemDetails itemDetails = GetItemDetails(itemKVP.Key);
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
                itemUI.itemId = itemDetails.itemId;
                //Add tooltip on hover event
                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerEnter;
                entry.callback.AddListener((eventData) =>
                {
                    SetTooltipToItem(itemUI.itemId);
                    itemUI.mainButton.Select();
                });
                itemUI.mainButton.GetComponent<EventTrigger>().triggers.Add(entry);
            }
        }
    }
    void LoadQuickslots()
    {
        int TOTOL_QUICKSLOTS = 4;
        for (int i = 0; i < TOTOL_QUICKSLOTS; i++)
        {
            if (playerInventory == null) break;
            if (playerInventory.quickSlotItems[i] == null) continue;
            string itemId = playerInventory.GetQuickSlotItemId(i);
            if (itemId != null)
            {
                ItemDetails itemDetails = GetItemDetails(itemId);
                quickslotItems[i].mainButtonForeground.GetComponent<Image>().sprite = itemDetails.icon;
                quickslotText[i].text = itemDetails.itemName;
            }
            else
            {
                quickslotText[i].text = "None";
            }
            
        }
    }
    void SetTooltipToItem(string itemId)
    {
        ItemDetails itemDetails = GetItemDetails(itemId);
        tooltip.headerText.text = itemDetails.itemName;
        tooltip.centerText.text = itemDetails.description;
        tooltip.bottomText.text = itemDetails.cost + " gp";
    }

    ItemDetails GetItemDetails(string itemId)
    {
        //foreach (ItemDetails details in inventoryItemDetails)
        //    if (details.itemName == itemName)
        //        return details;
        return itemDatabase.GetItem(itemId);
        //return null;
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
