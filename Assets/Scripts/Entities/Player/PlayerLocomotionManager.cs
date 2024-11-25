using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocomotionManager : CharacterLocomotionManager
{
    PlayerManager player;
    public GameObject rightBoosters;
    // public GameObject rightForwardBoosters;
    // public GameObject rightBackwardBoosters;
    public GameObject leftBoosters;
    // public GameObject leftForwardBoosters;
    // public GameObject leftBackwardBoosters;
    public GameObject backBoosters;
    public GameObject airDashBoosters;


    [HideInInspector] public CharacterManager characterManager;
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

    [Header("Jump")]
    [SerializeField] float jumpHeight = 2f;   
    [SerializeField] float jumpForwardSpeed = 5f;
    [SerializeField] float freeFallSpeed = 2f;
    private Vector3 jumpDirection;


    [Header("Dodge")]
    private Vector3 rollDirection;
    [SerializeField] float airBoostSpeed = 15f;
    public GameObject forceFieldGraphic;

    

    // Update is called once per frame
    protected override void Update() {
        base.Update();
        HandleAllMovement();

        //Cancelled forcefield cancel
        // if (!player.isPerformingAction && forceFieldGraphic.activeSelf) {
        //     forceFieldGraphic.SetActive(false);
        // }
    }

    public void HandleAllMovement() {
        HandleGroundedMovement();
        HandleRotation();
        HandleBackBoosterJets();

        //Aerial Movement
        HandleJumpingMovement();
        HandleFreeFallMovement();
    }

    protected override void Awake() {
        base.Awake();

        player = GetComponent<PlayerManager>();
        characterManager = GetComponent<CharacterManager>();
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

        if(characterManager.isSprinting) {
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

    private void HandleJumpingMovement() {
        if (player.isJumping) {
            player.characterController.Move(jumpDirection * jumpForwardSpeed * Time.deltaTime);
        }
    }

    public void HandleFreeFallMovement() {
        if (!player.isGrounded) {
            Vector3 freeFallDirection;
            freeFallDirection = PlayerCamera.instance.transform.forward * PlayerInputManager.instance.verticalInput;
            freeFallDirection += PlayerCamera.instance.transform.right * PlayerInputManager.instance.horizontalInput;

            freeFallDirection.y = 0;
            player.characterController.Move(freeFallDirection * freeFallSpeed * Time.deltaTime);


            //HandleOmniJumpJets(horizontalMovement, verticalMovement);
            //HandleOmniJumpJets(freeFallDirection.x, freeFallDirection.z);

            //Debug.Log(player.gameObject.transform.eulerAngles.y);
            //Facing Forward
            if ((player.gameObject.transform.eulerAngles.y >= 270 && player.gameObject.transform.eulerAngles.y <= 360) 
            || ( player.gameObject.transform.eulerAngles.y >= 0 && player.gameObject.transform.eulerAngles.y < 90)) {
                if (freeFallDirection.x > 0) {
                    DisableJumpJets("Right");
                    EnableJumpJets("Left");
                }
                else if (freeFallDirection.x < 0) {
                    DisableJumpJets("Left");
                    EnableJumpJets("Right");
                }
                else {
                    EnableJumpJets("Right");
                    EnableJumpJets("Left");
                }
            }
            //Facing Backward
            else {
                if (freeFallDirection.x < 0 && !characterManager.isBoosting) {
                DisableJumpJets("Right");
                EnableJumpJets("Left");
                }
                else if (freeFallDirection.x > 0 && !characterManager.isBoosting) {
                    DisableJumpJets("Left");
                    EnableJumpJets("Right");
                }
                else {
                    EnableJumpJets("Right");
                    EnableJumpJets("Left");
                }
            }
        }
        else {
            DisableJumpJets("Both");
            airDashBoosters.SetActive(false);
            //ResetOmniJumpJets();
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
            characterManager.isSprinting = false;
        }
        //If we're out of stamina, set sprinting to false
        if (player.playerStatsManager.currentStamina <= 0) {
            characterManager.isSprinting = false;
            return;
        }

        // If we are moving, set sprinting to true
        if (PlayerInputManager.instance.moveAmount > 0) {
            characterManager.isSprinting = true;

            //Play Booster Fire Sound Effect
            player.characterSoundFXManager.PlaySprintBoostSoundFX();
        }
        //If stationary, set it to false
        else {
            characterManager.isSprinting = false;
        }

        if (characterManager.isSprinting) {
            player.playerStatsManager.currentStamina -= player.playerStatsManager.sprintingStaminaCost * Time.deltaTime;
            player.playerStatsManager.ResetStaminaRegenTimer();
        }

    }
    public void AttemptToPerformDodge() {
        if (player.isPerformingAction) {
            return;
        }

        if (player.playerStatsManager.currentStamina <= 0) {
            return;
        }

        //Roll if moving before
        if (PlayerInputManager.instance.moveAmount > 0) {
            Quaternion playerRotation;
            if (player.isGrounded) {
                //Set roll direction
                rollDirection = PlayerCamera.instance.cameraObject.transform.forward * verticalMovement;
                rollDirection += PlayerCamera.instance.cameraObject.transform.right * horizontalMovement;
                rollDirection.y = 0;
                rollDirection.Normalize();

                //Set player facing
                playerRotation = Quaternion.LookRotation(rollDirection);
                player.transform.rotation = playerRotation;

                //Play roll animation
                player.playerAnimationManager.PlayTargetActionAnimation("Roll_Forward_01", true);
            }
            else {
                //Boosting flag
                player.isBoosting = true;

                //Activate booster Particle Effects
                airDashBoosters.SetActive(true);
                EnableJumpJets("Both");

                //Set boost direction
                jumpDirection = PlayerCamera.instance.cameraObject.transform.forward * PlayerInputManager.instance.verticalInput;
                jumpDirection += PlayerCamera.instance.cameraObject.transform.right * PlayerInputManager.instance.horizontalInput;
                jumpDirection.y = 0;
                jumpDirection.Normalize();

                //Movement caused by boosting
                player.characterController.Move(jumpDirection * Time.deltaTime * airBoostSpeed);

                //Set player facing
                playerRotation = Quaternion.LookRotation(jumpDirection);
                player.transform.rotation = playerRotation;

                //Play boost animation
                player.playerAnimationManager.PlayTargetActionAnimation("Boost_Forward_01", true, true, false, false);
            }

            //Activate Force Field Graphic
            //forceFieldGraphic.SetActive(true);

            //Set Stamina regen delay to 0
            player.playerStatsManager.ResetStaminaRegenTimer();

            //Subtract Stamina for roll or airdash
            player.playerStatsManager.currentStamina -= player.playerStatsManager.dodgeStaminaCost;
        }
        //Backstep if stationary before
        else {
            //Perform a Backstep Animation here
            if (player.isGrounded) {
                //Play Backstep Animation
                player.playerAnimationManager.PlayTargetActionAnimation("Back_Step_01", true, true);
                
                //Subtract Stamina for backstep
                player.playerStatsManager.currentStamina -= player.playerStatsManager.dodgeStaminaCost;
            }
        }
    }

    public void AttemptToPerformJump() {
        //If performing any action, we don't want to allow a jump (Will change when combat is added)
        if (player.isPerformingAction) {
            return;
        }

        //If out of stamina, we can't jump
        if (player.playerStatsManager.currentStamina <= 0) {
            return;
        }

        //If already jumping, we can't jump
        if (player.isJumping) {
            return;
        }

        //If not on the ground, we can't jump
        if (!player.isGrounded) {
            return;
        }

        //If using a 2-handed weapon, play the 2h weapon jump animation, otherwise play the one handed animation
        player.playerAnimationManager.PlayTargetActionAnimation("Main_Jump_Start_01", false);

        player.isJumping = true;

        player.playerStatsManager.currentStamina -= player.playerStatsManager.jumpStaminaCost;

        jumpDirection = PlayerCamera.instance.cameraObject.transform.forward * PlayerInputManager.instance.verticalInput;
        jumpDirection += PlayerCamera.instance.cameraObject.transform.right * PlayerInputManager.instance.horizontalInput;

        jumpDirection.y = 0;

        if (jumpDirection != Vector3.zero) {
            //Sprint means full jump distance
            if (player.isSprinting) {
                jumpDirection *= 1;
            }
            //Running means half jump distance
            else if (PlayerInputManager.instance.moveAmount > 0.5) {
                jumpDirection *= 0.5f;
            }
            //Walking means quarter jump distance
            else if (PlayerInputManager.instance.moveAmount < 0.5) {
                jumpDirection *= 0.25f;
            }
        }

        if (jumpDirection.x > 0 && !characterManager.isBoosting) {
            EnableJumpJets("Left");
        }
        else if (jumpDirection.x < 0 && !characterManager.isBoosting) {
            EnableJumpJets("Right");
        }
        else {
            EnableJumpJets("Both");
        }

    }

    public void ApplyJumpingVelocity() {
        //Apply an upward velocity depending on forces in our game such as gravity
        yVelocity.y = Mathf.Sqrt(jumpHeight * -2 * gravityForce);
    }

    public void EnableJumpJets(string side) {
        switch(side) {
            case "Left":
            leftBoosters.SetActive(true);
                break;
            case "Right":
            rightBoosters.SetActive(true);
                break;
            case "Both":
            rightBoosters.SetActive(true);
            leftBoosters.SetActive(true);
                break;
        }
    }

    public void DisableJumpJets(string side) {
        switch(side) {
            case "Left":
            leftBoosters.SetActive(false);
                break;
            case "Right":
            rightBoosters.SetActive(false);
                break;
            case "Both":
                rightBoosters.SetActive(false);
                leftBoosters.SetActive(false);
                // leftForwardBoosters.SetActive(false);
                // rightForwardBoosters.SetActive(false);
                break;
        }
    }

    public void HandleBackBoosterJets() {
        if (characterManager.isSprinting && !backBoosters.activeSelf) {
            backBoosters.SetActive(true);
        }
        else if (!characterManager.isSprinting && backBoosters.activeSelf == true) {
            backBoosters.SetActive(false);
        }
    }

    // public void HandleOmniJumpJets(float horizontalMovement, float verticalMovement) {
    //     //Vector2 movement = new Vector2(horizontalMovement, verticalMovement);

    //     //Case 1: Positive horizontal - Right
    //     if (horizontalMovement > 0) {
    //         rightBoosters.SetActive(false);
    //         leftBoosters.SetActive(true);

    //         //If Moving Forward
    //         if (verticalMovement >= 0) {
    //             rightBackwardBoosters.SetActive(true);
    //             leftBackwardBoosters.SetActive(true);
    //             rightForwardBoosters.SetActive(false);
    //             leftForwardBoosters.SetActive(false);
    //         }
    //         //If Moving Backward
    //         else {
    //             rightBackwardBoosters.SetActive(false);
    //             leftBackwardBoosters.SetActive(false);
    //             rightForwardBoosters.SetActive(true);
    //             leftForwardBoosters.SetActive(true);
    //         }
    //     }
    //     //Case 2: Negative horizontal - Left
    //     else if (horizontalMovement < 0) {
    //         rightBoosters.SetActive(true);
    //         leftBoosters.SetActive(false);

    //         //If Moving Forward
    //         if (verticalMovement >= 0) {
    //             rightBackwardBoosters.SetActive(true);
    //             leftBackwardBoosters.SetActive(true);
    //             rightForwardBoosters.SetActive(false);
    //             leftForwardBoosters.SetActive(false);
    //         }
    //         //If Moving Backward
    //         else {
    //             rightBackwardBoosters.SetActive(false);
    //             leftBackwardBoosters.SetActive(false);
    //             rightForwardBoosters.SetActive(true);
    //             leftForwardBoosters.SetActive(true);
    //         }
    //     }
    //     //Case 3: Neutral horizontal
    //     else {
    //         //Reset to neutral playing field
    //         //ResetOmniJumpJets();

    //         //I like just the 2 turned on for hover mode aesthetically
    //         rightBoosters.SetActive(true);
    //         leftBoosters.SetActive(true);
    //     }
    // }

    // public void ResetOmniJumpJets() {
    //     rightBoosters.SetActive(false);
    //     leftBoosters.SetActive(false);
    //     rightForwardBoosters.SetActive(false);
    //     leftForwardBoosters.SetActive(false);
    //     rightBackwardBoosters.SetActive(false);
    //     leftBackwardBoosters.SetActive(false);
    // }
}
