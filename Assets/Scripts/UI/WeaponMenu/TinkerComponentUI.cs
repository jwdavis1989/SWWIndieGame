using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TinkerComponentUI : MonoBehaviour
{
    public TextMeshProUGUI tooltip;//OLD Text on hover/help
    public GameObject tooltipHolder;//OLD tooltip obj
    public TooltipUI tooltipUI;//new
    public TextMeshProUGUI countText;
    public Button mainButton;
    public GameObject foregroundIcon;
    public int index = 0;
    [HideInInspector] public TinkerComponentStats refComponent;//ref to component
    [HideInInspector] public string refItemId;//ref to component
    public void ActivateTooltip()
    {
        if(tooltipHolder != null) 
            tooltipHolder.SetActive(true);
    }
    public void DeactivateTooltip()
    {
        if (tooltipHolder != null)
            tooltipHolder.SetActive(false);
    }
}
