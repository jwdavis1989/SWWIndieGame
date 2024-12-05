using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStatsManager : CharacterStatsManager
{
    [Header("Amount of gold dropped")]
    public int goldDrop = 0;
    [Header("Gold drop chance as 0-1")]
    public float goldDropChance = 0;// % as 0-1
    [Header("Amount of expierence points dropped for weapon")]
    public int expDropAmt = 0;
    [Header("Tinker component drop chance as 0-1")]
    public float[] componentDropChances = new float[(int)TinkerComponentType.Weapon]; //drop chance of all components (exluding weapon components)
    public KeyValuePair<TinkerComponentType, float>[] componentDropChances3 = new KeyValuePair<TinkerComponentType, float>[(int)TinkerComponentType.Weapon];
    //public Dictionary<TinkerComponentType, float> componentDropChances = new Dictionary<TinkerComponentType, float>();
    //public float[] weaponDropChances = new float[(int)WeaponType.UNKNOWN]; //drop chance of all weapons
    public void DoAllDrops(bool isMainHand)
    {
        if (expDropAmt > 0)
            DropExp(isMainHand);
        if (goldDropChance > Random.value)
            DropGold();
        for(int i = 0; i < componentDropChances.Length; i++)
        {
            if (componentDropChances[i] > Random.value)
            {
                DropComponent((TinkerComponentType)i);
            }
        }
        //for (int i = 0; i < weaponDropChances.Length; i++)
        //{
        //    if (weaponDropChances[i] > Random.value)
        //    {
        //        DropWeapon((WeaponType)i);
        //    }
        //}
    }
    public float GetComponentDropChance(TinkerComponentType type)
    {
        return componentDropChances[(int)type];
    }
    public void DropGold()
    {
        ItemDropContainer.instance.DropGold(transform, goldDrop);
    }
    public void DropExp(bool isMainHand)
    {
        Transform t = transform;
        Vector3 p = t.position; 
        float newX = p.x + (Random.value * (Random.value > 0.5? -1:1));
        float newY = p.y + (Random.value * (Random.value > 0.5 ? -1 : 1));
        float newZ = p.z + (Random.value * (Random.value > 0.5 ? -1 : 1));
        t.position = new Vector3 (newX, newY, newZ);
        ItemDropContainer.instance.DropExp(t, expDropAmt, isMainHand);
    }
    /** 
     *  Drops a TinkerComponent at this objects location 
     *  Returns a reference to the dropped TinkerComponent
     */
    public GameObject DropComponent(TinkerComponentType type)
    {
        return Instantiate(ItemDropContainer.instance.DropComponent(type, transform));
    }
    /** 
     *  Drops a weapon at this objects location 
     *  Returns a reference to the dropped weapon
     */
    //public GameObject DropWeapon(WeaponType type)
    //{
    //    return Instantiate(ItemDropContainer.instance.DropWeapon(type, transform));
    //}
    //public float GetWeaponDropChance(WeaponType type)
    //{
    //    return weaponDropChances[(int)type];
    //}
}
