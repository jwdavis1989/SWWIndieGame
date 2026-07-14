using UnityEngine;
using UnityEngine.UI;

// [RequireComponent(typeof(Image))]
public class UIRadialStatBar : MonoBehaviour
{
    [Header("Shader Properties")]
    [Tooltip("The total number of segments configured in your shader.")]
    [SerializeField] private float segmentCount = 10f;

    [Header("Smooth Animation")]
    [Tooltip("How fast the bar catches up to the real health value. Higher = faster.")]
    [SerializeField] private float smoothSpeed = 5f;


    //Cached property IDs for maximum rendering performance
    private int segmentCountID;
    private int removedSegmentsID;
    private Material runtimeMaterial;

    //Tracking variables for smooth interpolation
    private float targetRemovedSegments = 0f;
    private float currentRemovedSegments = 0f;

    void Awake()
    {
        //Cache the exact string names as reference IDs
        segmentCountID = Shader.PropertyToID("_SegmentCount");
        removedSegmentsID = Shader.PropertyToID("_RemovedSegments");

        //Instantiate the material to prevent modifying the project asset file
        Image uiImage = GetComponent<Image>();
        if (uiImage.material != null)
        {
            uiImage.material = new Material(uiImage.material);
            runtimeMaterial = uiImage.material;
            Debug.Log("Color: " + runtimeMaterial.color);
            Debug.Log("_Color: " + Shader.PropertyToID("_Color"));
        }
    }

    void Start()
    {
        if (runtimeMaterial != null)
        {
            runtimeMaterial.SetFloat(segmentCountID, segmentCount);
        }
    }

    void Update()
    {
        if (runtimeMaterial == null) return;

        //Interpolate the current display value toward the true target value
        currentRemovedSegments = Mathf.Lerp(currentRemovedSegments, targetRemovedSegments, Time.deltaTime * smoothSpeed);

        //Update the shader every frame
        runtimeMaterial.SetFloat(removedSegmentsID, currentRemovedSegments);
    }

    
    ///Updates the radial bar segment removal based on current/max stat combination
    public void UpdateStatBar(float currentValue, float maxValue)
    {
        //Prevent division-by-zero crashes if max value is invalid
        if (maxValue <= 0) return;

        //Clamp current value to safe bounds (0 to max)
        float clampedCurrent = Mathf.Clamp(currentValue, 0f, maxValue);

        //1. Find out what percentage of the stat is missing (0.0 to 1.0)
        float missingPercent = 1f - (clampedCurrent / maxValue);

        //2. Set the target destination for Update()'s Lerp
        targetRemovedSegments = missingPercent * segmentCount;
    }
}
