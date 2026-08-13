using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    //public bool poiseIsBroken = false;  //If a character's poise is broken, they will be "Stunned" and play a damage animation.

    [Header("Stamina Damage")]
    public float baseStaminaDamage;
    public float finalStaminaDamage;

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



    public void Awake()
    {
        //weaponScript = characterCausingDamage.GetComponent<WeaponScript>();
    }
    public override void ProcessEffect(CharacterManager character)
    {
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

            //Calculate Stamina Cost of Blocking
            CalculateStaminaDamage(character);

            //Check which direction damage came from


            //Play a damage animation
            PlayDirectionalBasedBlockingAnimation(character);

            //Check for build-ups (Poison, Bleed, ect)
            characterCausingDamage.ApplyOnHitEffects(character);

            //Play damage sound FX
            PlayDamageSFX(character);

            //Play Damage VFX
            PlayDamageVFX(character);

            //If Character is A.I., Check for new target if character causing damage is preset

        }

    }

    private void ApplyDamage(CharacterManager targetCharacter, CharacterManager characterCausingDamage)
    {
        //Monsters or player created damage
        if (characterCausingDamage != null)
        {
            if (!targetCharacter.isPlayer)
            {
                AICharacterManager enemy = targetCharacter.GetComponent<AICharacterManager>();
                CharacterWeaponManager characterWeaponManager = characterCausingDamage.characterWeaponManager;
                if (characterWeaponManager == null)
                    Debug.LogError("ERROR: Weapon manager not set!");
                WeaponScript weapon = isMainHand ? characterWeaponManager.GetMainHand() : characterWeaponManager.GetOffHand();
                finalDamageDealt = weapon.CalculateTotalDamage(targetCharacter, attackMotionValue, fullChargeModifier);
            }
            else
            {
                finalDamageDealt = CalculateNPCDamage(targetCharacter, attackMotionValue, fullChargeModifier);
            }
        }
        //Traps and environmental hazards
        else
        {
            finalDamageDealt = CalculateNPCDamage(targetCharacter);
        }


        if (targetCharacter.isPerfectBlocking)
        {
            Debug.Log("Perfect Block Damage Taken: " + finalDamageDealt);
        }
        else
        {
            Debug.Log("Normal Block Damage Taken: " + finalDamageDealt);
        }

        //Apply final damage to character's health
        targetCharacter.ApplyDamage(finalDamageDealt, characterCausingDamage, isMainHand);
        targetCharacter.characterStatsManager.currentHealth -= finalDamageDealt;
        if (targetCharacter.isPlayer)
        {
            PlayerUIManager.instance.playerUIHudManager.UpdateHealthBar(targetCharacter.characterStatsManager.currentHealth, targetCharacter.characterStatsManager.maxHealth);
        }

        if (targetCharacter != null && targetCharacter.characterUIManager != null)
        {   
            targetCharacter.characterUIManager.TriggerGlitchTextEffect();
        }

        //Calculate Poise Damage to determine if the character will be stunned
        //TODO
    }

    public float CalculateNPCDamage(CharacterManager targetCharacter, float attackMotionValue = 1f, float fullChargeModifier = 1f)
    {
        float result = physicalDamage * (1 - targetCharacter.characterStatsManager.physicalDefense);

        Dictionary<string, float> damageElements = elementalDamage.ToElementalDictionary();
        Dictionary<string, float> defenseElements = targetCharacter.characterStatsManager.elementalDefenses.ToElementalDictionary();
        foreach (KeyValuePair<string, float> stat in damageElements)
        {
            result += physicalDamage * (stat.Value * 0.005f) * ((1 - defenseElements[stat.Key]) * isReducedByArmor);
        }

        //Calculate block modifier
        float blockingState = targetCharacter.isPerfectBlocking ? targetCharacter.perfectBlockModifier : 1f;

        if (result > 0)
        {
            if (targetCharacter.isBlocking)
            {
                if (targetCharacter.characterWeaponManager != null && targetCharacter.characterWeaponManager.ownedWeapons.Count > 0)
                {
                    return result * attackMotionValue * fullChargeModifier * (1 - (blockingState * targetCharacter.characterWeaponManager.GetMainHand().stats.block) / 100f);
                }
                else
                {
                    return result * attackMotionValue * fullChargeModifier * (1 - (blockingState * targetCharacter.nonWeaponBlockingStrength) / 100f);
                }
            }
            else
            {
                return result * attackMotionValue * fullChargeModifier;
            }

        }
        else return 0;
    }

    public void CalculateStaminaDamage(CharacterManager character)
    {
        float finalStaminaDamage = baseStaminaDamage;
        float staminaDamageAbsorbtion = 0f;

        if (character.characterWeaponManager != null && character.characterWeaponManager.GetMainHand() != null)
        {
            staminaDamageAbsorbtion = finalStaminaDamage * character.characterWeaponManager.GetMainHand().stats.stability / 100;
        }

        finalStaminaDamage = finalStaminaDamage - staminaDamageAbsorbtion;

        character.characterStatsManager.currentStamina -= finalStaminaDamage;

        //11-3-25: Currently commenting out to allow the player to regen stamina by lowering their block, as you regen 0 stamina while blocking now.
        //character.characterStatsManager.ResetStaminaRegenTimer();
    }

    private bool CheckForGuardBreak(CharacterManager character)
    {
        bool result = false;
        if (character.characterStatsManager.currentStamina <= 0)
        {
            character.characterAnimatorManager.lastDamageAnimationPlayed = "Guard_Break_01";
            character.characterAnimatorManager.PlayTargetActionAnimation("Guard_Break_01", true);
            character.isBlocking = false;

            //Play SFX when Guard is Broken
            character.characterSoundFXManager.PlayGuardBrokenSoundFX();

            result = true;
        }

        return result;
    }

    private void PlayDamageVFX(CharacterManager character)
    {
        //e.g. If we have Fire Damage, Play Fire Particle Effects
        // if (finalDamageDealt > 0f)
        // {
        //     character.characterEffectsManager.PlayBloodSplatterVFX(contactPoint);
        // }

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
            //damagedCharacter.characterSoundFXManager.PlayTakeDamageGrunts();
        }
        // else
        // {
        //     //Play a Pinging SFX based on how heavy the hit was
        // }

        damagedCharacter.characterSoundFXManager.PlayBlockSoundFX();

        //Add flourish to give impact to the player's successful Perfect Block
        if (damagedCharacter.isPerfectBlocking)
        {
            damagedCharacter.characterSoundFXManager.PlayPerfectGuardSFX();
        }
    }

    private void PlayDirectionalBasedBlockingAnimation(CharacterManager characterTakingDamage)
    {

        //Works without this, but the tutorial suggests it so Idk man(?)
        if (characterTakingDamage.isDead)
        {
            return;
        }


        //1. Calculate an "Intensity" based on Poise Damage
        DamageIntensity damageIntensity = WorldUtilityManager.instance.GetDamageIntensityBasedOnPoiseDamage(poiseDamage);

        //2. Play a Proper Animation to match the "Intensity" of the blocked blow

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

        //If poise is broken, play a staggering damage animation instead
        if (!CheckForGuardBreak(characterTakingDamage))
        {
            //Play appropriate block impact animation
            characterTakingDamage.characterAnimatorManager.lastDamageAnimationPlayed = damageAnimation;
            characterTakingDamage.characterAnimatorManager.PlayTargetActionAnimation(damageAnimation, true);
        }

    }

}


