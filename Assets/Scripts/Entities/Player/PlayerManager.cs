using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
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

    [Header("Debug Menu")]
    [SerializeField] bool respawnCharacter = false;

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
        isPlayer = true;

    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        //Handle all movement every frame
        playerLocomotionManager.HandleAllMovement();

        //Update UI Resources
        PlayerUIManager.instance.playerUIHudManager.SetNewHealthValue(playerStatsManager.currentHealth);
        PlayerUIManager.instance.playerUIHudManager.SetNewStaminaValue(playerStatsManager.currentStamina);

        //Regenerates your stamina
        playerStatsManager.RegenerateStamina();

        DebugMenu();
    }

    protected override void LateUpdate() {
        base.LateUpdate();

        PlayerCamera.instance.HandleAllCameraActions();
    }

    public override IEnumerator ProcessDeathEvent(bool manuallySelectDeathAnimation = false)
    {
        if (isPlayer) {
            PlayerUIManager.instance.playerUIPopUpManager.SendYouDiedPopUp();
        }

        return base.ProcessDeathEvent(manuallySelectDeathAnimation);
    }

    public override void ReviveCharacter() {
        base.ReviveCharacter();

        isDead = false;
        playerStatsManager.currentHealth = playerStatsManager.maxHealth;
        playerStatsManager.currentStamina = playerStatsManager.maxStamina;
        

        //Play Rebirth Effects here

        //Reset player to default empty state
        playerAnimationManager.PlayTargetActionAnimation("Empty", false);

    }

    public void SaveGameDataToCurrentCharacterData(ref CharacterSaveData currentCharacterData) {
        //File Name
        currentCharacterData.characterName = playerStatsManager.characterName;

        //Positional Data
        currentCharacterData.sceneIndex = SceneManager.GetActiveScene().buildIndex;
        currentCharacterData.xPosition = transform.position.x;
        currentCharacterData.yPosition = transform.position.y;
        currentCharacterData.zPosition = transform.position.z;

        //Attributes
        currentCharacterData.fortitude = playerStatsManager.fortitude;
        currentCharacterData.endurance = playerStatsManager.endurance;

        //Resources
        currentCharacterData.currentHealth = playerStatsManager.currentHealth;
        currentCharacterData.currentStamina = playerStatsManager.currentStamina;

        //Add Weapon Arsenal Data later
        currentCharacterData.weapons = PlayerWeaponManager.instance.GetCurrentWeapons();
        currentCharacterData.indexOfEquippedWeapon = PlayerWeaponManager.instance.indexOfEquippedWeapon;
        currentCharacterData.indexOfEquippedSpecialWeapon = PlayerWeaponManager.instance.indexOfEquippedSpecialWeapon;
    }

    public void LoadGameFromCurrentCharacterData(ref CharacterSaveData currentCharacterData) {
        //File Name
        currentCharacterData.characterName = playerStatsManager.characterName;

        //Positional Data
        Vector3 myPosition = new Vector3(currentCharacterData.xPosition, currentCharacterData.yPosition, currentCharacterData.zPosition);
        transform.position = myPosition;

        //Attributes
        playerStatsManager.fortitude = currentCharacterData.fortitude;
        playerStatsManager.endurance = currentCharacterData.endurance;

        //Resources
        //Health
        playerStatsManager.maxHealth = playerStatsManager.CalculateHealthBasedOnfortitudeLevel(playerStatsManager.fortitude);
        playerStatsManager.currentHealth = currentCharacterData.currentHealth;

        PlayerUIManager.instance.playerUIHudManager.SetMaxHealthValue(playerStatsManager.maxHealth);
        PlayerUIManager.instance.playerUIHudManager.SetNewHealthValue(playerStatsManager.currentHealth);

        //Stamina
        playerStatsManager.maxStamina = playerStatsManager.CalculateStaminaBasedOnEnduranceLevel(playerStatsManager.endurance);
        playerStatsManager.currentStamina = currentCharacterData.currentStamina;

        PlayerUIManager.instance.playerUIHudManager.SetMaxStaminaValue(playerStatsManager.maxStamina);
        PlayerUIManager.instance.playerUIHudManager.SetNewStaminaValue(playerStatsManager.currentStamina);

        //Add Weapon Arsenal Data Loading here later
        PlayerWeaponManager.instance.indexOfEquippedWeapon = currentCharacterData.indexOfEquippedWeapon;
        PlayerWeaponManager.instance.indexOfEquippedSpecialWeapon = currentCharacterData.indexOfEquippedSpecialWeapon;
        PlayerWeaponManager.instance.setCurrentWeapons(currentCharacterData.weapons);
        //AttachCurrentlyEquippedWeaponObjectsToHand();
    }

    public void DebugAddWeapon() {
        WeaponType weaponType = (WeaponType)Random.Range(0, System.Enum.GetValues(typeof(WeaponType)).Length - 1);
        bool isSpecial = WeaponsController.instance.baseWeapons[(int)weaponType].GetComponent<WeaponScript>().isSpecialWeapon;
        PlayerWeaponManager.instance.SetAllWeaponsToInactive(isSpecial);
        PlayerWeaponManager.instance.AddWeaponToCurrentWeapons(weaponType);
        if (isSpecial)
        {
            PlayerWeaponManager.instance.indexOfEquippedSpecialWeapon = PlayerWeaponManager.instance.ownedSpecialWeapons.Count - 1;
        }
        else
            PlayerWeaponManager.instance.indexOfEquippedWeapon = PlayerWeaponManager.instance.ownedWeapons.Count - 1;
    }

    // public void AttachCurrentlyEquippedWeaponObjectsToHand() {
    //     //For each weapon in our currentlyOwnedWeapons
    //     // foreach (GameObject Weapon in WeaponsController.instance.currentlyOwnedWeapons) {
    //     //     //Turn object into child of weapon anchor point
    //     //     //Somehow turn into child, needs research
    //     // }

    //     //WeaponsController.instance.currentlyOwnedWeapons[WeaponsController.instance.indexOfCurrentlyEquippedWeapon].SetActive(true);
    // }

    public void ChangeCurrentlyEquippedWeaponObject(int newActiveIndex) {
        PlayerWeaponManager.instance.ChangeWeapon(newActiveIndex);
    }
    
    //Delete this later
    private void DebugMenu() {
        if (respawnCharacter) {
            respawnCharacter = false;
            ReviveCharacter();
        }
    }
}
