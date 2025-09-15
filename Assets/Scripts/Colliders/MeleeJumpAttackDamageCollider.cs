using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeJumpAttackDamageCollider : MeleeWeaponDamageCollider
{
    public GameObject fireJumpAttackVFX;

    public void PlayJumpAttackImpactVFX()
    {
        //if fire is highest element
        Instantiate(fireJumpAttackVFX, transform.position, Quaternion.identity);
    }
}
