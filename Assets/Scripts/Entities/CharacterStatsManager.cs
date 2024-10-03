using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStatsManager : MonoBehaviour
{
    [Header("Stats")]

    //Move these to the CharacterNetworkManager if adding multiplayer
    public int endurance = 1;

    [Header("Stamina")]
    public float currentStamina = 0;
    public float maxStamina = 100;
    public float staminaRegenerationTimer = 0f;
    public float staminaRegenerationDelay = 2f;
    public float staminaRegenAmount = 2f;


    public int CalculateStaminaBasedOnEnduranceLevel(int endurance) {
        //Create an equation for how stamina is calculated

        //Use Mathf.RoundToInt and a float called stamina if your formula is more complex
        // float stamina = 0;
        // stamina = endurance * 10;
        // return Mathf.RoundToInt(stamina);

        //If simple formula, use this simpler and more efficient method
        return endurance * 10;
    }
}
