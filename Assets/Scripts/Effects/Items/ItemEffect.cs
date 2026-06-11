using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Character Effects/Instant Effects/Item Effect")]

public class ItemEffect : InstantCharacterEffect
{
    [Header("Unique I.D.\nCase insensitive")]
    public string itemId;
}
