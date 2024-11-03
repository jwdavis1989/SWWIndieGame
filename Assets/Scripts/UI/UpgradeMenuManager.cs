using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeMenuManager : MonoBehaviour
{
    public GameObject weaponsGrid;
    public int curWeaponPage = 0;
    public GameObject componentsGrid;
    private GridLayoutGroup gridLayoutGroup;
    public Object genericIcon;
    // Start is called before the first frame update
    void Start()
    {
        gridLayoutGroup = componentsGrid.GetComponent<GridLayoutGroup>();

    }

    // Update is called once per frame
    void Update()
    {
        //grid.AddComponent<Image>();
    }
    private void OnEnable()
    {
        LoadComponentsToScreen();
        LoadWeaponsToScreen();
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
                Object gridElement = Instantiate(genericIcon, componentsGrid.transform);
                gridElement.GetComponent<GridElementController>().topText.text = componentScript.stats.itemName;
                gridElement.GetComponent<GridElementController>().bottomText.text = "" + componentScript.stats.count;
            }
            //gridElement.GetComponent<GridElementController>().image = componentScript.;
        }
        //weapon components
        foreach (GameObject component in TinkerComponentManager.instance.weaponComponents)
        {
            TinkerComponent componentScript = component.GetComponent<TinkerComponent>();
            Object gridElement = Instantiate(genericIcon, componentsGrid.transform);
            gridElement.GetComponent<GridElementController>().topText.text = componentScript.stats.itemName;
            gridElement.GetComponent<GridElementController>().bottomText.text = "Atk:" + componentScript.stats.attack;
        }
    }
    /**
     * Clear weapons grid and reload it with current values
     */
    void LoadWeaponsToScreen()
    {
        int numOfPage = PlayerWeaponManager.instance.ownedWeapons.Count / 2;
        wpnScroll.numberOfSteps = numOfPage;
        wpnScroll.size = 1.0f / numOfPage;
        int maxDisplayed = 4;
        int displayed = 0;
        int wpnPerRow = 2;
        foreach (Transform child in weaponsGrid.transform)
        {
            Destroy(child.gameObject);
        }
        //main hand weapons
        for(int i = curWeaponPage * wpnPerRow; i < PlayerWeaponManager.instance.ownedWeapons.Count; i++)
        {
            if (displayed >= maxDisplayed) break;
            GameObject wpn = PlayerWeaponManager.instance.ownedWeapons[i];
            if (wpn == null) continue;
            displayed++;
            WeaponScript wpnScrpt = wpn.GetComponent<WeaponScript>();
            Object gridElement = Instantiate(genericIcon, weaponsGrid.transform);
            GridElementController gridScript = gridElement.GetComponent<GridElementController>();
            gridScript.topText.text = wpnScrpt.stats.weaponName;
            gridScript.bottomText.text = "Lvl " + wpnScrpt.stats.level;
            //attach button behavior (equip weapon now. Could be select weapon for upgrade, etc.) 
            gridScript.index = i;
            gridScript.button.onClick.AddListener(() =>
            {
                PlayerWeaponManager.instance.ChangeWeapon(gridScript.index);
            });
        }
    }
    /**
     * This section controls the weapon scroll bar
     */
    public Scrollbar wpnScroll;
    public float currentStep = 0;
    public float lastStep = 0;
    public void OnValueChanged(float value)
    {
        int wpnPerRow = 2;
        int numOfPage = PlayerWeaponManager.instance.ownedWeapons.Count / wpnPerRow;
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
            Object gridElement = Instantiate(genericIcon, componentsGrid.transform);
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
        Debug.Log("Leveled up " + wpn.stats.weaponName + " to level " + wpn.stats.level + ".");
        LoadWeaponsToScreen();//update screen
    }
}
