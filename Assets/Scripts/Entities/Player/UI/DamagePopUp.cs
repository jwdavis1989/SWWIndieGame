using System.Collections; // Required for Coroutines
using TMPro;
using UnityEngine;

public class DamagePopUp : MonoBehaviour
{
    private TextMeshProUGUI textMesh;
    private float lifetime = 1f;
    private float timer;
    private Vector3 moveVector;
    private float minimumHorizontalSpeed = 2.5f;
    private float maximumHorizontalSpeed = 5f;

    private float jitterAmount = 0.5f;
    private float gravity = 30f;

    public void Setup(float damageAmount)
    {
        textMesh = GetComponentInChildren<TextMeshProUGUI>();

        //Round the damage value
        int roundedDamage = Mathf.RoundToInt(damageAmount);

        textMesh.SetText(roundedDamage.ToString());

        //1 Set the upward speed
        float upwardSpeed = 2f;

        //2 Randomly send the text flying left or right
        float sideDirection = Random.value > 0.5f ? 1f : -1f;

        //3 Set the horizontal magnitude
        float horizontalSpeed = Random.Range(minimumHorizontalSpeed, maximumHorizontalSpeed);

        moveVector = new Vector3(horizontalSpeed * sideDirection, upwardSpeed, 0);
        timer = lifetime;

        //Start the animation
        StartCoroutine(AnimateScale());
    }

    private IEnumerator AnimateScale()
    {
        float popTime = 0.15f; // How fast it grows
        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = originalScale * 1.5f; // Pop to 150% size

        //Grow phase
        float elapsed = 0;
        while (elapsed < popTime)
        {
            elapsed += Time.deltaTime;
            transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsed / popTime);
            yield return null;
        }

        //Shrink phase (back to original)
        elapsed = 0;
        while (elapsed < popTime)
        {
            elapsed += Time.deltaTime;
            transform.localScale = Vector3.Lerp(targetScale, originalScale, elapsed / popTime);
            yield return null;
        }
    }

    private void Update()
    {
        transform.position += moveVector * Time.deltaTime;
        moveVector -= moveVector * 2f * Time.deltaTime;

        //Jitters the text for juice
        Vector3 jitterOffset = new Vector3(
            Random.Range(-jitterAmount, jitterAmount),
            Random.Range(-jitterAmount, jitterAmount),
            0
        );

        // Apply jitter to the visual child (not the parent, to keep movement clean)
        textMesh.transform.localPosition = jitterOffset;

        timer -= Time.deltaTime;
        if (timer <= 0) Destroy(gameObject);
        else if (timer < (lifetime / 2))
        {
            Color color = textMesh.color;
            color.a -= (1 / (lifetime / 2)) * Time.deltaTime * 2f;
            textMesh.color = color;

            //Apply gravity
            moveVector.y -= gravity * Time.deltaTime;
        }
    }
}
