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

        //Comment this out for Multiplayer
        PlayerCamera.instance.player = this;
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
        // if (!IsOwner) {
        //     return;
        // }
        base.LateUpdate();

        PlayerCamera.instance.HandleAllCameraActions();
    }

}
