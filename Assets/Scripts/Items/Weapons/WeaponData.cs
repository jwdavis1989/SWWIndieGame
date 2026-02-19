using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Weapon Data")]
public class WeaponData : ScriptableObject
{
    [Header("Static weapon information")]

    [Header("Unique I.D.\nCase insensitive")]
    public string itemId;
    public string weaponFamiliy;

    [Header("Base Weapon Attributes")]
    public float baseAttack = 1.0f;
    public float maxAttack = 1.0f;
    public float basePoiseDamage = 35f; //Base 35 in case it's caused by traps
    public float baseDurability = 1;
    public float maxDurability = 1;
    public float baseBlock = 1.0f;
    public float maxBlock = 1.0f;
    public float baseStability = 1.0f;
    public float maxStability = 1.0f;
    public ElementalStats baseElemental;
    public ElementalStats maxElemental;


    [Header("NOTE: I'm uncertain about all data below here and if it will be used.")]

    public float maxSpeed = 1.0f;
    public float xpToLevel = 100.0f;
    public int tinkerPointsPerLvl = 1;
    public float experiencePointsToNextLevel = 100.0f;

    [Header("Stamina Costs")]
    public float baseStaminaCost = 20f;

    //Light
    public float lightAttack01StaminaCostModifier = 1f;
    public float lightAttack02StaminaCostModifier = 1f;
    public float lightAttack03StaminaCostModifier = 1f;

    //Heavy
    public float heavyAttack01StaminaCostModifier = 1.2f;
    public float heavyAttack02StaminaCostModifier = 1.2f;

    //Jump Attacks
    public float lightJumpAttack01StaminaCostModifier = 1f;
    public float heavyJumpAttack01StaminaCostModifier = 4f;

    //Running
    public float lightRunningAttack01StaminaCostModifier = 1f;

    //Rolling
    public float lightRollingAttack01StaminaCostModifier = 1f;

    //Backstepping
    public float lightBackstepAttack01StaminaCostModifier = 1f;

    [Header("Motion Values")]
    //Light
    public float lightAttack01DamageMotionValue = 1f;
    public float lightAttack02DamageMotionValue = 1.1f;
    public float lightAttack03DamageMotionValue = 1.2f;

    //Heavy
    public float heavyAttack01DamageMotionValue = 1.4f;
    public float heavyAttack02DamageMotionValue = 1.6f;

    //Charged Heavy
    public float heavyChargedAttack01DamageMotionValue = 2.0f;
    public float heavyChargedAttack02DamageMotionValue = 2.2f;

    //Jump Attacks
    public float lightJumpAttack01DamageMotionValue = 1f;
    public float heavyJumpAttack01DamageMotionValue = 1.8f;

    //Running
    public float lightRunningAttack01DamageMotionValue = 1f;

    //Rolling
    public float lightRollingAttack01DamageMotionValue = 1f;

    //Backstepping
    public float lightBackstepAttack01DamageMotionValue = 1f;

    //Spells
    public float areaSpellAttack01DamageMotionValue = 1f;

    //Guns
    public float singleTargetBulletAttack01DamageMotionValue = 1f;
}
