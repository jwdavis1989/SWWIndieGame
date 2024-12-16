using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character Actions/Weapon Actions/Heavy Attack Action")]
public class HeavyAttackWeaponItemAction : WeaponItemAction
{
    [SerializeField] string heavy_attack_01 = "Main_Hand_Heavy_Attack_01";

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

        PerformHeavyAttack(characterPerformingAction);
    }

    private void PerformHeavyAttack(CharacterManager characterPerformingAction) {
        characterPerformingAction.characterAnimatorManager.PlayTargetAttackActionAnimation(AttackType.HeavyAttack01, heavy_attack_01, true);
    }
}
