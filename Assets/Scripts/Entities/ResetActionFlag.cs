using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetActionFlag : StateMachineBehaviour
{
    CharacterManager character;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (character == null)
        {
            character = animator.GetComponent<CharacterManager>();
        }

        //This is called when an action ends, and the state returns to "Empty"
        character.isPerformingAction = false;
        character.canRotate = true;
        character.canMove = true;
        character.isJumping = false;
        character.isBoosting = false;
        character.isRolling = false;

        //TODO: Investigate if this is causing bugs for AI.
        //This was needed to keep enemies from being automatically set to no root motion
        //Which they use to move using their NavMeshes.
        if (character.isPlayer)
        {
            character.animator.applyRootMotion = false;
        }

        //TODO: Investigate why this was causing error
        if (character.characterCombatManager != null)
        {
            character.characterCombatManager.DisableCanDoCombo();
            character.characterCombatManager.DisableCanDoRollingAttack();
            character.characterCombatManager.DisableCanDoBackStepAttack();
        }

        //Deletes spell VFX if character is interuptted during their spellcasting animation
        if (character.characterEffectsManager.activeSpellWarmUpFX != null)
        {
            Destroy(character.characterEffectsManager.activeSpellWarmUpFX);
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
