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
    [HideInInspector] public PlayerCombatManager playerCombatManager;

    public GameObject flashlight;
    public GameObject cameraflashlight;
    [SerializeField] public PlayerSoundFXManager playerSoundFXManager;

    [Header("Debug Menu")]
    [SerializeField] bool respawnCharacter = false;

    protected override void Awake() {
        isPlayer = true;
        isRotatingAttacker = true;
        base.Awake();

        //Do more stuff, only for the player
        playerLocomotionManager = GetComponent<PlayerLocomotionManager>();
        playerCombatManager = GetComponent<PlayerCombatManager>();
        playerSoundFXManager = GetComponent<PlayerSoundFXManager>();
        
        //Turn on if adding multiplayer
        //playerNetworkManager = GetComponent<PlayerNetworkManager>();
        PlayerInputManager.instance.player = this;
        WorldSaveGameManager.instance.player = this;
        playerStatsManager = GetComponent<PlayerStatsManager>();

        playerAnimationManager = GetComponent<PlayerAnimationManager>();

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
        //Display Game Over Screen
        PlayerUIManager.instance.playerUIPopUpManager.SendYouDiedPopUp();

        //Remove current lock on target if any
        if (playerCombatManager.currentTarget != null)
        {
            PlayerCamera.instance.ClearLockOnTargets();

            //Lower the Camera over time
            PlayerCamera.instance.InvokeLowerCameraHeightCoroutine();

            isLockedOn = false;
            playerCombatManager.currentTarget = null;
        }

        return base.ProcessDeathEvent(manuallySelectDeathAnimation);
    }

    public override void ReviveCharacter() {
        base.ReviveCharacter();
        canMove = true;
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
        //Tinker Components owned
        currentCharacterData.ownedComponents = TinkerComponentManager.instance.CreateSaveData();
        currentCharacterData.ownedWpnComponents = TinkerComponentManager.instance.CreateSaveData(true);
        //Journal flags
        currentCharacterData.journalFlags = JournalManager.instance.journalFlags;
        //Idea Images
        InventionManager.instance.SaveIdeas();
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

        //Weapon Arsenal Data Loading here
        PlayerWeaponManager.instance.indexOfEquippedWeapon = currentCharacterData.indexOfEquippedWeapon;
        PlayerWeaponManager.instance.indexOfEquippedSpecialWeapon = currentCharacterData.indexOfEquippedSpecialWeapon;
        PlayerWeaponManager.instance.setCurrentWeapons(currentCharacterData.weapons);
        //AttachCurrentlyEquippedWeaponObjectsToHand();
        //Load TinkerComponents
        TinkerComponentManager.instance.LoadSaveData(currentCharacterData.ownedComponents);
        TinkerComponentManager.instance.LoadSaveData(currentCharacterData.ownedWpnComponents, true);
        //Load Journal Flags
        JournalManager.instance.journalFlags = currentCharacterData.journalFlags;
    }

    public void ToggleFlashlight() {
        if (flashlight != null) {
            if (flashlight.activeSelf) {
                flashlight.SetActive(false);
            }
            else {
                flashlight.SetActive(true);
            }
        }
        else {
            Debug.Log("ERROR: Player Flashlight Instance Not Set in Editor.");
        }

        if (cameraflashlight != null) {
            if (cameraflashlight.activeSelf) {
                cameraflashlight.SetActive(false);
            }
            else {
                cameraflashlight.SetActive(true);
            }
        }
        else {
            Debug.Log("ERROR: Camera Flashlight Instance Not Set in Editor.");
        }
    }
    
    public void DebugAddWeapon() {
        WeaponScript weaponScript;
        WeaponType weaponType;
        bool isSpecial;
        
        for (int i = 0; i < System.Enum.GetValues(typeof(WeaponType)).Length - 1; i++) {
            weaponScript = WeaponsController.instance.baseWeapons[i].GetComponent<WeaponScript>();
            weaponType = weaponScript.stats.weaponType;
            isSpecial = WeaponsController.instance.baseWeapons[(int)weaponType].GetComponent<WeaponScript>().isSpecialWeapon;

            //Only add a weapon if it's a player weapon
            if (!weaponScript.stats.isMonsterWeapon)
            {
                PlayerWeaponManager.instance.SetAllWeaponsToInactive(isSpecial);
                PlayerWeaponManager.instance.AddWeaponToCurrentWeapons(weaponType);
                if (isSpecial)
                {
                    PlayerWeaponManager.instance.indexOfEquippedSpecialWeapon = PlayerWeaponManager.instance.ownedSpecialWeapons.Count - 1;
                    //PlayerUIManager.instance.playerUIHudManager.SetLeftWeaponQuickSlotIcon();
                }
                else
                {
                    PlayerWeaponManager.instance.indexOfEquippedWeapon = PlayerWeaponManager.instance.ownedWeapons.Count - 1;
                    //PlayerUIManager.instance.playerUIHudManager.SetRightWeaponQuickSlotIcon();
                }
            }
        }
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
