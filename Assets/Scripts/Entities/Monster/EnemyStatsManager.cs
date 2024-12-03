using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStatsManager : CharacterStatsManager
{
    public int goldDrop = 0;
    public float goldDropChance = 0;// % as 0-1
    public int expDrop = 0;

    public float[] componentDropChances = new float[(int)TinkerComponentType.Weapon]; //drop chance of all components (exluding weapon components)
    public KeyValuePair<TinkerComponentType, float>[] componentDropChances3 = new KeyValuePair<TinkerComponentType, float>[(int)TinkerComponentType.Weapon];
    //public Dictionary<TinkerComponentType, float> componentDropChances = new Dictionary<TinkerComponentType, float>();
    public float[] weaponDropChances = new float[(int)WeaponType.UNKNOWN]; //drop chance of all weapons
    public void DoAllDrops()
    {
        if (expDrop > 0)
            DropExp();
        if (goldDropChance > Random.value)
            DropGold();
        for(int i = 0; i < componentDropChances.Length; i++)
        {
            if (componentDropChances[i] > Random.value)
            {
                DropComponent((TinkerComponentType)i);
            }
        }
        for (int i = 0; i < weaponDropChances.Length; i++)
        {
            if (weaponDropChances[i] > Random.value)
            {
                DropWeapon((WeaponType)i);
            }
        }
    }
    public float GetWeaponDropChance(WeaponType type)
    {
        return weaponDropChances[(int)type];
    }
    public float GetComponentDropChance(TinkerComponentType type)
    {
        return componentDropChances[(int)type];
    }
    public void DropGold()
    {
        ItemDropContainer.instance.DropGold(transform, goldDrop);
    }
    public void DropExp()
    {
        ItemDropContainer.instance.DropExp(transform, expDrop);
    }
    /** 
     *  Drops a weapon at this objects location 
     *  Returns a reference to the dropped weapon
     */
    public GameObject DropWeapon(WeaponType type)
    {
        return Instantiate(ItemDropContainer.instance.DropWeapon(type, transform));
    }
    /** 
     *  Drops a TinkerComponent at this objects location 
     *  Returns a reference to the dropped TinkerComponent
     */
    public GameObject DropComponent(TinkerComponentType type)
    {
        return Instantiate(ItemDropContainer.instance.DropComponent(type, transform));
    }
}
