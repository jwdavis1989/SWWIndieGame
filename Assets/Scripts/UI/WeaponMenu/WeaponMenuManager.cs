using Palmmedia.ReportGenerator.Core.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class WeaponMenuManager : MonoBehaviour
{
    [Header("Weapon Menu\n")]
    [Header("Active Weapon Preview")]
    public GameObject statsTextPrefab;
    public GameObject primaryStatsText;
    public GameObject expStatsText;
    public GameObject elementalStatsText;
    public TextMeshProUGUI weaponPreviewHeaderText;
    public TextMeshProUGUI activeWeaponTierLevelText;
    public TextMeshProUGUI tinkerPointsCountText;
    public Transform weaponPreviewHolder;
    public GameObject currentWeaponPreview;
    public GameObject wpnEvolveBtn1;
    public GameObject wpnEvolveBtn2;
    [Header("Tooltip Window")]
    public TooltipUI tooltipUI;
    //public GameObject specWpnEvolveBtn1;
    //public GameObject specWpnEvolveBtn2;
    [Header("Grid containing owned weapons")]
    public GameObject weaponsGrid;
    public int curWeaponPage = 0;
    GameObject activeWeapon = null;
    public Image salvageButtonIconGamepad;
    public TextMeshProUGUI salvageControlText;
    [Header("Grid containing tinker components")]
    public GameObject componentsGrid;
    public int curComponentPage = 0;
    [Header("Prefab for item UI object")]
    public GameObject tinkerComponentPrefab;
    public GameObject weaponButton;
    public Sprite defaultUnkownIcon;
    [Header("Input")]
    //Event system. There can apparently only be one active at time so need to make sure this doesnt conflict with other UI
    public EventSystem eventSystem;
    [HideInInspector] [SerializeField] bool switchWeaponUp = false;
    [HideInInspector][SerializeField] bool switchWeaponDown = false;
    [HideInInspector][SerializeField] bool equipWeaponInput = false;
    [HideInInspector][SerializeField] bool weaponSubmenuInput = false;
    [HideInInspector][SerializeField] bool helpInput = false;
    [HideInInspector][SerializeField] bool repairInput = false;
    [Header("Camera Movement Input")]
    PlayerControls playerControls;
    [SerializeField] Vector2 previewCameraInput;

    [Header("Submenus")]
    public bool submenuActive = false;
    public GameObject salvageConfirmWindow;
    public GameObject salvageErrorWindow;
    public GameObject weaponSubmenu;
    [Header("Input Tooltips")]
    public List<GameObject> gamepadTooltips;
    public List<GameObject> keyboardMouseTooltips;
    //[SerializeField] public Image holdToBreakdownWpnImage;
    //[SerializeField] private float holdDuration = 1.5f;//should match value in PlayerControls
    private bool isHolding;
    private float holdTime;

    private void Awake()
    {
        foreach (Transform child in weaponsGrid.transform)
            Destroy(child.gameObject);
        foreach (Transform child in primaryStatsText.transform)
            Destroy(child.gameObject);
        foreach (Transform child in elementalStatsText.transform)
            Destroy(child.gameObject);
        foreach (Transform child in expStatsText.transform)
            Destroy(child.gameObject);
        foreach (Transform child in componentsGrid.transform)
            Destroy(child.gameObject);
    }
    //called when arriving at this menu
    private void OnEnable()
    {
        //wpnScroll.value = 0;
        if(PlayerWeaponManager.instance.GetMainHand() != null)
            curWeaponPage = PlayerWeaponManager.instance.ownedWeapons.IndexOf(PlayerWeaponManager.instance.GetMainHand().gameObject);
        //cmpntScroll.value = 0;
        curComponentPage = 0;
        activeWeapon = null;
        LoadWeaponsToScreen();
        DisplayActiveWeapon();
        LoadComponentsToScreen();
        if(playerControls != null)
            playerControls.WeaponMenu.Enable();
        // load tooltips
        LoadControlTooltips();
        //close submenus
        CloseWeaponSubmenu();
    }
    private void OnDisable()
    {
        playerControls.WeaponMenu.Disable();
        //hide bottom tooltips
        foreach (GameObject gamepadeUI in gamepadTooltips)
            gamepadeUI.SetActive(false);
        foreach (GameObject gamepadeUI in keyboardMouseTooltips)
            gamepadeUI.SetActive(false);
    }
    // Start is called before the first frame update
    void Start()
    {
        if (playerControls == null)
        {
            //Debug.Log("setting weapon menu controls...");
            playerControls = new PlayerControls();
            playerControls.WeaponMenu.WeaponPreviewMovement.performed += i => previewCameraInput = i.ReadValue<Vector2>();
            playerControls.WeaponMenu.SwitchWeaponUp.performed += i => switchWeaponUp = true;
            playerControls.WeaponMenu.SwitchWeaponDown.performed += i => switchWeaponDown = true;
            playerControls.WeaponMenu.EquipWeapon.performed += i => equipWeaponInput = true;
            playerControls.WeaponMenu.HelpButton.performed += i => helpInput = true;
            //playerControls.WeaponMenu.FocusComponentsWindow.performed += i => focusComponentsInput = true;
            //playerControls.WeaponMenu.FocusEvolutionsWindow.performed += i => focusEvolutionsInput = true;
            playerControls.WeaponMenu.WeaponSubmenu.performed += i => weaponSubmenuInput = true;
            playerControls.Enable();
        }
    }
    // Update is called once per frame
    void LateUpdate()
    {
        if (eventSystem.currentSelectedGameObject == null && InputSwitchDetector.IsCurrentlyGamepad())
        { //Handle Lost gamepad Cursor
            if (submenuActive)
            {
                if (weaponSubmenu.activeInHierarchy)
                    weaponSubmenu.GetComponentInChildren<Button>().Select();
                else if (salvageErrorWindow.activeInHierarchy)
                    salvageErrorWindow.GetComponentInChildren<Button>().Select();
                else if (salvageConfirmWindow.activeInHierarchy)
                    salvageConfirmWindow.GetComponentInChildren<Button>().Select();
            }
            else if (helpActive){
                primaryStatsText.transform.GetChild(0).GetComponent<Button>().Select();
            }else if (componentsGrid.transform.childCount > 0){
                componentsGrid.transform.GetChild(0).GetComponentInChildren<Button>().Select();
            }
        }
        HandleWeaponPreviewInput();
        HandleSwitchWeaponInput();
        HandleEquipWeaponInput();
        HandleOpenWeaponSubmenuInput();
        HandleHelpInput();
        //HandleFocusComponentsInput();
        HandleGamepadSelectedObject();//Moving through components/etc.
        CheckControlsChanged();//Gamepad <> KB&M
        if (currentWeaponPreview != null && !currentWeaponPreview.activeSelf)
        {
            currentWeaponPreview.SetActive(true);
        }
    }

    //**************************** I N P U T ****************************
    float rotationSpeed = 100;
    float zoomSpeed = 1;
    float maxZoomDistance = 0.5f;
    float minZoomDistance = -0.5f;
    void HandleWeaponPreviewInput()
    {
        if (activeWeapon == null)
            return;
        if (submenuActive)
            return;
        //rotate
        if (previewCameraInput.x > 0.75f)
        {
            //currentWeaponPreview.transform.Rotate(Vector3.forward * rotationSpeed * Time.unscaledDeltaTime);
            //weaponPreviewHolder.Rotate(Vector3.one * rotationSpeed * Time.unscaledDeltaTime);
            weaponPreviewHolder.Rotate(0f,  rotationSpeed * Time.unscaledDeltaTime, 0, Space.World);

        }
        else if (previewCameraInput.x < -0.75f)
        {
            //currentWeaponPreview.transform.Rotate(Vector3.back * rotationSpeed * Time.unscaledDeltaTime);
            weaponPreviewHolder.transform.Rotate(0f, -rotationSpeed * Time.unscaledDeltaTime, 0f, Space.World);
        }
        //zoom
        if (previewCameraInput.y > 0.75f && weaponPreviewHolder.transform.position.z >= minZoomDistance)
        {
            Vector3 newPosition = weaponPreviewHolder.transform.position;
            newPosition.z -= zoomSpeed * Time.unscaledDeltaTime;
            newPosition.z = Mathf.Max(newPosition.z, minZoomDistance);
            weaponPreviewHolder.transform.position = newPosition;
        }
        else if (previewCameraInput.y < -0.75f && weaponPreviewHolder.transform.position.z <= maxZoomDistance)
        {
            Vector3 newPosition = weaponPreviewHolder.transform.position;
            newPosition.z += zoomSpeed * Time.unscaledDeltaTime;
            newPosition.z = Mathf.Min(newPosition.z, maxZoomDistance);
            weaponPreviewHolder.transform.position = newPosition;
        }
    }
    //void HandleFocusComponentsInput()
    //{
    //    if (focusComponentsInput)
    //    {
    //        focusComponentsInput = false;
    //        ToggleStatTooltipNavigation(false);
    //        ToggleEvolutionNavigation(false);
    //        ToggleComponentNavigation(true);
    //    }
    //}
    public void FocusEvolutions()
    {
        //focusEvolutionsInput = false;
        CloseWeaponSubmenu();
        WeaponScript wpn = activeWeapon.GetComponent<WeaponScript>();
        WeaponData wpnData = ItemDropManager.GetDB().GetWeaponData(wpn.stats.weaponId);
        List<string> evolves = wpnData.evolveWeaponIds;
        List<string> availEvolves = WeaponsController.instance.GetAvailableEvolves(wpn);
        if (availEvolves.Count == 0)
            return; // Do nothing
        ToggleComponentNavigation(false);
        ToggleStatTooltipNavigation(false);
        ToggleEvolutionNavigation(true);
        //select first available evol
        bool selected = false;
        if (evolves.Count > 0 && wpnEvolveBtn1 != null && wpnEvolveBtn1.gameObject.activeSelf)
        {  
            Button button = wpnEvolveBtn1.GetComponentInChildren<Button>();
            if (availEvolves.Contains(evolves[0]))
            {
                button.Select();
                selected = true;
            }
        }
        if (evolves.Count > 1 && wpnEvolveBtn2 != null && wpnEvolveBtn2.gameObject.activeSelf)
        {  
            Button button = wpnEvolveBtn2.GetComponentInChildren<Button>();
            if (!selected && availEvolves.Contains(evolves[1]))
            {
                button.Select();
                selected = true;
            }
        }
    }
    //float wpnScrollVal = 0;
    void HandleSwitchWeaponInput()
    {
        if (switchWeaponUp)
        {
            //Debug.Log("switchWeaponUp " + curWeaponPage);
            switchWeaponUp = false;
            if (PlayerWeaponManager.instance.TotalWeapons() <= 1)//only 1 weapon case
                return;
            if(submenuActive) //dont allow while on submenu
                return;
            int DISPLAYED_PAGES = 3;
            if (curWeaponPage <= PlayerWeaponManager.instance.TotalWeapons() - DISPLAYED_PAGES + 1)
            {
                curWeaponPage++;
                //PlayerWeaponManager.instance.NextWeapon();
            }
            if (curWeaponPage < PlayerWeaponManager.instance.TotalWeapons() - DISPLAYED_PAGES)
            {
                LoadWeaponsToScreen();
                DisplayActiveWeapon();
            }
            else if (curWeaponPage == PlayerWeaponManager.instance.TotalWeapons() - DISPLAYED_PAGES+1)//2nd to last
            {
                LoadWeaponsToScreen(1);//load weapons and add 1 empty extra panel
                DisplayActiveWeapon();
            }
            else if (curWeaponPage == PlayerWeaponManager.instance.TotalWeapons() - DISPLAYED_PAGES + 2)//last
            {
                LoadWeaponsToScreen(2);
                DisplayActiveWeapon();
            }
        }
        else if (switchWeaponDown)
        {
            //Debug.Log("switchWeaponDown " + curWeaponPage);
            switchWeaponDown = false;
            if (PlayerWeaponManager.instance.TotalWeapons() <= 1)//only 1 weapon case
                return;
            if (curWeaponPage > 0)
            {
                //PlayerWeaponManager.instance.PrevWeapon();
                curWeaponPage--;
                int DISPLAYED_PAGES = 3;
                if (curWeaponPage == PlayerWeaponManager.instance.TotalWeapons() - DISPLAYED_PAGES + 1)//2nd to last
                {
                    LoadWeaponsToScreen(1);//load weapons and add 1 empty extra panel
                }
                else
                LoadWeaponsToScreen();
                DisplayActiveWeapon();
            }
        }
    }
    public void WeaponUpOnclick()
    {
        switchWeaponUp = true;
    }
    public void WeaponUpDownclick()
    {
        switchWeaponDown = true;
    }
    void HandleEquipWeaponInput()
    {
        if (equipWeaponInput)
        {
            //Debug.Log("HandleEquipWeaponInput");
            equipWeaponInput = false;
            if (submenuActive)
                return;
            if (activeWeapon)
            {
                PlayerWeaponManager.instance.EquipWeapon(activeWeapon);
                LoadWeaponsToScreen();//to disable/enable buttons
            }
        }
    }
    void HandleOpenWeaponSubmenuInput()
    {
        if (weaponSubmenuInput)
        {
            weaponSubmenuInput = false;
            //Debug.Log("breakdownWeaponPerformed");
            //BreakDownActiveWeapon();
            if(!submenuActive)
                OpenWeaponSubmenu();
        }
    }
    public void SalvageConfirmOnClick() { BreakDownActiveWeapon(); CloseSalvageConfirmWindow(); }
    void OpenWeaponSubmenu()
    {
        ToggleComponentNavigation(false);
        Debug.Log("OpenWeaponSubmenu");
        if(weaponSubmenu != null && !submenuActive)
        {
            weaponSubmenu.SetActive(true);
            submenuActive = true;
        }
    }
    public void CloseWeaponSubmenu()
    {
        ToggleComponentNavigation(true);
        Debug.Log("CloseWeaponSubmenu");
        if (weaponSubmenu != null)
            weaponSubmenu.SetActive(false);
        submenuActive = false;
    }
    public void OpenSalvageConfirmWindow() {
        CloseWeaponSubmenu();
        if (salvageConfirmWindow != null && !submenuActive)
        {
            if(canBreakdownActiveWeapon) 
                salvageConfirmWindow.SetActive(true);
            else if (salvageErrorWindow  != null) 
                salvageErrorWindow.SetActive(true);
            submenuActive = true;
        }
    }
    public void CloseSalvageConfirmWindow() { 
        if (salvageConfirmWindow != null) 
            salvageConfirmWindow.SetActive(false);
        submenuActive = false;
    }
    public void CloseErrorWindow()
    {
        if (salvageErrorWindow != null)
            salvageErrorWindow.SetActive(false);
        submenuActive = false;
    }
    /* Stat tooltips */
    bool helpActive = false;
    bool handlingHelpInput = false;

    void HandleHelpInput()
    {

        if (helpInput)
        {
            helpInput = false;
            if(handlingHelpInput)
                return;
            if (submenuActive)
                return;
            handlingHelpInput = true;
            helpActive = !helpActive;
            if (helpActive)
            {
                ToggleStatTooltipNavigation(true);
                ToggleComponentNavigation(false);
                ToggleEvolutionNavigation(false);
                //select first stat
                foreach (Transform obj in primaryStatsText.transform)
                {
                    obj.GetComponent<Button>().Select();
                    break;
                }
            }
            else
            {
                ToggleStatTooltipNavigation(false);
                ToggleComponentNavigation(true);// will select first component if available
                ToggleEvolutionNavigation(false);// prolly not necessary but just in case
            }
            handlingHelpInput = false;
        }
    }
    GameObject currentCursorObj = null;
    public void HandleGamepadSelectedObject()
    {

        if (currentCursorObj != eventSystem.currentSelectedGameObject)
        { // Need to change active tooltip window
            //Debug.Log("TooltipActive & New cursor obj");
            currentCursorObj = eventSystem.currentSelectedGameObject;
            if (currentCursorObj != null)
            {
                //Debug.Log("currentCursorObj not null");
                TinkerComponentUI ui = currentCursorObj.GetComponentInParent<TinkerComponentUI>();
                if (ui != null)
                { // currently on a tinker component
                    activeComponent = ui.refComponent;
                    activeComponentId = ui.refItemId;
                    LoadActiveWeaponStats();
                    //Debug.Log("cur ui " + ui.tooltip.text);
                    foreach (Transform componentObj in componentsGrid.transform)  // refresh tooltip
                    {
                        if (componentObj.gameObject == currentCursorObj.GetComponentInParent<TinkerComponentUI>().gameObject)
                        {
                            TinkerComponentStats refComponent = componentObj.GetComponent<TinkerComponentUI>().refComponent;
                            string refItemId = componentObj.GetComponent<TinkerComponentUI>().refItemId;
                            if (refComponent != null)
                                SetTooltipToComponent(refComponent, refItemId);
                            else
                            {
                                tooltipUI.headerText.text = "";
                                tooltipUI.centerText.text = "";
                                tooltipUI.bottomText.text = "";
                            }
                        }
                    }
                }
                else if (activeComponent != null)
                {//last selected was a component
                    activeComponent = null;
                    //foreach (Transform obj in componentsGrid.transform)
                    //    obj.GetComponent<TinkerComponentUI>().tooltipHolder.SetActive(false);
                }
                foreach (Transform obj in primaryStatsText.transform)
                    obj.GetComponent<TogglingBehavior>().Toggle(false);
                foreach (Transform obj in elementalStatsText.transform)
                    obj.GetComponent<TogglingBehavior>().Toggle(false);
                TogglingBehavior togglingBehavior = currentCursorObj.GetComponent<TogglingBehavior>();
                if (togglingBehavior != null)
                {
                    togglingBehavior.Toggle(true);
                }
            }
            else // Nothing is selected. If on gamepad try to select something
            {
                //TODO: if (gamepad) 
                if (helpActive)
                {
                    foreach (Transform obj in primaryStatsText.transform)
                    {
                        obj.GetComponent<Button>().Select();
                        break;
                    }
                }
                else
                {
                    foreach (Transform obj in componentsGrid.transform)
                    {
                        obj.GetComponent<TinkerComponentUI>().mainButton.Select(); ;
                        //obj.GetComponent<TinkerComponentUI>().tooltipHolder.SetActive(true);
                        break;
                    }
                }
            }
        }
    }
    //************************** B U T T O N S **************************
    /**
     * Turn active weapon into a component
     * @param checkOnly - will not breakdown but only check if possible if set to true
     * return error string or "CanSalvage" if possible to salvage
     */
    public string BreakDownActiveWeapon(bool checkOnly=false)
    {
        try
        {
            if(activeWeapon == null)
                throw new Exception("No active weapon");
            WeaponScript activeWpnScript = activeWeapon.GetComponent<WeaponScript>();
            PlayerWeaponManager playerWpns = PlayerWeaponManager.instance;
            bool isSpecial = activeWpnScript.isSpecialWeapon;
            int index = isSpecial ? playerWpns.ownedSpecialWeapons.IndexOf(activeWeapon) : playerWpns.ownedWeapons.IndexOf(activeWeapon);
            if (activeWpnScript.stats.level < 5)
                throw new Exception("Must be Level 5 or over");
            if (activeWpnScript.isSpecialWeapon)
            {
                if(playerWpns.GetOffHand().gameObject == activeWeapon)
                    throw new Exception("Can't breakdown equipped weapon");
                if (playerWpns.ownedSpecialWeapons.Count <= 1)
                    throw new Exception("Last off hand weapon");
            }
            else
            {
                if (playerWpns.GetMainHand().gameObject == activeWeapon)
                    throw new Exception("Can't breakdown equipped weapon");
                if (playerWpns.ownedWeapons.Count <= 1)
                    throw new Exception("Last main hand weapon");
            }
            //all updates should be below here
            if (!checkOnly)
            {
                //break down weapon
                WeaponSalvageComponent newComp = TinkerComponentManager.instance.BreakDownWeapon(index, isSpecial, playerWpns);
                Inventory inventory = PlayerWeaponManager.instance.GetComponent<Inventory>();
                inventory.weaponSalvageComponents.Add(newComp);

                //Debug.Log("Broke down " + newComp.stats.itemName);
                //add to screen
                activeWeapon = null;
                ReloadUpgradeMenu();
            }
        }
        catch (Exception e) //Catches not lvl 5 or over error / no active weapon
        {
            if(!checkOnly)
                Debug.Log(e.Message);
            return (e.Message);
        }
        return ("CanSalvage");
    }
    /**
     * Levels up equipped weapon
     */
    public void DebugLvlUpEquippedWeapon()
    {
        PlayerWeaponManager wpnManager = PlayerWeaponManager.instance;
        WeaponScript wpn = activeWeapon != null? activeWeapon.GetComponent<WeaponScript>() :wpnManager.GetMainHand();
        wpn.stats.level++;
        wpn.stats.currentTinkerPoints += wpn.stats.tinkerPointsPerLvl;
        //Debug.Log("Leveled up " + wpn.stats.weaponName + " to level " + wpn.stats.level + ".");
        DisplayActiveWeapon();//update screen
        LoadWeaponsToScreen();
        LoadComponentsToScreen();
    }
    //************************** W E A P O N   S C R O L L **************************
    /**
     * This section controls the weapon scroll bar TODO - Pretty sure this is not used anymore, Attempt removal
     */
    [Header("Weapon Box Scroll")]
    public Scrollbar wpnScroll;
    public float currentStep = 0;
    public float lastStep = 0;
    public const int wpnPerRow = 1;
    public void WeaponScroll(float value)
    {
        int weaponsCount = PlayerWeaponManager.instance.ownedWeapons.Count + PlayerWeaponManager.instance.ownedSpecialWeapons.Count;
        int numOfPage = weaponsCount / wpnPerRow;
        if (numOfPage < 2)
        {
            wpnScroll.gameObject.SetActive(false);
        }
        else
        {
            wpnScroll.gameObject.SetActive(true);
            wpnScroll.numberOfSteps = numOfPage;
            wpnScroll.size = 1.0f / numOfPage;
            currentStep = Mathf.Round(wpnScroll.value * numOfPage);
        }
        if (currentStep == lastStep)
        {
            return; //no change
        }
        if (currentStep > lastStep)
        {
            curWeaponPage++;
        }
        else
        {
            curWeaponPage--;
        }
        if (curWeaponPage > numOfPage)
        {// past the end go to beg
            curWeaponPage = 0;
        }
        else if (curWeaponPage < 0)
        {//past beggining go to end
            curWeaponPage = numOfPage;
        }
        lastStep = currentStep;
        LoadWeaponsToScreen();
    }
    //************************** C O M P O N E N T   S C R O L L **************************
    /**
     * This section controls the Component scroll bar
     */
    [Header("Component Box Scroll")]
    public Scrollbar cmpntScroll;
    public float cmpntCurrentStep = 0;
    public float cmpntLastStep = 0;
    public const int cmpntPerRow = 14;
    public void ComponentScroll(float value)
    {
        int count = 0;//count total unique components owned

        Inventory inventory = PlayerWeaponManager.instance.GetComponent<Inventory>();
        foreach (var item in inventory.GetTinkerComponents())
        {
            if(item.Value.quantity > 0)
                count++;
        }
        //foreach (var item in TinkerComponentManager.instance.baseComponents)
        //{
        //    if(item.GetComponent<TinkerComponent>().stats.count > 0) count++;
        //}
        count += inventory.weaponSalvageComponents.Count;
        int numOfPage = count / cmpntPerRow;
        if (numOfPage < 2)
        {
            cmpntScroll.gameObject.SetActive(false);
        }
        else
        {
            cmpntScroll.gameObject.SetActive(true);
            cmpntScroll.numberOfSteps = numOfPage;
            cmpntScroll.size = 1.0f / numOfPage;
            cmpntCurrentStep = Mathf.Round(cmpntScroll.value * numOfPage);
        }
        if (cmpntCurrentStep == cmpntLastStep) 
            return; // no change
        if (cmpntCurrentStep > cmpntLastStep)
        {
            curComponentPage++;
        }
        else
        {
            curComponentPage--;
        }
        if (curComponentPage > numOfPage)
        {// past the end go to beg
            curComponentPage = 0;
        }
        else if (curComponentPage < 0)
        {//past beggining go to end
            curComponentPage = numOfPage;
        }
        cmpntLastStep = cmpntCurrentStep;
        LoadComponentsToScreen();
    }
    //************************** L O A D    I N V E N T O R Y **************************
    /**
     * Put equipped weapon data on screen
     */
    void DisplayActiveWeapon()
    {

        //string primaryStats = "";
        //string elementalStats = "";
        if (activeWeapon == null) 
            Debug.Log("Active weapon null");
        LoadActiveWeaponStats();
        LoadWeaponEvolveButtons();
        if (activeWeapon)
        {
            WeaponScript wpn = activeWeapon.GetComponent<WeaponScript>();
            //preview
            if (currentWeaponPreview != null)
                Destroy(currentWeaponPreview);
            currentWeaponPreview = Instantiate(activeWeapon, weaponPreviewHolder);
            currentWeaponPreview.SetActive(true);
            if (wpn.isSpecialWeapon && wpn.stats.weaponType != WeaponType.Dagger && wpn.stats.weaponType != WeaponType.BowieKnife)
            {
                currentWeaponPreview.transform.localPosition = new Vector3(0, -0.05f, 0);
                currentWeaponPreview.transform.localRotation = Quaternion.Euler(340f, 295f, 305f);
                weaponPreviewHolder.localPosition = new Vector3(0, 0, 0);
                weaponPreviewHolder.localRotation = Quaternion.Euler(0, 0, 315f);
            }
            else
            {
                currentWeaponPreview.transform.localPosition = new Vector3(0, -0.5f, 0);
                currentWeaponPreview.transform.localRotation = Quaternion.Euler(90f, 90f, 0);
                weaponPreviewHolder.localPosition = new Vector3(0, 0, 0);
                weaponPreviewHolder.localRotation = Quaternion.Euler(0, 0, 315f);
            }
            currentWeaponPreview.layer = LayerMask.NameToLayer("WeaponPreview");
            foreach (Transform t in currentWeaponPreview.GetComponentsInChildren<Transform>())
                t.gameObject.layer = LayerMask.NameToLayer("WeaponPreview");
        }
        else
        {
            //primaryStats = "Equipped - None\n\n\n\n";
            //wpnEvolveBtn1.SetActive(false);
            //wpnEvolveBtn2.SetActive(false);
        }
        //primaryStatsText.text = primaryStats;
        //elementalStatsText.text = elementalStats;
    }
    WeaponData GetWeaponData(string weaponId)
    {
        return ItemDropManager.GetDB().GetWeaponData(weaponId);
    }
    void LoadWeaponEvolveButtons()
    {
        if (activeWeapon)
        {
            WeaponScript wpn = activeWeapon.GetComponent<WeaponScript>();
            //weapon evolves
            WeaponsController weaponCntrller = WeaponsController.instance;
            WeaponData wpnData = GetWeaponData(wpn.stats.weaponId);
            List<string> evolves = wpnData.evolveWeaponIds;
            List<string> availEvolves = WeaponsController.instance.GetAvailableEvolves(wpn);
            //Debug.Log("availEvolves =" + availEvolves.Count);//astest
            if (evolves.Count >= 1 && evolves[0].Trim().Length > 0)
            {
                //Debug.Log("evolves[0]=" + evolves[0]);
                wpnEvolveBtn1.SetActive(true);
                //WeaponScript evolWpn = weaponCntrller.baseWeapons[(int)evolves[0]].GetComponent<WeaponScript>();
                WeaponData evolWpnData = GetWeaponData(evolves[0]);
                ItemDetails evolWpnDetails = ItemDropManager.GetDB().GetItem(evolves[0]);
                //Debug.Log("evolWpnDetails 1 ="+ evolWpnDetails.itemName);//astest
                GridElementController myBtnScrpt = wpnEvolveBtn1.GetComponent<GridElementController>();
                if (availEvolves.Contains(evolves[0]))
                {
                    myBtnScrpt.topText.text = evolWpnDetails.itemName;
                    myBtnScrpt.mainButton.interactable = true;
                    myBtnScrpt.mainButtonForeground.GetComponent<Image>().sprite = evolWpnDetails.icon;
                    myBtnScrpt.mainButton.onClick.RemoveAllListeners();
                    myBtnScrpt.mainButton.onClick.AddListener(() => //Evolve Weapon Button
                    {
                        weaponCntrller.EvolveWeapon(activeWeapon, evolves[0], PlayerWeaponManager.instance);
                        ReloadUpgradeMenu();
                    });
                }
                else
                {
                    myBtnScrpt.topText.text = evolWpnDetails.itemName;
                    //TODO: I'm setting it to always show ??? instead of the name when already discovered
                    //if (!WeaponsController.instance.CheckHasObtained(evolWpn.stats.weaponType))
                    //{
                    String mysteryText = "";
                    foreach (char c in myBtnScrpt.topText.text)
                        mysteryText += c == ' ' ? ' ' : '_';
                    myBtnScrpt.topText.text = mysteryText;
                    //}
                    myBtnScrpt.mainButton.interactable = false;
                    myBtnScrpt.mainButtonForeground.GetComponent<Image>().sprite = defaultUnkownIcon;
                    //myBtnScrpt.mainButtonForeground.GetComponent<RawImage>().texture = defaultUnkownIcon.texture;
                }
            }
            else if (wpnEvolveBtn1 != null)
                wpnEvolveBtn1.SetActive(false);
            if (evolves.Count >= 2 && evolves[1].Trim().Length > 0)
            {//2nd weapon evolve
                //Debug.Log("evolves[1]=" + evolves[1]);
                wpnEvolveBtn2.SetActive(true);
                //WeaponScript evolWpn = weaponCntrller.baseWeapons[(int)evolves[1]].GetComponent<WeaponScript>();
                WeaponData evolWpnData = GetWeaponData(evolves[1]);
                ItemDetails evolWpnDetails = ItemDropManager.GetDB().GetItem(evolves[1]);
                Debug.Log("evolWpnDetails 2 =" + evolWpnDetails.itemName);//astest
                GridElementController myBtnScrpt2 = wpnEvolveBtn2.GetComponent<GridElementController>();
                if (availEvolves.Contains(evolves[1]))
                {
                    myBtnScrpt2.topText.text = evolWpnDetails.itemName;
                    myBtnScrpt2.mainButton.interactable = true;
                    myBtnScrpt2.mainButtonForeground.GetComponent<Image>().sprite = evolWpnDetails.icon;
                    myBtnScrpt2.mainButton.onClick.RemoveAllListeners();
                    myBtnScrpt2.mainButton.onClick.AddListener(() => //Evolve 2 Weapon button
                    {
                        weaponCntrller.EvolveWeapon(activeWeapon, evolves[1], PlayerWeaponManager.instance);
                        ReloadUpgradeMenu();
                    });
                }
                else
                {
                    //TODO: I'm setting it to always show ??? instead of the name when already discovered
                    //if (!WeaponsController.instance.CheckHasObtained(evolWpn.stats.weaponType))
                    //{
                    String mysteryText = "";
                    foreach (char c in myBtnScrpt2.topText.text)
                        mysteryText += c == ' ' ? ' ' : '_';
                    myBtnScrpt2.topText.text = mysteryText;
                    //}
                    myBtnScrpt2.mainButton.interactable = false;
                    //myBtnScrpt2.bottomText.text = "";
                    myBtnScrpt2.mainButtonForeground.GetComponent<Image>().sprite = defaultUnkownIcon;
                }
            }
            else if (wpnEvolveBtn2 != null)
                wpnEvolveBtn2.SetActive(false);
        }
        else
        {
            //primaryStats = "Equipped - None\n\n\n\n";
            wpnEvolveBtn1.SetActive(false);
            wpnEvolveBtn2.SetActive(false);
        }
    }
    private TinkerComponentStats activeComponent = null;//selected with gamepad or on hover
    private string activeComponentId = "";//selected with gamepad or on hover
    void LoadActiveWeaponStats()
    {
        foreach (Transform child in primaryStatsText.transform)
            Destroy(child.gameObject);
        foreach (Transform child in elementalStatsText.transform)
            Destroy(child.gameObject);
        foreach (Transform child in expStatsText.transform)
            Destroy(child.gameObject);
        if (activeWeapon == null || activeWeapon.GetComponent<WeaponScript>() == null)
        {// no active weapon
            weaponPreviewHeaderText.text = "";
            activeWeaponTierLevelText.text = "";
            tinkerPointsCountText.text = "";
            return;
        }
        WeaponScript wpn = activeWeapon.GetComponent<WeaponScript>();
        weaponPreviewHeaderText.text = wpn.stats.weaponName;
        activeWeaponTierLevelText.text = wpn.GetWeaponFamilyFormatted() + "\nLevel " + wpn.stats.level;
        tinkerPointsCountText.text = "" + wpn.stats.currentTinkerPoints;
        WeaponStats stats = wpn.stats;
        ElementalStats el = stats.elemental;
        foreach (KeyValuePair<string, float> stat in wpn.GetPrimaryStatsForDisplay()) 
            LoadStat(stat, primaryStatsText.transform);
        // Durability
        LoadDurability(stats);
        // Exp
        GameObject curExpText1 = Instantiate(statsTextPrefab, expStatsText.transform);
        GameObject curExpText2 = Instantiate(statsTextPrefab, expStatsText.transform);
        GameObject neededExpText1 = Instantiate(statsTextPrefab, expStatsText.transform);
        GameObject neededExpText2 = Instantiate(statsTextPrefab, expStatsText.transform);
        curExpText1.GetComponent<EventTrigger>().enabled = false; // disable hover events
        curExpText2.GetComponent<EventTrigger>().enabled = false;
        neededExpText1.GetComponent<EventTrigger>().enabled = false;
        neededExpText2.GetComponent<EventTrigger>().enabled = false;
        curExpText1.GetComponent<TextMeshProUGUI>().text = "Current Exp:";
        curExpText2.GetComponent<TextMeshProUGUI>().text = "" + stats.currentExperiencePoints;
        neededExpText1.GetComponent<TextMeshProUGUI>().text = "To Next Level:";
        neededExpText2.GetComponent<TextMeshProUGUI>().text = "" + stats.experiencePointsToNextLevel;
        // Elemental
        foreach (KeyValuePair<string, float> stat in wpn.GetElementalStats()) 
            LoadStat(stat, elementalStatsText.transform);
    }
    KeyValuePair<string, float> LoadStat(KeyValuePair<string, float> stat, Transform trans)
    {
        GameObject statTextObj = Instantiate(statsTextPrefab, trans);
        bool greenTextShowing = false;
        string greenText = "";
        if (activeComponent != null)
        {
            if (activeComponent.GetStats().ContainsKey(stat.Key))
            {
                greenTextShowing = true;
                greenText += "<size=16> + " + activeComponent.GetStats()[stat.Key] + "</color></size>";
            }
        }
        statTextObj.GetComponent<TextMeshProUGUI>().text = stat.Key + ": " + (greenTextShowing ? "<color=\"green\">" : "") + stat.Value + greenText;
        Button tooltipNavButton = statTextObj.GetComponentInChildren<Button>();
        if (tooltipNavButton != null)
        { // This handles the helper tooltips for the stats.
            TogglingBehavior tooltipToggler = statTextObj.GetComponent<TogglingBehavior>();
            if (tooltipToggler != null)
            {
                TooltipUI tooltip = tooltipToggler.Toggle(true)[0].gameObject.GetComponent<TooltipUI>();
                tooltip.headerText.text = stat.Key;
                tooltip.centerText.text = WeaponScript.GetStatTooltip(stat.Key);
            }
            if (helpActive)
            {
                Navigation nav = tooltipNavButton.navigation;
                nav.mode = Navigation.Mode.Automatic;
                tooltipNavButton.navigation = nav;
            }
        }
        return stat;
    }
    void LoadDurability(WeaponStats stats)
    {
        // Durability
        GameObject durabiltyLeft = Instantiate(statsTextPrefab, expStatsText.transform);
        durabiltyLeft.GetComponent<TextMeshProUGUI>().text = "Durability:";
        GameObject durabiltyRight = Instantiate(statsTextPrefab, expStatsText.transform);
        // green text for max durability
        string greenTextEnd = "";
        string greenTextStart = "";
        if (activeComponent != null && activeComponent.durability > 0)
        {
            greenTextStart = "<color=\"green\">";
            greenTextEnd += "<size=16> + " + activeComponent.durability + "</color></size>";
        }
        durabiltyRight.GetComponent<TextMeshProUGUI>().text = stats.currentDurability + " / " + greenTextStart + stats.durability + greenTextEnd;
        //if (activeComponentId.Equals("repair_kit") && stats.currentDurability < stats.durability)
        //{ // green text for repair kit
        //    durabiltyRight.GetComponent<TextMeshProUGUI>().text = stats.currentDurability + "<color=\"green\"><size=16> + 25</color></size>" 
        //        + " / " + greenTextStart + stats.durability + greenTextEnd;
        //}
    }
    bool canBreakdownActiveWeapon = false;
    /**
     * Clear weapons grid and reload it with current values
     */
    void LoadWeaponsToScreen(int extra = 0)
    {
        //WeaponScroll(0);
        PlayerWeaponManager playerWpns = PlayerWeaponManager.instance;
        int numOfPage = (playerWpns.ownedWeapons.Count + playerWpns.ownedSpecialWeapons.Count) / wpnPerRow;
        wpnScroll.numberOfSteps = numOfPage;
        wpnScroll.size = 1.0f / numOfPage;
        int maxDisplayed = 3;
        int displayed = 0;
        foreach (Transform child in weaponsGrid.transform)
        {
            Destroy(child.gameObject);
        }
        //main hand weapons
        for (int i = curWeaponPage * wpnPerRow; i < playerWpns.ownedWeapons.Count; i++) //nonspecial weapons
        {
            if (displayed >= maxDisplayed) break;
            GameObject wpn = playerWpns.ownedWeapons[i];
            if (wpn == null) continue;
            displayed++;
            WeaponScript wpnScrpt = wpn.GetComponent<WeaponScript>();
            if (displayed == 1)
            {//first
                activeWeapon = wpnScrpt.gameObject;
                //Debug.Log("Setting Active Weapon to " + wpnScrpt.stats.weaponName);
                Color salvageButtonColor = salvageButtonIconGamepad.color;
                Color salvageTextColor = salvageControlText.color;
                string resultText = BreakDownActiveWeapon(true);
                if (resultText == "CanSalvage")
                {
                    canBreakdownActiveWeapon = true;
                    salvageButtonColor.a = 1;
                    salvageTextColor.a = 1;
                }
                else
                {
                    canBreakdownActiveWeapon = false;
                    salvageButtonColor.a = 0.5f;
                    salvageTextColor.a = 0.5f;
                    salvageErrorWindow.GetComponent<TooltipUI>().centerText.text = resultText;
                }
                salvageButtonIconGamepad.color = salvageButtonColor;
                salvageControlText.color = salvageTextColor;
            }
            GameObject gridElement = Instantiate(this.weaponButton, weaponsGrid.transform);
            WeaponButtonUI weaponButton = gridElement.GetComponent<WeaponButtonUI>();
            if (weaponButton.tooltip != null)
                weaponButton.tooltip.text = wpnScrpt.stats.weaponName;
            if (wpnScrpt.spr)//load icon
                weaponButton.mainButtonForeground.GetComponent<Image>().sprite = wpnScrpt.spr;
            if (i == playerWpns.indexOfEquippedWeapon)
            {//mark equipped weapon
                weaponButton.mainButton.interactable = false;
            }
            //if (wpn == activeWeapon)
            //{//mark actively editing weapon
            //    weaponButton.mainButton.GetComponent<Image>().color = Color.red;
            //}
            /**   ADD UNSPECIAL WEAPON CLICK EVENTS   */
            weaponButton.index = i;
            weaponButton.mainButton.onClick.AddListener(() =>
            {
                playerWpns.ChangeWeapon(weaponButton.index);//equip weapon
                activeWeapon = wpn;//set actively editing
                LoadWeaponsToScreen();
                DisplayActiveWeapon();
                LoadComponentsToScreen();
            });
        }
        int wpnsToSkip = curWeaponPage * wpnPerRow - playerWpns.ownedWeapons.Count;
        int index = 0;
        foreach (GameObject weapon in playerWpns.ownedSpecialWeapons) //special weapons
        {
            if (index < wpnsToSkip)
            {
                index++;
                continue;
            }
            if (weapon == null) continue;
            if (displayed >= maxDisplayed) break;
            displayed++;
            WeaponScript wpnScrpt = weapon.GetComponent<WeaponScript>();
            GameObject gridElement = Instantiate(this.weaponButton, weaponsGrid.transform);
            WeaponButtonUI weaponButton = gridElement.GetComponent<WeaponButtonUI>();
            if (displayed == 1)
            {//first
                activeWeapon = wpnScrpt.gameObject;
                //Debug.Log("Setting Active Weapon to " + wpnScrpt.stats.weaponName);
                Color salvageButtonColor = salvageButtonIconGamepad.color;
                Color salvageTextColor = salvageControlText.color;
                if (BreakDownActiveWeapon(true) == "CanSalvage")
                {
                    canBreakdownActiveWeapon = true;
                    salvageButtonColor.a = 1;
                    salvageTextColor.a = 1;
                }
                else
                {
                    canBreakdownActiveWeapon = false;
                    salvageButtonColor.a = 0.5f;
                    salvageTextColor.a = 0.5f;
                }
                salvageButtonIconGamepad.color = salvageButtonColor;
                salvageControlText.color = salvageTextColor;
            }
            if (weaponButton.tooltip != null)
                weaponButton.tooltip.text = wpnScrpt.stats.weaponName;
            if (wpnScrpt.spr)//load icon
                weaponButton.mainButtonForeground.GetComponent<Image>().sprite = wpnScrpt.spr;
            if (index == playerWpns.indexOfEquippedSpecialWeapon)
            {//mark equipped weapon
                weaponButton.mainButton.interactable = false;
            }
            //    if (weapon == activeWeapon)
            //{//mark actively editing weapon
            //    weaponButton.mainButton.Select();
            //}
            weaponButton.index = index;
            /**   ADD SPECIAL WEAPON CLICK EVENTS   */
            weaponButton.mainButton.onClick.AddListener(() =>
            {
                activeWeapon = weapon;//set actively editing
                LoadWeaponsToScreen();
                DisplayActiveWeapon();
                LoadComponentsToScreen();
            });
            index++;
        }
        if (PlayerWeaponManager.instance.TotalWeapons() == 0)//edge case no weapons
            extra = 3;
        else if (PlayerWeaponManager.instance.TotalWeapons() == 1)//edge case only 1 weapon
            extra = 2;
        else if (PlayerWeaponManager.instance.TotalWeapons() == 2)//edge case only 2 weapons
            extra = 1;
        if (extra > 0)
        {
            for (int i = 0; i < extra; i++)
            {
                Instantiate(this.weaponButton, weaponsGrid.transform)
                    .GetComponent<WeaponButtonUI>().mainButtonForeground.SetActive(false);
            }
        }
    }
    /**
     * Clear component list and reload it with current values
     */
    int currentlySelectedComponentIndex = 0;
    //bool componentButtonSelected = false;
    void LoadComponentsToScreen()
    {
        foreach (Transform child in componentsGrid.transform)
        {
            Destroy(child.gameObject);
        }
        int displayedCount = 0;
        int maxDisplayed = 28;
        int componentsToSkip = curComponentPage * cmpntPerRow;
        //basic components
        int index = 0;
        //componentButtonSelected = false;
        Inventory inventory = PlayerWeaponManager.instance.GetComponent<Inventory>();
        Dictionary<string,InventoryItem> ownedComponents = inventory.GetTinkerComponents();
        if(ownedComponents.Count == 0)
        {
            tooltipUI.headerText.text = "";
            tooltipUI.centerText.text = "";
            tooltipUI.bottomText.text = "";
        }
        foreach (KeyValuePair<string, InventoryItem> kvp in ownedComponents)
        {
            string itemId = kvp.Key;
            int quantity = kvp.Value.quantity;
            ItemDatabase itemDatabase = ItemDropManager.GetDB();
            TinkerComponentData tinkerComponentData = itemDatabase.GetTinkerComponentData(itemId);
            ItemDetails itemDetails = itemDatabase.GetItem(itemId);
            if (quantity > 0)
            {
                if(componentsToSkip > 0)
                {
                    componentsToSkip--;
                    continue;
                }
                index++;
                if (++displayedCount > maxDisplayed) break;
                GameObject gridElement = Instantiate(tinkerComponentPrefab, componentsGrid.transform);
                TinkerComponentUI tinkerComponentUI = gridElement.GetComponent<TinkerComponentUI>();
                if (tinkerComponentUI == null) break;
                if (tinkerComponentData == null) Debug.Log("tinkerComponentData null:" + itemId);
                else if (tinkerComponentData.stats == null) Debug.Log("tinkerComponentData stats null:" + itemId);
                tinkerComponentUI.index = index;
                tinkerComponentUI.refComponent = tinkerComponentData.stats;
                tinkerComponentUI.refItemId = tinkerComponentData.itemId;
                //Add tooltip on hover event
                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerEnter;
                entry.callback.AddListener((eventData) =>
                {
                    SetTooltipToComponent(tinkerComponentData.stats, tinkerComponentData.itemId);
                    tinkerComponentUI.mainButton.Select();
                });
                tinkerComponentUI.mainButton.GetComponent<EventTrigger>().triggers.Add(entry);
                //if (tinkerComponentUI.tooltipUI != null)
                //if (tooltipUI != null)
                //{
                //    //TooltipUI tooltipUI = tinkerComponentUI.tooltipUI;
                //    tooltipUI.headerText.text = componentScript.stats.itemName;
                //    tooltipUI.bottomText.text = componentScript.stats.price + " gp";//gold points - placeholder name
                //    tooltipUI.centerText.text = "";
                //    foreach(KeyValuePair<string,float> stat in componentScript.GetStats())
                //    {
                //        tooltipUI.centerText.text += stat.Key + ": +" + stat.Value + "\n";
                //    }
                //    tooltipUI.gameObject.SetActive(false);
                //}
                //tinkerComponentUI.tooltipHolder.SetActive(false);
                tinkerComponentUI.countText.text = "" + quantity;
                //tinkerComponent.cornerButton.gameObject.SetActive(false);
                if(itemDetails.icon)//Icon
                    tinkerComponentUI.foregroundIcon.GetComponent<Image>().sprite = itemDetails.icon;
                //if (TinkerComponentManager.instance.CanUseComponent(PlayerWeaponManager.instance.GetEquippedWeapon(), component))
                if (displayedCount == currentlySelectedComponentIndex)
                {
                    tinkerComponentUI.mainButton.Select();
                    //Debug.Log("currentlySelectedComponentIndex:" + currentlySelectedComponentIndex + " displayedCount=" + displayedCount);
                    //componentButtonSelected = true;
                }
                if (TinkerComponentManager.instance.CanUseComponent(activeWeapon, itemId, tinkerComponentData.stats))
                {
                    Debug.Log("Can use:" + itemId);
                    /**   ADD EVENT TO COMPONENT CLICK   */
                    tinkerComponentUI.mainButton.onClick.AddListener(() =>
                    {
                        if (itemId.Equals("repair_kit"))
                        {
                            RepairActiveWeapon();
                            DisplayActiveWeapon();
                            LoadComponentsToScreen();
                        }
                        else if (activeWeapon != null && TinkerComponentManager.instance.UseComponent(activeWeapon, itemId, tinkerComponentData.stats))
                        {
                            int newCount = tinkerComponentUI.countText.text.Trim().ParseLargeInteger() - 1;
                            if (newCount > 0)
                            {
                                tinkerComponentUI.countText.text = "" + newCount;
                            }
                            else
                            {
                                Destroy(gridElement);
                            }
                            DisplayActiveWeapon();
                            currentlySelectedComponentIndex = tinkerComponentUI.index;
                            if(currentlySelectedComponentIndex < 0)
                                currentlySelectedComponentIndex = 0;
                            //Debug.Log("onclick currentlySelectedComponentIndex:"+ currentlySelectedComponentIndex);
                            LoadComponentsToScreen();
                        }
                        else
                        {
                            Debug.Log("Failed to use component " + itemDetails.itemName);
                        }
                    });
                }
                //else Debug.Log("Can't use:" + itemId);
                //else // cant use component. disable the button
                //   tinkerComponentUI.mainButton.interactable = false;
            }
        }
        int count = 0;//count total unique components owned
        foreach(var item in ownedComponents)
        {
            if(item.Value.quantity > 0)
                count++;
        }
        count += inventory.weaponSalvageComponents.Count;
        int numOfPage = count / cmpntPerRow;
        if (numOfPage < 2)
        {
            cmpntScroll.gameObject.SetActive(false);
        }
        else
        {
            cmpntScroll.gameObject.SetActive(true);
            cmpntScroll.numberOfSteps = numOfPage;
            cmpntScroll.size = 1.0f / numOfPage;
            cmpntCurrentStep = Mathf.Round(cmpntScroll.value * numOfPage);
        }
    }
    private void ReloadUpgradeMenu()
    {
        LoadWeaponsToScreen();
        DisplayActiveWeapon();
        LoadComponentsToScreen();
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
            LoadControlTooltips();
        }
    }
    void LoadControlTooltips()
    {
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
            EnableAllNavigation();
        }
    }
    private void EnableAllNavigation()
    {
        ToggleComponentNavigation(true);
        ToggleEvolutionNavigation(true);
        //ToggleStatTooltipNavigation(true);
        SetKBMStatTooltipNavigation();
    }
    private void ToggleComponentNavigation(bool enable)
    {
        bool selected = false;
        foreach (Transform t in componentsGrid.transform)
        {
            // toggle navigation
            TinkerComponentUI componentUI = t.GetComponent<TinkerComponentUI>();
            if (componentUI == null) continue;
            Button button = componentUI.mainButton;
            button.interactable = enable;
            Navigation nav = button.navigation;
            nav.mode = enable?Navigation.Mode.Automatic :Navigation.Mode.None;
            button.navigation = nav;
            if (enable && !selected && InputSwitchDetector.IsCurrentlyGamepad())
            { // Select first
                selected = true;
                button.Select();
            }
        }
    }
    private void ToggleEvolutionNavigation(bool enable)
    {
        if (wpnEvolveBtn1 != null && wpnEvolveBtn1.gameObject.activeSelf)
        {// turn off navigation
            Button button = wpnEvolveBtn1.GetComponentInChildren<Button>();
            button.interactable = enable;
            Navigation nav = button.navigation;
            nav.mode = enable?Navigation.Mode.Automatic :Navigation.Mode.None;
            button.navigation = nav;
        }
        if (wpnEvolveBtn2 != null && wpnEvolveBtn2.gameObject.activeSelf)
        {// turn off navigation
            Button button = wpnEvolveBtn2.GetComponentInChildren<Button>();
            button.interactable = enable;
            Navigation nav = button.navigation;
            nav.mode = enable ? Navigation.Mode.Automatic : Navigation.Mode.None;
            button.navigation = nav;
        }
    }
    private void ToggleStatTooltipNavigation(bool enable)
    {
        helpActive = enable;
        Func<Transform, Transform> handleTooltip = (obj) =>
        {
            obj.GetComponent<TogglingBehavior>().Toggle(helpActive);
            // Make navigatiable if tooltip active or turn off navigation if not
            Button button = obj.GetComponent<Button>();
            button.interactable = helpActive;
            Navigation nav = button.navigation;
            nav.mode = helpActive ? Navigation.Mode.Automatic : Navigation.Mode.None;
            obj.GetComponent<Button>().navigation = nav;
            return obj;
        };
        foreach (Transform obj in primaryStatsText.transform) handleTooltip(obj);
        foreach (Transform obj in elementalStatsText.transform) handleTooltip(obj);
    }
    private void SetKBMStatTooltipNavigation()
    {
        Func<Transform, Transform> handleTooltip = (obj) =>
        {
            //obj.GetComponent<TogglingBehavior>().Toggle(helpActive);
            // Make navigatiable if tooltip active or turn off navigation if not
            Button button = obj.GetComponent<Button>();
            if (button == null) return obj;
            button.interactable = true;
            Navigation nav = button.navigation;
            nav.mode = Navigation.Mode.None;
            obj.GetComponent<Button>().navigation = nav;
            return obj;
        };
        foreach (Transform obj in primaryStatsText.transform) handleTooltip(obj);
        foreach (Transform obj in elementalStatsText.transform) handleTooltip(obj);
    }
    private void SetTooltipToComponent(TinkerComponentStats component, string itemId)
    {

        ItemDatabase itemDatabase = ItemDropManager.GetDB();
        string itemName = itemDatabase.GetItem(itemId).itemName;
        tooltipUI.headerText.text = itemName;
        tooltipUI.centerText.text = "";
        foreach (KeyValuePair<string, float> stat in component.GetStats())
        {
            tooltipUI.centerText.text += stat.Key + ": +" + stat.Value + ", ";
        }
        tooltipUI.centerText.text = tooltipUI.centerText.text.Substring(0, tooltipUI.centerText.text.Length - 2);
        tooltipUI.bottomText.text = "" + ItemDropManager.GetDB().GetItem(itemId).cost + " gp";
    }
    public void RepairActiveWeapon()
    {
        string repairItemId = "repair_kit";
        if (activeWeapon != null)
        {
            WeaponScript weapon = activeWeapon.GetComponent<WeaponScript>();
            if (weapon.stats.currentDurability < weapon.stats.durability)
            {
                Inventory inventory = PlayerWeaponManager.instance.GetComponent<Inventory>();
                if(inventory.CheckOwnedQty(repairItemId) > 0)
                {
                    inventory.items[repairItemId].quantity--;
                    weapon.stats.currentDurability = weapon.stats.durability;
                    CloseWeaponSubmenu();
                    LoadComponentsToScreen();
                    LoadActiveWeaponStats();
                }
            }
        }
    }
}
