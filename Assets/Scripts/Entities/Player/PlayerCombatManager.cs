using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatManager : CharacterCombatManager
{
    PlayerManager player;

    //[Header("Flags")]
    //public bool canComboWithMainHandWeapon = false;

    public override void SetTarget(CharacterManager newTarget) {
        if (currentTarget) {
            currentTarget.characterCombatManager.DisableLockOnVFX();
        }
        base.SetTarget(newTarget);
        PlayerCamera.instance.SetLockCameraHeight();
        newTarget.characterCombatManager.EnableLockOnVFX();
    }

    public override void Awake() {
        base.Awake();
        player = GetComponent<PlayerManager>();
    }

    //Animation Event Calls
    public override void EnableCanDoCombo() {
        canComboWithMainHandWeapon = true;
    }

    public override void DisableCanDoCombo() {
        canComboWithMainHandWeapon = false;
    }

}
