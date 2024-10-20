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
        Debug.Log("BaseWeaponScript attackTarget called.");//ASTEST
        if (target != null) {
            //calculateElementalDamage(attack, target);
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
    public void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

}
/** Change Log  
 *  Date         Developer  Description
 *  09/16/2024   Alec       New.
 *  
 * */