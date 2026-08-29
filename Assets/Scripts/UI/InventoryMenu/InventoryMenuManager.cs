using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class InventoryMenuManager : MonoBehaviour
{
    //
    [Header("Tooltip")]
    public TooltipUI tooltip;
    [Header("Grid window where items appear")]
    public GridLayoutGroup inventoryWindow;
    public QuickslotUI[] quickslotUIs = new QuickslotUI[4];
    public Sprite emptyQuickslotSpr = null;
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI sortText;
    public TextMeshProUGUI filterText;
    [Header("Prefab for inventory item UI Element")]
    public GameObject itemUIPrefab;
    [Header("ScriptableObjects - Contain static item data")]
    public ItemDatabase itemDatabase;
    //public List<ItemDetails> inventoryItemDetails;

    [Header("Reference to inventory stored on player")]
    public Inventory playerInventory;
    public PlayerStatsManager playerStatsManager;

    [Header("Input")]
    public EventSystem eventSystem;
    PlayerControls playerControls;
    [HideInInspector][SerializeField] bool useButtonPerformed = false;
    [HideInInspector][SerializeField] bool quickSlotGamepad = false;
    [HideInInspector][SerializeField] bool quickSlot1 = false;
    [HideInInspector][SerializeField] bool quickSlot2 = false;
    [HideInInspector][SerializeField] bool quickSlot3 = false;
    [HideInInspector][SerializeField] bool quickSlot4 = false; 
    [HideInInspector][SerializeField] float cycleQuickSlotGamepad = 0;
    [HideInInspector][SerializeField] bool sortItemsInput = false;
    [HideInInspector][SerializeField] bool filterItemsByCatergoryInput = false;
    GameObject currentCursorObj = null; // mostly used for gamepad
    [Header("Tooltips and any elements that are activated/deactivated when switching inputs")]
    public List<GameObject> gamepadTooltips = new List<GameObject>();
    public List<GameObject> keyboardMouseTooltips = new List<GameObject>();

    public static InventoryMenuManager instance;
    public void Awake()
    {
        if (instance == null) {
            instance = this;
            WorldUtilityManager.StaticObjects.Add(gameObject);
        } else {
            Destroy(gameObject);
        }
    }
    public void OnEnable()
    {
        if(playerInventory == null) {
            playerInventory = GameObject.Find("Player").GetComponent<Inventory>();
            playerStatsManager = playerInventory.GetComponent<PlayerStatsManager>();
        }
        if (playerControls != null)
            playerControls.InventoryMenu.Enable();
        LoadItemsToWindow();
        LoadQuickslots();
        // load tooltips
        LoadControlTooltips();
        sortText.text = "";
        filterText.text = "";
    }

    public void OnDisable()
    {
        if (playerControls != null) {
            useButtonPerformed = false;
            quickSlotGamepad = false;
            quickSlot1 = false;
            quickSlot2 = false;
            quickSlot3 = false;
            quickSlot4 = false;
            sortItemsInput = false;
            filterItemsByCatergoryInput = false;
            currentSortType = "";
            currentFilter = "";
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
        if (playerControls == null) {
            //Debug.Log("setting weapon menu controls...");
            playerControls = new PlayerControls();
            playerControls.InventoryMenu.UseButton.performed += i => useButtonPerformed = true;
            playerControls.InventoryMenu.QuickslotButtonGamepad.performed += i => quickSlotGamepad = true;
            playerControls.InventoryMenu.QuickslotButton1.performed += i => quickSlot1 = true;
            playerControls.InventoryMenu.QuickslotButton2.performed += i => quickSlot2 = true;
            playerControls.InventoryMenu.QuickslotButton3.performed += i => quickSlot3 = true;
            playerControls.InventoryMenu.QuickslotButton4.performed += i => quickSlot4 = true;
            playerControls.InventoryMenu.SortItems.performed += i => sortItemsInput = true;
            playerControls.InventoryMenu.FilterItemsByCategory.performed += i => filterItemsByCatergoryInput = true;
            playerControls.PlayerActions.CycleQuickslot.performed += i => cycleQuickSlotGamepad = playerControls.PlayerActions.CycleQuickslot.ReadValue<float>();
            playerControls.Enable();
        }
    }

    // Update is called once per frame
    void Update()
    {
        CheckControlsChanged();
        HandleGamepadSelectedObject();
        HandleUseButtonInput();
        HandlequickSlotGamepadInput();
        HandlequickSlotKeyboardInput();
        HandleSortItemsInput();
        HandleFilterItemsByCategory();
    }
    private void OnDestroy()
    {
        instance = null; // For main menu button
        playerControls.Dispose();
    }

    /***********************************************************************************************
     ********************************  I N P U T   H A N D L E R S  ********************************
     ***********************************************************************************************/
    GameObject currentlySelectedObj;
    public void HandleUseButtonInput()
    {
        if (useButtonPerformed) {
            useButtonPerformed = false;
            if (eventSystem.currentSelectedGameObject != null) {
                if (inventoryWindow.transform.childCount > 0) {
                    string itemId = eventSystem.currentSelectedGameObject.GetComponentInParent<InventoryItemUI>().itemId;
                    ItemEffect itemEffect = itemDatabase.GetItemEffect(itemId);
                    if (itemEffect != null) {
                        playerInventory.UseItem(itemId);
                        LoadItemsToWindow();
                        LoadQuickslots();
                    }
                    //UsableItem usableItem = playerInventory.GetItem(itemId).GetComponent<UsableItem>();
                    //if (usableItem != null)
                    //{
                    //    usableItem.Use(playerInventory.gameObject);
                    //}
                }
            }
        }
    }
    int currentSelectedQuickslot = 0;
    public void HandlequickSlotGamepadInput()
    {
        //cycling
        if (cycleQuickSlotGamepad != 0)
        {
            if (cycleQuickSlotGamepad < 0) {
                //Debug.Log("cycle gamepad 1");
                if (currentSelectedQuickslot < 3)
                    currentSelectedQuickslot++;
                else currentSelectedQuickslot = 0;
            } else {
                //Debug.Log("cycle gamepad 2");
                if (currentSelectedQuickslot > 0)
                    currentSelectedQuickslot--;
                else currentSelectedQuickslot = 3;
            }
            cycleQuickSlotGamepad = 0;
            for(int i = 0; i < 4; i++ ) {
                quickslotUIs[i].gamepadSelectedIcon.SetActive(i == currentSelectedQuickslot);
            }
        }
        //using
        if(quickSlotGamepad) {
            quickSlotGamepad = false;
            SetQuickslotItem(currentSelectedQuickslot);
        }
    }
    public void HandlequickSlotKeyboardInput()
    {
        if (quickSlot1) {
            quickSlot1 = false;
            SetQuickslotItem(0);
        }
        if (quickSlot2) { 
            quickSlot2 = false;
            SetQuickslotItem(1);
        }
        if (quickSlot3) {
            quickSlot3 = false;
            SetQuickslotItem(2);
        }
        if (quickSlot4) {
            quickSlot4 = false;
            SetQuickslotItem(3);
        }
    }
    void SetQuickslotItem(int quickslotIndex)
    {
        if(eventSystem.currentSelectedGameObject == null)
            return; 
        InventoryItemUI itemUI = eventSystem.currentSelectedGameObject.GetComponentInParent<InventoryItemUI>();
        if (itemUI != null) {
            //UsableItem usableItem = playerInventory.GetItem(itemUI.itemId).GetComponent<UsableItem>();
            ItemEffect itemEffect = itemDatabase.GetItemEffect(itemUI.itemId);
            ItemDetails itemDetails = itemDatabase.GetItem(itemUI.itemId);
            if (itemEffect != null && "usable,consumable".Contains(itemDetails.itemType)) {
                playerInventory.quickSlotItems[quickslotIndex] = itemUI.itemId;
                LoadQuickslots();
            }
        }
    }
    private string currentSortType = "";
    public void HandleSortItemsInput()
    {
        if (sortItemsInput)
        {
            sortItemsInput = false;
            if (currentSortType == "Cost")
                currentSortType = "Category";
            else if (currentSortType == "Category")
                currentSortType = "";
            else
                currentSortType = "Cost";

            if (currentSortType == "")
                sortText.text = "";
            else
                sortText.text = currentSortType + " -";
            LoadItemsToWindow();
        }
    }
    private string currentFilter = "";
    public void HandleFilterItemsByCategory()
    {
        if (filterItemsByCatergoryInput)
        {
            filterItemsByCatergoryInput = false;
            if (currentFilter == "Consumable")
                currentFilter = "Component";
            else if (currentFilter == "Component")
                currentFilter = "Weapon";
            else if (currentFilter == "Weapon")
                currentFilter = "";
            else
                currentFilter = "Consumable";

            if (currentFilter == "")
                filterText.text = "";
            else
                filterText.text = "- " + currentFilter;
            LoadItemsToWindow();
        }
    }
    private void CheckControlsChanged()
    {
        //Debug.Log("PauseScript.CheckControlsChanged");
        InputSwitchDetector inputSwitchDetector = InputSwitchDetector.instance;
        inputSwitchDetector.CheckControlsChanged();
        if (inputSwitchDetector.deviceChanged) {
            //Debug.Log("PauseScript.CheckControlsChanged Device Changed!" + inputSwitchDetector.currentDevice);
            inputSwitchDetector.deviceChanged = false;
            LoadControlTooltips();
        }
    }
    public void LoadControlTooltips()
    {
        bool isGamepad = InputSwitchDetector.IsCurrentlyGamepad();
        foreach (GameObject gamepadeUI in gamepadTooltips)
            gamepadeUI.SetActive(isGamepad);
        foreach (GameObject gamepadeUI in keyboardMouseTooltips)
            gamepadeUI.SetActive(!isGamepad);
    }
    public void HandleGamepadSelectedObject()
    {
        // Lost gamepad Cursor
        if (eventSystem.currentSelectedGameObject == null && InputSwitchDetector.IsCurrentlyGamepad()) {
            if (inventoryWindow.transform.childCount > 0) {
                inventoryWindow.transform.GetChild(0).GetComponentInChildren<Button>().Select();
            }
        }
        // Change in selected objcet
        if (currentCursorObj != eventSystem.currentSelectedGameObject) {
            currentCursorObj = eventSystem.currentSelectedGameObject;
            if (currentCursorObj != null) {
                //InventoryItemUI ui = currentCursorObj.GetComponentInParent<InventoryItemUI>();
                //if (ui != null) { // currently on a tinker component
                //    ItemDetails itemDetails = GetItemDetails(ui.itemId);
                //    if (itemDetails.itemType == "weapon"){
                //        InventoryItem item = playerInventory.GetItem(ui.itemId);
                //        SetTooltipToItem(ui.itemId, playerInventory.WeaponManager().FindWeaponByInventoryItem(item));
                //    }else
                //        SetTooltipToItem(ui.itemId);
                //}
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
        //load gold
        goldText.text = playerStatsManager.gold + " gp";
        //reload items
        foreach (Transform child in inventoryWindow.transform) {
            Destroy(child.gameObject);
        }
        int displayedCount = 0;
        int maxDisplayed = 55;
        int itemsToSkip = curItemRow * itemsPerRow;
        int index = 0;
        //if(filters != null)
        //{
        //    //TODO
        //}

        Dictionary<string, InventoryItem> items = playerInventory.GetItems(currentFilter, currentSortType);
        foreach (KeyValuePair<string, InventoryItem> itemKVP in items)
        {
            InventoryItem item = itemKVP.Value;
            string itemId = itemKVP.Key;
            WeaponScript weapon = null;
            //if (item.uniqueItem){
            //    itemId = itemId.Split("##")[0];
            //}
            ItemDetails itemDetails = GetItemDetails(itemId);
            if (item == null || itemDetails == null || item.quantity <= 0) continue;
            if (itemsToSkip > 0) {
                itemsToSkip--;
                continue;
            }
            index++;
            if (++displayedCount > maxDisplayed) break;
            if(itemDetails.itemType == "weapon")
                weapon = playerInventory.GetComponent<CharacterWeaponManager>().FindWeaponByInventoryItem(item);
            GameObject itemGridElement = Instantiate(itemUIPrefab, inventoryWindow.transform);
            InventoryItemUI itemUI = itemGridElement.GetComponent<InventoryItemUI>();
            itemUI.mainButtonForeground.sprite = itemDetails.icon;
            itemUI.itemId = itemDetails.itemId;
            // Add mouse cursor tooltip
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerEnter;
            entry.callback.AddListener((eventData) =>
            {
                SetTooltipToItem(itemUI.itemId, weapon);
                itemUI.mainButton.Select();
            });
            itemUI.mainButton.GetComponent<EventTrigger>().triggers.Add(entry);
            // Add select tooltip for gamepad
            entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.Select;
            entry.callback.AddListener((eventData) =>
            {
                SetTooltipToItem(itemUI.itemId, weapon);
                itemUI.mainButton.Select();
            });
            itemUI.mainButton.GetComponent<EventTrigger>().triggers.Add(entry);
        }
    }
    void LoadQuickslots()
    {
        int TOTAL_QUICKSLOTS = 4;
        for (int i = 0; i < TOTAL_QUICKSLOTS; i++) {
            if (InputSwitchDetector.IsCurrentlyGamepad())
                quickslotUIs[i].gamepadSelectedIcon.SetActive(i == currentSelectedQuickslot);
            if (playerInventory == null) break;
            if (playerInventory.quickSlotItems[i] == null) continue;
            string itemId = playerInventory.GetQuickSlotItemId(i);
            if (itemId != null && itemId != "") {
                ItemDetails itemDetails = GetItemDetails(itemId);
                if (itemDetails != null) {
                    quickslotUIs[i].itemUI.mainButtonForeground.sprite = itemDetails.icon;
                    quickslotUIs[i].itemText.text = itemDetails.itemName;
                }
                else Debug.Log("Item Details Not Found:"+itemId);
            } else {
                quickslotUIs[i].itemText.text = "None";
                quickslotUIs[i].itemUI.mainButtonForeground.sprite = emptyQuickslotSpr;
            }
        }
    }
    void SetTooltipToItem(string itemId, WeaponScript weapon = null)
    {
        int qty = playerInventory.GetItem(itemId).quantity;
        ItemDetails itemDetails = GetItemDetails(itemId);
        tooltip.headerText.text = itemDetails.itemName;
        tooltip.centerText.text = itemDetails.description;
        if (weapon != null) {
            tooltip.centerText.text += "\n" + weapon.GetInventoryStatsDisplay();
        }
        tooltip.bottomText.text = "x" + qty + "  -  " + itemDetails.cost + " gp";
    }

    ItemDetails GetItemDetails(string itemId)
    {
        return itemDatabase.GetItem(itemId);
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
