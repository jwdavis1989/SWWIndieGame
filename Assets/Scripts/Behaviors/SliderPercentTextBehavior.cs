using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderPercentTextBehavior : MonoBehaviour
{
    public TextMeshProUGUI percentText;
    Slider self;
    public void OnEnable()
    {
        self = GetComponent<Slider>();
    }
    public void OnSliderUpdatePercent(float value)
    {
        if(self == null) self = GetComponent<Slider>();
        // note: would need to check self.minValue if it were not 0
        int percent = (int)(100 * self.value / self.maxValue);
        percentText.text = percent + "%";
    }
}
