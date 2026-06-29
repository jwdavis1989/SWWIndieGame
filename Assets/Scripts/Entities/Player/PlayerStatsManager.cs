using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class PlayerStatsManager : CharacterStatsManager
{
    PlayerManager player;
    //Using instead of a network variable
    public string characterName = "Character Name";
    //gold
    public BigInteger gold = 0;

    [Header("Flashlight Flicker Settings")]
    [SerializeField] private int flickerCount = 6;             // How many times it blinks before dying
    [SerializeField] private float minFlickerDelay = 0.05f;    // Shortest time light stays on/off
    [SerializeField] private float maxFlickerDelay = 0.25f;    // Longest time light stays on/off
    private bool isFlickering = false;

    protected override void Awake()
    {
        base.Awake();

        player = GetComponent<PlayerManager>();
    }

    protected override void Start()
    {
        base.Start();

        //Called here in tutorial, but likely obsolete with other changes made in our version
        // CalculateHealthBasedOnfortitudeLevel(fortitude);
        // CalculateStaminaBasedOnEnduranceLevel(endurance);
    }

    public override void Update()
    {
        base.Update();

        HandleCheckFuelTank();
    }

    public void FullyRestoreResources()
    {
        SetNewMaxHealthValue();
        SetNewMaxStaminaValue();
        SetNewMaxFuelValue();
        player.isOutOfFuel = false;
        if (player.isDead)
        {
            player.ReviveCharacter();
        }
        totalPoiseDamage = 0f;

        //Reset Special Weapon Cooldown
        player.characterWeaponManager.isSpecialWeaponOffCooldown = true;
    }

    public void SetNewMaxFuelValue()
    {
        maxStamina = CalculateFuelBasedOnCapacityLevel(capacity);
        PlayerUIManager.instance.playerUIHudManager.SetMaxFuelValue(maxFuel);
        currentFuel = maxFuel;
        player.isOutOfFuel = false;
        player.isRunningOnEmergencyPowerLevels = false;
        PlayerUIManager.instance.radialFuelBar.UpdateStatBar(currentFuel, maxFuel);
    }

    protected virtual void HandleCheckFuelTank()
    {
        if (!player.isOutOfFuel)
        {
            if (currentFuel <= 0)
            {
                currentFuel = 0;
                player.isOutOfFuel = true;
                PlayerInputManager.instance.currentSprintCameraFieldOfViewMaximum = PlayerInputManager.instance.sprintCameraFieldOfViewMaximum;                Debug.Log("Camera FoV: " + PlayerCamera.instance.cameraObject.fieldOfView);

                //TODO: Add a coroutine call here to play some beeping and red flashing for polish
            }
            else if (!player.isRunningOnEmergencyPowerLevels && currentFuel < maxFuel / 2)
            {
                player.isRunningOnEmergencyPowerLevels = true;

                if (!isFlickering)
                {
                    StartCoroutine(FlickerAndShutOffFlashlight());
                }

            }
            else
            {
                PlayerInputManager.instance.currentSprintCameraFieldOfViewMaximum = PlayerInputManager.instance.sprintCameraFieldOfViewMaximumWithFuel;
            }
        }
        else
        {
            PlayerInputManager.instance.currentSprintCameraFieldOfViewMaximum = PlayerInputManager.instance.sprintCameraFieldOfViewMaximum;
        }
    }

    private IEnumerator FlickerAndShutOffFlashlight()
    {
        isFlickering = true;

        //Loop through the flicker count to create an erratic blinking pattern
        for (int i = 0; i < flickerCount; i++)
        {
            bool toggleState = (i % 2 == 0); // Alternates true/false every iteration

            if (player.flashlight != null) player.flashlight.SetActive(toggleState);
            if (player.cameraflashlight != null) player.cameraflashlight.SetActive(toggleState);

            //TODO Add SFX

            //Wait a random chunk of time to make the flicker look unstable and broken
            yield return new UnityEngine.WaitForSeconds(UnityEngine.Random.Range(minFlickerDelay, maxFlickerDelay));
        }

        //Hard shutoff
        if (player.flashlight != null) player.flashlight.SetActive(false);
        if (player.cameraflashlight != null) player.cameraflashlight.SetActive(false);

        isFlickering = false;
    }

}
