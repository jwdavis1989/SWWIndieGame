using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIHudManager : MonoBehaviour
{
    [Header("Status Bars")]
    [SerializeField] UIRadialStatBar healthBar;
    [SerializeField] UIRadialStatBar staminaBar;
    [SerializeField] UIRadialStatBar fuelBar;

    [Header("Quick Slots")]
    [SerializeField] Image rightWeaponQuickSlotIcon;
    [SerializeField] Image leftWeaponQuickSlotIcon;

    public void Awake() {
        //
    }

    public void UpdateHealthBar(float currentValue, float maxValue)
    {
        if(healthBar != null) healthBar.UpdateStatBar(currentValue, maxValue);
    }

    public void UpdateStaminaBar(float currentValue, float maxValue)
    {
        if(healthBar != null) staminaBar.UpdateStatBar(currentValue, maxValue);
    }

    public void UpdateFuelBar(float currentValue, float maxValue)
    {
        if(healthBar != null) fuelBar.UpdateStatBar(currentValue, maxValue);
    }

    public void RefreshHud() {
        //Reboot UI objects to force an update
        //Turn off
        healthBar.gameObject.SetActive(false);
        staminaBar.gameObject.SetActive(false);
        fuelBar.gameObject.SetActive(false);

        //Turn on
        healthBar.gameObject.SetActive(true);
        staminaBar.gameObject.SetActive(true);
        fuelBar.gameObject.SetActive(true);
    }

    //Remember to call this after a weapon has been added to your hand, or arsenal
    public void SetRightWeaponQuickSlotIcon() {
        if (PlayerWeaponManager.instance != null) {
            GameObject currentRightWeapon = PlayerWeaponManager.instance.GetMainHand().gameObject;
            if (currentRightWeapon == null) {
                rightWeaponQuickSlotIcon.enabled = false;
                rightWeaponQuickSlotIcon.sprite = null;
                Debug.Log("No Right Hand Weapon Currently Equipped.");
                return;
            }

            if (currentRightWeapon.GetComponent<WeaponScript>().spr == null) {
                Debug.Log("ERROR: Item has no Item Icon!");
                return;
            }

            rightWeaponQuickSlotIcon.enabled = true;
            rightWeaponQuickSlotIcon.sprite = currentRightWeapon.GetComponent<WeaponScript>().spr;
        }
        else {
            Debug.Log("ERROR: PlayerWeaponManager.instance does not exist!");
            return;
        }

    }

    //Remember to call this after a weapon has been added to your hand, or arsenal
    public void SetLeftWeaponQuickSlotIcon()
    {
        if (PlayerWeaponManager.instance != null)
        {
            GameObject currentLeftWeapon = PlayerWeaponManager.instance.GetOffHand().gameObject;
            if (currentLeftWeapon == null)
            {
                leftWeaponQuickSlotIcon.enabled = false;
                leftWeaponQuickSlotIcon.sprite = null;
                Debug.Log("No Left Hand Weapon Currently Equipped.");
                return;
            }

            if (currentLeftWeapon.GetComponent<WeaponScript>().spr == null)
            {
                Debug.Log("ERROR: Item has no Item Icon!");
                return;
            }

            leftWeaponQuickSlotIcon.enabled = true;
            leftWeaponQuickSlotIcon.sprite = currentLeftWeapon.GetComponent<WeaponScript>().spr;
        }
        else
        {
            Debug.Log("ERROR: PlayerWeaponManager.instance does not exist!");
            return;
        }

    }

}
