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

    //Rotation sensitivities
    [SerializeField] float leftAndRightRotationSpeed = 220f;
    [SerializeField] float upAndDownRotationSpeed = 220f;

    //Lowest point you can look down
    [SerializeField] float minimumPivot = -30f;

    //Highest point you can look up
    [SerializeField] float maximumPivot = 60f;

    //Displays camera values
    [Header("Camera Values")]
    private Vector3 cameraVelocity;
    [SerializeField] float leftAndRightLookAngle;
    [SerializeField] float upAndDownLookAngle;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
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
            //3. Collide with objects
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
        cameraRotation.x = upAndDownLookAngle;
        targetRotation = Quaternion.Euler(cameraRotation);
        cameraPivotTransform.localRotation = targetRotation;
    }
}
