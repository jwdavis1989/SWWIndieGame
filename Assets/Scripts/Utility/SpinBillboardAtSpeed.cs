using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinBillboardAtSpeed : MonoBehaviour
{
    public float rotationSpeed;
    public bool isSpinningFromStart = false;
    private bool isSpinning = false;
    private float originalXScale;
    // private float maxXScale;
    // private float minXScale;
    //private bool shrinkingX = true;
    private float waveTimer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        if (isSpinningFromStart)
        {
            isSpinning = true;
        }

        originalXScale = transform.localScale.x;
    }

    // Update is called once per frame
    void Update()
    {
        // if (isSpinning)
        // {
        //     if (shrinkingX)
        //     {
        //         if (transform.localScale.x > minXScale)
        //         {
        //             float newXScale = Mathf.Lerp(transform.localScale.x, minXScale, Time.deltaTime * rotationSpeed);
        //             transform.localScale = new Vector3(newXScale, transform.localScale.y, transform.localScale.z);
        //         }
        //         else
        //         {
        //             shrinkingX = !shrinkingX;
        //         }
        //     }
        //     else if (!shrinkingX)
        //     {
        //         if (transform.localScale.x < maxXScale)
        //         {
        //             float newXScale = Mathf.Lerp(transform.localScale.x, maxXScale, Time.deltaTime * rotationSpeed);
        //             transform.localScale = new Vector3(newXScale, transform.localScale.y, transform.localScale.z);
        //         }
        //         else
        //         {
        //             shrinkingX = !shrinkingX;
        //         }
        //     }
        // }

        if (isSpinning)
        {
            // Advance time scaled by your rotation speed
            waveTimer += Time.deltaTime * rotationSpeed;

            // Mathf.Sin smoothly oscillates between -1 and 1
            float scaleModifier = Mathf.Sin(waveTimer);

            // Multiply the original scale by the modifier to shrink, invert, and grow it
            float newXScale = originalXScale * scaleModifier;

            transform.localScale = new Vector3(newXScale, transform.localScale.y, transform.localScale.z);
        }


    }

    public void InitializeSpinning(float rotationSpeedInDegreesPerSecond)
    {
        isSpinning = true;
    }
}
