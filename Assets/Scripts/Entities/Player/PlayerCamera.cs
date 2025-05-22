using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public static PlayerCamera instance;
    public PlayerManager player;
    public Camera cameraObject;
    [SerializeField] public Transform cameraPivotTransform;
    [SerializeField] Vector3 cameraYOffsetVector3;
    [SerializeField] float cameraYOffset = 1;

    //Change these to tweak camera performance
    [Header("Camera Settings")]
    //Larger cameraSmoothSpeed values equal longer time for the camera to reach its position during movement
    public float cameraSmoothSpeed = 2f;
    public bool isCameraInverted = true;

    //Rotation sensitivities
    [SerializeField] float leftAndRightRotationSpeed = 220f;
    [SerializeField] float upAndDownRotationSpeed = 220f;

    //Lowest point you can look down
    [SerializeField] float minimumPivot = -30f;

    //Highest point you can look up
    [SerializeField] float maximumPivot = 60f;

    //Camera collision radius
    [SerializeField] float cameraCollisionRadius = 0.2f;
    [SerializeField] LayerMask collideWithLayers;

    //Displays camera values
    [Header("Camera Values")]
    private Vector3 cameraVelocity;
    //Used for camera collisions, moves the camera object to this position upon colliding
    private Vector3 cameraObjectPosition;
    [SerializeField] public float leftAndRightLookAngle;
    [SerializeField] float upAndDownLookAngle;

    //Used for Camera Collisions
    private float cameraZPosition;
    private float targetCameraZPosition;
    private float cameraCollisionLerpDuration = 0.2f;

    [Header("Lock On")]
    [SerializeField] private float lockOnRadius = 12f;
    [SerializeField] private float minimumViewableAngle = -70f;
    [SerializeField] private float maximumViewableAngle = 70f;
    private List<CharacterManager> availableTargets = new List<CharacterManager>();
    public CharacterManager nearestLockOnTarget;
    public CharacterManager leftLockOnTarget;
    public CharacterManager rightLockOnTarget;
    [SerializeField] float lockOnTargetFollowSpeed = 0.2f;
    [SerializeField] float setCameraHeightSpeed = 0.05f;
    [SerializeField] public float unlockedCameraHeight = 0.6f;
    [SerializeField] float lockedCameraHeight = 1.2f;
    private Coroutine cameraLockOnHeightCoroutine;
    private Coroutine cameraLockOnHeightCoroutineOnUpdate;
    private bool isCameraLockOnHeightCoroutineActive = false;
    private bool cameraLoweringOutsideCoroutine = false;


    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        cameraZPosition = cameraObject.transform.localPosition.z;
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // public void Update() {
    //     HandleCameraHeightOutsideOfInitialLockOn();
    // }

    public void HandleAllCameraActions()
    {
        if (player != null)
        {
            HandleFollowTarget();
            HandleRotations();
            HandleCollisions();
            IdeaCameraController.instance.HandleRotations();
        }
    }

    private void HandleFollowTarget()
    {
        cameraYOffsetVector3 = new Vector3(player.transform.position.x, player.transform.position.y + cameraYOffset, player.transform.position.z);
        Vector3 targetCameraPosition = Vector3.SmoothDamp(transform.position, cameraYOffsetVector3, ref cameraVelocity, cameraSmoothSpeed * Time.deltaTime);
        transform.position = targetCameraPosition;
    }

    private void HandleRotations()
    {
        //If locked-on, force rotation towards target
        if (player.isLockedOn)
        {
            //This rotates this gameObject
            Vector3 rotationDirection = player.playerCombatManager.currentTarget.characterCombatManager.LockOnTransform.position - transform.position;
            rotationDirection.Normalize();
            rotationDirection.y = 0;

            Quaternion targetRotation = Quaternion.LookRotation(rotationDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, lockOnTargetFollowSpeed);

            //This rotates the pivot object
            //We don't set rotationDirection.y = 0 because this is the up/down rotation
            rotationDirection = player.playerCombatManager.currentTarget.characterCombatManager.LockOnTransform.position - cameraPivotTransform.position;
            rotationDirection.Normalize();

            targetRotation = Quaternion.LookRotation(rotationDirection);
            cameraPivotTransform.transform.rotation = Quaternion.Slerp(cameraPivotTransform.rotation, targetRotation, lockOnTargetFollowSpeed);

            //Save our rotation values, so when we unlock it doesn't snap too far away
            leftAndRightLookAngle = transform.eulerAngles.y;
            upAndDownLookAngle = transform.eulerAngles.x;
        }
        //Else, rotate normally
        else
        {
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
            if (isCameraInverted)
            {
                cameraRotation.x = -upAndDownLookAngle;
            }
            else
            {
                cameraRotation.x = upAndDownLookAngle;
            }
            targetRotation = Quaternion.Euler(cameraRotation);
            cameraPivotTransform.localRotation = targetRotation;
        }

    }

    private void HandleCollisions()
    {
        targetCameraZPosition = cameraZPosition;
        RaycastHit hit;
        //Direction for collision check
        Vector3 direction = cameraObject.transform.position - cameraPivotTransform.position;
        direction.Normalize();

        //Check if an object is in front of our camera's desired direction
        if (Physics.SphereCast(cameraPivotTransform.position, cameraCollisionRadius, direction, out hit, Mathf.Abs(targetCameraZPosition), collideWithLayers))
        {
            //If there is, we get our distance from it
            float distanceFromHitObject = Vector3.Distance(cameraPivotTransform.position, hit.point);
            //We then equate our target Z position to the following
            targetCameraZPosition = -(distanceFromHitObject - cameraCollisionRadius);
        }

        //If our target position is less than our collision radius, we subtract our collision radius (Making it snap back)
        if (Mathf.Abs(targetCameraZPosition) < cameraCollisionRadius)
        {
            targetCameraZPosition = -cameraCollisionRadius;
        }

        //We then apply our final position using a lerp over a time of cameraCollisionLerpDuration
        cameraObjectPosition.z = Mathf.Lerp(cameraObject.transform.localPosition.z, targetCameraZPosition, cameraCollisionLerpDuration);
        cameraObject.transform.localPosition = cameraObjectPosition;
    }

    public void HandleLocatingLockOnTargets()
    {
        //Will be used to determine the target closest to us
        float shortestDistance = Mathf.Infinity;

        //Will be used to determine shortest distance on one axis to the right of current target 
        //aka Closest target to the right of current target
        float shortestDistanceOfRightTarget = Mathf.Infinity;

        //Will be used to determine shortest distance on one axis to the right of current target 
        //aka Closest target to the left of current target
        float shortestDistanceOfLeftTarget = Mathf.Infinity;

        //Uses a Character Layermask to improve efficiency
        Collider[] colliders = Physics.OverlapSphere(player.transform.position, lockOnRadius, WorldUtilityManager.instance.GetCharacterLayers());

        for (int i = 0; i < colliders.Length; i++)
        {
            CharacterManager lockOnTarget = colliders[i].GetComponent<CharacterManager>();

            if (lockOnTarget != null)
            {
                //Check if they are within our Field of View
                Vector3 lockOnTargetsDirection = lockOnTarget.transform.position - player.transform.position;
                float distanceFromTarget = Vector3.Distance(player.transform.position, lockOnTarget.transform.position);
                float viewableAngle = Vector3.Angle(lockOnTargetsDirection, cameraObject.transform.forward);

                //If target is dead, check the next potential target
                if (lockOnTarget.isDead)
                {
                    continue;
                }

                //If the target is us, check the next potential target
                if (lockOnTarget.transform.root == player.transform.root)
                {
                    continue;
                }

                //If the target is outside of the field of view or blocked by environment, check the next potential target
                if (viewableAngle > minimumViewableAngle && viewableAngle < maximumViewableAngle)
                {
                    RaycastHit hit;

                    //Check Line of sight by environment layers
                    if (Physics.Linecast(player.playerCombatManager.LockOnTransform.position, lockOnTarget.characterCombatManager.LockOnTransform.position, out hit, WorldUtilityManager.instance.GetEnvironmentLayers()))
                    {
                        //We hit something in the environment, blocking line of sight
                        continue;
                    }
                    else
                    {
                        //Add the target to the available targets list since it's within Line of Sight
                        availableTargets.Add(lockOnTarget);
                    }
                }

            }
        }

        //Sort through potential targets to see which one we lock onto first
        for (int j = 0; j < availableTargets.Count; j++)
        {
            if (availableTargets[j] != null)
            {
                float distanceFromTarget = Vector3.Distance(player.transform.position, availableTargets[j].transform.position);

                if (distanceFromTarget < shortestDistance)
                {
                    shortestDistance = distanceFromTarget;
                    nearestLockOnTarget = availableTargets[j];
                }

                //If already locked on when searching for targets, search for our nearest left/right targets
                if (player.isLockedOn)
                {
                    Vector3 relativeEnemyPosition = player.transform.InverseTransformPoint(availableTargets[j].transform.position);

                    var distanceFromLeftTarget = relativeEnemyPosition.x;
                    var distanceFromRightTarget = relativeEnemyPosition.x;

                    if (availableTargets[j] == player.playerCombatManager.currentTarget)
                    {
                        continue;
                    }

                    //Check the left side for targets
                    //Might need to change distanceFromLeftTarget < shortestDistanceOfLeftTarget to distanceFromLeftTarget > shortestDistanceOfLeftTarget
                    if (relativeEnemyPosition.x <= 0.00f && distanceFromLeftTarget < shortestDistanceOfLeftTarget)
                    {
                        shortestDistanceOfLeftTarget = distanceFromLeftTarget;
                        leftLockOnTarget = availableTargets[j];
                    }
                    //Check the right side for targets
                    else if (relativeEnemyPosition.x >= 0.00f && distanceFromRightTarget < shortestDistanceOfRightTarget)
                    {
                        shortestDistanceOfRightTarget = distanceFromRightTarget;
                        rightLockOnTarget = availableTargets[j];
                    }
                }
            }
            else
            {
                ClearLockOnTargets();
                player.isLockedOn = false;
            }
        }
    }

    // public void HandleCameraHeightOutsideOfInitialLockOn() {
    //     if (!isCameraLockOnHeightCoroutineActive) {
    //         if (PlayerInputManager.instance.lockOnInput && player.playerCombatManager.currentTarget != null && !cameraLoweringOutsideCoroutine) {
    //             cameraLoweringOutsideCoroutine = true;
    //             //Lower the Camera over time
    //             if (cameraLockOnHeightCoroutineOnUpdate != null) {
    //                 StopCoroutine(cameraLockOnHeightCoroutineOnUpdate);
    //                 cameraLockOnHeightCoroutineOnUpdate = null;
    //             }
    //             cameraLockOnHeightCoroutineOnUpdate = StartCoroutine(LowerCameraHeight());
    //         }
    //     }
    //     else {
    //         if (cameraLoweringOutsideCoroutine) {
    //             StopCoroutine(cameraLockOnHeightCoroutineOnUpdate);
    //         }
    //         cameraLoweringOutsideCoroutine = false;
    //     }
    // }

    public void SetLockCameraHeight()
    {
        if (cameraLockOnHeightCoroutine != null)
        {
            StopCoroutine(cameraLockOnHeightCoroutine);
        }

        cameraLockOnHeightCoroutine = StartCoroutine(SetCameraHeight());
    }

    public void ClearLockOnTargets()
    {
        ClearLockOnVFX();
        nearestLockOnTarget = null;
        leftLockOnTarget = null;
        rightLockOnTarget = null;
        availableTargets.Clear();

        //Lower the Camera over time
        // cameraLoweringOutsideCoroutine = true;
        // if (!isCameraLockOnHeightCoroutineActive && player.isLockedOn) {
        //     if (cameraLockOnHeightCoroutineOnUpdate != null) {
        //         StopCoroutine(cameraLockOnHeightCoroutineOnUpdate);
        //         cameraLockOnHeightCoroutineOnUpdate = null;
        //     }
        //     cameraLockOnHeightCoroutineOnUpdate = StartCoroutine(LowerCameraHeight());
        // }
    }

    public void InvokeLowerCameraHeightCoroutine()
    {
        //Lower the Camera over time
        if (cameraLockOnHeightCoroutineOnUpdate != null)
        {
            StopCoroutine(cameraLockOnHeightCoroutineOnUpdate);
            cameraLockOnHeightCoroutineOnUpdate = null;
        }
        cameraLockOnHeightCoroutineOnUpdate = StartCoroutine(LowerCameraHeight());
    }
    public void ClearLockOnVFX()
    {
        foreach (CharacterManager target in availableTargets)
        {
            if (target.characterCombatManager.LockOnVFX != null)
            {
                target.characterCombatManager.DisableLockOnVFX();
            }
        }
    }

    public IEnumerator WaitThenFindNewTarget()
    {
        while (player.isPerformingAction)
        {
            yield return null;
        }

        ClearLockOnTargets();
        HandleLocatingLockOnTargets();

        if (nearestLockOnTarget != null)
        {
            player.playerCombatManager.SetTarget(nearestLockOnTarget);
            player.isLockedOn = true;
        }
        else
        {
            InvokeLowerCameraHeightCoroutine();
        }

        yield return null;
    }

    public IEnumerator SetCameraHeight()
    {
        float durationInSeconds = 1f;
        float timer = 0f;
        isCameraLockOnHeightCoroutineActive = true;

        Vector3 velocity = Vector3.zero;
        Vector3 newLockedCameraHeight = new Vector3(cameraPivotTransform.transform.localPosition.x, lockedCameraHeight);
        Vector3 newUnlockedCameraHeight = new Vector3(cameraPivotTransform.transform.localPosition.x, unlockedCameraHeight);

        while (timer < durationInSeconds)
        {
            timer += Time.deltaTime;

            if (player != null)
            {
                if (player.playerCombatManager.currentTarget != null)
                {
                    //Set Height
                    cameraPivotTransform.transform.localPosition =
                        Vector3.SmoothDamp(cameraPivotTransform.transform.localPosition, newLockedCameraHeight, ref velocity, setCameraHeightSpeed);

                    //Set Rotation
                    cameraPivotTransform.transform.localRotation =
                        Quaternion.Slerp(cameraPivotTransform.transform.localRotation, Quaternion.Euler(0, 0, 0), lockOnTargetFollowSpeed);
                }
                else
                {
                    //Set Height
                    cameraPivotTransform.transform.localPosition =
                        Vector3.SmoothDamp(cameraPivotTransform.transform.localPosition, newUnlockedCameraHeight, ref velocity, setCameraHeightSpeed);
                }
            }

            yield return null;
        }

        if (player != null)
        {
            //Failsafe
            if (player.playerCombatManager.currentTarget != null)
            {
                cameraPivotTransform.transform.localPosition = newLockedCameraHeight;
                cameraPivotTransform.transform.localRotation = Quaternion.Euler(0, 0, 0);
            }
            else
            {
                cameraPivotTransform.transform.localPosition = newUnlockedCameraHeight;
            }
        }

        isCameraLockOnHeightCoroutineActive = false;

        yield return null;
    }

    public IEnumerator LowerCameraHeight()
    {
        float durationInSeconds = 1f;
        float timer = 0f;

        Vector3 velocity = Vector3.zero;
        Vector3 newLockedCameraHeight = new Vector3(cameraPivotTransform.transform.localPosition.x, lockedCameraHeight);
        Vector3 newUnlockedCameraHeight = new Vector3(cameraPivotTransform.transform.localPosition.x, unlockedCameraHeight);

        while (timer < durationInSeconds)
        {
            timer += Time.deltaTime;

            if (player.playerCombatManager.currentTarget != null)
            {
                if (!player.playerCombatManager.currentTarget.isDead) {
                    //Set Height
                    cameraPivotTransform.transform.localPosition =
                        Vector3.SmoothDamp(cameraPivotTransform.transform.localPosition, newLockedCameraHeight, ref velocity, setCameraHeightSpeed);

                    //Set Rotation
                    cameraPivotTransform.transform.localRotation =
                        Quaternion.Slerp(cameraPivotTransform.transform.localRotation, Quaternion.Euler(0, 0, 0), lockOnTargetFollowSpeed);
                }
                else {
                    //Set Height
                    cameraPivotTransform.transform.localPosition =
                        Vector3.SmoothDamp(cameraPivotTransform.transform.localPosition, newUnlockedCameraHeight, ref velocity, setCameraHeightSpeed);

                    //Set Rotation
                    cameraPivotTransform.transform.localRotation =
                        Quaternion.Slerp(cameraPivotTransform.transform.localRotation, Quaternion.Euler(0, 0, 0), lockOnTargetFollowSpeed);
                }
            }
            else
            {
                //Set Height
                cameraPivotTransform.transform.localPosition =
                    Vector3.SmoothDamp(cameraPivotTransform.transform.localPosition, newUnlockedCameraHeight, ref velocity, setCameraHeightSpeed);
            }

            yield return null;
        }

        if (player != null)
        {
            //Failsafe
            if (player.playerCombatManager.currentTarget != null)
            {
                if (!player.playerCombatManager.currentTarget.isDead)
                {
                    cameraPivotTransform.transform.localPosition = newLockedCameraHeight;
                    cameraPivotTransform.transform.localRotation = Quaternion.Euler(0, 0, 0);
                }
                else
                {
                    cameraPivotTransform.transform.localPosition = newUnlockedCameraHeight;
                    cameraPivotTransform.transform.localRotation = Quaternion.Euler(0, 0, 0);
                }
            }
            else
            {
                cameraPivotTransform.transform.localPosition = newUnlockedCameraHeight;
            }

        }

        yield return null;
    }

    public Transform GetPivotTransform() { return cameraPivotTransform; }
}
