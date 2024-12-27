using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCombatManager : MonoBehaviour
{
    [HideInInspector] protected CharacterManager character;
    public WeaponScript currentWeaponBeingUsed;

    [Header("Attack Target")]
    public CharacterManager currentTarget;

    [Header("Last Attack Animation Performed")]
    public string lastAttackAnimationPerformed;

    [Header("Lock On Transform")]
    public Transform LockOnTransform;

    [Header("Flags")]
    public bool canComboWithMainHandWeapon = false;
    public bool canPerformRollingAttack = false;
    public bool canPerformBackStepAttack = false;

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
    
    public virtual void EnableCanDoCombo() {
        canComboWithMainHandWeapon = true;
    }

    public virtual void DisableCanDoCombo() {
        canComboWithMainHandWeapon = false;
    }

    public void EnableCanDoRollingAttack() {
        canPerformRollingAttack = true;
    }

    public void DisableCanDoRollingAttack() {
        canPerformRollingAttack = false;
    }

    public void EnableCanDoBackStepAttack() {
        canPerformBackStepAttack = true;
    }

    public void DisableCanDoBackStepAttack() {
        canPerformBackStepAttack = false;
    }

}
