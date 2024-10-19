using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
/** 
 * Base weapon script inherited by other weapons - use for things all weapons should do
 */
[Serializable]
public class BaseWeaponScript : MonoBehaviour
{


    [Header("Weapon Type - Important - Set in Prefab")]
    public WeaponType weaponType = 0;
    [Header("Weapon Attributes (Intialized by JSON)")]
    public float attack = 1.0f;
    public float maxAttack = 1.0f;
    public int durability = 1;
    public int maxDurability = 1;
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
    
    [Header("Weapon State (Intialized by JSON)")]
    public float currentDurability = 1.0f;
    public int level = 1;
    public float currentExperiencePoints = 0.0f;
    public int currentTinkerPoints = 0;
    public String weaponName = "BaseWeaponName";
    [Header("Will appear as ??? on weapon sheet until obtained")]
    public bool hasObtained = false;

    /**
     * Currently set to virtual so must override with other script. Can call this from override
     */
    public virtual void attackTarget(GameObject target)
    {
        Debug.Log("BaseWeaponScript attackTarget called.");//ASTEST
        if (target != null) {
            //calculateElementalDamage(attack, target);
            //calculateAntiTypeDamage(attack, target);
            //target.GetComponent<EnemyController>().hp -= attack;
            //TODO
            //play weapon animation
            //set reload/recharge
        }
    }
    private float calculateElementalDamage(float attack, Collider other)
    {
        float result = 0;
        //foreach(int element in elementList) {
        //    result += attack * (element * 0.005) * (1 - other.elementalDefense(element));
        //}
        if(result < 0) {
            return result;
        }
        else return 0;
    }
    private float calculateAntiTypeDamage(float attack, Collider other)
    {
        float result = 0;
        //foreach(int antiType in antiTypeList) {
        //    result += attack * (antiType * 0.005) * (1 - other.antiTypeDefense(antiType));
        //}
        return result;
    }
    //move JSON Values into this object
    public void copy(BaseWeaponStats script)
    {
        this.attack = script.attack;
        this.speed = script.speed;
        this.specialtyCooldown = script.specialtyCooldown;
        this.block = script.block;
        this.stability = script.stability;
        this.xpToLevel = script.xpToLevel;
        this.durability = script.durability;
        this.elemental = script.elemental;
        this.tinkerPointsPerLvl = script.tinkerPointsPerLvl;
        this.maxAttack = script.maxAttack;
        this.maxSpeed = script.maxSpeed;
        this.maxSpecialtyCooldown = script.maxSpecialtyCooldown;
        this.maxBlock = script.maxBlock;
        this.maxStability = script.maxStability;
        this.maxDurability = script.maxDurability;
        this.maxElemental = script.maxElemental;

        this.level = script.level;
        this.currentExperiencePoints = script.currentExperiencePoints;
        this.currentTinkerPoints = script.currentTinkerPoints;
        this.weaponName = script.weaponName;
        this.weaponType = script.weaponType;
    }

 }
/** Change Log  
 *  Date         Developer  Description
 *  09/16/2024   Alec       New.
 *  
 * */