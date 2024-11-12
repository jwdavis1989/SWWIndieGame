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
    public TextMeshProUGUI equippedWpnTxt;
    [Header("Grid object containing weapons")]
    public GameObject weaponsGrid;
    public int curWeaponPage = 0;
    GameObject activeWeapon = null;
    [Header("Grid object containing tinker components")]
    public GameObject componentsGrid;
    public int curComponentPage = 0;
    [Header("Prefab for item UI object")]
    public GameObject elementPrefab;
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
    }

    //************************** B U T T O N S **************************
    /**
     * Turn equipped weapon into a component
     */
    public void BreakDownEquippedWeapon()
    {
        try
        {
            //break down weapon
            TinkerComponent newComp = TinkerComponentManager.instance.BreakDownWeapon(PlayerWeaponManager.instance.indexOfEquippedWeapon, false, PlayerWeaponManager.instance);
            //add to screen
            Object gridElement = Instantiate(elementPrefab, componentsGrid.transform);
            gridElement.GetComponent<GridElementController>().topText.text = newComp.stats.itemName;
            gridElement.GetComponent<GridElementController>().bottomText.text = "Atk:" + newComp.stats.attack;
            Debug.Log("Broke down " + newComp.stats.itemName);
        }
        catch (System.Exception e) //Catches not lvl 5 or over error
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
        }
        wpnScroll.numberOfSteps = numOfPage;
        wpnScroll.size = 1.0f / numOfPage;
        currentStep = Mathf.Round(wpnScroll.value * numOfPage);
        if (currentStep == lastStep) return;
        if (currentStep > lastStep)
        {
            curWeaponPage++;
        }
        else
        {
            curWeaponPage--;
        }
        if (curWeaponPage > weaponsCount / wpnPerRow)
        {// past the end go to beg
            curWeaponPage = 0;
        }
        else if (curWeaponPage < 0)
        {//past beggining go to end
            curWeaponPage = weaponsCount / wpnPerRow;
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
        int count = 0;
        foreach (var item in TinkerComponentManager.instance.baseComponents)
        {
            if(item.GetComponent<TinkerComponent>().stats.count > 0) count++;
        }
        count += TinkerComponentManager.instance.weaponComponents.Count;
        int numOfPage = count / cmpntPerRow;
        //if (numOfPage < 2)
        //{
        //    cmpntScroll.gameObject.SetActive(false);
        //}
        //else
        //{
        //    cmpntScroll.gameObject.SetActive(true);
        //}
        cmpntScroll.numberOfSteps = numOfPage;
        cmpntScroll.size = 1.0f / numOfPage;
        cmpntCurrentStep = Mathf.Round(cmpntScroll.value * numOfPage);
        if (cmpntCurrentStep == cmpntLastStep) return;
        if (cmpntCurrentStep > cmpntLastStep)
        {
            curComponentPage++;
        }
        else
        {
            curComponentPage--;
        }
        if (curComponentPage > count / cmpntPerRow)
        {// past the end go to beg
            curComponentPage = 0;
        }
        else if (curComponentPage < 0)
        {//past beggining go to end
            curComponentPage = count / cmpntPerRow;
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
        if (PlayerWeaponManager.instance.GetEquippedWeapon())
        {
            WeaponScript wpn = PlayerWeaponManager.instance.GetEquippedWeapon().GetComponent<WeaponScript>();
            WeaponStats stats = wpn.stats;
            ElementalStats el = stats.elemental;
            text = "Equipped - " + stats.weaponName + "   TP " + stats.currentTinkerPoints + " Durability " + stats.durability +
            "\r\n  Stats: Attack " + stats.attack + " Block " + stats.block + " Stability " + stats.stability +
            "\r\n  Elemental: Fire " + el.firePower + ", Ice " + el.icePower + ", Lightning " + el.lightningPower +
            "\r\n  Wind " + el.windPower + ", Earth " + el.earthPower + ", Light " + el.lightPower + ", Beast " + el.beastPower +
            "\r\n  Scale " + el.scalesPower + ", Tech " + el.techPower;
        }
        else text = "Equipped - None\n\n\n\n";
        if (PlayerWeaponManager.instance.GetEquippedWeapon(true))
        {
            WeaponScript specWpn = PlayerWeaponManager.instance.GetEquippedWeapon(true).GetComponent<WeaponScript>();
            WeaponStats specStats = specWpn.stats;
            ElementalStats sEl = specStats.elemental;
            text += "\r\n\r\nSpecial - " + specStats.weaponName + "   TP " + specStats.currentTinkerPoints + " Durability " + specStats.durability +
            "\r\n  Stats: Attack " + specStats.attack + " Block " + specStats.block + " Stability " + specStats.stability +
            "\r\n  Elemental: Fire " + sEl.firePower + ", Ice " + sEl.icePower + ", Lightning " + sEl.lightningPower +
            "\r\n  Wind " + sEl.windPower + ", Earth " + sEl.earthPower + ", Light " + sEl.lightPower + ", Beast " + sEl.beastPower +
            "\r\n  Scale " + sEl.scalesPower + ", Tech " + sEl.techPower;
        }
        else text += "Special - None";
        equippedWpnTxt.text = text;
    }
    /**
     * Clear weapons grid and reload it with current values
     */
    void LoadWeaponsToScreen(bool setCursor = false)
    {
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
        for (int i = curWeaponPage * wpnPerRow; i < playerWpns.ownedWeapons.Count; i++)
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
            //mark equipped weapon
            if (i == playerWpns.indexOfEquippedWeapon)
            {
                gridScript.button.GetComponent<Image>().color = Color.green;
            }
            /**   ADD WEAPON CLICK EVENT   */
            gridScript.index = i;
            gridScript.button.onClick.AddListener(() =>
            {
                activeWeapon = wpn;
                playerWpns.ChangeWeapon(gridScript.index);
                LoadWeaponsToScreen(true);
                LoadEquippedWeapons();
                LoadComponentsToScreen();
            });
        }
        int wpnsToSkip = curWeaponPage * wpnPerRow;
        int index = 0;
        foreach (GameObject weapon in playerWpns.ownedSpecialWeapons)
        {
            if (weapon == null) continue;
            if (++displayed > maxDisplayed) break;
            WeaponScript wpnScrpt = weapon.GetComponent<WeaponScript>();
            GameObject gridElement = Instantiate(elementPrefab, weaponsGrid.transform);
            GridElementController gridScript = gridElement.GetComponent<GridElementController>();
            gridScript.topText.text = wpnScrpt.stats.weaponName;
            gridScript.bottomText.text = "Lvl " + wpnScrpt.stats.level;
            if (index == playerWpns.indexOfEquippedSpecialWeapon)
            {
                gridScript.button.GetComponent<Image>().color = Color.green;
            }
            gridScript.index = index;
            gridScript.button.onClick.AddListener(() =>
            {
                activeWeapon = weapon;
                playerWpns.ChangeSpecialWeapon(gridScript.index);
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
        int iconCount = 0;
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
                if (++iconCount > maxDisplayed) break;
                Object gridElement = Instantiate(elementPrefab, componentsGrid.transform);
                GridElementController gridScript = gridElement.GetComponent<GridElementController>();
                gridScript.topText.text = componentScript.stats.itemName;
                gridScript.bottomText.text = "" + componentScript.stats.count;
                //Only affecting equipped non-special weapon here //activeWeapon
                //if (TinkerComponentManager.instance.CanUseComponent(PlayerWeaponManager.instance.GetEquippedWeapon(), component))
                if (TinkerComponentManager.instance.CanUseComponent(activeWeapon, component))
                {
                    /**   ADD EVENT TO COMPONENT CLICK   */
                    gridScript.button.onClick.AddListener(() =>
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
                        }
                        else
                        {
                            Debug.Log("Failed to use component " + componentScript.stats.itemName);
                        }
                    });
                }// cant use component. disable the button
                else gridScript.button.interactable = false;
            }
            //gridElement.GetComponent<GridElementController>().image = componentScript.;
        }
        //weapon components
        foreach (GameObject component in TinkerComponentManager.instance.weaponComponents)
        {
            if (componentsToSkip > 0)
            {
                componentsToSkip--;
                continue;
            }
            if (++iconCount > maxDisplayed) break;
            TinkerComponent componentScript = component.GetComponent<TinkerComponent>();
            Object gridElement = Instantiate(elementPrefab, componentsGrid.transform);
            GridElementController gridScript = gridElement.GetComponent<GridElementController>();
            gridScript.topText.text = componentScript.stats.itemName;
            gridScript.bottomText.text = "Atk:" + componentScript.stats.attack;
            if (TinkerComponentManager.instance.CanUseComponent(PlayerWeaponManager.instance.GetEquippedWeapon(), component))
            {
                /**   ADD EVENT TO WEAPON COMPONENT CLICK   */
                gridScript.button.onClick.AddListener(() =>
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
            else gridScript.button.interactable = false;
        }
        int numOfPage = iconCount / cmpntPerRow;
        cmpntScroll.numberOfSteps = numOfPage;
        cmpntScroll.size = 1.0f / numOfPage;
    }
}
