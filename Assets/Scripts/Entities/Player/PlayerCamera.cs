using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public static PlayerCamera instance;
    public PlayerManager player;
    public Camera cameraObject;
    [SerializeField] Transform cameraPivotTransform;

    //Change these to tweak camera performance
    [Header("Camera Settings")]
    //Larger cameraSmoothSpeed values equal longer time for the camera to reach its position during movement
    private float cameraSmoothSpeed = 1f;
    public bool isCameraInverted = true;

    //Rotation sensitivities
    [SerializeField] float leftAndRightRotationSpeed = 220f;
    [SerializeField] float upAndDownRotationSpeed = 220f;

    //Lowest point you can look down
    [SerializeField] float minimumPivot = -30f;

    //Highest point you can look up
    [SerializeField] float maximumPivot = 60f;

    //Camera collision radius
    [SerializeField]  float cameraCollisionRadius = 0.2f;
    [SerializeField] LayerMask collideWithLayers;

    //Displays camera values
    [Header("Camera Values")]
    private Vector3 cameraVelocity;
    //Used for camera collisions, moves the camera object to this position upon colliding
    private Vector3 cameraObjectPosition;
    [SerializeField] float leftAndRightLookAngle;
    [SerializeField] float upAndDownLookAngle;
    
    //Used for Camera Collisions
    private float cameraZPosition;
    private float targetCameraZPosition;
    private float cameraCollisionLerpDuration = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        cameraZPosition = cameraObject.transform.localPosition.z;
    }

    private void Awake() {
        if (instance == null) {
            instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }

    public void HandleAllCameraActions() {
        if (player != null) {
            HandleFollowTarget();
            HandleRotations();
            HandleCollisions();
        }
    }

    private void HandleFollowTarget() {
        Vector3 targetCameraPosition = Vector3.SmoothDamp(transform.position, player.transform.position, ref cameraVelocity, cameraSmoothSpeed * Time.deltaTime);
        transform.position = targetCameraPosition;
    }

    private void HandleRotations() {
        //If locked-on, force rotation towards target
        //Else, rotate normally

        //Normal Rotations
        //Rotate left and right based on horizontal movement on the right joystick
        leftAndRightLookAngle += (PlayerInputManager.instance.cameraHorizontalInput * leftAndRightRotationSpeed) * Time.deltaTime;

        //Rotate up and down based on the vertical movement on the right Joystick
        upAndDownLookAngle -= (PlayerInputManager.instance.cameraVerticalInput * upAndDownRotationSpeed) * Time.deltaTime;
        
        //Clamp the up and down look angle between min/max values
        upAndDownLookAngle = Mathf.Clamp(upAndDownLookAngle, minimumPivot, maximumPivot);

        //Temp variables used for the below assignments
        Vector3 cameraRotation = Vector3.zero;
        Quaternion targetRotation;

        //Rotate this gameobject left and right
        cameraRotation.y = leftAndRightLookAngle;
        targetRotation = Quaternion.Euler(cameraRotation);
        transform.rotation = targetRotation;

        //Rotate this gameobject up and down
        cameraRotation = Vector3.zero;
        if (isCameraInverted) {
            cameraRotation.x = -upAndDownLookAngle;
        }
        else {
            cameraRotation.x = upAndDownLookAngle;
        }
        targetRotation = Quaternion.Euler(cameraRotation);
        cameraPivotTransform.localRotation = targetRotation;
    }

    private void HandleCollisions() {
        targetCameraZPosition = cameraZPosition;
        RaycastHit hit;
        //Direction for collision check
        Vector3 direction = cameraObject.transform.position - cameraPivotTransform.position;
        direction.Normalize();

        //Check if an object is in front of our camera's desired direction
        if (Physics.SphereCast(cameraPivotTransform.position, cameraCollisionRadius, direction, out hit, Mathf.Abs(targetCameraZPosition), collideWithLayers)) {
            //If there is, we get our distance from it
            float distanceFromHitObject = Vector3.Distance(cameraPivotTransform.position, hit.point);
            //We then equate our target Z position to the following
            targetCameraZPosition = -(distanceFromHitObject - cameraCollisionRadius);
        }

        //If our target position is less than our collision radius, we subtract our collision radius (Making it snap back)
        if (Mathf.Abs(targetCameraZPosition) < cameraCollisionRadius) {
            targetCameraZPosition = -cameraCollisionRadius;
        }

        //We then apply our final position using a lerp over a time of cameraCollisionLerpDuration
        cameraObjectPosition.z = Mathf.Lerp(cameraObject.transform.localPosition.z, targetCameraZPosition, cameraCollisionLerpDuration);
        cameraObject.transform.localPosition = cameraObjectPosition;
    }
}
