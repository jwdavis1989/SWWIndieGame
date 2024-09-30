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
    [Header("Weapon Attributes (Intilized by JSON)")]
    public float attack = 1.0f;
    public float speed = 1.0f;
    public float specialtyCooldown = 0;
    public float block = 1.0f;
    public float stability = 1.0f;
    //public float[] xpToLevel;
    public float xpToLevel = 100.0f;
    public int maxDurability = 1;
    public int firePower = 0;
    public int icePower = 0;
    public int lightningPower = 0;
    public int windPower = 0;
    public int earthPower = 0;
    public int lightPower = 0;
    public int beastPower = 0;
    public int scalesPower = 0;
    public int techPower = 0;
    public int tinkerPointsPerLvl = 0;
    
    [Header("Weapon State (Intilized by JSON)")]
    public float currentDurability = 1.0f;
    public int level = 1;
    public float currentExperiencePoints = 0.0f;
    public int currentTinkerPoints = 0;
    public String weaponName = "BaseWeaponName";

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
    public void copy(BaseWeaponStats script)
    {
        this.attack = script.attack;
        this.speed = script.speed;
        this.specialtyCooldown = script.specialtyCooldown;
        this.block = script.block;
        this.stability = script.stability;
        this.xpToLevel = script.xpToLevel;
        this.maxDurability = script.maxDurability;
        this.firePower = script.firePower;
        this.icePower = script.icePower;
        this.lightningPower = script.lightningPower;
        this.windPower = script.windPower;
        this.earthPower = script.earthPower;
        this.lightPower = script.lightPower;
        this.beastPower = script.beastPower;
        this.scalesPower = script.scalesPower;
        this.techPower = script.techPower;
        this.tinkerPointsPerLvl = script.tinkerPointsPerLvl;
        this.currentDurability = script.currentDurability;
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