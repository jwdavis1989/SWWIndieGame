using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimatorManager : MonoBehaviour
{
    CharacterManager character;

    protected virtual void Awake() {
        character = GetComponent<CharacterManager>();
    }

    public void UpdateAnimatorMovementParameters(float horizontalValue, float verticalValue) {
        //If you want to snap the animations to values of 0, 0.5, or 1 only, add clamped version. This is good when your animations don't blend well.

        character.animator.SetFloat("Horizontal", horizontalValue);
        character.animator.SetFloat("Vertical", verticalValue);
    }
}
