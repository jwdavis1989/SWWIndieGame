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

    [Header("Final Damage")]
    private float finalDamageDealt = 0f;      //Factors in all defenses and modifiers

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



    public override void ProcessEffect(CharacterManager character) {
        base.ProcessEffect(character);

        //If the character is dead, no additional damage effects should be processed
        if (character.isDead) {
            return;
        }

        //Check for "Invulnerability"

        //Calculate then apply the Damage
        //ApplyDamage(character);

        //Check which direction damage came from
        //Play a damage animation
        //Check for build-ups (Poison, Bleed, ect)
        //Play damage sound FX
        //Play Damage VFX

        //If Character is A.I., Check for new target if character causing damage is preset

    }

    private void ApplyDamage(CharacterManager character, Collider other) {
        if (characterCausingDamage != null) {
            if (characterCausingDamage.isPlayer) {
                //Call Weapon Manager damage function
                finalDamageDealt = WeaponsController.instance.ownedWeapons[WeaponsController.instance.indexOfEquippedWeapon].GetComponent<WeaponScript>().CalculateTotalDamage(other);
            }
            else {
                //Call Monster damage function
                //finalDamageDealt = 
            }
        }

        //Apply final damage to character's health
        Debug.Log("Damage Taken: " + finalDamageDealt);
        character.characterStatsManager.currentHealth -= finalDamageDealt;
        
        //Calculate Poise Damage to determine if the character will be stunned
        //TODO
    }

}
