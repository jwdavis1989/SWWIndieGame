using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCombatManager : MonoBehaviour
{
    [HideInInspector] CharacterManager character;
    public WeaponScript currentWeaponBeingUsed;

    [Header("Attack Target")]
    public CharacterManager currentTarget;

    [Header("Last Attack Animation Performed")]
    public string lastAttackAnimationPerformed;

    [Header("Lock On Transform")]
    public Transform LockOnTransform;

    [Header("Flags")]
    public bool canComboWithMainHandWeapon = false;

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
