using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    [Header("Special Weapon Cooldown Animation")]
    [SerializeField] Image leftWeaponQuickSlotCooldownFillBar;
    [SerializeField] TextMeshProUGUI leftWeaponQuickSlotCooldownText;
    private float specialCooldown = 5f;
    private float specialCurrentCooldown = 0.0f;
    private bool specialIsCoolingDown = false;


    public void Awake() {
        //
    }

    public void Start()
    {
        if (leftWeaponQuickSlotCooldownFillBar != null)
        {
            leftWeaponQuickSlotCooldownFillBar.fillAmount = 0.0f;
        }
        if (leftWeaponQuickSlotCooldownText != null)
        {
            leftWeaponQuickSlotCooldownText.text = "";
        }
    }

    public void Update()
    {
        if (specialIsCoolingDown)
        {
            // Subtract time to tick downward
            specialCurrentCooldown -= Time.deltaTime;

            // Calculate the remaining fill percentage (goes from 1.0 down to 0.0)
            leftWeaponQuickSlotCooldownFillBar.fillAmount = specialCurrentCooldown / specialCooldown;

            //Update Cooldown Text
            if (specialCurrentCooldown < 1.0f)
            {
                leftWeaponQuickSlotCooldownText.text = specialCurrentCooldown.ToString("F1");
            }
            else
            {
                leftWeaponQuickSlotCooldownText.text = Mathf.CeilToInt(specialCurrentCooldown).ToString();
            }

            // Stop the cooldown when the timer hits zero
            if (specialCurrentCooldown <= 0.0f)
            {
                specialIsCoolingDown = false;
                specialCurrentCooldown = 0.0f;
                leftWeaponQuickSlotCooldownFillBar.fillAmount = 0.0f;
                leftWeaponQuickSlotCooldownText.text = "";
            }
        }
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

    public void StartSpecialCooldownAnimation(float cooldownDurationInSeconds)
    {
        specialCooldown = cooldownDurationInSeconds;
        specialCurrentCooldown = specialCooldown;
        specialIsCoolingDown = true;
        if (leftWeaponQuickSlotCooldownFillBar != null)
        {
            leftWeaponQuickSlotCooldownFillBar.fillAmount = 1.0f;
        }
        if (leftWeaponQuickSlotCooldownText != null)
        {
            leftWeaponQuickSlotCooldownText.text = Mathf.CeilToInt(specialCurrentCooldown).ToString();
        }
    }

}
