using Palmmedia.ReportGenerator.Core.Common;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UpgradeMenuManager : MonoBehaviour
{
    [Header("Equipped weapons")]
    public TextMeshProUGUI equippedWpnTxt;
    public GameObject wpnEvolveBtn1;
    public GameObject wpnEvolveBtn2;
    public GameObject specWpnEvolveBtn1;
    public GameObject specWpnEvolveBtn2;
    public Sprite defaultUnkownIcon;
    [Header("Grid containing owned weapons")]
    public GameObject weaponsGrid;
    public int curWeaponPage = 0;
    GameObject activeWeapon = null;
    [Header("Grid containing tinker components")]
    public GameObject componentsGrid;
    public int curComponentPage = 0;
    [Header("Prefab for item UI object")]
    public GameObject elementPrefab;
    [Header("Buttons")]
    public Button breakdownBtn;
    public EventSystem eventSystem;


    //called when arriving at this menu
    private void OnEnable()
    {
        wpnScroll.value = 0;
        curWeaponPage = 0;
        cmpntScroll.value = 0;
        curComponentPage = 0;
        activeWeapon = null;
        LoadComponentsToScreen();
        LoadWeaponsToScreen();
        LoadEquippedWeapons();
    }
    // Start is called before the first frame update
    void Start(){}
    // Update is called once per frame
    void Update()
    {
        if (eventSystem.currentSelectedGameObject == null)
        { //grid system become null when equipping weapon because the grid is reloaded
            eventSystem.SetSelectedGameObject(eventSystem.firstSelectedGameObject);
        }
        if (activeWeapon != null && activeWeapon.GetComponent<WeaponScript>().stats.level >= 5)
        {
            breakdownBtn.interactable = true;
        }
        else
        {
            breakdownBtn.interactable= false;
        }
    }

    //************************** B U T T O N S **************************
    /**
     * Turn active weapon into a component
     */
    public void BreakDownActiveWeapon()
    {
        try
        {
            if (activeWeapon.GetComponent<WeaponScript>().stats.level < 5)
                throw new System.Exception("Must be Level 5 or over");
            //break down weapon
            PlayerWeaponManager pWpns = PlayerWeaponManager.instance;
            bool isSpecial = activeWeapon.GetComponent<WeaponScript>().isSpecialWeapon;
            int index = isSpecial ? pWpns.ownedSpecialWeapons.IndexOf(activeWeapon) : pWpns.ownedWeapons.IndexOf(activeWeapon);
            TinkerComponent newComp = TinkerComponentManager.instance.BreakDownWeapon(index, isSpecial, pWpns);
            Debug.Log("Broke down " + newComp.stats.itemName);
            //add to screen
            activeWeapon = null;
            ReloadUpgradeMenu();
        }
        catch (System.Exception e) //Catches not lvl 5 or over error / no active weapon
        {
            Debug.Log(e.Message);
        }
    }
    /**
     * Levels up equipped weapon
     */
    public void DebugLvlUpEquippedWeapon()
    {
        PlayerWeaponManager wpns = PlayerWeaponManager.instance;
        WeaponScript wpn = activeWeapon != null? activeWeapon.GetComponent<WeaponScript>() :
            wpns.ownedWeapons[wpns.indexOfEquippedWeapon].GetComponent<WeaponScript>();
        wpn.stats.level++;
        wpn.stats.currentTinkerPoints += wpn.stats.tinkerPointsPerLvl;
        Debug.Log("Leveled up " + wpn.stats.weaponName + " to level " + wpn.stats.level + ".");
        LoadEquippedWeapons();//update screen
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
    public const int wpnPerRow = 3;
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
    public const int cmpntPerRow = 6;
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
    void LoadEquippedWeapons()
    {
        string text = "";
        GameObject equippedWpn = PlayerWeaponManager.instance.GetEquippedWeapon();
        if (equippedWpn)
        {
            WeaponScript wpn = equippedWpn.GetComponent<WeaponScript>();
            WeaponStats stats = wpn.stats;
            ElementalStats el = stats.elemental;
            text = "Equipped - " + stats.weaponName + "   TP " + stats.currentTinkerPoints + " Lvl " + stats.level +
            "\n  Attack " + stats.attack + " Durability " + stats.durability +
            "\n  Block " + stats.block + " Stability " + stats.stability +
            "\n  Fire " + el.firePower + ", Ice " + el.icePower + ", Lightning " + el.lightningPower +
            "\n  Wind " + el.windPower + ", Earth " + el.earthPower + ", Light " + el.lightPower +
            "\n  Beast " + el.beastPower + ", Scale " + el.scalesPower + ", Tech " + el.techPower;
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
                    myBtnScrpt.bottomText.text = "Evolve!";
                    myBtnScrpt.mainButtonForeground.GetComponent<Image>().sprite = evolWpn.spr;
                    myBtnScrpt.mainButton.onClick.AddListener(() => //Evolve Weapon Button
                    {
                        weaponCntrller.EvolveWeapon(equippedWpn, evolves[0], PlayerWeaponManager.instance);
                        ReloadUpgradeMenu();
                    });
                }
                else
                {
                    myBtnScrpt.topText.text = "???"; // TODO isObtained? weaponName : "???";
                    myBtnScrpt.mainButton.interactable = false;
                    myBtnScrpt.bottomText.text = "";
                    myBtnScrpt.mainButtonForeground.GetComponent<Image>().sprite = defaultUnkownIcon;
                }
            }
            else wpnEvolveBtn1.SetActive(false);
            if (evolves.Count >= 2)
            {//2nd weapon evolve
                wpnEvolveBtn2.SetActive(true);
                WeaponScript evolWpn = weaponCntrller.baseWeapons[(int)evolves[1]].GetComponent<WeaponScript>();
                GridElementController myBtnScrpt2 = wpnEvolveBtn2.GetComponent<GridElementController>();
                if (availEvolves.Contains(evolves[1]))
                {
                    myBtnScrpt2.topText.text = evolWpn.stats.weaponName;
                    myBtnScrpt2.mainButton.interactable = true;
                    myBtnScrpt2.bottomText.text = "Evolve!";
                    myBtnScrpt2.mainButtonForeground.GetComponent<Image>().sprite = evolWpn.spr;
                    myBtnScrpt2.mainButton.onClick.AddListener(() => //Evolve 2 Weapon button
                    {
                        weaponCntrller.EvolveWeapon(equippedWpn, evolves[1], PlayerWeaponManager.instance);
                        ReloadUpgradeMenu();
                    });
                }
                else
                {
                    myBtnScrpt2.topText.text = "???";
                    myBtnScrpt2.mainButton.interactable = false;
                    myBtnScrpt2.bottomText.text = "";
                    myBtnScrpt2.mainButtonForeground.GetComponent<Image>().sprite = defaultUnkownIcon;
                }
            }
            else wpnEvolveBtn2.SetActive(false);
        }
        else
        {
            text = "Equipped - None\n\n\n\n";
            wpnEvolveBtn1.SetActive(false);
            wpnEvolveBtn2.SetActive(false);
        }
        GameObject equippedSpecialWpn = PlayerWeaponManager.instance.GetEquippedWeapon(true);
        if (equippedSpecialWpn)//special weapon stats
        {
            WeaponScript specWpn = equippedSpecialWpn.GetComponent<WeaponScript>();
            WeaponStats specStats = specWpn.stats;
            ElementalStats sEl = specStats.elemental;
            text += "\n\n\nSpecial - " + specStats.weaponName + "   TP " + specStats.currentTinkerPoints + " Lvl " + specStats.level +
            "\n  Attack " + specStats.attack +
            "\n  Fire " + sEl.firePower + ", Ice " + sEl.icePower + ", Lightning " + sEl.lightningPower +
            "\n  Wind " + sEl.windPower + ", Earth " + sEl.earthPower + ", Light " + sEl.lightPower +
            "\n  Beast " + sEl.beastPower + ", Scale " + sEl.scalesPower + ", Tech " + sEl.techPower;
            //secial weapon evolves
            WeaponsController weaponCntrller = WeaponsController.instance;
            List <WeaponType> evolves = weaponCntrller.GetAllEvolutions(specWpn.stats.weaponType);
            List<WeaponType> availEvolves = WeaponsController.instance.GetAvailableEvolves(specWpn);
            if (evolves.Count >= 1)
            {
                specWpnEvolveBtn1.SetActive(true);
                WeaponScript evolWpn = weaponCntrller.baseWeapons[(int)evolves[0]].GetComponent<WeaponScript>();
                GridElementController myBtnScrpt3 = specWpnEvolveBtn1.GetComponent<GridElementController>();
                if (availEvolves.Contains(evolves[0]))
                {
                    myBtnScrpt3.topText.text = evolWpn.stats.weaponName;
                    if (evolWpn.spr)
                        myBtnScrpt3.mainButtonForeground.GetComponent<Image>().sprite = evolWpn.spr;
                    myBtnScrpt3.mainButton.interactable = true;
                    myBtnScrpt3.bottomText.text = "Evolve!";
                    myBtnScrpt3.mainButton.onClick.AddListener(() => //evolve special wpn btn
                    {
                        weaponCntrller.EvolveWeapon(equippedSpecialWpn, evolves[0], PlayerWeaponManager.instance);
                        ReloadUpgradeMenu();
                    });
                }
                else
                {
                    myBtnScrpt3.topText.text = "???";
                    myBtnScrpt3.mainButton.interactable = false;
                    myBtnScrpt3.bottomText.text = "";
                    myBtnScrpt3.mainButtonForeground.GetComponent<Image>().sprite = defaultUnkownIcon;
                }
            }
            else specWpnEvolveBtn1.SetActive(false);
            if (evolves.Count >= 2)
            {//2nd special wpn evolve
                specWpnEvolveBtn2.SetActive(true);
                WeaponScript evolWpn = weaponCntrller.baseWeapons[(int)evolves[1]].GetComponent<WeaponScript>();
                GridElementController myBtnScrpt4 = specWpnEvolveBtn2.GetComponent<GridElementController>();
                if (availEvolves.Contains(evolves[1]))
                {
                    myBtnScrpt4.topText.text = evolWpn.stats.weaponName;
                    myBtnScrpt4.mainButton.interactable = true;
                    myBtnScrpt4.bottomText.text = "Evolve!";
                    if (evolWpn.spr)
                        myBtnScrpt4.mainButtonForeground.GetComponent<Image>().sprite = evolWpn.spr;
                    myBtnScrpt4.mainButton.onClick.AddListener(() => //evolve special wpn btn 2
                    {
                        weaponCntrller.EvolveWeapon(equippedSpecialWpn, evolves[1], PlayerWeaponManager.instance);
                        ReloadUpgradeMenu();
                    });
                }
                else
                {
                    myBtnScrpt4.topText.text = "???";
                    myBtnScrpt4.mainButton.interactable = false;
                    myBtnScrpt4.bottomText.text = "Evolve?";
                    myBtnScrpt4.mainButtonForeground.GetComponent<Image>().sprite = defaultUnkownIcon;
                }
            }
            else specWpnEvolveBtn2.SetActive(false);
        }
        else
        {
            text += "Special - None";
            specWpnEvolveBtn1.SetActive(false);
            specWpnEvolveBtn2.SetActive(false);
        }
        equippedWpnTxt.text = text;
    }
    /**
     * Clear weapons grid and reload it with current values
     */
    void LoadWeaponsToScreen(bool setCursor = false)
    {
        WeaponScroll(0);
        PlayerWeaponManager playerWpns = PlayerWeaponManager.instance;
        int numOfPage = (playerWpns.ownedWeapons.Count + playerWpns.ownedSpecialWeapons.Count) / wpnPerRow;
        wpnScroll.numberOfSteps = numOfPage;
        wpnScroll.size = 1.0f / numOfPage;
        int maxDisplayed = 6;
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
            GameObject gridElement = Instantiate(elementPrefab, weaponsGrid.transform);
            GridElementController gridScript = gridElement.GetComponent<GridElementController>();
            gridScript.topText.text = wpnScrpt.stats.weaponName;
            gridScript.bottomText.text = "Lvl " + wpnScrpt.stats.level;
            gridScript.cornerButton.gameObject.SetActive(true);
            if (wpnScrpt.spr)//load icon
                gridScript.mainButtonForeground.GetComponent<Image>().sprite = wpnScrpt.spr;
            if (i == playerWpns.indexOfEquippedWeapon)
            {//mark equipped weapon
                gridScript.mainButton.GetComponent<Image>().color = Color.green;
                gridScript.cornerButton.gameObject.SetActive(false);
            }
            else 
                gridScript.cornerButton.gameObject.SetActive(true);
            if (wpn == activeWeapon)
            {//mark actively editing weapon
                gridScript.mainButton.GetComponent<Image>().color = Color.red;
            }
            /**   ADD UNSPECIAL WEAPON CLICK EVENTS   */
            gridScript.index = i;
            gridScript.mainButton.onClick.AddListener(() =>
            {
                activeWeapon = wpn;//set actively editing
                LoadWeaponsToScreen(true);
                LoadEquippedWeapons();
                LoadComponentsToScreen();
            });
            gridScript.cornerButton.onClick.AddListener(() =>
            {
                playerWpns.ChangeWeapon(gridScript.index);//equip weapon
                LoadWeaponsToScreen(true);
                LoadEquippedWeapons();
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
            if (++displayed > maxDisplayed) break;
            WeaponScript wpnScrpt = weapon.GetComponent<WeaponScript>();
            GameObject gridElement = Instantiate(elementPrefab, weaponsGrid.transform);
            GridElementController gridScript = gridElement.GetComponent<GridElementController>();
            gridScript.topText.text = wpnScrpt.stats.weaponName;
            gridScript.bottomText.text = "Lvl " + wpnScrpt.stats.level;
            gridScript.cornerButton.gameObject.SetActive(true);
            if (wpnScrpt.spr)//load icon
                gridScript.mainButtonForeground.GetComponent<Image>().sprite = wpnScrpt.spr;
            if (index == playerWpns.indexOfEquippedSpecialWeapon)
            {//mark equipped weapon
                gridScript.mainButton.GetComponent<Image>().color = Color.green;
                gridScript.cornerButton.gameObject.SetActive(false);
            }
            else 
                gridScript.cornerButton.gameObject.SetActive(true);
            if (weapon == activeWeapon)
            {//mark actively editing weapon
                gridScript.mainButton.GetComponent<Image>().color = Color.red;
            }
            gridScript.index = index;
            /**   ADD SPECIAL WEAPON CLICK EVENTS   */
            gridScript.mainButton.onClick.AddListener(() =>
            {
                activeWeapon = weapon;//set actively editing
                LoadWeaponsToScreen(true);
                LoadEquippedWeapons();
                LoadComponentsToScreen();
            });
            gridScript.cornerButton.onClick.AddListener(() =>
            {
                playerWpns.ChangeSpecialWeapon(gridScript.index);//equip weapon
                LoadWeaponsToScreen(true);
                LoadEquippedWeapons();
                LoadComponentsToScreen();
            });
            index++;
        }
    }
    /**
     * Clear component list and reload it with current values
     */
    void LoadComponentsToScreen()
    {
        foreach (Transform child in componentsGrid.transform)
        {
            Destroy(child.gameObject);
        }
        int displayedCount = 0;
        int maxDisplayed = 12;
        int componentsToSkip = curComponentPage * cmpntPerRow;
        //basic components
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
                if (++displayedCount > maxDisplayed) break;
                Object gridElement = Instantiate(elementPrefab, componentsGrid.transform);
                GridElementController gridScript = gridElement.GetComponent<GridElementController>();
                gridScript.topText.text = componentScript.stats.itemName;
                gridScript.bottomText.text = "" + componentScript.stats.count;
                gridScript.cornerButton.gameObject.SetActive(false);
                if(componentScript.spr)
                    gridScript.mainButtonForeground.GetComponent<Image>().sprite = componentScript.spr;
                //Only affecting equipped non-special weapon here //activeWeapon
                //if (TinkerComponentManager.instance.CanUseComponent(PlayerWeaponManager.instance.GetEquippedWeapon(), component))
                if (TinkerComponentManager.instance.CanUseComponent(activeWeapon, component))
                {
                    /**   ADD EVENT TO COMPONENT CLICK   */
                    gridScript.mainButton.onClick.AddListener(() =>
                    {
                        //if (PlayerWeaponManager.instance.GetEquippedWeapon() != null && TinkerComponentManager.instance.UseComponent(PlayerWeaponManager.instance.GetEquippedWeapon(), component))
                        if (activeWeapon != null && TinkerComponentManager.instance.UseComponent(activeWeapon, component))
                        {
                            int newCount = gridScript.bottomText.text.Trim().ParseLargeInteger() - 1;
                            if (newCount > 0)
                            {
                                gridScript.bottomText.text = "" + newCount;
                            }
                            else Destroy(gridElement);
                            LoadEquippedWeapons();
                            LoadComponentsToScreen();
                        }
                        else
                        {
                            Debug.Log("Failed to use component " + componentScript.stats.itemName);
                        }
                    });
                }// cant use component. disable the button
                else gridScript.mainButton.interactable = false;
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
            if (++displayedCount > maxDisplayed) break;
            TinkerComponent componentScript = component.GetComponent<TinkerComponent>();
            Object gridElement = Instantiate(elementPrefab, componentsGrid.transform);
            GridElementController gridScript = gridElement.GetComponent<GridElementController>();
            gridScript.topText.text = componentScript.stats.itemName;
            gridScript.bottomText.text = "Atk:" + componentScript.stats.attack;
            gridScript.cornerButton.gameObject.SetActive(false);
            if (componentScript.spr)
                gridScript.mainButtonForeground.GetComponent<Image>().sprite = componentScript.spr;
            if (TinkerComponentManager.instance.CanUseComponent(PlayerWeaponManager.instance.GetEquippedWeapon(), component))
            {
                /**   ADD EVENT TO WEAPON COMPONENT CLICK   */
                gridScript.mainButton.onClick.AddListener(() =>
                {
                    if (PlayerWeaponManager.instance.GetEquippedWeapon() != null && TinkerComponentManager.instance.UseComponent(PlayerWeaponManager.instance.GetEquippedWeapon(), component))
                    {
                        LoadEquippedWeapons();
                        Destroy(gridElement);
                    }
                    else
                    {
                        Debug.Log("Failed to use component " + componentScript.stats.itemName);
                    }
                });
            }// cant use component. disable the button
            else gridScript.mainButton.interactable = false;
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
        LoadEquippedWeapons();
        LoadComponentsToScreen();
    }
}
