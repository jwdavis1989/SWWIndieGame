using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetIsJumping : StateMachineBehaviour
{
    CharacterManager character;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (character == null) {
            character = animator.GetComponent<CharacterManager>(); 
        }

        character.isJumping = false;
    }
}
