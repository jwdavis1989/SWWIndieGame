using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName ="A.I./States/Combat Stance")]

public class CombatStanceState : AIState
{
    //1. Select an attack for the attack state, depending on distance/angle in relation to target and by weighted chance
    //2. Process any combat logic here whilst waiting to attack (blocking, strafing, dodging, etc.)
    //3. If target moves out of combat range, switch to pursue target
    //4. If target is no longer present, switch to the idle state

    [Header("Attacks")]
    public List<AiCharacterAttackAction> aiCharacterAttacks;    //List of all possible attacks this character can do
    protected List<AiCharacterAttackAction> potentialAttacks;   //All attacks possible in this situation (Based on angle/distance/weight)
    private AiCharacterAttackAction chosenAttack;
    private AiCharacterAttackAction previousChosenAttack;
    protected bool hasSelectedAttack = false;

    [Header("Combo")]
    [SerializeField] protected bool canPerformCombo = false;    //If character can perform a combo attack, after the initial attack
    [SerializeField] protected int chanceToPerformCombo = 25;   //The %Chance to combo
    protected bool hasRolledForComboChance = false;      //If we've already rolled for the combo chance this state tick

    [Header("Engagement Distance")]
    [SerializeField] public float maximumEngagementDistance = 5f;    //Distance at which monster switches back to pursue target state

    public override AIState Tick(AICharacterManager aiCharacter)
    {
        if (aiCharacter.isPerformingAction) {
            return this;
        }

        if (!aiCharacter.navMeshAgent.enabled) {
            aiCharacter.navMeshAgent.enabled = true;
        }

        //If you want the AI Character to face and turn towards its target when its outside its Field of View
        if (!aiCharacter.isMoving) {
            if (aiCharacter.aiCharacterCombatManager.viewableAngle < -30 || aiCharacter.aiCharacterCombatManager.viewableAngle > 30) {
                aiCharacter.aiCharacterCombatManager.PivotTowardsTarget(aiCharacter);
            }
        }

        //Rotate to face our target
        aiCharacter.aiCharacterCombatManager.RotateTowardsAgent(aiCharacter);

        //If Target is no longer present, return to the Idle State
        if (aiCharacter.aiCharacterCombatManager.currentTarget == null) {
            return SwitchState(aiCharacter, aiCharacter.idleState);
        }

        //If we do not have an attack selected, select one
        if (!hasSelectedAttack) {
            GetNewAttack(aiCharacter);
        }
        else {
            //Pass attack to attack state
            aiCharacter.attackState.currentAttack = chosenAttack;

            //Roll for combo chance

            //Roll for other chances (e.g. Parry, Block, etc)

            //Switch State
            return SwitchState(aiCharacter, aiCharacter.attackState);
        }

        //If outside combat engagement range, switch to pursue target state
        if (aiCharacter.aiCharacterCombatManager.distanceFromTarget > maximumEngagementDistance) {
            return SwitchState(aiCharacter, aiCharacter.pursueTargetState);
        }

        NavMeshPath path = new NavMeshPath();
        aiCharacter.navMeshAgent.CalculatePath(aiCharacter.aiCharacterCombatManager.currentTarget.transform.position, path);
        aiCharacter.navMeshAgent.SetPath(path);

        return this;
    }

    protected virtual void GetNewAttack(AICharacterManager aiCharacter) {
        //1. Sort through all possible attacks
        potentialAttacks = new List<AiCharacterAttackAction>();

        //2. Remove attacks that can't be used in this situation (based on angle.direction)
        foreach (var potentialAttack in aiCharacterAttacks) {
            //Target is too Close
            if (potentialAttack.minimumAttackDistance > aiCharacter.aiCharacterCombatManager.distanceFromTarget) {
                continue;
            }
            //Target is too Far
            if (potentialAttack.maximumAttackDistance < aiCharacter.aiCharacterCombatManager.distanceFromTarget) {
                continue;
            }
            //Target Outside Minimum Attack Angle
            if (potentialAttack.minimumAttackAngle > aiCharacter.aiCharacterCombatManager.viewableAngle) {
                continue;
            }
            //Target Outside Maximum Attack Angle
            if (potentialAttack.maximumAttackAngle < aiCharacter.aiCharacterCombatManager.viewableAngle) {
                continue;
            }

            //3. Place remaining attacks into a list
            potentialAttacks.Add(potentialAttack);
        }

        if (potentialAttacks.Count <= 0) {
            Debug.Log("ERROR: No Potential Attacks.");
            return;
        }

        //4. Pick one of the remaining attacks randomly, based on a weighted chance
        var totalWeight = 0;
        foreach(var attack in potentialAttacks) {
            totalWeight += attack.attackWeight;
        }
        
        var randomWeightValue = Random.Range(1, totalWeight + 1);
        var processedWeight = 0;

        foreach (var attack in potentialAttacks) {
            processedWeight += attack.attackWeight;

            if (randomWeightValue <= processedWeight) {
                //This is our chosen attack
                chosenAttack = attack;
                previousChosenAttack = chosenAttack;
                hasSelectedAttack = true;
                return;
            }
        }

        //5. Select attack and pass it to the attack state

    }

    protected virtual bool RollForOutcomeChance(int outcomeChance) {
        bool outcomeWillBePerformed = false;

        int randomPercentage = Random.Range(0, 100);
        if (randomPercentage < outcomeChance) {
            outcomeWillBePerformed = true;
        }

        return outcomeWillBePerformed;
    }

    protected override void ResetStateFlags(AICharacterManager aiCharacter)
    {
        base.ResetStateFlags(aiCharacter);

        hasSelectedAttack = false;
        hasRolledForComboChance = false;
    }

}
