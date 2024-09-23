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

        //Handle all movement every frame
        playerLocomotionManager.HandleAllMovement();
    }

}
