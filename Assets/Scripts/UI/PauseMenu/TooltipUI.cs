using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TooltipUI : MonoBehaviour
{
    public TextMeshProUGUI headerText;
    public TextMeshProUGUI centerText;
    public TextMeshProUGUI bottomText;
    // Start is called before the first frame update
    public void ActivateFromButtonOnclick(GameObject tooltipObj)
    {
        tooltipObj.SetActive(true);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
