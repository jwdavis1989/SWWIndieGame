using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIStatBar : MonoBehaviour
{
    private Slider slider;
    //Variable to scale bar size depending on stat (Higher Stat = Longer Bar Across Screen)
    //Secondary bar behind main bar for polish effect (Yellow bar that shows how much an action/damage takes away from current stat)

    protected virtual void Awake() {
        slider = GetComponent<Slider>();
    }

    public virtual void SetStat(int newValue) {
        slider.value = newValue;
    }

    public virtual void SetMaxStat(int maxValue) {
        slider.maxValue = maxValue;
        slider.value = maxValue;
    }

}
