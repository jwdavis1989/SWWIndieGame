using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatManager : CharacterCombatManager
{
    PlayerManager player;

    //[Header("Flags")]
    //public bool canComboWithMainHandWeapon = false;

    public override void SetTarget(CharacterManager newTarget) {
        base.SetTarget(newTarget);

        PlayerCamera.instance.SetLockCameraHeight();
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
