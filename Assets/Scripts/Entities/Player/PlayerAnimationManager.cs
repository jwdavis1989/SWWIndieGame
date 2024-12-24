using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationManager : CharacterAnimatorManager
{
    // PlayerManager player;

    // protected override void Awake() {
    //     base.Awake();
    //     player = GetComponent<PlayerManager>();
    // }


    //Animation Event Calls
    public override void EnableCanDoCombo() {
        //Might need to add this variable somewhere, as it's added in episode 33.
        //if (player.isUsingRightHand) {
            character.characterCombatManager.canComboWithMainHandWeapon = true;
        //}
        //else {
            //"Enable combo for off hand weapon"
        //}
    }

    public override void DisableCanDoCombo() {
        character.characterCombatManager.canComboWithMainHandWeapon = false;
        //canComboWithOffHandWeapon = false;
    }


}
