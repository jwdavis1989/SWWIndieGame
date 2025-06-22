using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[CreateAssetMenu(menuName ="A.I./States/Attack")]
public class AttackState : AIState {

    [Header("Current Attack")]
    [HideInInspector] public AiCharacterAttackAction currentAttack;
    [HideInInspector] public bool willPerformCombo = false;

    [Header("State Flags")]
    protected bool hasPerformedAttack = false;
    protected bool hasPerformedCombo = false;

    [Header("Pivot After Attack")]
    [SerializeField] protected bool pivotAfterAttack = false;

    public override AIState Tick(AICharacterManager aiCharacter) {
        if (aiCharacter.aiCharacterCombatManager.currentTarget == null || aiCharacter.aiCharacterCombatManager.currentTarget.isDead) {
            return SwitchState(aiCharacter, aiCharacter.idleState);
        }

        //Rotate towards the target whilst attacking
        aiCharacter.aiCharacterCombatManager.RotateTowardsTargetWhileAttacking(aiCharacter);

        //Set movement values to 0
        aiCharacter.characterAnimatorManager.UpdateAnimatorMovementParameters(0, 0, false);

        //Perform a Combo
        if (willPerformCombo && !hasPerformedCombo) {
            if (currentAttack.comboAction != null) {
                //If can combo
                // hasPerformedCombo = true;
                // currentAttack.comboAction.AttemptToPerformAction(aiCharacter);
            }
        }

        if (aiCharacter.isPerformingAction) {
                return this;
            }
        
        if (!hasPerformedAttack)
        {
            //If we are still recovering from an action, wait before performing another
            if (aiCharacter.aiCharacterCombatManager.actionRecoveryTimer > 0)
            {
                return this;
            }

            PerformAttack(aiCharacter);

            //Return to the top of the state, so if we have a combo we process that when are able
            return this;
        }

        if (pivotAfterAttack) {
            aiCharacter.aiCharacterCombatManager.PivotTowardsTarget(aiCharacter);
        }

        return SwitchState(aiCharacter, aiCharacter.combatStanceState);
    }

    protected void PerformAttack(AICharacterManager aiCharacter) {
        hasPerformedAttack = true;
        currentAttack.AttemptToPerformAction(aiCharacter);
        aiCharacter.aiCharacterCombatManager.actionRecoveryTimer = currentAttack.actionRecoveryTime;
    }

    protected override void ResetStateFlags(AICharacterManager aiCharacter) {
        base.ResetStateFlags(aiCharacter);

        hasPerformedAttack = false;
        hasPerformedCombo = false;
    }

}
