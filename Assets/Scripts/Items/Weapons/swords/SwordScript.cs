using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordScript : WeaponScript
{
    public override void attackTarget(GameObject target)
    {
        //Currently will call base attackTarget then add any extra operations needed for this weapon
        base.attackTarget(target);
        Debug.Log("Sword attackTarget called.");//ASTEST
    }
}
/** Change Log  
 *  Date         Developer  Description
 *  09/16/2024   Alec       New.
 *  
 * */