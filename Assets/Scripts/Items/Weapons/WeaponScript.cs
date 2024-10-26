using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
/** 
 * Base weapon script inherited by other weapons - use for things all weapons should do
 */
public class WeaponScript : MonoBehaviour
{
    [Header("Currently set on prefab")]
    public bool isSpecialWeapon = false;
    [Header("Will appear as ??? on weapon sheet until obtained")]
    public bool hasObtained = false;
    [Header("Important: Set weapon type (Otherwise intialized by JSON)")]
    public WeaponStats stats;
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