using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WrenchScript : WeaponScript
{
    public override void attackTarget(GameObject target)
    {
        //Currently will call base attackTarget then add any extra operations needed for this weapon
        base.attackTarget(target);
        Debug.Log("WrenchScript attackTarget called.");//ASTEST
    }
}
