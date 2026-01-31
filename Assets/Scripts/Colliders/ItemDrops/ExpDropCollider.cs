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
    //[Header("Could be used to scale exp to amount like KH?")]
    //public float scale = 1.0f;//Could be used to scale exp to amount like KH?
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
    // Abandoned the idea of making this an inventory item as it's behavior is sufficently differnt.
    //public override void HandlePickup(GameObject player)
    //{
    //    base.HandlePickup(player);
    //    bool isSplit = isMainHandExp && isOffHandExp;
    //    //add exp to wpn and special wpn
    //    if (isMainHandExp && PlayerWeaponManager.instance.GetMainHand())
    //    {
    //        player.GetComponent<CharacterWeaponManager>().GetMainHand().GetComponent<WeaponScript>().AddExp(isSplit ? exp / 2 : exp);
    //    }
    //    if (isOffHandExp && PlayerWeaponManager.instance.GetOffHand())
    //    {
    //        player.GetComponent<CharacterWeaponManager>().GetOffHand().GetComponent<WeaponScript>().AddExp(isSplit ? exp / 2 : exp);
    //        //PlayerWeaponManager.instance.GetOffHand().GetComponent<WeaponScript>().AddExp(isSplit ? exp / 2 : exp);
    //    }
    //    //Destroy(gameObject);
    //}
}
