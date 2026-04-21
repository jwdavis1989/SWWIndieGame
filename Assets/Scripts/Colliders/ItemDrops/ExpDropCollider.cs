using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpDropCollider : MonoBehaviour
{
    [Header("Amount of exp to give to player's weapon")]
    public float exp = 0;
    [Header("Flag to tell which weapon to give exp to. Will split if both true")]
    public bool isMainHandExp = false;
    public bool isOffHandExp = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            bool isSplit = isMainHandExp && isOffHandExp;
            //add exp to wpn and special wpn
            if (isMainHandExp && PlayerWeaponManager.instance.GetMainHand())
            {
                PlayerWeaponManager.instance.GetMainHand().GetComponent<WeaponScript>().AddExp(isSplit ? exp / 2 : exp);
            }
            if (isOffHandExp && PlayerWeaponManager.instance.GetOffHand())
            {
                PlayerWeaponManager.instance.GetOffHand().GetComponent<WeaponScript>().AddExp(isSplit ? exp / 2 : exp);
            }
            Destroy(gameObject.GetComponentInParent<Rigidbody>().gameObject);
        }
    }
}
