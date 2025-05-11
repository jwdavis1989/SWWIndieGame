using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleAttackType : StateMachineBehaviour
{
    CharacterManager character;

    [SerializeField] AttackType attackType;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (character == null) {
            character = animator.GetComponent<CharacterManager>();
        }

        character.characterWeaponManager.currentAttackType = attackType;
    }
}