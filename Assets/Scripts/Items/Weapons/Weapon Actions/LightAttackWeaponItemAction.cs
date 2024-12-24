using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character Actions/Weapon Actions/Light Attack Action")]
public class LightAttackWeaponItemAction : WeaponItemAction
{
    [SerializeField] string light_attack_01 = "Main_Hand_Light_Attack_01";
    [SerializeField] string light_attack_02 = "Main_Hand_Light_Attack_02";
    [SerializeField] string light_attack_03 = "Main_Hand_Light_Attack_03";

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
        //If we are attack currently, and able to combo, we perform a combo attack
        if (characterPerformingAction.characterCombatManager.canComboWithMainHandWeapon && characterPerformingAction.isPerformingAction) {
            characterPerformingAction.characterCombatManager.canComboWithMainHandWeapon = false;

            //Perform an attack based on the previous attack we just played
            if (characterPerformingAction.characterCombatManager.lastAttackAnimationPerformed == light_attack_01) {
                characterPerformingAction.characterAnimatorManager.PlayTargetAttackActionAnimation(AttackType.LightAttack02, light_attack_02, true);
            }
            else if (characterPerformingAction.characterCombatManager.lastAttackAnimationPerformed == light_attack_02) {
                characterPerformingAction.characterAnimatorManager.PlayTargetAttackActionAnimation(AttackType.LightAttack03, light_attack_03, true);
            }
        }
        //Otherwise, just perform a regular attack if you aren't already performing an action
        else if (!characterPerformingAction.isPerformingAction) {
            characterPerformingAction.characterAnimatorManager.PlayTargetAttackActionAnimation(AttackType.LightAttack01, light_attack_01, true);
        }
    }
}
