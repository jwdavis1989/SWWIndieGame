using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatManager : CharacterCombatManager
{
    public override void SetTarget(CharacterManager newTarget) {
        base.SetTarget(newTarget);

        PlayerCamera.instance.SetCameraHeight();
    }
}
