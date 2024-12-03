using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpDropCollider : MonoBehaviour
{
    public float exp = 0;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //add exp to wpn and special wpn
            if (PlayerWeaponManager.instance.GetEquippedWeapon())
            {
                PlayerWeaponManager.instance.GetEquippedWeapon().GetComponent<WeaponScript>().AddExp(exp);
            }
            if (PlayerWeaponManager.instance.GetEquippedWeapon(true))
            {
                PlayerWeaponManager.instance.GetEquippedWeapon(true).GetComponent<WeaponScript>().AddExp(exp);
            }
            Destroy(gameObject);
        }
    }
}
