using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Item Data/Tinker Component Data")]

public class TinkerComponentData : ScriptableObject
{
    [Header("Unique I.D.\nCase insensitive")]
    public string itemId;
    public string componentTier = "tier_1"; //tier_1,tier_2

    [Header("Base Stats")]
    public TinkerComponentStats stats = new TinkerComponentStats();
}
