using System;
using System.Collections.Generic;
using UnityEngine;

public class AICharacterStatsManager : CharacterStatsManager
{
    [Header("Amount of gold dropped")]
    public int goldDrop = 0;
    [Header("Gold drop chance as 0-1")]
    public float goldDropChance = 0;// % as 0-1
    [Header("Amount of expierence points dropped")]
    public int expDropAmt = 0;
    [Header("Tinker component drop chance as 0-1")]
    public List<TinkerComponentDropChance> componentDropChances = new List<TinkerComponentDropChance>();
    [Serializable] public class TinkerComponentDropChance { public TinkerComponentType type; public float dropChance; } //Allows for simply setting drop chances in editor
    public void DoAllDrops(bool isHitByMainHand, bool isHitByOffHand)
    {
        if (expDropAmt > 0)
            DropExp(isHitByMainHand, isHitByOffHand);
        if (goldDropChance > UnityEngine.Random.value)
            DropGold();
        foreach(TinkerComponentDropChance componentDropChance in componentDropChances)
        {
            if (componentDropChance.dropChance > UnityEngine.Random.value)
            {
                DropComponent(componentDropChance.type);
            }
        }
    }
    /** drop this characters gold */
    public void DropGold()
    {
        ItemDropManager.instance.DropGold(transform, goldDrop);
    }
    public void DropExp(bool isHitByMainHand, bool isHitByOffHand)
    {
        //randomize posistion
        Transform t = transform;
        Vector3 p = t.position;
        const float RADIUS = 1.0f;//drop radius should be as small as the scrawniest enemy to avoid dropping through walls
        float newX = p.x + ((UnityEngine.Random.value * RADIUS) * (UnityEngine.Random.value > 0.5? -1:1));
        //float newY = p.y + ((Random.value * RADIUS) * (Random.value > 0.5 ? -1 : 1));
        float newZ = p.z + ((UnityEngine.Random.value * RADIUS) * (UnityEngine.Random.value > 0.5 ? -1 : 1));
        t.position = new Vector3 (newX, p.y, newZ);
        //drop the exp
        ItemDropManager.instance.DropExp(t, expDropAmt, isHitByMainHand, isHitByOffHand);
    }
    /** 
     *  Drops a TinkerComponent at this objects location 
     *  Returns a reference to the dropped TinkerComponent
     */
    public GameObject DropComponent(TinkerComponentType type)
    {
        return ItemDropManager.DropComponent(type, transform);
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
