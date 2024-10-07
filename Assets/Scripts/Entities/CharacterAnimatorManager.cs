using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimatorManager : MonoBehaviour
{
    CharacterManager character;
    int horizontal;
    int vertical;

    protected virtual void Awake() {
        character = GetComponent<CharacterManager>();

        //Memory saving trick
        vertical = Animator.StringToHash("Vertical");
        horizontal = Animator.StringToHash("Horizontal");
    }

    public void UpdateAnimatorMovementParameters(float horizontalValue, float verticalValue, bool isSprinting) {
        //If you want to snap the animations to values of 0, 0.5, or 1 only, add clamped version. This is good when your animations don't blend well.

        float horizontalAmount = horizontalValue;
        float verticalAmount = verticalValue;

        if (isSprinting) {
            verticalAmount = 2;
        }
        
        character.animator.SetFloat(horizontal, horizontalAmount, 0.1f, Time.deltaTime);
        character.animator.SetFloat(vertical, verticalAmount, 0.1f, Time.deltaTime);
        
    }

    public virtual void PlayTargetActionAnimation(
        string targetAnimation, 
        bool isPerformingAction, 
        bool applyRootMotion = true, 
        bool canRotate = false, 
        bool canMove = false) {
            character.animator.applyRootMotion = applyRootMotion;
            character.animator.CrossFade(targetAnimation, 0.2f);

            //Can be used to stop character from attempting new actions
            //For example, if you get damaged, and begin performing a damage animation, this will stop from doing anything else.
            //This flag will turn true if you are stunned
            //We can then check for this before attempting new actions
            character.isPerformingAction = isPerformingAction;
            character.canRotate = canRotate;
            character.canMove = canMove;
    }
}
