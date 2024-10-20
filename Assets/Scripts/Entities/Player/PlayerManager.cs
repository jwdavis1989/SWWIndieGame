using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerManager : CharacterManager
{

    [HideInInspector] public PlayerLocomotionManager playerLocomotionManager;
    //Turn on if adding multiplayer
    //[HideInInspector] public PlayerNetworkManager playerNetworkManager;
    [HideInInspector] public PlayerStatsManager playerStatsManager;
    [HideInInspector] public PlayerAnimationManager playerAnimationManager;
    public GameObject mainHandWeaponAnchor;
    public GameObject offHandWeaponAnchor;

    protected override void Awake() {
        base.Awake();

        //Do more stuff, only for the player
        playerLocomotionManager = GetComponent<PlayerLocomotionManager>();
        
        //Turn on if adding multiplayer
        //playerNetworkManager = GetComponent<PlayerNetworkManager>();
        PlayerInputManager.instance.player = this;
        WorldSaveGameManager.instance.player = this;
        playerStatsManager = GetComponent<PlayerStatsManager>();

        playerAnimationManager = GetComponent<PlayerAnimationManager>();
        
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

    public void SaveGameDataToCurrentCharacterData(ref CharacterSaveData currentCharacterData) {
        currentCharacterData.sceneIndex = SceneManager.GetActiveScene().buildIndex;
        currentCharacterData.characterName = playerStatsManager.characterName;
        currentCharacterData.xPosition = transform.position.x;
        currentCharacterData.yPosition = transform.position.y;
        currentCharacterData.zPosition = transform.position.z;

        //Add Weapon Arsenal Data later
        currentCharacterData.weapons = WeaponsController.instance.GetCurrentWeapons();
        currentCharacterData.indexOfCurrentlyEquippedWeapon = WeaponsController.instance.indexOfCurrentlyEquippedWeapon;
    }

    public void LoadGameFromCurrentCharacterData(ref CharacterSaveData currentCharacterData) {
        currentCharacterData.characterName = playerStatsManager.characterName;
        Vector3 myPosition = new Vector3(currentCharacterData.xPosition, currentCharacterData.yPosition, currentCharacterData.zPosition);
        transform.position = myPosition;

        //Add Weapon Arsenal Data Loading here later
        WeaponsController.instance.indexOfCurrentlyEquippedWeapon = currentCharacterData.indexOfCurrentlyEquippedWeapon;
        WeaponsController.instance.setCurrentWeapons(currentCharacterData.weapons);
        AttachCurrentlyEquippedWeaponObjectsToHand();
    }

    public void DebugAddWeapon() {
        WeaponsController.instance.AddWeaponToCurrentWeapons(WeaponType.Wrench);
    }

    public void AttachCurrentlyEquippedWeaponObjectsToHand() {
        //For each weapon in our currentlyOwnedWeapons
        // foreach (GameObject Weapon in WeaponsController.instance.currentlyOwnedWeapons) {
        //     //Turn object into child of weapon anchor point
        //     //Somehow turn into child, needs research
        // }

        //WeaponsController.instance.currentlyOwnedWeapons[WeaponsController.instance.indexOfCurrentlyEquippedWeapon].SetActive(true);
    }

    public void ChangeCurrentlyEquippedWeaponObject(int newActiveIndex) {
        //Turn off old weapon
        WeaponsController.instance.currentlyOwnedWeapons[WeaponsController.instance.indexOfCurrentlyEquippedWeapon].SetActive(false);

        //Tell weaponcontroller what its new weapon index is
        WeaponsController.instance.ChangeWeapon(newActiveIndex);

        //Turn on new weapon
        WeaponsController.instance.currentlyOwnedWeapons[newActiveIndex].SetActive(true);
    }
    
}
