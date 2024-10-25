using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character Effects/Instant Effects/Take Health Damage")]
public class TakeHealthDamageCharacterEffect : InstantCharacterEffect
{
    //Store which character did damage to you.
    [Header("Character Causing Damage")]
    public CharacterManager characterCausingDamage;

    [Header("Damage")]
    //Elemental Damage will probably change to an ElementalStats object later.
    //public ElementalStats elementalDamage = new ElementalStats();
    // public WeaponScript weaponScript;
    public float physicalDamage = 0f;   
    public float fireDamage = 0f;
    public float iceDamage = 0f;
    public float lightningDamage = 0f;
    public float windDamage = 0f;
    public float earthDamage = 0f;
    public float lightDamage = 0f;
    public float beastDamage = 0f;
    public float scalesDamage = 0f;
    public float techDamage = 0f;

    //1 = True, 0 = False
    [Header("Armor Reduces? 1 = T, 0 = F")]
    public int isReducedByArmor = 1;

    [Header("Final Damage")]
    public float finalDamageDealt = 0f;      //Factors in all defenses and modifiers

    [Header("Poise")]
    public float PoiseDamage = 0f;
    public bool poiseIsBroken = false;  //If a character's poise is broken, they will be "Stunned" and play a damage animation.

    [Header("Debuff Build-Up")]
    //Build up amounts for effects

    [Header("Animation")]
    public bool playerDamageAnimation = true;
    public bool manuallySelectDamageAnimation = false;
    public string damageAnimation;

    [Header("Sound Effect")]
    public bool willPlayDamageSFX = true;
    public AudioClip elementalDamageSoundSFX;       //Used on top of regular SFX if there is a large quantity of elemental damage present

    [Header("Direction Damage Taken From")]
    public float angleHitFrom;                      //Used to determine what damage animation to play
    public Vector3 contactPoint;                    //Used to determine where impact occured for SFX instantiating



    public void Awake() {
        //weaponScript = characterCausingDamage.GetComponent<WeaponScript>();
    }
    public override void ProcessEffect(CharacterManager character) {
        base.ProcessEffect(character);

        //If the character is dead, no additional damage effects should be processed
        if (character.isDead) {
            return;
        }

        //Check for "Invulnerability"

        //Calculate then apply the Damage
        ApplyDamage(character, characterCausingDamage);

        //Check which direction damage came from
        //Play a damage animation
        //Check for build-ups (Poison, Bleed, ect)
        //Play damage sound FX
        //Play Damage VFX

        //If Character is A.I., Check for new target if character causing damage is preset

    }

    private void ApplyDamage(CharacterManager targetCharacter, CharacterManager characterCausingDamage) {
        //Monsters or player created damage
        if (characterCausingDamage != null) {
            if (!targetCharacter.isPlayer) {
                finalDamageDealt = WeaponsController.instance.ownedWeapons[WeaponsController.instance.indexOfEquippedWeapon].GetComponent<WeaponScript>().CalculateTotalDamage(targetCharacter);
            }
            else {
                finalDamageDealt = CalculateNPCDamage(targetCharacter);
            }
        }
        //Traps and environmental hazards
        else {
            finalDamageDealt = CalculateNPCDamage(targetCharacter);
        }


        //Apply final damage to character's health
        Debug.Log("Damage Taken: " + finalDamageDealt);
        targetCharacter.characterStatsManager.currentHealth -= finalDamageDealt;
        
        //Calculate Poise Damage to determine if the character will be stunned
        //TODO
    }

    public float CalculateNPCDamage (CharacterManager targetCharacter) {
        float result = physicalDamage * (1 - targetCharacter.characterStatsManager.physicalDefense);

        //I feel like there should be a way to do this iteratively, but with the ElementalStats class as it is, I don't know of any way to do so atm.
        result += physicalDamage * (fireDamage * 0.005f) * ((1 - targetCharacter.characterStatsManager.elementalDefenses.firePower) * isReducedByArmor);
        result += physicalDamage * (iceDamage * 0.005f) * ((1 - targetCharacter.characterStatsManager.elementalDefenses.icePower) * isReducedByArmor);
        result += physicalDamage * (lightningDamage * 0.005f) * ((1 - targetCharacter.characterStatsManager.elementalDefenses.lightningPower) * isReducedByArmor);
        result += physicalDamage * (windDamage * 0.005f) * ((1 - targetCharacter.characterStatsManager.elementalDefenses.windPower) * isReducedByArmor);
        result += physicalDamage * (earthDamage * 0.005f) * ((1 - targetCharacter.characterStatsManager.elementalDefenses.earthPower) * isReducedByArmor);
        result += physicalDamage * (lightDamage * 0.005f) * ((1 - targetCharacter.characterStatsManager.elementalDefenses.lightPower) * isReducedByArmor);
        result += physicalDamage * (beastDamage * 0.005f) * ((1 - targetCharacter.characterStatsManager.elementalDefenses.beastPower) * isReducedByArmor);
        result += physicalDamage * (scalesDamage * 0.005f) * ((1 - targetCharacter.characterStatsManager.elementalDefenses.scalesPower) * isReducedByArmor);
        result += physicalDamage * (techDamage * 0.005f) * ((1 - targetCharacter.characterStatsManager.elementalDefenses.techPower) * isReducedByArmor);

        if(result > 0) {
            return result;
        }
        else return 0;
    }

}
