using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinObjectAtSpeed : MonoBehaviour
{
    public Vector3 rotationSpeed;
    public bool isSpinningFromStart = false;
    private bool isSpinning = false;

    // Start is called before the first frame update
    void Start()
    {
        if (isSpinningFromStart)
        {
            isSpinning = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isSpinning)
        {
            transform.Rotate(rotationSpeed * Time.deltaTime);
        }
    }

    public void InitializeSpinning(Vector3 rotationSpeedInDegreesPerSecond)
    {
        rotationSpeed = rotationSpeedInDegreesPerSecond;
        isSpinning = true;
    }


}
