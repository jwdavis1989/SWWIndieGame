using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : CharacterManager
{
    PlayerLocomotionManager playerLocomotionManager;

    protected override void Awake() {
        base.Awake();

        //Do more stuff, only for the player
        playerLocomotionManager = GetComponent<PlayerLocomotionManager>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        //Uncomment if adding networked coop
        //If we don't own this gameobject, we do not control or edit it
        // if (!IsOwner) {
        //     return;
        // }

        //Handle all movement every frame
        playerLocomotionManager.HandleAllMovement();
    }

    //Uncomment for Multiplayer
    // override OnNetworkSpawn() {
    //     if (IsOwner) {
    //         PlayerCamera.instance.player = this;
    //     }
    // }

    protected override void LateUpdate() {
        //Uncomment for Multiplayer
        //Also make sure to unset the PlayerCamera's player prefab in the unity editor
        // if (!IsOwner) {
        //     return;
        // }
        base.LateUpdate();

        PlayerCamera.instance.HandleAllCameraActions();
    }

}
