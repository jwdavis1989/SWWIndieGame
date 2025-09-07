using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character Actions/Weapon Actions/Heavy Attack Action")]
public class HeavyAttackWeaponItemAction : WeaponItemAction
{
    [Header("Heavy Attacks")]
    [SerializeField] string heavy_attack_01 = "Main_Hand_Heavy_Attack_01";
    [SerializeField] string heavy_attack_02 = "Main_Hand_Heavy_Attack_02";

    [Header("Jump Attacks")]
    [SerializeField] string heavy_jump_attack_01 = "Main_Heavy_Jump_Attack_01";

    public override void AttemptToPerformAction(CharacterManager characterPerformingAction)
    {
        base.AttemptToPerformAction(characterPerformingAction);

        //Check for Stops:

        //Out of Stamina?
        if (characterPerformingAction.characterStatsManager.currentStamina <= 0)
        {
            return;
        }

        //Not grounded? This should be a jump attack instead
        if (!characterPerformingAction.isGrounded)
        {
            PerformHeavyJumpAttack(characterPerformingAction);
            return;
        }

        //Stops us from starting a heavy attack just as we jump if timed perfectly
        if (characterPerformingAction.isJumping)
        {
            return;
        }

        PerformHeavyAttack(characterPerformingAction);
    }

    private void PerformHeavyAttack(CharacterManager characterPerformingAction)
    {
        //characterPerformingAction.characterAnimatorManager.PlayTargetAttackActionAnimation(AttackType.HeavyAttack01, heavy_attack_01, true);

        //If we are attack currently, and able to combo, we perform a combo attack
        if (characterPerformingAction.characterCombatManager.canComboWithMainHandWeapon && characterPerformingAction.isPerformingAction)
        {
            characterPerformingAction.characterCombatManager.canComboWithMainHandWeapon = false;

            //Perform an attack based on the previous attack we just played
            if (characterPerformingAction.characterCombatManager.lastAttackAnimationPerformed == heavy_attack_01)
            {
                characterPerformingAction.characterAnimatorManager.PlayTargetAttackActionAnimation(AttackType.HeavyAttack02, heavy_attack_02, true);
            }
        }
        //Otherwise, just perform a regular attack if you aren't already performing an action
        else if (!characterPerformingAction.isPerformingAction)
        {
            characterPerformingAction.characterAnimatorManager.PlayTargetAttackActionAnimation(AttackType.HeavyAttack01, heavy_attack_01, true);
        }
    }
    
    private void PerformHeavyJumpAttack(CharacterManager characterPerformingAction)
    {
        if (characterPerformingAction.isPerformingAction)
        {
            return;
        }

        characterPerformingAction.characterAnimatorManager.PlayTargetAttackActionAnimation(AttackType.HeavyJumpAttack01, heavy_jump_attack_01, true);
    }
}
