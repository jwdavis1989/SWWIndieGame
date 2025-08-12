using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashingBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("FlashingBehavior will activate/deactivate lightObject at interval")]
    public GameObject lightObject = null;
    public float flashInterval = 1f;
    public bool activateOnStart = false;
    private bool flashingActive = false;
    private bool lightActive = false;
    float flashTimer = 0; 
    /** Turn on the lights */
    public void ActivateFlashing()
    {
        flashingActive = true;
    }
    void Start()
    {
        if (activateOnStart)
        {
            flashingActive = true;
            lightObject.SetActive(true);
        }
        else
        {
            flashingActive = false;
            lightObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (flashingActive)
        {
            flashTimer += Time.deltaTime;
            if (flashTimer > flashInterval)
            {
                flashTimer = 0;
                if (lightActive){
                    lightActive = false;
                    lightObject.SetActive(false);
                }
                else
                {
                    lightActive = true;
                    lightObject.SetActive(true);
                }
            }
        }
    }
    
}
