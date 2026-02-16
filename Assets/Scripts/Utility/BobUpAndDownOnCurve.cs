using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BobUpAndDownOnCurve : MonoBehaviour
{
    public AnimationCurve bobCurve;

    [Header("Bobbing Parameters")]
    public float bobSpeed = 1f;
    public float heightMultiplier = 1f;

    private Vector3 originalLocalPosition;

    // Start is called before the first frame update
    void Start()
    {
        originalLocalPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        float time = Time.time * bobSpeed;
        float yOffset = bobCurve.Evaluate(time % bobCurve.length) * heightMultiplier;

        transform.localPosition = new Vector3(originalLocalPosition.x, originalLocalPosition.y + yOffset, originalLocalPosition.z);
    }
}
