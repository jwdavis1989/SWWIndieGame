using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIStatBar : MonoBehaviour
{
    protected Slider slider;
    protected RectTransform rectTransform;

    //Variable to scale bar size depending on stat (Higher Stat = Longer Bar Across Screen)
    [Header("Bar Options")]
    [SerializeField] protected bool scaleBarLengthWithStats = true;
    [SerializeField] protected float widthScaleMultiplier = 1;

    //Secondary bar behind main bar for polish effect (Yellow bar that shows how much an action/damage takes away from current stat)

    protected virtual void Awake() {
        slider = GetComponent<Slider>();
        rectTransform = GetComponent<RectTransform>();
    }

    protected virtual void Start()
    {
        //Stub
    }

    public virtual void SetStat(float newValue)
    {
        slider.value = newValue;
    }

    public virtual void SetMaxStat(float maxValue) {
        slider.maxValue = maxValue;
        slider.value = maxValue;

        if (scaleBarLengthWithStats) {
            //Scale the transform of this object's width based on max stat
            rectTransform.sizeDelta = new Vector2(maxValue * widthScaleMultiplier, rectTransform.sizeDelta.y);

            //Resets position of bars based on their layout group settings
            PlayerUIManager.instance.playerUIHudManager.RefreshHud();
        }
    }

}
