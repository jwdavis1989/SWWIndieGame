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
    [Header("Grid object containing tinker components")]
    public GameObject componentsGrid;
    [Header("Prefab for item UI object")]
    public GameObject elementPrefab;
    public EventSystem eventSystem;

    //called when arriving at this menu
    private void OnEnable()
    {
        LoadComponentsToScreen();
        wpnScroll.value = 0;
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
        WeaponScript wpn = wpns.ownedWeapons[wpns.indexOfEquippedWeapon].GetComponent<WeaponScript>();
        wpn.stats.level++;
        wpn.stats.currentTinkerPoints += 10;
        Debug.Log("Leveled up " + wpn.stats.weaponName + " to level " + wpn.stats.level + ".");
        LoadWeaponsToScreen();//update screen
    }
    //************************** W E A P O N   S C R O L L **************************
    /**
     * This section controls the weapon scroll bar
     */
    [Header("Weapon Box Scroll")]
    public Scrollbar wpnScroll;
    public float currentStep = 0;
    public float lastStep = 0;
    public const int wpnPerRow = 2;
    public void WeaponScroll(float value)
    {
        int numOfPage = PlayerWeaponManager.instance.ownedWeapons.Count / wpnPerRow;
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
        if (curWeaponPage > PlayerWeaponManager.instance.ownedWeapons.Count / wpnPerRow)
        {// past the end go to beg
            curWeaponPage = 0;
        }
        else if (curWeaponPage < 0)
        {//past beggining go to end
            curWeaponPage = PlayerWeaponManager.instance.ownedWeapons.Count / wpnPerRow;
        }
        lastStep = currentStep;
        LoadWeaponsToScreen();
    }
    //************************** L O A D    I N V E N T O R Y **************************
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
     * Clear component list and reload it with current values
     */
    void LoadComponentsToScreen()
    {
        foreach (Transform child in componentsGrid.transform)
        {
            Destroy(child.gameObject);
        }
        //basic components
        foreach (GameObject component in TinkerComponentManager.instance.baseComponents)
        {
            if (component == null) continue;
            TinkerComponent componentScript = component.GetComponent<TinkerComponent>();
            if (componentScript.stats.count > 0)
            {
                Object gridElement = Instantiate(elementPrefab, componentsGrid.transform);
                GridElementController gridScript = gridElement.GetComponent<GridElementController>();
                gridScript.topText.text = componentScript.stats.itemName;
                gridScript.bottomText.text = "" + componentScript.stats.count;
                if (TinkerComponentManager.instance.CanUseComponent(PlayerWeaponManager.instance.GetEquippedWeapon(), component))
                {
                    /**   ADD EVENT TO COMPONENT CLICK   */
                    gridScript.button.onClick.AddListener(() =>
                    {
                        if (PlayerWeaponManager.instance.GetEquippedWeapon() != null && TinkerComponentManager.instance.UseComponent(PlayerWeaponManager.instance.GetEquippedWeapon(), component))
                        {
                            if (componentScript.stats.isWeapon)
                            {
                                Destroy(gridElement);
                            }
                            else
                            {
                                int newCount = gridScript.bottomText.text.Trim().ParseLargeInteger() - 1;
                                if (newCount > 0)
                                {
                                    gridScript.bottomText.text = "" + newCount;
                                }
                                else Destroy(gridElement);
                            }
                            LoadEquippedWeapons();
                        }
                        else
                        {
                            Debug.Log("Failed to use component " + componentScript.stats.itemName);
                        }
                    });
                }
                else gridScript.button.interactable = false;
            }
            //gridElement.GetComponent<GridElementController>().image = componentScript.;
        }
        //weapon components
        foreach (GameObject component in TinkerComponentManager.instance.weaponComponents)
        {
            TinkerComponent componentScript = component.GetComponent<TinkerComponent>();
            Object gridElement = Instantiate(elementPrefab, componentsGrid.transform);
            gridElement.GetComponent<GridElementController>().topText.text = componentScript.stats.itemName;
            gridElement.GetComponent<GridElementController>().bottomText.text = "Atk:" + componentScript.stats.attack;
        }
    }
    /**
     * Clear weapons grid and reload it with current values
     */
    void LoadWeaponsToScreen(bool setCursor = false)
    {
        PlayerWeaponManager playerWpns = PlayerWeaponManager.instance;
        int numOfPage = playerWpns.ownedWeapons.Count / wpnPerRow;
        wpnScroll.numberOfSteps = numOfPage;
        wpnScroll.size = 1.0f / numOfPage;
        int maxDisplayed = 4;
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
                //if (setCursor) //used to reset cursor when changing weapon
                //    eventSystem.SetSelectedGameObject(gridElement);
            }
            /**   ADD WEAPON CLICK EVENT   */
            gridScript.index = i;
            gridScript.button.onClick.AddListener(() =>
            {
                playerWpns.ChangeWeapon(gridScript.index);
                LoadWeaponsToScreen(true);
                LoadEquippedWeapons();
            });
        }
    }
}
