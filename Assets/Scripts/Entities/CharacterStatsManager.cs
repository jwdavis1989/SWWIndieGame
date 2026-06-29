using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStatsManager : MonoBehaviour
{
    CharacterManager character;
    [Header("Stats")]
    //Move these to the CharacterNetworkManager if adding multiplayer
    public int endurance = 10;
    public int fortitude = 10;
    public int capacity = 10;

    [Header("Defenses")]
    public float physicalDefense = 0f;

    //Elemental Defenses, currently changing to ElementalStats class
    public ElementalStats elementalDefenses = new ElementalStats();

    [Header("Poise")]
    public float totalPoiseDamage;              //How much poise damage we have taken
    public float offensivePoiseBonus;           //The poise bonus gained from using weapons (Heavy weapons have a much larger bonus)
    public float basePoiseDefense;              //Can be increased with some inventions
    public float defaultPoiseResetTimer = 8f;   //Time it takes for pose damage to reset after being hit last
    public float currentPoiseResetTimer = 0f;

    [Header("Health")]
    public float currentHealth = 1;
    public float maxHealth = 100;

    [Header("Stamina")]
    public float currentStamina = 0;
    public float maxStamina = 100;
    public float staminaRegenerationTimer = 0f;
    public float staminaRegenerationDelay = 0.25f;
    public float staminaRegenAmount = 2.5f;
    public float sprintingStaminaCost = 12f;
    public float dodgeStaminaCost = 25f;
    public float airDashStaminaCost = 40f;
    public float jumpStaminaCost = 25f;
    public float staminaTickTimer = 0.1f;

    [Header("Fuel")]
    public float currentFuel = 0;
    public float maxFuel = 100;
    public float sprintingFuelCost = 0.5f;
    public float airDashFuelCost = 2f;
    public float meteorStrikeFuelCost = 10f;

    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
    }

    protected virtual void Start()
    {
        CalculateHealthBasedOnfortitudeLevel(fortitude);
        CalculateHealthBasedOnfortitudeLevel(endurance);
    }

    public virtual void Update()
    {
        CheckHP();
        HandlePoiseResetTimer();
    }

    public float CalculateHealthBasedOnfortitudeLevel(int fortitude)
    {
        //Create an equation for how stamina is calculated

        //Use Mathf.RoundToInt and a float called stamina if your formula is more complex
        // float stamina = 0;
        // stamina = endurance * 10;
        // return Mathf.RoundToInt(stamina);

        //If simple formula, use this simpler and more efficient method
        return fortitude * 10f;
    }
    public float CalculateStaminaBasedOnEnduranceLevel(int endurance)
    {
        //Create an equation for how stamina is calculated

        //Use Mathf.RoundToInt and a float called stamina if your formula is more complex
        // float stamina = 0;
        // stamina = endurance * 10;
        // return Mathf.RoundToInt(stamina);

        //If simple formula, use this simpler and more efficient method
        return endurance * 10f;
    }

    public float CalculateFuelBasedOnCapacityLevel(int capacity)
    {
        //Create an equation for how stamina is calculated

        //Use Mathf.RoundToInt and a float called stamina if your formula is more complex
        // float stamina = 0;
        // stamina = endurance * 10;
        // return Mathf.RoundToInt(stamina);

        //If simple formula, use this simpler and more efficient method
        return capacity * 10f;
    }

    //Only called when player gets an upgrade to these Resources
    public void SetNewMaxHealthValue()
    {
        maxHealth = CalculateHealthBasedOnfortitudeLevel(fortitude);
        currentHealth = maxHealth;
        PlayerUIManager.instance.playerUIHudManager.UpdateHealthBar(currentHealth, maxHealth);
    }
    public void SetNewMaxStaminaValue()
    {
        maxStamina = CalculateStaminaBasedOnEnduranceLevel(endurance);
        currentStamina = maxStamina;
        PlayerUIManager.instance.playerUIHudManager.UpdateStaminaBar(currentStamina, maxStamina);
    }

    public void SetNewMaxFuelValue()
    {
        maxHealth = CalculateHealthBasedOnfortitudeLevel(capacity);
        currentFuel = maxFuel;
        PlayerUIManager.instance.playerUIHudManager.UpdateFuelBar(currentFuel, maxFuel);
    }

    public void RegenerateStamina()
    {

        if (character.isSprinting)
        {
            return;
        }

        if (character.isPerformingAction)
        {
            return;
        }

        if (character.isBlocking)
        {
            return;
        }

        staminaRegenerationTimer += Time.deltaTime;

        if (staminaRegenerationTimer >= staminaRegenerationDelay)
        {
            if (currentStamina < maxStamina)
            {
                staminaTickTimer += Time.deltaTime;

                if (staminaTickTimer >= 0.1)
                {
                    staminaTickTimer = 0;
                    currentStamina += staminaRegenAmount;
                }
            }
        }
    }

    public virtual void ResetStaminaRegenTimer()
    {
        staminaRegenerationTimer = 0;
    }

    public void CheckHP()
    {
        if (currentHealth <= 0 && !character.isDead)
        {
            //Disable Minimap Marker
            if (character.miniMapSprite != null)
            {
                character.miniMapSprite.SetActive(false);
            }

            StartCoroutine(character.ProcessDeathEvent());
        }

        //Clamp health to avoid over-healing
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }

    public float CalculateRemainingPoise()
    {
        return basePoiseDefense + offensivePoiseBonus + totalPoiseDamage;
    }

    protected virtual void HandlePoiseResetTimer()
    {
        if (currentPoiseResetTimer > 0)
        {
            currentPoiseResetTimer -= Time.deltaTime;
        }
        else
        {
            totalPoiseDamage = 0;
        }
    }

}
