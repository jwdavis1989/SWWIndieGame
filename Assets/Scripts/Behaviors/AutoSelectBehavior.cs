using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoSelectBehavior : MonoBehaviour
{
    [Header("AutoSelectBehavior will enable buttonToEnable if set *OR* a Button on the same GameObject")]
    public Button buttonToEnable;
    private void OnEnable()
    {
        if(buttonToEnable != null )
            buttonToEnable.Select();
        else
            GetComponent<Button>().Select();
    }
}
