using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character Effects/Instant Effects/Take Blocked Health Damage")]
public class TakeBlockedHealthDamageCharacterEffect : InstantCharacterEffect
{
    //Store which character did damage to you.
    [Header("Character Causing Damage")]
    public CharacterManager characterCausingDamage;

    [HideInInspector] public WeaponFamily weaponFamily;

    [Header("Damage")]
    public ElementalStats elementalDamage = new ElementalStats();
    public float physicalDamage = 0f;   

    //Damage modifier for specific attack, which differs between attacks in a combo
    public float attackMotionValue = 1f;

    //Damage modifier for successfully charging an attack fully (e.g. Heavy melee or Magic)
    public float fullChargeModifier = 1f;

    //1 = True, 0 = False
    [Header("Armor Reduces? 1 = T, 0 = F")]
    public int isReducedByArmor = 1;

    [Header("Final Damage")]
    public float finalDamageDealt = 0f;      //Factors in all defenses and modifiers

    [Header("Poise")]
    public float poiseDamage = 0f;
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

    [Header("Main Hand / Off Hand weapon")]
    public bool isMainHand = false;



    public void Awake() {
        //weaponScript = characterCausingDamage.GetComponent<WeaponScript>();
    }
    public override void ProcessEffect(CharacterManager character) {
        base.ProcessEffect(character);

        Debug.Log("Hit was blocked!");

        //If the character is dead, no additional damage effects should be processed
        if (character.isDead)
        {
            return;
        }

        //Check for "Invulnerability"
        if (!character.isInvulnerable)
        {
            //Calculate then apply the Damage
            ApplyDamage(character, characterCausingDamage);

            //Check which direction damage came from


            //Play a damage animation
            PlayDirectionalBasedBlockingAnimation(character);

            //Check for build-ups (Poison, Bleed, ect)

            //Play damage sound FX
            PlayDamageSFX(character);

            //Play Damage VFX
            PlayDamageVFX(character);

            //If Character is A.I., Check for new target if character causing damage is preset

        }

    }

    private void ApplyDamage(CharacterManager targetCharacter, CharacterManager characterCausingDamage) {
        //Monsters or player created damage
        if (characterCausingDamage != null) {
            if (!targetCharacter.isPlayer) {
                AICharacterManager enemy = targetCharacter.GetComponent<AICharacterManager>();
                //finalDamageDealt = PlayerWeaponManager.instance.ownedWeapons[PlayerWeaponManager.instance.indexOfEquippedWeapon].GetComponent<WeaponScript>().CalculateTotalDamage(targetCharacter, attackMotionValue, fullChargeModifier);
                if (isMainHand)
                {
                    finalDamageDealt = PlayerWeaponManager.instance.GetMainHand().CalculateTotalDamage(targetCharacter, attackMotionValue, fullChargeModifier);
                    if (enemy != null)
                    {
                        enemy.isHitByMainHand = true;
                    }
                }
                else
                {
                    finalDamageDealt = PlayerWeaponManager.instance.GetOffHand().CalculateTotalDamage(targetCharacter, attackMotionValue, fullChargeModifier);
                    if (enemy != null)
                    {
                        enemy.isHitByOffHand = true;
                    }
                }
            }
            else {
                finalDamageDealt = CalculateNPCDamage(targetCharacter, attackMotionValue, fullChargeModifier);
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

    public float CalculateNPCDamage (CharacterManager targetCharacter, float attackMotionValue = 1f, float fullChargeModifier = 1f) {
        float result = physicalDamage * (1 - targetCharacter.characterStatsManager.physicalDefense);

        //I feel like there should be a way to do this iteratively, but with the ElementalStats class as it is, I don't know of any way to do so atm.
        result += physicalDamage * (elementalDamage.firePower * 0.005f) * ((1 - targetCharacter.characterStatsManager.elementalDefenses.firePower) * isReducedByArmor);
        result += physicalDamage * (elementalDamage.icePower * 0.005f) * ((1 - targetCharacter.characterStatsManager.elementalDefenses.icePower) * isReducedByArmor);
        result += physicalDamage * (elementalDamage.lightningPower * 0.005f) * ((1 - targetCharacter.characterStatsManager.elementalDefenses.lightningPower) * isReducedByArmor);
        result += physicalDamage * (elementalDamage.windPower * 0.005f) * ((1 - targetCharacter.characterStatsManager.elementalDefenses.windPower) * isReducedByArmor);
        result += physicalDamage * (elementalDamage.earthPower * 0.005f) * ((1 - targetCharacter.characterStatsManager.elementalDefenses.earthPower) * isReducedByArmor);
        result += physicalDamage * (elementalDamage.lightPower * 0.005f) * ((1 - targetCharacter.characterStatsManager.elementalDefenses.lightPower) * isReducedByArmor);
        result += physicalDamage * (elementalDamage.beastPower * 0.005f) * ((1 - targetCharacter.characterStatsManager.elementalDefenses.beastPower) * isReducedByArmor);
        result += physicalDamage * (elementalDamage.scalesPower * 0.005f) * ((1 - targetCharacter.characterStatsManager.elementalDefenses.scalesPower) * isReducedByArmor);
        result += physicalDamage * (elementalDamage.techPower * 0.005f) * ((1 - targetCharacter.characterStatsManager.elementalDefenses.techPower) * isReducedByArmor);

        if(result > 0) {
            return result * attackMotionValue * fullChargeModifier;
        }
        else return 0;
    }

    private void PlayDamageVFX(CharacterManager character)
    {
        //e.g. If we have Fire Damage, Play Fire Particle Effects
        if (finalDamageDealt > 0f)
        {
            character.characterEffectsManager.PlayBloodSplatterVFX(contactPoint);
        }
        
        //Play a Sparking Impact VFX for the blocking impact as well
        
    }

    private void PlayDamageSFX(CharacterManager damagedCharacter)
    {
        AudioClip impactSFX;
        //e.g. If Fire damage is greater, play burn SFX
        //e.g. If Lightning damage is greater, play Zap SFX

        switch (weaponFamily)
        {
            case WeaponFamily.Swords:
                impactSFX = WorldSoundFXManager.instance.ChooseRandomSFXFromArray(WorldSoundFXManager.instance.slashingImpactSFX);
                damagedCharacter.characterSoundFXManager.PlayAdvancedSoundFX(impactSFX, 1, 1f, true, 0.1f);
                break;
            case WeaponFamily.GreatSwords:
                impactSFX = WorldSoundFXManager.instance.ChooseRandomSFXFromArray(WorldSoundFXManager.instance.slashingImpactSFX);
                damagedCharacter.characterSoundFXManager.PlayAdvancedSoundFX(impactSFX, 1, 0.8f, true, 0.1f);
                break;
            case WeaponFamily.HammersOrWrenches:
                impactSFX = WorldSoundFXManager.instance.ChooseRandomSFXFromArray(WorldSoundFXManager.instance.bludgeoningImpactSFX);
                damagedCharacter.characterSoundFXManager.PlayAdvancedSoundFX(impactSFX, 1, 1f, true, 0.1f);
                break;
            case WeaponFamily.Scythes:
                impactSFX = WorldSoundFXManager.instance.ChooseRandomSFXFromArray(WorldSoundFXManager.instance.slashingImpactSFX);
                damagedCharacter.characterSoundFXManager.PlayAdvancedSoundFX(impactSFX, 1, 1f, true, 0.1f);
                break;
            case WeaponFamily.Daggers:
                impactSFX = WorldSoundFXManager.instance.ChooseRandomSFXFromArray(WorldSoundFXManager.instance.piercingImpactSFX);
                damagedCharacter.characterSoundFXManager.PlayAdvancedSoundFX(impactSFX, 1, 1f, true, 0.1f);
                break;
            case WeaponFamily.SemiAutoGuns:
                impactSFX = WorldSoundFXManager.instance.ChooseRandomSFXFromArray(WorldSoundFXManager.instance.gunImpactSFX);
                damagedCharacter.characterSoundFXManager.PlayAdvancedSoundFX(impactSFX, 1, 1f, true, 0.1f);
                break;
            case WeaponFamily.BurstFireGuns:
                impactSFX = WorldSoundFXManager.instance.ChooseRandomSFXFromArray(WorldSoundFXManager.instance.gunImpactSFX);
                damagedCharacter.characterSoundFXManager.PlayAdvancedSoundFX(impactSFX, 1, 1.2f, true, 0.1f);
                break;
            case WeaponFamily.LaserGuns:
                impactSFX = WorldSoundFXManager.instance.ChooseRandomSFXFromArray(WorldSoundFXManager.instance.fireImpactSFX);
                damagedCharacter.characterSoundFXManager.PlayAdvancedSoundFX(impactSFX, 1, 1.2f, true, 0.1f);
                break;
            case WeaponFamily.Shotguns:
                impactSFX = WorldSoundFXManager.instance.ChooseRandomSFXFromArray(WorldSoundFXManager.instance.gunImpactSFX);
                damagedCharacter.characterSoundFXManager.PlayAdvancedSoundFX(impactSFX, 1, 0.6f, true, 0.1f);
                break;
            case WeaponFamily.GrenadeLaunchers:
                impactSFX = WorldSoundFXManager.instance.ChooseRandomSFXFromArray(WorldSoundFXManager.instance.explosionImpactSFX);
                damagedCharacter.characterSoundFXManager.PlayAdvancedSoundFX(impactSFX, 1, 1f, true, 0.1f);
                break;
            case WeaponFamily.MagicRosary:
                //Do the thing
                break;
            case WeaponFamily.MagicWands:
                //Do the thing thing
                break;
            case WeaponFamily.MagicStaves:
                //Do the thing thing thing
                break;
            case WeaponFamily.MagicRings:
                //Do the thing thing thing thing
                break;
            case WeaponFamily.Drones:
                //Do the thing thing thing thing thing
                break;
            case WeaponFamily.NotYetSet:
                Debug.Log("ERROR: Weapon Family not set on Prefab!");
                break;
            default:
                Debug.Log("ERROR: Weapon Family not set on Prefab!");
                break;
        }

        if (finalDamageDealt > 0f)
        {
            damagedCharacter.characterSoundFXManager.PlayTakeDamageGrunts();
        }
        else
        {
            //Play a Pinging SFX based on how heavy the hit was
        }
    }

    private void PlayDirectionalBasedBlockingAnimation(CharacterManager characterTakingDamage) {

        //Works without this, but the tutorial suggests it so Idk man(?)
        if (characterTakingDamage.isDead) {
            return;
        }


        //1. Calculate an "Intensity" based on Poise Damage
        DamageIntensity damageIntensity = WorldUtilityManager.instance.GetDamageIntensityBasedOnPoiseDamage(poiseDamage);
        //2. Play a Proper Animation to match the "Intensity" of the blocked blow

        //TODO: Check for Two-Hand status, if two-handing then use 2h version of block animations instead
        switch (damageIntensity)
        {
            case DamageIntensity.Ping:
                damageAnimation = "Block_Ping_01";
                break;
            case DamageIntensity.Light:
                damageAnimation = "Block_Light_01";
                break;
            case DamageIntensity.Medium:
                damageAnimation = "Block_Medium_01";
                break;
            case DamageIntensity.Heavy:
                damageAnimation = "Block_Heavy_01";
                break;
            case DamageIntensity.Colossal:
                damageAnimation = "Block_Colossal_01";
                break;
        }

        //If poise is broken, play a staggering damage animation
        if (poiseIsBroken) {
            characterTakingDamage.characterAnimatorManager.lastDamageAnimationPlayed = damageAnimation;
            characterTakingDamage.characterAnimatorManager.PlayTargetActionAnimation(damageAnimation, true);
        }
    }

}


