using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocomotionManager : CharacterLocomotionManager
{
    PlayerManager player;
    private Vector3 moveDirection;
    [SerializeField] float walkingSpeed = 2f;
    [SerializeField] float runningSpeed = 5f;
    //Values taken from Input Manager
    public float verticalMovement;
    public float horizontalMovement;
    public float moveAmount;

    // Update is called once per frame
    void Update()
    {
        HandleAllMovement();
    }

    public void HandleAllMovement() {
        //Grounded Movement
        HandleGroundedMovement();

        //Aerial Movement
    }

    protected override void Awake() {
        base.Awake();

        player = GetComponent<PlayerManager>();
    }

    private void HandleGroundedMovement() {
        GetVerticalAndHorizontalInputs();

        //Our movement direction is based on our camera's facing perspective and our movement inputs
        moveDirection = PlayerCamera.instance.transform.forward * verticalMovement;
        moveDirection = moveDirection + PlayerCamera.instance.transform.right * horizontalMovement;
        moveDirection.Normalize();
        moveDirection.y = 0;

        if (PlayerInputManager.instance.moveAmount > 0.5f) {
            //Move at a running speed
            player.characterController.Move(moveDirection * runningSpeed * Time.deltaTime);
        }
        else if (PlayerInputManager.instance.moveAmount <= 0.5f) {
            //Move at a walking speed
            player.characterController.Move(moveDirection * walkingSpeed * Time.deltaTime);
        }
    }

    private void GetVerticalAndHorizontalInputs() {
        verticalMovement = PlayerInputManager.instance.verticalInput;
        horizontalMovement = PlayerInputManager.instance.horizontalInput;

        //Clamp the movements when we add animations
    }
}
