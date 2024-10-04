using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : CharacterManager
{

    [HideInInspector] public PlayerLocomotionManager playerLocomotionManager;
    //Turn on if adding multiplayer
    //[HideInInspector] public PlayerNetworkManager playerNetworkManager;
    [HideInInspector] public PlayerStatsManager playerStatsManager;

    protected override void Awake() {
        base.Awake();

        //Do more stuff, only for the player
        playerLocomotionManager = GetComponent<PlayerLocomotionManager>();
        
        //Turn on if adding multiplayer
        //playerNetworkManager = GetComponent<PlayerNetworkManager>();
        PlayerInputManager.instance.player = this;
        playerStatsManager = GetComponent<PlayerStatsManager>();
        
        //Remove this when adding multiplayer. 
        //This will also be removed when saving and loading are added!
        //This should be called on any sort of increase to endurance stat as well, like level ups or inventions
        //{
            playerStatsManager.maxStamina = playerStatsManager.CalculateStaminaBasedOnEnduranceLevel(playerStatsManager.endurance);
            playerStatsManager.currentStamina = playerStatsManager.maxStamina;

            PlayerUIManager.instance.playerUIHudManager.SetMaxStaminaValue(playerStatsManager.maxStamina);
            PlayerUIManager.instance.playerUIHudManager.SetNewStaminaValue(playerStatsManager.maxStamina);
        //}

    }

    public void Start() {

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

        //Remove when adding multiplayer
        //Debug.Log("playerStatsManager.currentStamina? " + playerStatsManager.currentStamina);
        PlayerUIManager.instance.playerUIHudManager.SetNewStaminaValue(playerStatsManager.currentStamina);

        //Regenerates your stamina
        playerStatsManager.RegenerateStamina();

    }

    //Uncomment for Multiplayer
    // override OnNetworkSpawn() {
    //     if (IsOwner) {
    //         PlayerCamera.instance.player = this;
    //         PlayerInputManager.instance.player = this;

    //         PlayerNetworkManager.currentStamina.OnValueChanged += PlayerUIManager.instance.playerUIHudManager.SetNewStaminaValue;

               //This will be moved when saving/loading is added
               //PlayerNetworkManager.maxStamina.Value = PlayerStatsManager.CalculateStaminaBasedOnEnduranceLevel(playerNetworkManager.endurance);
               //PlayerUIManager.instance.playerUIHudManager.SetMaxStaminaValue(PlayerNetworkManager.maxStamina.Value);
               //playerNetworkManager.currentStamina.Value = PlayerStatsManager.CalculateStaminaBasedOnEnduranceLevel(playerNetworkManager.endurance.Value);

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
