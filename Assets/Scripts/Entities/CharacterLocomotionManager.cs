using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterLocomotionManager : MonoBehaviour
{
    public CharacterManager character;

    [Header("Ground Check & Jumping")]
    [SerializeField] protected float gravityForce = -5.55f;
    [SerializeField] LayerMask groundLayer;

    //Offsets the ground check to the feet of the character
    //[SerializeField] float groundCheckYOffset = -0.5f;
    [SerializeField] float groundCheckSphereRadius = 0.1f;
    //Upward or Downward force applied to player. e.g. Gravity or jumping.
    [SerializeField] protected Vector3 yVelocity;

    //Gravity
    [SerializeField] protected float groundedYVelocity = -20f;
    //The force at which our character begins to fall when ungrounded. This value increases over time when in the air. 
    [SerializeField] protected float fallStartYVelocity = -7f;

    protected bool fallingVelocityHasBeenSet = false;
    protected float inAirTimer = 0f;

    [Header("Debug")]
    bool debugGroundCollisionSphere = false;

    protected virtual void Awake()
    {
        //DontDestroyOnLoad(this);
        character = GetComponent<CharacterManager>();
    }

    protected virtual void Update()
    {
        HandleGroundCheck();

        if (character.isGrounded)
        {
            //If we are not attempting to jump or move upward
            if (yVelocity.y < 0)
            {
                inAirTimer = 0;
                fallingVelocityHasBeenSet = false;
                yVelocity.y = groundedYVelocity;
            }

            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            //  Floating bug was occuring in this case, as yVelocity.y = Mathf.Sqrt(jumpHeight * -2 * gravityForce) and is then applied repeatedly.
            //  Problem has been solved by noticing that jumping velocity is >4, while the float glitch is ~0.46, so checking for a sub-1 velocity 
            //  fixes the floating glitch!
            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            if (yVelocity.y < 1)
            {
                yVelocity.y = groundedYVelocity;
            }
        }
        else
        {
            //If not jumping, and falling velocity has not been set
            if (!character.isJumping && !fallingVelocityHasBeenSet)
            {
                fallingVelocityHasBeenSet = true;
                yVelocity.y = fallStartYVelocity;
            }

            //Air Timer
            inAirTimer += Time.deltaTime;
            character.animator.SetFloat("InAirTimer", inAirTimer);

            //Increases gravity's effect over time
            yVelocity.y += (gravityForce * Time.deltaTime);
        }

        //Apply downward force to character
        if (!character.isBoosting)
        {
            character.characterController.Move(yVelocity * Time.deltaTime);
        }
    }

    protected void HandleGroundCheck()
    {
        //Workaround Version
        // Vector3 groundVector = new Vector3(character.transform.position.x, character.transform.position.y + groundCheckYOffset, character.transform.position.z);
        // character.isGrounded = Physics.CheckSphere(groundVector, groundCheckSphereRadius, groundLayer);

        //Intended Version
        character.isGrounded = Physics.CheckSphere(character.transform.position, groundCheckSphereRadius, groundLayer);
    }

    //Draws ground check sphere in editor
    protected void OnDrawGizmosSelected()
    {
        //Bugged to hell and back
        if (debugGroundCollisionSphere)
        {
            Gizmos.DrawSphere(character.transform.position, groundCheckSphereRadius);
        }
        //Better work-around
        // Vector3 groundVector = new Vector3(character.transform.position.x, character.transform.position.y + groundCheckYOffset, character.transform.position.z);
        // Gizmos.DrawSphere(groundVector, groundCheckSphereRadius);

        //Temporary work-around based on player's starting position in TitleScreen scene
        // Vector3 tempVector = new Vector3(0, -0.5f, 0);
        // Gizmos.DrawSphere(tempVector, groundCheckSphereRadius);
    }

    public void EnableCanRotate()
    {
        character.canRotate = true;
    }

    public void DisableCanRotate()
    {
        character.canRotate = false;
    }

}
