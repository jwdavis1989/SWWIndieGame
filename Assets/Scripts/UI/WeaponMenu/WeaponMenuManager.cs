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

public class WeaponMenuManager : MonoBehaviour
{
    [Header("Weapon Menu\n")]
    [Header("Active Weapon Preview")]
    public GameObject statsTextPrefab;
    public GameObject primaryStatsText;
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
    [SerializeField] bool switchWeaponUp = false;
    [SerializeField] bool switchWeaponDown = false;
    [SerializeField] bool equipWeaponInput = false;
    //[SerializeField] bool breakdownWeaponStarted = false;
    [SerializeField] bool breakdownWeaponPerformed = false;
    //[SerializeField] bool breakdownWeaponCanceled = false;
    [SerializeField] bool helpInput = false;
    [SerializeField] bool focusEvolutionsInput = false;
    [SerializeField] bool focusComponentsInput = false;
    [Header("Camera Movement Input")]
    PlayerControls playerControls;
    [SerializeField] Vector2 previewCameraInput;

    [Header("Submenus")]
    public bool submenuActive = false;
    public GameObject salvageConfirmWindow;
    public GameObject salvageErrorWindow;
    [Header("Input Tooltips")]
    public List<GameObject> gamepadControlsUI;
    public List<GameObject> keyboardControlsUI;
    //[SerializeField] public Image holdToBreakdownWpnImage;
    //[SerializeField] private float holdDuration = 1.5f;//should match value in PlayerControls
    private bool isHolding;
    private float holdTime;


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
            playerControls.WeaponMenu.FocusComponentsWindow.performed += i => focusComponentsInput = true;
            playerControls.WeaponMenu.FocusEvolutionsWindow.performed += i => focusEvolutionsInput = true;
            //playerControls.PauseMenu.BreakdownWeapon.started += i => breakdownWeaponStarted = true;
            playerControls.WeaponMenu.BreakdownWeapon.performed += i => breakdownWeaponPerformed = true;
            //playerControls.PauseMenu.BreakdownWeapon.canceled += i => breakdownWeaponCanceled = true;
            playerControls.Enable();
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (eventSystem.currentSelectedGameObject == null && InputSwitchDetector.IsCurrentlyGamepad())
        { //Handle Lost gamepad Cursor
            if (helpActive){
                primaryStatsText.transform.GetChild(0).GetComponent<Button>().Select();
            }else if (componentsGrid.transform.childCount > 0){
                componentsGrid.transform.GetChild(0).GetComponentInChildren<Button>().Select();
            }
        }
        HandleWeaponPreviewInput();
        HandleSwitchWeaponInput();
        HandleEquipWeaponInput();
        HandleBreakdownWeaponInput();
        HandleHelpInput();
        HandleFocusComponentsInput();
        HandleFocusEvolutionsInput();
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
    void HandleFocusComponentsInput()
    {
        if (focusComponentsInput)
        {
            focusComponentsInput = false;
            ToggleStatTooltipNavigation(false);
            ToggleEvolutionNavigation(false);
            ToggleComponentNavigation(true);
        }
    }
    void HandleFocusEvolutionsInput()
    {
        if (focusEvolutionsInput)
        {
            focusEvolutionsInput = false;
            WeaponScript wpn = activeWeapon.GetComponent<WeaponScript>();
            List<WeaponType> evolves = WeaponsController.instance.GetAllEvolutions(wpn.stats.weaponType);
            List<WeaponType> availEvolves = WeaponsController.instance.GetAvailableEvolves(wpn);
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
    void HandleBreakdownWeaponInput()
    {
        //if (isHolding)
        //{
        //    holdTime += Time.unscaledDeltaTime;
        //    holdToBreakdownWpnImage.fillAmount = holdTime / holdDuration;
        //    //Color currentColor = holdToBreakdownWpnImage.color; // Get the current color
        //    //currentColor.a = holdTime / holdDuration;      // Modify the alpha component
        //    //holdToBreakdownWpnImage.color = currentColor;
        //    //Debug.Log("SETTING FILL AMOUNT TO " + holdToBreakdownWpnImage.fillAmount);  
        //    if (holdTime >= holdDuration)
        //    {
        //        isHolding = false;
        //        holdTime = 0f;
        //        holdToBreakdownWpnImage.fillAmount = 0f;
        //    }
        //}
        //if (breakdownWeaponStarted)
        //{
        //    breakdownWeaponStarted = false;
        //    //Debug.Log("breakdownWeaponStarted");
        //    isHolding = canBreakdownActiveWeapon;
        //    holdTime = 0f;
        //    holdToBreakdownWpnImage.fillAmount = 0f;
        //}
        if (breakdownWeaponPerformed)
        {
            breakdownWeaponPerformed = false;
            //Debug.Log("breakdownWeaponPerformed");
            //BreakDownActiveWeapon();
            if(!submenuActive)
                OpenSalvageConfirmWindow();
        }
        //if (breakdownWeaponCanceled)
        //{
        //    breakdownWeaponCanceled = false;
        //    //Debug.Log("breakdownWeaponCanceled");
        //    isHolding = false;
        //    holdTime = 0f;
        //    holdToBreakdownWpnImage.fillAmount = 0f;
        //}
    }
    public void SalvageConfirmOnClick() { BreakDownActiveWeapon(); CloseSalvageConfirmWindow(); }
    private void OpenSalvageConfirmWindow() {
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
                    LoadActiveWeaponStats();
                    //Debug.Log("cur ui " + ui.tooltip.text);
                    foreach (Transform obj in componentsGrid.transform)  // refresh tooltip
                    {
                        if (obj.gameObject == currentCursorObj.GetComponentInParent<TinkerComponentUI>().gameObject)
                        {
                            TinkerComponent refComponent = obj.GetComponent<TinkerComponentUI>().refComponent;
                            tooltipUI.headerText.text = refComponent.stats.itemName;
                            tooltipUI.centerText.text = "";
                            foreach (KeyValuePair<string, float> stat in refComponent.GetStats())
                            {
                                tooltipUI.centerText.text += stat.Key + ": +" + stat.Value + ", ";
                            }
                            tooltipUI.centerText.text = tooltipUI.centerText.text.Substring(0, tooltipUI.centerText.text.Length - 2);

                        }
                        //    obj.GetComponent<TinkerComponentUI>().tooltipHolder.SetActive(true);
                        //else
                        //    obj.GetComponent<TinkerComponentUI>().tooltipHolder.SetActive(false);
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
                TinkerComponent newComp = TinkerComponentManager.instance.BreakDownWeapon(index, isSpecial, playerWpns);
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
     * This section controls the weapon scroll bar
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
        foreach (var item in TinkerComponentManager.instance.baseComponents)
        {
            if(item.GetComponent<TinkerComponent>().stats.count > 0) count++;
        }
        count += TinkerComponentManager.instance.weaponComponents.Count;
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
    void DisplayActiveWeapon(bool useSpecialWeapon = false)
    {
        //this should make KB&M show special weapon when clicking
        if (activeWeapon != null && activeWeapon.GetComponent<WeaponScript>() != null && activeWeapon.GetComponent<WeaponScript>().isSpecialWeapon)
            useSpecialWeapon = true;

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
    void LoadWeaponEvolveButtons()
    {
        if (activeWeapon)
        {
            WeaponScript wpn = activeWeapon.GetComponent<WeaponScript>();
            //weapon evolves
            WeaponsController weaponCntrller = WeaponsController.instance;
            List<WeaponType> evolves = WeaponsController.instance.GetAllEvolutions(wpn.stats.weaponType);
            List<WeaponType> availEvolves = WeaponsController.instance.GetAvailableEvolves(wpn);
            if (evolves.Count >= 1)
            {
                wpnEvolveBtn1.SetActive(true);
                WeaponScript evolWpn = weaponCntrller.baseWeapons[(int)evolves[0]].GetComponent<WeaponScript>();
                GridElementController myBtnScrpt = wpnEvolveBtn1.GetComponent<GridElementController>();
                if (availEvolves.Contains(evolves[0]))
                {
                    myBtnScrpt.topText.text = evolWpn.stats.weaponName;
                    myBtnScrpt.mainButton.interactable = true;
                    myBtnScrpt.mainButtonForeground.GetComponent<Image>().sprite = evolWpn.spr;
                    myBtnScrpt.mainButton.onClick.RemoveAllListeners();
                    myBtnScrpt.mainButton.onClick.AddListener(() => //Evolve Weapon Button
                    {
                        weaponCntrller.EvolveWeapon(activeWeapon, evolves[0], PlayerWeaponManager.instance);
                        ReloadUpgradeMenu();
                    });
                }
                else
                {
                    myBtnScrpt.topText.text = evolWpn.stats.weaponName;
                    //TODO: I'm setting it to always show ??? instead of the name when already discovered
                    //if (!WeaponsController.instance.CheckHasObtained(evolWpn.stats.weaponType))
                    //{
                    String mysteryText = "";
                    foreach (char c in myBtnScrpt.topText.text)
                        mysteryText += c == ' ' ? ' ' : '?';
                    myBtnScrpt.topText.text = mysteryText;
                    //}
                    myBtnScrpt.mainButton.interactable = false;
                    myBtnScrpt.mainButtonForeground.GetComponent<Image>().sprite = defaultUnkownIcon;
                    //myBtnScrpt.mainButtonForeground.GetComponent<RawImage>().texture = defaultUnkownIcon.texture;
                }
            }
            else if (wpnEvolveBtn1 != null)
                wpnEvolveBtn1.SetActive(false);
            if (evolves.Count >= 2)
            {//2nd weapon evolve
                wpnEvolveBtn2.SetActive(true);
                WeaponScript evolWpn = weaponCntrller.baseWeapons[(int)evolves[1]].GetComponent<WeaponScript>();
                GridElementController myBtnScrpt2 = wpnEvolveBtn2.GetComponent<GridElementController>();
                if (availEvolves.Contains(evolves[1]))
                {
                    myBtnScrpt2.topText.text = evolWpn.stats.weaponName;
                    //TODO: I'm setting it to always show ??? instead of the name when already discovered
                    //if (!WeaponsController.instance.CheckHasObtained(evolWpn.stats.weaponType))
                    //{
                    String mysteryText = "";
                    foreach (char c in myBtnScrpt2.topText.text)
                        mysteryText += c == ' ' ? ' ' : '?';
                    myBtnScrpt2.topText.text = mysteryText;
                    //}
                    myBtnScrpt2.mainButton.interactable = true;
                    myBtnScrpt2.mainButtonForeground.GetComponent<Image>().sprite = evolWpn.spr;
                    myBtnScrpt2.mainButton.onClick.RemoveAllListeners();
                    myBtnScrpt2.mainButton.onClick.AddListener(() => //Evolve 2 Weapon button
                    {
                        weaponCntrller.EvolveWeapon(activeWeapon, evolves[1], PlayerWeaponManager.instance);
                        ReloadUpgradeMenu();
                    });
                }
                else
                {
                    myBtnScrpt2.topText.text = WeaponsController.instance.CheckHasObtained(evolWpn.stats.weaponType) ? evolWpn.stats.weaponName : "???";
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
    private TinkerComponent activeComponent = null;//selected with gamepad or on hover
    void LoadActiveWeaponStats()
    {
        foreach (Transform child in primaryStatsText.transform)
            Destroy(child.gameObject);
        foreach (Transform child in elementalStatsText.transform)
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
        Func<KeyValuePair<string, float>, Transform, KeyValuePair<string, float>> handleStatText = (stat,trans) =>
        {
            GameObject statTextObj = Instantiate(statsTextPrefab, trans);
            bool greenTextShowing = false;
            String greenText = "";
            if(activeComponent != null)
            {
                if (activeComponent.GetStats().ContainsKey(stat.Key))
                {
                    greenTextShowing = true;
                    greenText += "<size=16> + " + activeComponent.GetStats()[stat.Key] + "</color></size>";
                }
            }
            statTextObj.GetComponent<TextMeshProUGUI>().text = stat.Key + ": " + (greenTextShowing? "<color=\"green\">" : "") + stat.Value + greenText;
            Button tooltipNavButton = statTextObj.GetComponentInChildren<Button>();
            if (tooltipNavButton != null)
            { // This handles the helper tooltips for the stats. It is not currently functioning
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
        };
        Dictionary<string, float> primaryStats = wpn.GetPrimaryStats();
        foreach (KeyValuePair<string, float> stat in primaryStats) handleStatText(stat, primaryStatsText.transform);
        Dictionary<string, float> elementalStats = wpn.GetElementalStats();
        foreach (KeyValuePair<string, float> stat in elementalStats) handleStatText(stat, elementalStatsText.transform);
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
                DisplayActiveWeapon(true);
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
        foreach (GameObject component in TinkerComponentManager.instance.baseComponents)
        {
            if (component == null) continue;
            TinkerComponent componentScript = component.GetComponent<TinkerComponent>();
            if (componentScript.stats.count > 0)
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
                tinkerComponentUI.index = index;
                tinkerComponentUI.refComponent = componentScript;
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
                tinkerComponentUI.countText.text = "" + componentScript.stats.count;
                //tinkerComponent.cornerButton.gameObject.SetActive(false);
                if(componentScript.spr)//Icon
                    tinkerComponentUI.foregroundIcon.GetComponent<Image>().sprite = componentScript.spr;
                //if (TinkerComponentManager.instance.CanUseComponent(PlayerWeaponManager.instance.GetEquippedWeapon(), component))
                if (displayedCount == currentlySelectedComponentIndex)
                {
                    tinkerComponentUI.mainButton.Select();
                    Debug.Log("currentlySelectedComponentIndex:" + currentlySelectedComponentIndex + " displayedCount=" + displayedCount);
                    //componentButtonSelected = true;
                }
                if (TinkerComponentManager.instance.CanUseComponent(activeWeapon, component))
                {
                    /**   ADD EVENT TO COMPONENT CLICK   */
                    tinkerComponentUI.mainButton.onClick.AddListener(() =>
                    {
                        //if (PlayerWeaponManager.instance.GetEquippedWeapon() != null && TinkerComponentManager.instance.UseComponent(PlayerWeaponManager.instance.GetEquippedWeapon(), component))
                        if (activeWeapon != null && TinkerComponentManager.instance.UseComponent(activeWeapon, component))
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
                            Debug.Log("Failed to use component " + componentScript.stats.itemName);
                        }
                    });
                }
                //else // cant use component. disable the button
                //   tinkerComponentUI.mainButton.interactable = false;
            }
        }
        //weapon components
        foreach (GameObject component in TinkerComponentManager.instance.weaponComponents)
        {
            if (componentsToSkip > 0)
            {
                componentsToSkip--;
                continue;
            }
            index++;
            if (++displayedCount > maxDisplayed) break;
            TinkerComponent componentScript = component.GetComponent<TinkerComponent>();
            GameObject gridElement = Instantiate(tinkerComponentPrefab, componentsGrid.transform);
            TinkerComponentUI tinkerComponentUI = gridElement.GetComponent<TinkerComponentUI>();
            tinkerComponentUI.countText.text = ""+componentScript.stats.count;
            //if (tinkerComponentUI.tooltip != null)
            //    tinkerComponentUI.tooltip.text = componentScript.stats.itemName;
            //if (tinkerComponentUI.tooltipHolder != null)
            //    tinkerComponentUI.tooltipHolder.SetActive(false);
            //if (tinkerComponentUI.tooltipUI != null)
            //{
            //    TooltipUI tooltipUI = tinkerComponentUI.tooltipUI;
            //    tooltipUI.headerText.text = componentScript.stats.itemName;
            //    tooltipUI.bottomText.text = componentScript.stats.price + " gp";//gold points - placeholder name
            //    tooltipUI.centerText.text = "";
            //    foreach (KeyValuePair<string, float> stat in componentScript.GetStats())
            //    {
            //        tooltipUI.centerText.text += stat.Key + ": +" + stat.Value + "\n";
            //    }
            //    tooltipUI.centerText.text += "\nCan only combine weapons of the same hand\n";
            //    tooltipUI.gameObject.SetActive(false);
            //}
            if (componentScript.spr)
                tinkerComponentUI.foregroundIcon.GetComponent<Image>().sprite = componentScript.spr;
            if (TinkerComponentManager.instance.CanUseComponent(PlayerWeaponManager.instance.GetEquippedWeapon(), component))
            {
                if (index == currentlySelectedComponentIndex)
                {
                    tinkerComponentUI.mainButton.Select();
                    //componentButtonSelected = true;
                }
                /**   ADD EVENT TO WEAPON COMPONENT CLICK   */
                tinkerComponentUI.mainButton.onClick.AddListener(() =>
                {
                    tinkerComponentUI.mainButton.onClick.AddListener(() =>
                    {
                        //if (PlayerWeaponManager.instance.GetEquippedWeapon() != null && TinkerComponentManager.instance.UseComponent(PlayerWeaponManager.instance.GetEquippedWeapon(), component))
                        if (activeWeapon != null && TinkerComponentManager.instance.UseComponent(activeWeapon, component))
                        {
                            int newCount = tinkerComponentUI.countText.text.Trim().ParseLargeInteger() - 1;
                            if (newCount > 0)
                            {
                                tinkerComponentUI.countText.text = "" + newCount;
                            }
                            else Destroy(gridElement);
                            DisplayActiveWeapon();
                            currentlySelectedComponentIndex = index - 1;
                            if (index < 0)
                                index = 0;
                            LoadComponentsToScreen();
                        }
                        else
                        {
                            Debug.Log("Failed to use component " + componentScript.stats.itemName);
                        }
                    });
                });
            }// cant use component. disable the button
            //else tinkerComponentUI.mainButton.interactable = false;
        }
        int count = 0;//count total unique components owned
        foreach (var item in TinkerComponentManager.instance.baseComponents)
        {
            if (item.GetComponent<TinkerComponent>().stats.count > 0) count++;
        }
        count += TinkerComponentManager.instance.weaponComponents.Count;
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
            if (InputSwitchDetector.IsCurrentlyGamepad())
            {
                //Show controller UI
                foreach (GameObject gamepadeUI in gamepadControlsUI)
                    gamepadeUI.SetActive(true);
                foreach (GameObject gamepadeUI in keyboardControlsUI)
                    gamepadeUI.SetActive(false);
            }
            else //Keyboard
            {
                //Hide Controller UI
                foreach (GameObject gamepadeUI in gamepadControlsUI)
                    gamepadeUI.SetActive(false);
                foreach (GameObject gamepadeUI in keyboardControlsUI)
                    gamepadeUI.SetActive(true);
                //enable buttons
                EnableAllNavigation();
            }
        }
    }
    private void EnableAllNavigation()
    {
        ToggleComponentNavigation(true);
        ToggleEvolutionNavigation(true);
        ToggleStatTooltipNavigation(true);
    }
    private void ToggleComponentNavigation(bool enable)
    {
        bool selected = false;
        foreach (Transform t in componentsGrid.transform)
        {
            // toggle navigation
            Button button = t.gameObject.GetComponent<TinkerComponentUI>().mainButton;
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
}
