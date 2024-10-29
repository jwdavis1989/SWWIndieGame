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

    [Header("Defenses")]
    public float physicalDefense = 0f;

    //Elemental Defenses, currently changing to ElementalStats class
    public ElementalStats elementalDefenses = new ElementalStats();
    // public float fireDefense = 0f;
    // public float iceDefense = 0f;
    // public float lightningDefense = 0f;
    // public float windDefense = 0f;
    // public float earthDefense = 0f;
    // public float lightDefense = 0f;
    // public float beastDefense = 0f;
    // public float scalesDefense = 0f;
    // public float techDefense = 0f;

    [Header("Health")]
    public float currentHealth = 1;
    public float maxHealth = 100;

    [Header("Stamina")]
    public float currentStamina = 0;
    public float maxStamina = 100;
    public float staminaRegenerationTimer = 0f;
    public float staminaRegenerationDelay = 1.75f;
    public float staminaRegenAmount = 2.5f;
    public float sprintingStaminaCost = 12f;
    public float dodgeStaminaCost = 25f;
    public float jumpStaminaCost = 0f;
    public float staminaTickTimer = 0.1f;

    protected virtual void Awake() {
        character = GetComponent<CharacterManager>();
    }

    protected virtual void Start() {
        CalculateHealthBasedOnfortitudeLevel(fortitude);
        CalculateHealthBasedOnfortitudeLevel(endurance);
    }

    public void Update() {
        CheckHP();
    }

    public float CalculateHealthBasedOnfortitudeLevel(int fortitude) {
        //Create an equation for how stamina is calculated

        //Use Mathf.RoundToInt and a float called stamina if your formula is more complex
        // float stamina = 0;
        // stamina = endurance * 10;
        // return Mathf.RoundToInt(stamina);

        //If simple formula, use this simpler and more efficient method
        return fortitude * 10f;
    }
    public float CalculateStaminaBasedOnEnduranceLevel(int endurance) {
        //Create an equation for how stamina is calculated

        //Use Mathf.RoundToInt and a float called stamina if your formula is more complex
        // float stamina = 0;
        // stamina = endurance * 10;
        // return Mathf.RoundToInt(stamina);

        //If simple formula, use this simpler and more efficient method
        return endurance * 10f;
    }

    //Only called when player gets an upgrade to these Resources
    public void SetNewMaxHealthValue() {
        maxHealth = CalculateHealthBasedOnfortitudeLevel(fortitude);
        PlayerUIManager.instance.playerUIHudManager.SetMaxHealthValue(maxHealth);
        currentHealth = maxHealth;
    }
    public void SetNewMaxStaminaValue() {
        maxStamina = CalculateStaminaBasedOnEnduranceLevel(endurance);
        PlayerUIManager.instance.playerUIHudManager.SetMaxStaminaValue(maxStamina);
        currentStamina = maxStamina;
    }

    public void RegenerateStamina() {

        if (character.isSprinting) {
            return;
        }

        if (character.isPerformingAction) {
            return;
        }

        staminaRegenerationTimer += Time.deltaTime;

        if (staminaRegenerationTimer >= staminaRegenerationDelay) {
            if (currentStamina < maxStamina) {
                staminaTickTimer += Time.deltaTime;

                if (staminaTickTimer >= 0.1) {
                    staminaTickTimer = 0;
                    currentStamina += staminaRegenAmount;
                }
            }
        }
    }

    public virtual void ResetStaminaRegenTimer() {
        staminaRegenerationTimer = 0;
    }

    public void CheckHP() {
        if (currentHealth <= 0 && !character.isDead) {
            StartCoroutine(character.ProcessDeathEvent());
        }

        //Clamp health to avoid over-healing
        if (character.isPlayer) {
            if (currentHealth > maxHealth) {
                currentHealth = maxHealth;
            }
        }
    }
    
}
