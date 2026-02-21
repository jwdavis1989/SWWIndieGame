using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Item Data/Tinker Component Data")]

public class TinkerComponentData : ScriptableObject
{
    [Header("Unique I.D.\nCase insensitive")]
    public string itemId;
    public string componentTier = "tier_1"; //tier_1,tier_2

    [Header("Base Stats")]
    public float attack = 0;
    public float durability = 0;
    public float block = 0;
    public float stability = 0;
    public ElementalStats elementalStats = new ElementalStats();
}
