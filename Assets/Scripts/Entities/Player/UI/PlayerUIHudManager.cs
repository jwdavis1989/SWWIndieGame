using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUIHudManager : MonoBehaviour
{
    [SerializeField] UIStatBar staminaBar;

    public void SetNewStaminaValue(float newValue) {
        staminaBar.SetStat(newValue);
    }

    public void SetMaxStaminaValue(float maxStamina) {
        staminaBar.SetMaxStat(maxStamina);
    }
}
