using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCombatManager : MonoBehaviour
{
    CharacterManager character;
    public WeaponScript currentWeaponBeingUsed;

    [Header("Attack Target")]
    public CharacterManager currentTarget;

    [Header("Lock On Transform")]
    public Transform LockOnTransform;

    public virtual void Awake() {
        character = GetComponent<CharacterManager>();
    }

    public virtual void SetTarget(CharacterManager newTarget) {
        if (newTarget != null) {
            currentTarget = newTarget;
        }
        else {
            currentTarget = null;
        }
    }
    
}
