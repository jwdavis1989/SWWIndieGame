using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="A.I./Actions/Attack")]
public class AiCharacterAttackAction : ScriptableObject
{
    [Header("Attack")]
    [SerializeField] protected string attackAnimation;

    [Header("Combo Action")]
    public AiCharacterAttackAction comboAction; //The combo action of this attack action

    [Header("Action Values")]
    [SerializeField] AttackType attackType;
    public int attackWeight = 50;
    //Attack Type
    //Attack can be repeated
    public float actionRecoveryTime = 1.5f;     //Time before character can make another attack after this one (Doesn't affect combos)
    public float minimumAttackAngle = -35f;
    public float maximumAttackAngle = 35f;
    public float minimumAttackDistance = 0f;
    public float maximumAttackDistance = 2f;    //May need to change if it's too short ranged

    public void AttemptToPerformAction(AICharacterManager aICharacter) {
        aICharacter.characterAnimatorManager.PlayTargetAttackActionAnimation(attackType, attackAnimation, true);
    }


}
