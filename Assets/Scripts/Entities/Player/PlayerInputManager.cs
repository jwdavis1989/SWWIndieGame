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
    [SerializeField] Vector2 movementInput;
    public float horizontalInput;
    public float verticalInput;
    public float moveAmount;

    [Header("Player Action Input")]
    [SerializeField] bool dodgeInput = false;
    [SerializeField] bool sprintInput = false;
    [SerializeField] bool jumpInput = false;
    [SerializeField] Vector2 mouseWheelInput;
    [SerializeField] float mouseWheelVerticalInput;
    [SerializeField] float prevMouseWheelVerticalInput;

    [Header("Camera Movement Input")]
    [SerializeField] Vector2 cameraInput;
    public float cameraHorizontalInput;
    public float cameraVerticalInput;


    //Start is called before the first frame update
    void Start() {
        //Has to happen before we disable the instance
        DontDestroyOnLoad(gameObject);

        //When the scene changes, run this logic
        //This is to do with subscribing and might require research
        SceneManager.activeSceneChanged += OnSceneChange;
        
        instance.enabled = false;
    }

    //Update is called once per frame
    void Update()
    {
        HandleAllInputs();
    }

    private void HandleAllInputs() {
        HandleMovementInput();
        HandleCameraMovementInput();
        HandleDodgeInput();
        HandleSprintInput();
        HandleJumpInput();
        HandleMouseWheelInput();
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

            //Holding the input sets the bool to true
            playerControls.PlayerActions.Sprint.performed += i => sprintInput = true;
            //Releasing sets the sprint bool to false
            playerControls.PlayerActions.Sprint.canceled += i => sprintInput = false;

            //Weapon Quick-Changing
            playerControls.PlayerActions.NextWeapon.performed += i => mouseWheelInput = i.ReadValue<Vector2>();

            //Debug Buttons
            playerControls.PlayerActions.DebugTestAddWeapon.performed += i => player.DebugAddWeapon();
            playerControls.PlayerActions.DebugTeleportToAlecDev.performed += i => SceneManager.LoadSceneAsync(2);
            playerControls.PlayerActions.DebugTeleportToJacobDev.performed += i => SceneManager.LoadSceneAsync(3);

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
        }
        //Otherwise, we're in a menu so disable player controls
        //This is so our player can't move around if we enter things like a character creation menu, etc
        else {
            instance.enabled = false;
        }
    }

    private void OnDestroy() {
        //If we destroy this object, we unsubcribe from this event
        //This is to do with subscribing and might require research
        SceneManager.activeSceneChanged -= OnSceneChange;
    }

    //Movement
    private void HandleMovementInput() {
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
        //If not locked on
        //We pass 0 for horizontal because we're not locked on, as we always rotate to face the way we walk.
        player.playerAnimationManager.UpdateAnimatorMovementParameters(0, moveAmount, player.isSprinting);

        //If locked on
        //If we are locked on, we want to pass the horizontal and vertical, probably
    }
    private void HandleMouseWheelInput()
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
            
            //Perform the dodge
            player.playerLocomotionManager.AttemptToPerformDodge();
        }
    }

    private void HandleSprintInput() {
        if (sprintInput) {
            player.playerLocomotionManager.HandleSprinting();
            //Debug.Log("Attempt to Sprint in InputManager.");
        }
        else {
            //If adding multiplayer, this will instead use the following:
            //player.playerNetworkManager.isSprinting = false;
            player.playerLocomotionManager.characterManager.isSprinting = false;
        }
    }

    private void HandleJumpInput() {
        if (jumpInput) {
            jumpInput = false;

            //If we have a UI window open, simply return without doing anything

            //Attempt to perform a jump
            player.playerLocomotionManager.AttemptToPerformJump();
        }
    }
}
