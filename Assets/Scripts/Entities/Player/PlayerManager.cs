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
    [HideInInspector] public PlayerInteractionManager playerInteractionManager;

    public GameObject flashlight;
    public GameObject cameraflashlight;
    public GameObject capeSystem;
    private Cloth capeClothComponent;
    private float capeClothWorldAccelerationModifier;
    [SerializeField] public PlayerSoundFXManager playerSoundFXManager;

    [Header("Debug Menu")]
    [SerializeField] bool respawnCharacter = false;

    protected override void Awake()
    {
        isPlayer = true;
        isRotatingAttacker = true;
        base.Awake();

        //Do more stuff, only for the player
        playerLocomotionManager = GetComponent<PlayerLocomotionManager>();
        playerCombatManager = GetComponent<PlayerCombatManager>();
        playerSoundFXManager = GetComponent<PlayerSoundFXManager>();
        playerInteractionManager = GetComponent<PlayerInteractionManager>();
        capeClothComponent = capeSystem.GetComponentInChildren<Cloth>();
        capeClothWorldAccelerationModifier = capeClothComponent.worldAccelerationScale;

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

    protected override void LateUpdate()
    {
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

    public override void ReviveCharacter()
    {
        base.ReviveCharacter();
        canMove = true;
        isDead = false;
        playerStatsManager.currentHealth = playerStatsManager.maxHealth;
        playerStatsManager.currentStamina = playerStatsManager.maxStamina;


        //Play Rebirth Effects here

        //Reset player to default empty state
        playerAnimationManager.PlayTargetActionAnimation("Empty", false);

    }

    public void SaveGameDataToCurrentCharacterData(ref CharacterSaveData currentCharacterData)
    {
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

        //Weapons
        currentCharacterData.weapons = PlayerWeaponManager.instance.GetCurrentWeapons();
        currentCharacterData.indexOfEquippedWeapon = PlayerWeaponManager.instance.indexOfEquippedWeapon;
        currentCharacterData.indexOfEquippedSpecialWeapon = PlayerWeaponManager.instance.indexOfEquippedSpecialWeapon;
        //Tinker Components owned
        //currentCharacterData.ownedComponents = TinkerComponentManager.instance.CreateSaveData();
        //currentCharacterData.ownedWpnComponents = TinkerComponentManager.instance.CreateSaveData(true);
        //Journal flags
        currentCharacterData.journalFlags = JournalManager.instance.journalFlags;
        //Ideas
        currentCharacterData.ideas = InventionManager.instance.obtainedIdeas;
        //Inventions
        currentCharacterData.inventions = InventionManager.instance.SaveInventions();
        //Inventory
        Inventory inventory = GetComponent<Inventory>();
        currentCharacterData.inventoryItems = inventory.SaveItems();
        currentCharacterData.weaponSalvage = inventory.SaveWeaponComponents();
        //Dungeon
        currentCharacterData.savedDungeons = DungeonManager.SaveDungeons();
    }

    public void LoadGameFromCurrentCharacterData(ref CharacterSaveData currentCharacterData)
    {
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
        //Load TinkerComponents
        //TinkerComponentManager.instance.LoadComponentSaveData(currentCharacterData.ownedComponents);
        //TinkerComponentManager.instance.LoadComponentSaveData(currentCharacterData.ownedWpnComponents, true);
        //Load Journal Flags
        JournalManager.instance.journalFlags = currentCharacterData.journalFlags;
        //Ideas
        InventionManager.instance.obtainedIdeas = currentCharacterData.ideas;
        //Inventions
        InventionManager.instance.LoadInventions(currentCharacterData.inventions);
        //Inventory
        GetComponent<Inventory>().LoadInventory(currentCharacterData.inventoryItems, currentCharacterData.weaponSalvage);
        //Dungeon
        DungeonManager.LoadDungeons(currentCharacterData.savedDungeons);
    }

    public void ToggleFlashlight()
    {
        if (flashlight != null)
        {
            if (flashlight.activeSelf)
            {
                flashlight.SetActive(false);
            }
            else
            {
                flashlight.SetActive(true);
            }
        }
        else
        {
            Debug.Log("ERROR: Player Flashlight Instance Not Set in Editor.");
        }

        if (cameraflashlight != null)
        {
            if (cameraflashlight.activeSelf)
            {
                cameraflashlight.SetActive(false);
            }
            else
            {
                cameraflashlight.SetActive(true);
            }
        }
        else
        {
            Debug.Log("ERROR: Camera Flashlight Instance Not Set in Editor.");
        }
    }

    public void DebugAddWeapon()
    {
        WeaponScript weaponScript;
        WeaponType weaponType;
        bool isSpecial;

        for (int i = 0; i < System.Enum.GetValues(typeof(WeaponType)).Length - 1; i++)
        {
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

        //Update Weapon HUD Display
        PlayerUIManager.instance.playerUIHudManager.SetRightWeaponQuickSlotIcon();
        PlayerUIManager.instance.playerUIHudManager.SetLeftWeaponQuickSlotIcon();
    }

    public void DebugChangeWeaponElementByHand(bool isMainHand)
    {
        WeaponScript updatedWeapon;
        ElementalDamageType newHighestElement;
        int newHighestElementIndex = 0;

        if (isMainHand)
        {
            updatedWeapon = PlayerWeaponManager.instance.GetMainHand();
        }
        else
        {
            updatedWeapon = PlayerWeaponManager.instance.GetOffHand();
        }

        newHighestElement = updatedWeapon.stats.elemental.currentHighestElementalStat;

        switch (newHighestElement)
        {
            case ElementalDamageType.Fire:
                newHighestElement = ElementalDamageType.Ice;
                newHighestElementIndex = 1;
                //if (isInDebugMode) Debug.Log("Highest Element: Fire");
                break;
            case ElementalDamageType.Ice:
                newHighestElement = ElementalDamageType.Lightning;
                newHighestElementIndex = 2;
                //if (isInDebugMode) Debug.Log("Highest Element: Ice");
                break;
            case ElementalDamageType.Lightning:
                newHighestElement = ElementalDamageType.Wind;
                newHighestElementIndex = 3;
                //if (isInDebugMode) Debug.Log("Highest Element: Lightning");
                break;
            case ElementalDamageType.Wind:
                newHighestElement = ElementalDamageType.Earth;
                newHighestElementIndex = 4;
                //if (isInDebugMode) Debug.Log("Highest Element: Wind");
                break;
            case ElementalDamageType.Earth:
                newHighestElement = ElementalDamageType.Light;
                newHighestElementIndex = 5;
                //if (isInDebugMode) Debug.Log("Highest Element: Earth");
                break;
            case ElementalDamageType.Light:
                newHighestElement = ElementalDamageType.Beast;
                newHighestElementIndex = 6;
                //if (isInDebugMode) Debug.Log("Highest Element: Light");
                break;
            case ElementalDamageType.Beast:
                newHighestElement = ElementalDamageType.Scales;
                newHighestElementIndex = 7;
                //if (isInDebugMode) Debug.Log("Highest Element: Beast");
                break;
            case ElementalDamageType.Scales:
                newHighestElement = ElementalDamageType.Tech;
                newHighestElementIndex = 8;
                //if (isInDebugMode) Debug.Log("Highest Element: Scales");
                break;
            case ElementalDamageType.Tech:
                newHighestElement = ElementalDamageType.Fire;
                newHighestElementIndex = 0;
                //if (isInDebugMode) Debug.Log("Highest Element: Tech");
                break;
            default:
                newHighestElement = ElementalDamageType.Fire;
                newHighestElementIndex = 0;
                //if (isInDebugMode) Debug.Log("Highest Element: Unaspected");
                break;
        }

        updatedWeapon.stats.elemental.currentHighestElementalStat = newHighestElement;
        
        //Sets all glowing materials to match the current highest element
        updatedWeapon.SetElementalWeaponMaterials(newHighestElementIndex);

        if (updatedWeapon.bladeTrailVFX)
        {
            //Sets blade trail VFX materials to match the current highest element
            if (updatedWeapon.bladeTrailVFX)
            {
                updatedWeapon.bladeTrailVFX.gameObject.SetActive(true);
                updatedWeapon.bladeTrailVFX.SetElementalTrailMaterial(newHighestElementIndex);
                updatedWeapon.bladeTrailVFX.gameObject.SetActive(false);
            }
        }
        
        if (isMainHand)
        {

            Debug.Log("Mainhand: Element changed to: " + newHighestElement);
        }
        else
        {
            Debug.Log("Offhand: Element changed to: " + newHighestElement);
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

    public void ChangeCurrentlyEquippedWeaponObject(int newActiveIndex)
    {
        PlayerWeaponManager.instance.ChangeWeapon(newActiveIndex);
    }

    //Delete this later
    private void DebugMenu()
    {
        if (respawnCharacter)
        {
            respawnCharacter = false;
            ReviveCharacter();
        }
    }

    public override void DisableInvulnerable()
    {
        if (!InventionManager.instance.CheckHasUpgrade(InventionID.ROLLER_JOINT))
        {
            isInvulnerable = false;
        }
    }

    //Not currently being used because the dodge roll already begins invulnerability immediately, but is needed if that changes.
    public void EnableRollerJointInvulnerable()
    {
        if (InventionManager.instance.CheckHasUpgrade(InventionID.ROLLER_JOINT))
        {
            isInvulnerable = true;
        }
    }

    public override void DisableRollerJointInvulnerable()
    {
        isInvulnerable = false;
    }

    public override void DisableIsRolling()
    {
        isRolling = false;
    }

    public override void DisableBoosting()
    {
        base.DisableBoosting();
        isBoosting = false;
    }

    public override void SetGunToFiringTransform()
    {
        PlayerWeaponManager.instance.GetOffHand().SetGunToFiringTransform();
    }

    public override void SetGunToHandTransform()
    {
        PlayerWeaponManager.instance.GetOffHand().SetGunToHandTransform();
    }

    public override void ResetGunTransformBools()
    {
        PlayerWeaponManager.instance.GetOffHand().ResetGunTransformBools();
    }

    public void EnableCapeSystem()
    {
        capeSystem.SetActive(true);
    }

    public void DisableCapeSystem()
    {
        capeSystem.SetActive(false);
    }

    public void TeleportPlayerToSceneAndCoordinates(int sceneID, float destinationX = 0f, float destinationY = 0f, float destinationZ = 0f, string sceneIdString=null, bool enableAfterLoad=true)
    {
        TeleportData.Destination = new Vector3(destinationX, destinationY, destinationZ);
        TeleportData.playerManager = this;
        /** Will use string name of scene if not null else uses index */
        TeleportData.SceneIdString = sceneIdString;
        TeleportData.SceneID = sceneID;
        TeleportData.enableAfterLoad = enableAfterLoad;
        //Disable Controls
        PlayerInputManager.instance.SafeDisable(true, true);
        Time.timeScale = 0;

        SceneManager.LoadScene("LoadingScene");

        //DisableCapeSystem();
        //transform.position = new Vector3(destinationX, destinationY, destinationZ);
        //SceneManager.LoadSceneAsync(sceneID);
        //EnableCapeSystem();
    }

    public override void CallPlayJumpAttackImpactVFX()
    {
        base.CallPlayJumpAttackImpactVFX();

        //Turn off Meteor Boosters just in case effect is interrupted
        DisableMeteorBoosterVFX();
        DisableMeteorDescentBoosterVFX();
    }

    public override void EnableMeteorBoosterVFX()
    {
        playerLocomotionManager.EnableMeteorBoosters();
        playerLocomotionManager.JumpAttackQuickFall();
    }

    public override void DisableMeteorBoosterVFX()
    {
        playerLocomotionManager.DisableMeteorBoosters();
    }

    public override void DisableMeteorDescentBoosterVFX()
    {
        playerLocomotionManager.DisableMeteorDescentBoosters();
    }

    public void PauseClothPhysics()
    {
        //capeClothComponent.worldAccelerationScale = 0f;
        capeClothComponent.enabled = false;
    }

    public void ResumeClothPhysics()
    {
        //capeClothComponent.worldAccelerationScale = capeClothWorldAccelerationModifier;
        capeClothComponent.enabled = true;
    }

}
