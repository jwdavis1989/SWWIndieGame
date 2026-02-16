using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BobUpAndDownOnCurve : MonoBehaviour
{
    public AnimationCurve bobCurve;
    private bool isBobbing = false;

    [Header("Bobbing Parameters")]
    public float bobSpeed = 1f;
    public float heightMultiplier = 1f;

    [Header("Bobbing Delay Parameters")]
    public bool isBobbingDelayed = false;
    public float delayBeforeBobbingInSeconds = 0f;
    public bool isBobbingDelayRandom = false;
    public float bobbingDelayRandomRange = 0.1f;

    private Vector3 originalLocalPosition;

    // Start is called before the first frame update
    void Start()
    {
        originalLocalPosition = transform.localPosition;
        if (!isBobbingDelayed)
        {
            isBobbing = true;
        }
        else
        {
            float newBobDelay = delayBeforeBobbingInSeconds;

            if (isBobbingDelayRandom)
            {
                newBobDelay = Random.Range(0, bobbingDelayRandomRange);
            }
            
            StartCoroutine(BobAfterDelay(newBobDelay));
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isBobbing)
        {
            float time = Time.time * bobSpeed;
            float yOffset = bobCurve.Evaluate(time % bobCurve.length) * heightMultiplier;

            transform.localPosition = new Vector3(originalLocalPosition.x, originalLocalPosition.y + yOffset, originalLocalPosition.z);
        }
    }

    IEnumerator BobAfterDelay(float delayDurationInSeconds)
    {
        yield return new WaitForSeconds(delayDurationInSeconds);

        isBobbing = true;
    }
}
