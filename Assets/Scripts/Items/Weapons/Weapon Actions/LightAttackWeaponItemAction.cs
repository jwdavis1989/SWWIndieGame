using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character Actions/Weapon Actions/Light Attack Action")]
public class LightAttackWeaponItemAction : WeaponItemAction
{
    [SerializeField] string light_attack_01 = "Main_Hand_Light_Attack_01";

    public override void AttemptToPerformAction(CharacterManager characterPerformingAction) {
        base.AttemptToPerformAction(characterPerformingAction);

        //Check for Stops:

        //Out of Stamina?
        if (characterPerformingAction.characterStatsManager.currentStamina <= 0) {
            return;
        }

        //Not grounded? This should be a jump attack instead
        if (!characterPerformingAction.isGrounded) {
            return;
        }

        PerformLightAttack(characterPerformingAction);
    }

    private void PerformLightAttack(CharacterManager characterPerformingAction) {
        characterPerformingAction.characterAnimatorManager.PlayTargetAttackActionAnimation(light_attack_01, true);
    }
}
