using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Unity.Netcode;

//If creating online coop, replace public class CharacterManager : MonoBehaviour with the following line:
//public class CharacterManager : NetworkBehavior
public class CharacterManager : MonoBehaviour
{
    //CharacterNetworkManager characterNetworkManager;
    [HideInInspector] public CharacterController characterController;
    [HideInInspector] public Animator animator;
    [HideInInspector] public CharacterStatsManager characterStatsManager;
    [HideInInspector] public CharacterEffectsManager characterEffectsManager;
    [HideInInspector] public CharacterAnimatorManager characterAnimatorManager;
    [HideInInspector] public CharacterSoundFXManager characterSoundFXManager;

    [Header("Status")]
    public bool isDead = false;

    [Header("Flags")]
    public bool isPlayer = false;
    public bool isPerformingAction = false;
    public bool isJumping = false;
    public bool isGrounded = true;
    public bool isBoosting = false;
    public bool applyRootMotion = false;
    public bool canRotate = true;
    public bool canMove = true;
    public bool isSprinting = false;

    protected virtual void Awake() {
        DontDestroyOnLoad(this);

        characterController = GetComponent<CharacterController>();
        characterStatsManager = GetComponent<CharacterStatsManager>();
        animator = GetComponent<Animator>();
        characterEffectsManager = GetComponent<CharacterEffectsManager>();
        characterAnimatorManager = GetComponent<CharacterAnimatorManager>();
        characterSoundFXManager = GetComponent<CharacterSoundFXManager>();
    }

    protected virtual void Start() {
        IgnoreMyOwnColliders();
    }

    protected virtual void Update() {
        animator?.SetBool("isGrounded", isGrounded);
    }

    protected virtual void LateUpdate() {
        
    }


    public virtual IEnumerator ProcessDeathEvent(bool manuallySelectDeathAnimation = false) {
        if (isPlayer) {
            characterStatsManager.currentHealth = 0;
            isDead = true;

            //Reset any Flags here that need to be reset
            //Todo: Add these later

            //If not grounded, play an aerial death animation

            if (!manuallySelectDeathAnimation) {
                //Could change this to choose a random death animation in the future if we wanted to.
                characterAnimatorManager.PlayTargetActionAnimation("Dead_01", true);
            }
        }

        //Play Death SFX
        //characterSoundFXManager.audioSource.PlayOneShot(WorldSoundFXManager.instance.deathSFX);

        yield return new WaitForSeconds(5);

        if (!isPlayer) {
            //If monster: Award players with Gold or items
            
        }

        //Disable Character

    }

    public virtual void ReviveCharacter() {
        //
    }

    protected virtual void IgnoreMyOwnColliders() {
        Collider characterControllerCollider = GetComponent<Collider>();
        Collider[] damageableCharacterColliders = GetComponentsInChildren<Collider>();
        List<Collider> ignoreColliders = new List<Collider>();

        //Add all limb damage collider to the list to ignore
        foreach (var collider in damageableCharacterColliders) {
            ignoreColliders.Add(collider);
        }

        //Adding primary collider from character controller to the list to ignore
        ignoreColliders.Add(characterControllerCollider);

        //Go through each collider in the list, and ignore collision with each other
        foreach (var collider in ignoreColliders) {
            foreach (var otherCollider in ignoreColliders) {
                Physics.IgnoreCollision(collider, otherCollider);
            }
        }
    }

}
