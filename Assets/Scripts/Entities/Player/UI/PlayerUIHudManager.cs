using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUIHudManager : MonoBehaviour
{
    [SerializeField] UIStatBar healthBar;
    [SerializeField] UIStatBar staminaBar;

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
}
