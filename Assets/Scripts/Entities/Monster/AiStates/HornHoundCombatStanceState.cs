using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "A.I./States/HournHound Combat Stance")]

public class HornHoundCombatStanceState : CombatStanceState
{
    [SerializeField] private float flankingStoppingDistance = 0f;
    private float originalStoppingDistance = -1f;

    public override AIState Tick(AICharacterManager aiCharacter)
    {
        AiCharacterCombatManager aiCharacterCombatManager = aiCharacter.aiCharacterCombatManager;

        //Cache the stopping distance once for efficiency
        if (originalStoppingDistance < 0)
        {
            originalStoppingDistance = aiCharacter.navMeshAgent.stoppingDistance;
        }

        if (aiCharacter.isPerformingAction)
        {
            return this;
        }

        if (!aiCharacter.navMeshAgent.enabled)
        {
            aiCharacter.navMeshAgent.enabled = true;
        }

        //If you want the AI Character to face and turn towards its target when its outside its Field of View
        if (!aiCharacter.isMoving)
        {
            if (aiCharacterCombatManager.viewableAngle < -30 || aiCharacterCombatManager.viewableAngle > 30)
            {
                aiCharacterCombatManager.PivotTowardsTarget(aiCharacter);
            }
        }

        //Rotate to face our target
        aiCharacterCombatManager.RotateTowardsAgent(aiCharacter);

        //If Target is no longer present, return to the Idle State
        if (aiCharacterCombatManager.currentTarget == null)
        {
            ResetStoppingDistance(aiCharacter);
            //Reset Animation Speed to Idle Speed
            aiCharacterCombatManager.SetIdleSpeed(aiCharacter);
            return SwitchState(aiCharacter, aiCharacter.idleState);
        }

        //If outside combat engagement range, switch to pursue target state
        if (aiCharacterCombatManager.distanceFromTarget > maximumEngagementDistance)
        {
            ResetStoppingDistance(aiCharacter);
            //Set Animation Speed to AI's Movement Speed
            aiCharacterCombatManager.SetBasicSpeed(aiCharacter);
            return SwitchState(aiCharacter, aiCharacter.pursueTargetState);
        }

        //If we do not have an attack selected, select one
        if (!hasSelectedAttack)
        {
            GetNewAttack(aiCharacter);
        }
        else
        {
            //Calculate if the HornHound is behind its target
            Vector3 directionToAI = (aiCharacter.transform.position - aiCharacter.aiCharacterCombatManager.currentTarget.transform.position).normalized;
            float dotProduct = Vector3.Dot(aiCharacter.aiCharacterCombatManager.currentTarget.transform.forward, directionToAI);

            if (dotProduct < 0)
            {
                ResetStoppingDistance(aiCharacter);
                Debug.Log("Attempting to Attack");
                //Pass attack to attack state
                aiCharacter.attackState.currentAttack = chosenAttack;

                //Roll for combo chance

                //Roll for other chances (e.g. Parry, Block, etc)

                //Switch State
                return SwitchState(aiCharacter, aiCharacter.attackState);
            }
            else
            {
                //Character is not behind target, so try to flank
                aiCharacter.navMeshAgent.stoppingDistance = flankingStoppingDistance;
                aiCharacter.BeginFlankingTargetFast();
                return this;
            }
        }

        //Standard Movement
        ResetStoppingDistance(aiCharacter);
        NavMeshPath path = new NavMeshPath();
        aiCharacter.navMeshAgent.CalculatePath(aiCharacterCombatManager.currentTarget.transform.position, path);
        aiCharacter.navMeshAgent.SetPath(path);

        return this;
    }

    protected virtual void ResetStoppingDistance(AICharacterManager aiCharacter)
    {
        aiCharacter.navMeshAgent.stoppingDistance = originalStoppingDistance;
    }
    protected override void GetNewAttack(AICharacterManager aiCharacter)
    {
        AiCharacterCombatManager aiCharacterCombatManager = aiCharacter.aiCharacterCombatManager;
        //1. Sort through all possible attacks
        potentialAttacks = new List<AiCharacterAttackAction>();

        //2. Remove attacks that can't be used in this situation (based on angle.direction)
        foreach (var potentialAttack in aiCharacterAttacks)
        {
            //Target is too Close
            if (potentialAttack.minimumAttackDistance > aiCharacterCombatManager.distanceFromTarget)
            {
                continue;
            }
            //Target is too Far
            if (potentialAttack.maximumAttackDistance < aiCharacterCombatManager.distanceFromTarget)
            {
                continue;
            }
            //Target Outside Minimum Attack Angle
            if (potentialAttack.minimumAttackAngle > aiCharacterCombatManager.viewableAngle)
            {
                continue;
            }
            //Target Outside Maximum Attack Angle
            if (potentialAttack.maximumAttackAngle < aiCharacterCombatManager.viewableAngle)
            {
                continue;
            }

            //3. Place remaining attacks into a list
            potentialAttacks.Add(potentialAttack);
        }

        if (potentialAttacks.Count <= 0)
        {
            //Debug.Log("ERROR: No Potential Attacks.");
            return;
        }

        //4. Pick one of the remaining attacks randomly, based on a weighted chance
        var totalWeight = 0;
        foreach (var attack in potentialAttacks)
        {
            totalWeight += attack.attackWeight;
        }

        var randomWeightValue = Random.Range(1, totalWeight + 1);
        var processedWeight = 0;

        foreach (var attack in potentialAttacks)
        {
            processedWeight += attack.attackWeight;

            if (randomWeightValue <= processedWeight)
            {
                //This is our chosen attack
                chosenAttack = attack;
                previousChosenAttack = chosenAttack;
                hasSelectedAttack = true;
                return;
            }
        }

        //5. Select attack and pass it to the attack state

    }

    protected override bool RollForOutcomeChance(int outcomeChance)
    {
        bool outcomeWillBePerformed = false;

        int randomPercentage = Random.Range(0, 100);
        if (randomPercentage < outcomeChance)
        {
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
