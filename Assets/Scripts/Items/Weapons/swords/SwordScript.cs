using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordScript : BaseWeaponScript
{
    ////Copied from table @ https://docs.google.com/document/d/1XTMwjepFqAUYWYT6TnrkX5UhiAmdh7Zp2XyzFjKgP5M/edit#heading=h.2gazcsgmxkub
    ////Weapon Attributes
    //new public float attack = 15.0f;
    //new public int maxDurability = 100;
    //new public float block = 20.0f;
    //new public float stability = 0.0f;
    //new public int firePower = 0;
    //new public int icePower = 0;
    //new public int lightningPower = 0;
    //new public int windPower = 0;
    //new public int earthPower = 10;
    //new public int lightPower = 0;
    //new public int beastPower = 10;
    //new public int scalesPower = 0;
    //new public int techPower = 0;
    //new public int tinkerPointsPerLvl = 1;
    //new public WeaponType weaponType = WeaponType.Shortsword;
    ////Default value copied from base class.
    //new public float speed = 1.0f;
    
    ////Weapon State
    ////Set to maxDurability. This could be done in Start() if convienent
    //new public float currentDurability = 100.0f;

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