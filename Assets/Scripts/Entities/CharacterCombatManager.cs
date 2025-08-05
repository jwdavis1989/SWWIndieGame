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
    public GameObject LockOnVFX;

    [Header("Flags")]
    public bool canComboWithMainHandWeapon = false;
    public bool canPerformRollingAttack = false;
    public bool canPerformBackStepAttack = false;
    public bool canBlock = true;

    public virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
    }

    public virtual void SetTarget(CharacterManager newTarget)
    {
        if (newTarget != null)
        {
            currentTarget = newTarget;
        }
        else
        {
            currentTarget = null;
        }
    }

    public virtual void EnableCanDoCombo()
    {
        canComboWithMainHandWeapon = true;
    }

    public virtual void DisableCanDoCombo()
    {
        canComboWithMainHandWeapon = false;
    }

    public void EnableCanDoRollingAttack()
    {
        canPerformRollingAttack = true;
    }

    public void DisableCanDoRollingAttack()
    {
        canPerformRollingAttack = false;
    }

    public void EnableCanDoBackStepAttack()
    {
        canPerformBackStepAttack = true;
    }

    public void DisableCanDoBackStepAttack()
    {
        canPerformBackStepAttack = false;
    }

    public void EnableLockOnVFX()
    {
        LockOnVFX.SetActive(true);
    }

    public void DisableLockOnVFX()
    {
        LockOnVFX.SetActive(false);
    }

    //Animation Event Calls
    public void InstantiateSpellWarmUpFX()
    {
        character.characterWeaponManager.ownedSpecialWeapons[character.characterWeaponManager.indexOfEquippedSpecialWeapon].GetComponent<WeaponScript>().InstantiateWarmUpSpellFX(character);
    }

    public void SuccessfullyCastSpell()
    {
        character.characterWeaponManager.ownedSpecialWeapons[character.characterWeaponManager.indexOfEquippedSpecialWeapon].GetComponent<WeaponScript>().SuccessfullyCastSpell(character);
    }

    public void SuccessfullyCastSpellFullCharge()
    {
        character.characterWeaponManager.ownedSpecialWeapons[character.characterWeaponManager.indexOfEquippedSpecialWeapon].GetComponent<WeaponScript>().SuccessfullyCastSpellFullCharge(character);
    }

    //Used to destroy things like a "Drawn Arrow" or "Spell Warm Up FX" when the character's poise is broken
    public void DestroyAllCurrentActionFX()
    {
        if (character.characterEffectsManager.activeSpellWarmUpFX != null)
        {
            Destroy(character.characterEffectsManager.activeSpellWarmUpFX);
        }
    }

}
