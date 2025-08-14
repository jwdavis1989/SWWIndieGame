using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIHudManager : MonoBehaviour
{
    [Header("Status Bars")]
    [SerializeField] UIStatBar healthBar;
    [SerializeField] UIStatBar staminaBar;

    [Header("Quick Slots")]
    [SerializeField] Image rightWeaponQuickSlotIcon;
    [SerializeField] Image leftWeaponQuickSlotIcon;

    public void Awake() {
        //
    }

    public void SetNewHealthValue(float newValue) {
        healthBar.SetStat(newValue);
    }

    public void SetMaxHealthValue(float maxHealth) {
        healthBar.SetMaxStat(maxHealth);
    }

    public void SetNewStaminaValue(float newValue) {
        staminaBar.SetStat(newValue);
    }

    public void SetMaxStaminaValue(float maxStamina) {
        staminaBar.SetMaxStat(maxStamina);
    }

    public void RefreshHud() {
        //Reboot UI objects to force an update
        //Turn off
        healthBar.gameObject.SetActive(false);
        staminaBar.gameObject.SetActive(false);

        //Turn on
        healthBar.gameObject.SetActive(true);
        staminaBar.gameObject.SetActive(true);
    }

    //Remember to call this after a weapon has been added to your hand, or arsenal
    public void SetRightWeaponQuickSlotIcon() {
        if (PlayerWeaponManager.instance != null) {
            GameObject currentRightWeapon = PlayerWeaponManager.instance.ownedWeapons[PlayerWeaponManager.instance.indexOfEquippedWeapon];
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
            GameObject currentLeftWeapon = PlayerWeaponManager.instance.ownedSpecialWeapons[PlayerWeaponManager.instance.indexOfEquippedSpecialWeapon];
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
