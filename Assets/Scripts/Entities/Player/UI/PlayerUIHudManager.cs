using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUIHudManager : MonoBehaviour
{
    [SerializeField] UIStatBar staminaBar;

    public void SetNewStaminaValue(int newValue, int oldValue = 0) {
        staminaBar.SetStat(newValue);
    }

    public void SetMaxStaminaValue(int maxStamina) {
        staminaBar.SetMaxStat(maxStamina);
    }
}