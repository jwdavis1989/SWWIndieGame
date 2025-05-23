using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputManager instance;
    public PlayerManager player;
    PlayerControls playerControls;

    [Header("Movement Input")]
    [SerializeField] public Vector2 movementInput;
    public float horizontalInput;
    public float verticalInput;
    public float moveAmount;

    [Header("Player Action Input")]
    [SerializeField] bool dodgeInput = false;
    [SerializeField] bool sprintInput = false;
    [SerializeField] bool jumpInput = false;
    [SerializeField] bool lightAttackInput = false;
    [SerializeField] Vector2 mouseWheelInput;
    [SerializeField] bool heavyAttackInput = false;
    [SerializeField] bool holdHeavyAttackInput = false;

    [Header("Player UI Inputs")]
    [SerializeField] bool interactInput = false;
    [SerializeField] bool useItemQuickslotInput = false;//using for idea camera. This will be an item?
    [SerializeField] bool uiInteractInput = false;//(A)
    [SerializeField] bool uiInteractInput2 = false;//(X)
    [SerializeField] bool pauseInput = false;
    [SerializeField] bool capturePhotoInput = false;


    [Header("Queued Inputs")]
    [SerializeField] bool InputQueueIsActive = false;
    [SerializeField] float defaultQueueInputTimer = 0.5f;
    [SerializeField] float queueInputTimer = 0f;
    [SerializeField] bool queueLightAttackInput = false;
    [SerializeField] bool queueHeavyAttackInput = false;

    [Header("Camera Movement Input")]
    [SerializeField] Vector2 cameraInput;
    public float cameraHorizontalInput;
    public float cameraVerticalInput;
    public float defaultCameraFieldOfView = 60f;
    public float sprintCameraFieldOfViewMaximum = 90f;
    public float sprintCameraFieldOfViewDecreaseSpeed = 30f;
    public float sprintCameraFieldOfViewIncreaseSpeed = 15f;

    [Header("Lock-On Input")]
    [SerializeField] bool lockOnInput;
    [SerializeField] bool lockOnSelectLeftInput;
    [SerializeField] bool lockOnSelectRightInput;
    private Coroutine lockOnCoroutine;

    [Header("Weapon Swapping")]
    //Mouse & Keyboard
    [SerializeField] float mouseWheelVerticalInput;
    [SerializeField] float prevMouseWheelVerticalInput;

    //Gamepad
    [SerializeField] bool ChangeRightWeaponDPad = false;
    [SerializeField] bool ChangeLeftWeaponDPad = false;


    //Start is called before the first frame update
    void Start() {
        //Has to happen before we disable the instance
        DontDestroyOnLoad(gameObject);

        //When the scene changes, run this logic
        //This is to do with subscribing and might require research
        SceneManager.activeSceneChanged += OnSceneChange;
        
        instance.enabled = false;
        if (playerControls != null) {
                playerControls.Disable();
        }
    }

    //Update is called once per frame
    void Update()
    {
        HandleAllInputs();
    }

    private void HandleAllInputs() {
        HandleInteractInput();
        HandleUIInteractInput();
        HandlePauseInput();
        HandleUseItemQuickSlotInput();
        HandleCapturePhotoInput();

        HandleMovementInput();
        HandleCameraMovementInput();
        HandleDodgeInput();
        HandleSprintInput();
        HandleJumpInput();
        HandleMainHandLightAttackInput();
        HandleMouseKBWeaponSwapInput();
        HandleLockOnInput();
        HandleLockOnSwitchTargetInput();
        HandleMainHandHeavyAttackInput();
        HandleChargeMainHandHeavyAttackInput();
        HandleGamePadRightWeaponSwapInput();
        HandleGamePadLeftWeaponSwapInput();
        HandleQueuedInputs();
        HandleCameraFieldOfView();
    }

    //Goals:
    //1. Read joystick values
    //2. Move Character using those values
    private void OnEnable() {
        if (playerControls == null) {
            playerControls = new PlayerControls();

            //I believe these are establishing event listeners/subscribing
            playerControls.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
            playerControls.PlayerCamera.Movement.performed += i => cameraInput = i.ReadValue<Vector2>();
            playerControls.PlayerActions.Dodge.performed += i => dodgeInput = true;
            playerControls.PlayerActions.Jump.performed += i => jumpInput = true;

            //Melee Attacking
            playerControls.PlayerActions.LightAttack.performed += i => lightAttackInput = true;
            playerControls.PlayerActions.HeavyAttack.performed += i => heavyAttackInput = true;
            playerControls.PlayerActions.ChargeHeavyAttack.performed += i => holdHeavyAttackInput = true;
            playerControls.PlayerActions.ChargeHeavyAttack.canceled += i => holdHeavyAttackInput = false;

            //Attack Queueing
            playerControls.PlayerActions.QueueLightAttack.performed += i => QueueInput(ref queueLightAttackInput);
            playerControls.PlayerActions.QueueHeavyAttack.performed += i => QueueInput(ref queueHeavyAttackInput);

            //Switch Weapons on Gamepad
            playerControls.PlayerActions.ChangeRightWeaponDPad.performed += i => ChangeRightWeaponDPad = true;
            playerControls.PlayerActions.ChangeLeftWeaponDPad.performed += i => ChangeLeftWeaponDPad = true;

            //Lock On
            playerControls.PlayerActions.LockOn.performed += i => lockOnInput = true;
            playerControls.PlayerActions.SeekLeftLockOnTarget.performed += i => lockOnSelectLeftInput = true;
            playerControls.PlayerActions.SeekRightLockOnTarget.performed += i => lockOnSelectRightInput = true;

            //Holding the input sets Sprinting to true
            playerControls.PlayerActions.Sprint.performed += i => sprintInput = true;
            //Releasing sets the sprint Sprinting to false
            playerControls.PlayerActions.Sprint.canceled += i => sprintInput = false;

            //Flashlight
            playerControls.PlayerActions.ToggleFlashlight.performed += i => player.ToggleFlashlight();

            //Weapon Quick-Changing
            playerControls.PlayerActions.NextWeapon.performed += i => mouseWheelInput = i.ReadValue<Vector2>();

            //Debug Buttons
            playerControls.PlayerActions.DebugTestAddWeapon.performed += i => player.DebugAddWeapon();
            playerControls.PlayerActions.DebugTeleportToJerryDev.performed += (i => { 
                SceneManager.LoadSceneAsync(1); 
            });
            playerControls.PlayerActions.DebugTeleportToAlecDev.performed += (i => { 
                player.transform.position = new Vector3(0, 9, 0); 
                SceneManager.LoadSceneAsync(2);
            });
            playerControls.PlayerActions.DebugTeleportToJacobDev.performed += i => SceneManager.LoadSceneAsync(3);
            playerControls.PlayerActions.DebugTeleportToSurfaceDemo.performed += (i => { 
                player.transform.position = new Vector3(0, 9, 0); 
                SceneManager.LoadSceneAsync(4);
            });
            playerControls.PlayerActions.DebugFullResources.performed += i => player.playerStatsManager.FullyRestoreResources();

            //Player UI interactions
            playerControls.PlayerActions.Interact.performed += i => interactInput = true;
            playerControls.UI.UIButtonA.performed += i => uiInteractInput = true;
            playerControls.UI.UIButtonX.performed += i => uiInteractInput2 = true;
            playerControls.UI.PauseButton.performed += i => pauseInput = true;
            playerControls.PlayerActions.UseItemQuickSlot.performed += i => useItemQuickslotInput = true;
            playerControls.UI.CaptureIdeaPhotoBtn.performed += i => capturePhotoInput = true;
        }

        playerControls.Enable();
    }

    private void Awake() {

        //Establish Singleton Instance
        if (instance == null) {
            instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }

    private void OnSceneChange(Scene oldScene, Scene newScene) {
        //If we are loading into our world scene, enable our player controls
        //if (newScene.buildIndex == WorldSaveGameManager.instance.GetWorldSceneIndex() /*Debug only*/ || newScene.buildIndex == 2 /*Debug only*/) {
        if (newScene.buildIndex != 0) {
            //Script being enabled, not the game object
            instance.enabled = true;
            if (playerControls != null) {
                playerControls.Enable();
            }
        }
        //Otherwise, we're in a menu so disable player controls
        //This is so our player can't move around if we enter things like a character creation menu, etc
        else {
            instance.enabled = false;
            if (playerControls != null) {
                playerControls.Disable();
            }
        }
    }

    private void OnDestroy() {
        //If we destroy this object, we unsubcribe from this event
        //This is to do with subscribing and might require research
        SceneManager.activeSceneChanged -= OnSceneChange;
    }
    //Interact Button
    void HandleInteractInput()
    {
        if (interactInput)// [E], (A)
        {
            interactInput = false;

            //Note: Disable this if in combat for efficency?
            // If not busy doing something else
            if (!DialogueManager.IsInDialogue() && !IdeaCameraController.isBusy())  //player.isLockedOn &&
            {
                //Find NPC dialogue.
                NPCDialogue dialogue = DialogueManager.instance.HandleLocatingDialogueTargets();
                //NPCDialogue dialogue = player.playerCombatManager.currentTarget.GetComponent<NPCDialogue>();
                if (dialogue != null)
                { //play the dialogue if found
                    DialogueManager.instance.PlayDialoge(dialogue);
                    //Lock player. (Unlocked on exit in DialgoueManager.NextLine)
                    player.isPerformingAction = true;
                    player.canMove = false;
                    player.canRotate = false;
                }

            }
            
        }
    }
    //Interact Button during dialogue box
    void HandleUIInteractInput()
    {
        //if they press the button during a dialogue
        if (uiInteractInput || uiInteractInput2)// [LMB], [E], (A)
        {
            uiInteractInput = false;
            uiInteractInput2 = false;//(X) I wanted this to continue dialgue for now... Could change later
            if (DialogueManager.IsInDialogue())
            {
                DialogueManager.instance.DialogueBoxContinue();
            }
        }
    }
    //Use item button
    void HandleUseItemQuickSlotInput()
    {
        if (useItemQuickslotInput) // [1], (Y)
        {
            useItemQuickslotInput = false;
            if (SceneManager.GetActiveScene().buildIndex == 0) 
                return; //dont use on title screen

            //currently have camera here. Not sure if it gets it's own button or is an item
            IdeaCameraController.instance.ActivateDeactiveCameraView();
        }
    }
    //Pause button
    void HandlePauseInput()
    {
        if (pauseInput) // [Esc], (Start/Menu)
        {
            pauseInput = false;
            PauseScript.instance.PauseUnpause();
        }
    }
   
    //Idea Capture button
    void HandleCapturePhotoInput()
    {
        if (capturePhotoInput) // [Space], (X)
        {
            capturePhotoInput = false;
            IdeaCameraController.instance.TakeScreenshotInput();
        }
    }


    //Movement
    private void HandleMovementInput() {
        //check if busy
        if (DialogueManager.IsInDialogue() || IdeaCameraController.isBusy() || PauseScript.instance.gamePaused)
            return;
        verticalInput = movementInput.y;
        horizontalInput = movementInput.x;

        //Returns the absolute value of both movement inputs
        moveAmount = Mathf.Clamp01(Mathf.Abs(verticalInput) + Mathf.Abs(horizontalInput));

        //Clamp moveAmount's values to improve smoothness. Values will be 0, 0.5, or 1 (Optional)
        if (moveAmount <= 0.5 && moveAmount > 0) {
            moveAmount = 0.5f;
        }
        else if (moveAmount > 0.5 && moveAmount <= 1) {
            moveAmount = 1;
        }

        if (player == null) {
            return;
        }

        if (moveAmount != 0) {
            player.isMoving = true;
        }
        else {
            player.isMoving = false;
        }

        //If not locked on
        //We pass 0 for horizontal because we're not locked on, as we always rotate to face the way we walk.
        if (!player.isLockedOn || player.isSprinting) {
            player.playerAnimationManager.UpdateAnimatorMovementParameters(0, moveAmount, player.isSprinting);
        }
        //If locked on
        //If we are locked on, we want to pass the horizontal and vertical
        else {
            player.playerAnimationManager.UpdateAnimatorMovementParameters(horizontalInput, verticalInput, player.isSprinting);
        }

    }
    private void HandleMouseKBWeaponSwapInput()
    {
        mouseWheelVerticalInput = mouseWheelInput.y;
        //if(mouseWheelVerticalInput != prevMouseWheelVerticalInput)
        //{
        //    PlayerWeaponManager.instance.nextWeapon();
        //}
        if (mouseWheelVerticalInput == 1)
        {
            PlayerWeaponManager.instance.nextWeapon();
        }
        else if (mouseWheelVerticalInput == -1 )
        {
            PlayerWeaponManager.instance.nextSpecialWeapon();
        }
        prevMouseWheelVerticalInput = mouseWheelVerticalInput;
    }

    private void HandleGamePadRightWeaponSwapInput() {
        if (ChangeRightWeaponDPad) {
            ChangeRightWeaponDPad = false;
            PlayerWeaponManager.instance.nextWeapon();
        }
    }

    private void HandleGamePadLeftWeaponSwapInput() {
        if (ChangeLeftWeaponDPad) {
            ChangeLeftWeaponDPad = false;
            PlayerWeaponManager.instance.nextSpecialWeapon();
        }
    }

    private void HandleCameraMovementInput() {
        cameraVerticalInput = cameraInput.y;
        cameraHorizontalInput = cameraInput.x;
    }

    private void OnApplicationFocus(bool focus) {
        if (enabled) {
            if (focus) {
                playerControls.Enable();
            }
            else {
                playerControls.Disable();
            }
        }
    }

    //Actions
    private void HandleDodgeInput() {
        if (dodgeInput) {
            dodgeInput = false;

            //Future Note: Return if Menu or UI window is open, do nothing.
            if (PauseScript.instance.gamePaused || DialogueManager.IsInDialogue())
                return;
            //Perform the dodge
            player.playerLocomotionManager.AttemptToPerformDodge();
        }
    }

    private void HandleSprintInput() {
        if (sprintInput) {
            player.playerLocomotionManager.HandleSprinting();
            
            //Camera Zoom-Out Juice to give the illusion of great speed
            // if (!player.isLockedOn && player.playerStatsManager.currentStamina > 0 || player.isBoosting) {
            //     if (PlayerCamera.instance.cameraObject.fieldOfView < sprintCameraFieldOfViewMaximum) {
            //         PlayerCamera.instance.cameraObject.fieldOfView += sprintCameraFieldOfViewIncreaseSpeed * Time.deltaTime;
            //     }
            //     else {
            //         PlayerCamera.instance.cameraObject.fieldOfView = sprintCameraFieldOfViewMaximum;
            //     }
            // }
            // //Locked onto an enemy and need to reduce Field of View Extremely Rapidly
            // else if (PlayerCamera.instance.cameraObject.fieldOfView > defaultCameraFieldOfView) {
            //     PlayerCamera.instance.cameraObject.fieldOfView -= 3 * sprintCameraFieldOfViewDecreaseSpeed * Time.deltaTime;
            // }
            // else {
            //     PlayerCamera.instance.cameraObject.fieldOfView = defaultCameraFieldOfView;
            // }
        }
        else {
            player.playerLocomotionManager.characterManager.isSprinting = false;
            //Camera Zoom-Out Juice to give the illusion of Slowing Rapidly
            // if (PlayerCamera.instance.cameraObject.fieldOfView > defaultCameraFieldOfView && !player.isBoosting) {
            //     PlayerCamera.instance.cameraObject.fieldOfView -= sprintCameraFieldOfViewDecreaseSpeed * Time.deltaTime;
            // }
            // else {
            //     PlayerCamera.instance.cameraObject.fieldOfView = defaultCameraFieldOfView;
            // }
        }
    }

    private void HandleCameraFieldOfView() {
        if (sprintInput) {
            //Camera Zoom-Out Juice to give the illusion of great speed
            if (!player.isLockedOn && player.playerStatsManager.currentStamina > 0 || player.isBoosting) {
                if (PlayerCamera.instance.cameraObject.fieldOfView < sprintCameraFieldOfViewMaximum) {
                    PlayerCamera.instance.cameraObject.fieldOfView += sprintCameraFieldOfViewIncreaseSpeed * Time.deltaTime;
                }
                else {
                    PlayerCamera.instance.cameraObject.fieldOfView = sprintCameraFieldOfViewMaximum;
                }
            }
            //Locked onto an enemy and need to reduce Field of View Extremely Rapidly
            else if (PlayerCamera.instance.cameraObject.fieldOfView > defaultCameraFieldOfView) {
                PlayerCamera.instance.cameraObject.fieldOfView -= 3 * sprintCameraFieldOfViewDecreaseSpeed * Time.deltaTime;
            }
            else {
                PlayerCamera.instance.cameraObject.fieldOfView = defaultCameraFieldOfView;
            }
        }
        else {
            //Camera Zoom-Out Juice to give the illusion of Slowing Rapidly
            if (PlayerCamera.instance.cameraObject.fieldOfView > defaultCameraFieldOfView && !player.isBoosting) {
                PlayerCamera.instance.cameraObject.fieldOfView -= sprintCameraFieldOfViewDecreaseSpeed * Time.deltaTime;
            }
            else {
                PlayerCamera.instance.cameraObject.fieldOfView = defaultCameraFieldOfView;
            }
        }
    }

    private void HandleJumpInput() {
        if (jumpInput) {
            jumpInput = false;

            //If we have a UI window open, simply return without doing anything
            if(PauseScript.instance.gamePaused || DialogueManager.IsInDialogue() || IdeaCameraController.isBusy()) 
                return;
            //Attempt to perform a jump
            player.playerLocomotionManager.AttemptToPerformJump();
        }
    }

    private void HandleMainHandLightAttackInput() {
        if (lightAttackInput) {
            lightAttackInput = false;

            //TODO: Return if we have a UI Window Open

            if (PlayerWeaponManager.instance.ownedWeapons.Count > 0) {

                PlayerWeaponManager.instance.PerformWeaponBasedAction(PlayerWeaponManager.instance.ownedWeapons[PlayerWeaponManager.instance.indexOfEquippedWeapon].GetComponent<WeaponScript>().mainHandLightAttackAction, 
                                                PlayerWeaponManager.instance.ownedWeapons[PlayerWeaponManager.instance.indexOfEquippedWeapon].GetComponent<WeaponScript>());
            }
        }
    }

    private void HandleMainHandHeavyAttackInput() {
        if (heavyAttackInput) {
            heavyAttackInput = false;

            //TODO: Return if we have a UI Window Open

            if (PlayerWeaponManager.instance.ownedWeapons.Count > 0) {

                PlayerWeaponManager.instance.PerformWeaponBasedAction(PlayerWeaponManager.instance.ownedWeapons[PlayerWeaponManager.instance.indexOfEquippedWeapon].GetComponent<WeaponScript>().mainHandHeavyAttackAction, 
                                                PlayerWeaponManager.instance.ownedWeapons[PlayerWeaponManager.instance.indexOfEquippedWeapon].GetComponent<WeaponScript>());
            }
        }
    }

    private void HandleChargeMainHandHeavyAttackInput() {
        //We only want to check for a charge if we are in an action that requires it (e.g. Attacking)
        if (player.isPerformingAction) {
            player.isChargingAttack = holdHeavyAttackInput;
        }
    }


    //Lock On
    private void HandleLockOnInput() {
        //Is our current target dead? (If so, Unlock)
        if (player.isLockedOn) {
            if (player.playerCombatManager.currentTarget == null) {
                return;
            }
            if (player.playerCombatManager.currentTarget.isDead) {
                player.isLockedOn = false;
            }

            //Attempt to Find new Target
            //This assures us that the couroutine never runs multiple times
            if (lockOnCoroutine != null) {
                StopCoroutine(lockOnCoroutine);

                lockOnCoroutine = StartCoroutine(PlayerCamera.instance.WaitThenFindNewTarget());
            }
        }


        //Are we already locked on?
        if (lockOnInput && player.isLockedOn) {
            //Disable Lock On
            lockOnInput = false;
            PlayerCamera.instance.ClearLockOnTargets();
            player.isLockedOn = false;
            player.characterCombatManager.currentTarget = null;
            return;
        }

        if (lockOnInput && !player.isLockedOn) {
            lockOnInput = false;

            //Enable Lock On
            PlayerCamera.instance.HandleLocatingLockOnTargets();

            if (PlayerCamera.instance.nearestLockOnTarget != null) {
                //Set the target as our current target
                player.playerCombatManager.SetTarget(PlayerCamera.instance.nearestLockOnTarget);
                player.isLockedOn = true;
            }
        }
    }

    private void HandleLockOnSwitchTargetInput() {
        if (lockOnSelectLeftInput) {
            lockOnSelectLeftInput = false;

            if (player.isLockedOn) {
                PlayerCamera.instance.HandleLocatingLockOnTargets();

                if (PlayerCamera.instance.leftLockOnTarget != null) {
                    player.playerCombatManager.SetTarget(PlayerCamera.instance.leftLockOnTarget);
                }
            }
        }

        if (lockOnSelectRightInput) {
            lockOnSelectRightInput = false;

            if (player.isLockedOn) {
                PlayerCamera.instance.HandleLocatingLockOnTargets();

                if (PlayerCamera.instance.rightLockOnTarget != null) {
                    player.playerCombatManager.SetTarget(PlayerCamera.instance.rightLockOnTarget);
                }
            }
        }

    }

    private void QueueInput(ref bool queuedInput) {
        //Reset all queued inputs so only one can queue at a time
        ResetQueuedInputs();

        //TODO: Check for UI Window Being Open

        if (player.isPerformingAction || player.isJumping) {
            //Since this is passed by reference, this will set the parameterized bool to true
            queuedInput = true;

            //Attempt this new input for x amount of time
            queueInputTimer = defaultQueueInputTimer;
            InputQueueIsActive = true;
        }
    }

    private void ResetQueuedInputs() {
        queueLightAttackInput = false;
        queueHeavyAttackInput = false;
    }

    private void ProcessQueuedInput() {
        if (player.isDead) {
            return;
        }

        if(queueLightAttackInput) {
            lightAttackInput = true;
        }
        else if (queueHeavyAttackInput) {
            heavyAttackInput = true;
        }
        
    }

    private void HandleQueuedInputs() {
        if (InputQueueIsActive) {
            //While the timer is above zero, keep attempting the input
            if (queueInputTimer > 0f) {
                queueInputTimer -= Time.deltaTime;
                ProcessQueuedInput();
            }
            else {
                ResetQueuedInputs();
                InputQueueIsActive = false;
                queueInputTimer = 0f;
            }
        }
    }

}
