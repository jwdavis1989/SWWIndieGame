using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
/** 
 * Enum of all weapon types.
 */
public enum WeaponType
{
    Shortsword,
    Wrench,
    BastardSword,
    BroadSword,
    BoneBlade,
    ReinforcedWrench,
    //specialty weapons
    Dagger,
    Flintlock,
    SparkCaster,
    BowieKnife,
    Revolver,
    ScrapGun,
    ZapCaster,
    BurnCaster,
    FreezeCaster,
    //T3 Weapons
    DiamondSword,
    //Limit
    UNKNOWN
}
/*
 * Serializable WeaponStats used for JSON saving
 */
[Serializable]
public class WeaponStats
{
    [Header("Weapon Type - Important - Set in Prefab")]
    public WeaponType weaponType = 0;
    [Header("Weapon Attributes (Intialized by JSON)")]
    public float attack = 1.0f;
    public float maxAttack = 1.0f;
    public float durability = 1;
    public float maxDurability = 1;
    public float block = 1.0f;
    public float maxBlock = 1.0f;
    public float stability = 1.0f;
    public float maxStability = 1.0f;
    public ElementalStats elemental;
    public ElementalStats maxElemental;
    public float speed = 1.0f;
    public float maxSpeed = 1.0f;
    public float specialtyCooldown = 0;
    public float maxSpecialtyCooldown = 0;
    public float xpToLevel = 100.0f;
    public int tinkerPointsPerLvl = 0;
    public float currentDurability = 1.0f;
    public int level = 1;
    public float currentExperiencePoints = 0.0f;
    public int currentTinkerPoints = 0;
    public String weaponName = "BaseWeaponName";
}
/*
 * Serializable ElementalStats used for JSON saving
 */
[Serializable]
public class ElementalStats
{
    public float firePower = 0;
    public float icePower = 0;
    public float lightningPower = 0;
    public float windPower = 0;
    public float earthPower = 0;
    public float lightPower = 0;
    public float beastPower = 0;
    public float scalesPower = 0;
    public float techPower = 0;
    public ElementalStats Subract(ElementalStats subtractor)
    {
        ElementalStats diff = new ElementalStats();
        diff.firePower = firePower - subtractor.firePower;
        diff.icePower = icePower - subtractor.icePower;
        diff.lightningPower = lightningPower - subtractor.lightningPower;
        diff.windPower = windPower - subtractor.windPower;
        diff.earthPower = earthPower - subtractor.earthPower;
        diff.lightPower = lightPower - subtractor.lightningPower;
        diff.beastPower = beastPower - subtractor.beastPower;
        diff.scalesPower = scalesPower - subtractor.scalesPower;
        diff.techPower = techPower - subtractor.techPower;
        return diff;
    }
    public ElementalStats Add(ElementalStats other)
    {
        ElementalStats sum = new ElementalStats();
        sum.firePower = firePower + other.firePower;
        sum.icePower = icePower + other.icePower;
        sum.lightningPower = lightningPower + other.lightningPower;
        sum.windPower = windPower + other.windPower;
        sum.earthPower = earthPower + other.earthPower;
        sum.lightPower = lightPower + other.lightningPower;
        sum.beastPower = beastPower + other.beastPower;
        sum.scalesPower = scalesPower + other.scalesPower;
        sum.techPower = techPower + other.techPower;
        return sum;
    }
}
/** 
 * Base weapon script inherited by other weapons - use for things all weapons should do
 * MonoBehaviour - Can add to Game Objects
 */
public class WeaponScript : MonoBehaviour
{
    [Header("Weapon Damage Collider")]
    [SerializeField] MeleeWeaponDamageCollider meleeWeaponDamageCollider;

    [Header("Currently set on prefab")]
    public bool isSpecialWeapon = false;
    [Header("Will appear as ??? on weapon sheet until obtained")]
    public bool hasObtained = false;
    [Header("Important: Set weapon type (Otherwise intialized by JSON)")]
    public WeaponStats stats;

    public void Awake() {
        meleeWeaponDamageCollider = GetComponentInChildren<MeleeWeaponDamageCollider>();
        if (!isSpecialWeapon && meleeWeaponDamageCollider) {
            SetWeaponDamage();
        }
    }

    //Might remove
    public void SetWeaponDamage() {
        //Redundant check for now, but can be used later if we decide to update monsters to use the weapon system
        // if (WeaponsController.instance.characterThatOwnsThisArsenal.isPlayer) {
            meleeWeaponDamageCollider.characterCausingDamage = PlayerWeaponManager.instance.characterThatOwnsThisArsenal;
        // }
        // else {
        //     //Monster CharacterManager Weapon Assignment in hypothetical rework
        // }
        
        meleeWeaponDamageCollider.physicalDamage = stats.attack;
        meleeWeaponDamageCollider.fireDamage = stats.elemental.firePower;
        meleeWeaponDamageCollider.iceDamage = stats.elemental.icePower;
        meleeWeaponDamageCollider.lightningDamage = stats.elemental.lightningPower;
        meleeWeaponDamageCollider.windDamage = stats.elemental.windPower;
        meleeWeaponDamageCollider.earthDamage = stats.elemental.earthPower;
        meleeWeaponDamageCollider.lightDamage = stats.elemental.lightPower;
        meleeWeaponDamageCollider.beastDamage = stats.elemental.beastPower;
        meleeWeaponDamageCollider.scalesDamage = stats.elemental.scalesPower;
        meleeWeaponDamageCollider.techDamage = stats.elemental.techPower;
    }

    public virtual void attackTarget(GameObject target)
    {
        Debug.Log("BaseWeaponScript stats.attackTarget called.");//ASTEST
        if (target != null) {
            //calculateElementalDamage(stats.attack, target);
            //target.GetComponent<EnemyController>().hp -= stats.attack;
            //TODO
            //play weapon animation
            //set reload/recharge
        }
    }
    public float CalculateTotalDamage(CharacterManager targetCharacter)
    {
        float result = stats.attack * (1 - targetCharacter.characterStatsManager.physicalDefense);

        //I feel like there should be a way to do this iteratively, but with the ElementalStats class as it is, I don't know of any way to do so atm.
        result += stats.attack * (stats.elemental.firePower * 0.005f) * (1 - targetCharacter.characterStatsManager.elementalDefenses.firePower);
        result += stats.attack * (stats.elemental.icePower * 0.005f) * (1 - targetCharacter.characterStatsManager.elementalDefenses.icePower);
        result += stats.attack * (stats.elemental.lightningPower * 0.005f) * (1 - targetCharacter.characterStatsManager.elementalDefenses.lightningPower);
        result += stats.attack * (stats.elemental.windPower * 0.005f) * (1 - targetCharacter.characterStatsManager.elementalDefenses.windPower);
        result += stats.attack * (stats.elemental.earthPower * 0.005f) * (1 - targetCharacter.characterStatsManager.elementalDefenses.earthPower);
        result += stats.attack * (stats.elemental.lightPower * 0.005f) * (1 - targetCharacter.characterStatsManager.elementalDefenses.lightPower);
        result += stats.attack * (stats.elemental.beastPower * 0.005f) * (1 - targetCharacter.characterStatsManager.elementalDefenses.beastPower);
        result += stats.attack * (stats.elemental.scalesPower * 0.005f) * (1 - targetCharacter.characterStatsManager.elementalDefenses.scalesPower);
        result += stats.attack * (stats.elemental.techPower * 0.005f) * (1 - targetCharacter.characterStatsManager.elementalDefenses.techPower);

        if(result > 0) {
            return result;
        }
        else return 0;
    }

}
/** Change Log  
 *  Date         Developer  Description
 *  09/16/2024   Alec       New.
 *  
 * */