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
    public CharacterController characterController;

    [Header("Flags")]
    public bool isPerformingAction = false;
    public bool applyRootMotion = false;
    public bool canRotate = true;
    public bool canMove = true;

    protected virtual void Awake() {
        DontDestroyOnLoad(this);

        characterController = GetComponent<CharacterController>();
    }

    protected virtual void Update() {
        //Uncomment if adding networked multiplayer
        // if (IsOwner) {
        //     characterNetworkManager.networkPosition.Value = transform.position;
        //     characterNetworkManager.networkRotation.Value = transform.rotation;
        // }
        // else {
        //Position
        //     transform.position = Vector3.SmoothDamp(
        //         transform.position, characterNetworkManager.networkPosition.Value, 
        //         ref characterNetworkManager.networkPositionVelocity, 
        //         characterNetworkManager.networkPositionSmoothTime)

        //Rotation
        //         transform.rotation = Quaternion.Slerp(transform.rotation, characterNetworkManager.networkRotation.Value, characterNetworkManager.networkRotationSmoothTime);
        // }
    }

    protected virtual void LateUpdate() {
        
    }

    //Uncomment for multiplayer, then remove the version in PlayerLocomotionManager
    // public virtual void RegenerateStamina() {
    //     if (!IsOwner) {
    //         return;
    //     }

    //     if (characterNetworkManager.isSprinting.Value) {
    //         return;
    //     }

    //     if (isPerformingAction) {
    //         return;
    //     }
    // }
}
