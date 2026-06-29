using UnityEngine;
using UnityEngine.UI;

public class UIHexagonStatBar : MonoBehaviour
{
    [Header("UI Component Hookups")]
    [SerializeField] private Image statBarImage;

    [Header("Interpolation Settings")]
    [SerializeField] private float smoothSpeed = 1.5f;

    private Material statBarMaterial;
    private static readonly int StatPercentPropertyID = Shader.PropertyToID("_Health");
    
    private float targetPercent = 1f;
    private float currentDisplayedPercent = 1f;

    private void Awake()
    {
        if (statBarImage != null && statBarImage.material != null)
        {
            // Creates a local unique instance so modifying one bar doesn't break the others
            statBarMaterial = new Material(statBarImage.material);
            statBarImage.material = statBarMaterial;
        }
    }

    private void Update()
    {
        if (statBarMaterial == null) return;

        if (!Mathf.Approximately(currentDisplayedPercent, targetPercent))
        {
            currentDisplayedPercent = Mathf.MoveTowards(
                currentDisplayedPercent, 
                targetPercent, 
                smoothSpeed * Time.deltaTime
            );

            statBarMaterial.SetFloat(StatPercentPropertyID, currentDisplayedPercent);
        }
    }

    /// <summary>
    /// Call this universally for health, stamina, or fuel updates.
    /// </summary>
    public void UpdateStatBar(float currentValue, float maxValue)
    {
        targetPercent = Mathf.Clamp01(currentValue / maxValue);
    }
}