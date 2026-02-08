using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FadeElementalMeteorSmashDecalOverTime : MonoBehaviour
{
    private Renderer objectRenderer;
    [Header("WARNING: Array lengths must match!")]
    [Header("Order: Fire, Ice, Lightning, Wind, Earth, Light, Beast, Scales, Tech")]
    public Color[] startCoreColorArray;         //Array that contains a multitude of starting core colors (Pit of cracks)
    public Color[] startInnerColorArray;        //Array that contains a multitude of starting inner colors (Inside walls of cracks)
    public int currentColorIndex = 0;
    public Color endColor = new Vector4(0, 0, 0, 0);
    [Header("If true, the color will make the endColor the startColor with 0 opacity")]
    public bool useStartColorAsEndColor = true;
    public float colorChangeDuration = 2.0f; //Duration of color change in seconds

    // Start is called before the first frame update
    public void InitializeColorFading()
    {
        objectRenderer = GetComponent<Renderer>();
        if (startCoreColorArray.Length == startInnerColorArray.Length)
        {
            //Set the Core Color to the weapon's highest element's color
            objectRenderer.material.SetColor("_CoreColor", startCoreColorArray[currentColorIndex]);

            //Set the Inner Color to the weapon's highest element's color
            objectRenderer.material.SetColor("_CoreColor", startInnerColorArray[currentColorIndex]);

            //Set the emission color to the same as the Core Color
            objectRenderer.material.SetColor("_EmissionColor", startCoreColorArray[currentColorIndex]);

            if (useStartColorAsEndColor)
            {
                endColor = startCoreColorArray[currentColorIndex];
                endColor.a = 0;
            }
            StartCoroutine(ChangeOpacityOverTimeCoroutine());
        }
        else
        {
            Debug.Log("WARNING: Array lengths must match!");
        }
    }

    private IEnumerator ChangeOpacityOverTimeCoroutine()
    {
        float elapsedTime = 0;

        while (elapsedTime < colorChangeDuration)
        {
            //Calculate time between 0 and 1
            elapsedTime += Time.deltaTime;
            float time = Mathf.Clamp01(elapsedTime / colorChangeDuration);

            //Lower the opacity of the Core Color, which applies to the entire Decal.
            objectRenderer.material.color = Color.Lerp(startCoreColorArray[currentColorIndex], endColor, time);

            yield return null;
        }

        // Ensure the color is exactly the endColor at the end of the duration
        objectRenderer.material.color = endColor;
        Destroy(gameObject);
    }

}
