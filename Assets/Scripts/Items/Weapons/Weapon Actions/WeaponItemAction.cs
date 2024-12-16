using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character Actions/Weapon Actions/Test Action")]
public class WeaponItemAction : ScriptableObject
{
    public int actionID;

    public virtual void AttemptToPerformAction(CharacterManager characterPerformingAction) {
        // What does every weapon action have in common?
        // 1. Keep track of which weapon is currently being used
        // playerPerformingAction.PlayerCombatManager.currentWeaponBeingUsed = weaponPerformingAction;
        // PlayerWeaponManager.instance.

        //Debug.Log("The Action has fired!");
    }
}
