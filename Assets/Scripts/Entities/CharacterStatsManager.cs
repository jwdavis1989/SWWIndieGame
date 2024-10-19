using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStatsManager : MonoBehaviour
{
    CharacterManager character;
    [Header("Stats")]
    //Move these to the CharacterNetworkManager if adding multiplayer
    public int endurance = 1;

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

    public int CalculateStaminaBasedOnEnduranceLevel(int endurance) {
        //Create an equation for how stamina is calculated

        //Use Mathf.RoundToInt and a float called stamina if your formula is more complex
        // float stamina = 0;
        // stamina = endurance * 10;
        // return Mathf.RoundToInt(stamina);

        //If simple formula, use this simpler and more efficient method
        return endurance * 10;
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

    //Remove this version if adding multiplayer
    public virtual void ResetStaminaRegenTimer() {
        staminaRegenerationTimer = 0;
    }

    //Use this version if adding multiplayer
        // public virtual void ResetStaminaRegenTimer(float previousStaminaAmount, float currentStaminaAmount) {
        //     if (currentStaminaAmount < previousStaminaAmount) {
        //         staminaRegenerationTimer = 0;
        //     }
        // }
}
