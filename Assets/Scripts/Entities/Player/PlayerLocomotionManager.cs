using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocomotionManager : CharacterLocomotionManager
{
    PlayerManager player;
    //Values taken from Input Manager
    [HideInInspector] public float verticalMovement;
    [HideInInspector] public float horizontalMovement;
    //[HideInInspector] public float moveAmount; //Currently does nothing, might remove. See PlayerInputManager.instance.moveAmount for correct value.
    
    [Header("Movement Settings")]
    private Vector3 moveDirection;
    private Vector3 targetRotationDirection;
    [SerializeField] float walkingSpeed = 2f;
    [SerializeField] float runningSpeed = 5f;
    [SerializeField] float sprintingSpeed = 6.5f;
    [SerializeField] float rotationSpeed = 15f;
    [SerializeField] float sprintingStaminaCost = 6f;
    [SerializeField] float staminaTickTimer = 0.1f;

    [Header("Dodge")]
    private Vector3 rollDirection;
    public GameObject forceFieldGraphic;

    [Header("Sprinting")]
    //IF we add multiplayer, move this to the CharacterNetworkManager
    public bool isSprinting = false;
    

    // Update is called once per frame
    void Update() {
        HandleAllMovement();
    }

    public void HandleAllMovement() {
        HandleGroundedMovement();
        HandleRotation();

        //Aerial Movement
    }

    protected override void Awake() {
        base.Awake();

        player = GetComponent<PlayerManager>();
    }

    private void HandleGroundedMovement() {
        if (!player.canMove) {
            return;
        }

        GetVerticalAndHorizontalInputs();

        //Our movement direction is based on our camera's facing perspective and our movement inputs
        moveDirection = PlayerCamera.instance.transform.forward * verticalMovement;
        moveDirection = moveDirection + PlayerCamera.instance.transform.right * horizontalMovement;
        moveDirection.Normalize();
        moveDirection.y = 0;

        if(isSprinting) {
            player.characterController.Move(moveDirection * sprintingSpeed * Time.deltaTime);
        }
        else {
            if (PlayerInputManager.instance.moveAmount > 0.5f) {
                //Move at a running speed
                player.characterController.Move(moveDirection * runningSpeed * Time.deltaTime);
            }
            else if (PlayerInputManager.instance.moveAmount <= 0.5f) {
                //Move at a walking speed
                player.characterController.Move(moveDirection * walkingSpeed * Time.deltaTime);
            }
        }

    }

    private void HandleRotation() {
        if (!player.canRotate) {
            return;
        }

        targetRotationDirection = Vector3.zero;
        targetRotationDirection = PlayerCamera.instance.cameraObject.transform.forward * verticalMovement;
        targetRotationDirection = targetRotationDirection + PlayerCamera.instance.cameraObject.transform.right * horizontalMovement;
        targetRotationDirection.Normalize();
        targetRotationDirection.y = 0;

        if (targetRotationDirection == Vector3.zero) {
            targetRotationDirection = transform.forward;
        }

        Quaternion newRotation = Quaternion.LookRotation(targetRotationDirection);
        Quaternion targetRotation = Quaternion.Slerp(transform.rotation, newRotation, rotationSpeed * Time.deltaTime);
        transform.rotation = targetRotation;
    }

    private void GetVerticalAndHorizontalInputs() {
        verticalMovement = PlayerInputManager.instance.verticalInput;
        horizontalMovement = PlayerInputManager.instance.horizontalInput;

        //Clamp the movements when we add animations
    }
    
    public void HandleSprinting() {
        if (player.isPerformingAction) {
            isSprinting = false;
        }
        //If we're out of stamina, set sprinting to false
        if (player.playerStatsManager.currentStamina <= 0) {
            isSprinting = false;
            return;
        }

        // If we are moving, set sprinting to true
        if (PlayerInputManager.instance.moveAmount > 0) {
            isSprinting = true;
        }
        //If stationary, set it to false
        else {
            isSprinting = false;
        }

        if (isSprinting) {
            player.playerStatsManager.currentStamina -= sprintingStaminaCost * Time.deltaTime;
        }

    }
    public void AttemptToPerformDodge() {
        //Debug.Log("AttemptToPerformDodge Called");
        if (player.isPerformingAction) {
            return;
        }
        //Roll if moving before
        if (PlayerInputManager.instance.moveAmount > 0) {
            rollDirection = PlayerCamera.instance.cameraObject.transform.forward * verticalMovement;
            rollDirection += PlayerCamera.instance.cameraObject.transform.right * horizontalMovement;
            rollDirection.y = 0;
            rollDirection.Normalize();

            Quaternion playerRotation = Quaternion.LookRotation(rollDirection);
            player.transform.rotation = playerRotation;

            //Debug.Log("Roll Attempted!");

            //Activate Force Field Graphic
            forceFieldGraphic.SetActive(true);

            //Perform a Roll Animation here
            //Look to episode 6 for animation tutorial for this part
        }
        //Backstep if stationary before
        else {
            //Debug.Log("Backstep Attempted!");

            //Perform a Backstep Animation here
            //Look to episode 6 for animation tutorial for this part
        }
    }

    public void RegenerateStamina() {

        if (isSprinting) {
            return;
        }

        if (player.isPerformingAction) {
            return;
        }

        player.playerStatsManager.staminaRegenerationTimer += Time.deltaTime;

        if (player.playerStatsManager.staminaRegenerationTimer >= player.playerStatsManager.staminaRegenerationDelay) {
            if (player.playerStatsManager.currentStamina < player.playerStatsManager.maxStamina) {
                staminaTickTimer += Time.deltaTime;

                if (staminaTickTimer >= 0.1) {
                    staminaTickTimer = 0;
                    player.playerStatsManager.currentStamina += player.playerStatsManager.staminaRegenAmount;
                }
            }
        }
    }
}
