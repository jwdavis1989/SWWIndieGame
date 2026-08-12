using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Item Data/Weapon Trait Data")]
public class WeaponTraitData : ScriptableObject
{
    public string traitId;
    public string displayName;
    [TextArea]public string description;
    public Sprite icon;
    public bool inheritable = false;
    public InstantCharacterEffect onHitEffect = null;
}
