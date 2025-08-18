using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class CharacterAnimatorManager : MonoBehaviour
{
    protected CharacterManager character;
    int horizontal;
    int vertical;

    [Header("Damage Animations")]
    public string lastDamageAnimationPlayed;

    //Ping Hit Reactions
    [SerializeField] string hit_Forward_Ping_01 = "Hit_Forward_Ping_01";
    [SerializeField] string hit_Forward_Ping_02 = "Hit_Forward_Ping_02";
    [SerializeField] string hit_Backward_Ping_01 = "Hit_Backward_Ping_01";
    [SerializeField] string hit_Backward_Ping_02 = "Hit_Backward_Ping_02";
    [SerializeField] string hit_Right_Ping_01 = "Hit_Right_Ping_01";
    [SerializeField] string hit_Right_Ping_02 = "Hit_Right_Ping_02";
    [SerializeField] string hit_Left_Ping_01 = "Hit_Left_Ping_01";
    [SerializeField] string hit_Left_Ping_02 = "Hit_Left_Ping_02";

    public List<string> forward_Ping_Damage = new List<string>();
    public List<string> backward_Ping_Damage = new List<string>();
    public List<string> right_Ping_Damage = new List<string>();
    public List<string> left_Ping_Damage = new List<string>();

    //Medium Hit Reactions
    [SerializeField] string hit_Forward_Medium_01 = "Hit_Forward_Medium_01";
    [SerializeField] string hit_Forward_Medium_02 = "Hit_Forward_Medium_02";
    [SerializeField] string hit_Backward_Medium_01 = "Hit_Backward_Medium_01";
    [SerializeField] string hit_Backward_Medium_02 = "Hit_Backward_Medium_02";
    [SerializeField] string hit_Right_Medium_01 = "Hit_Right_Medium_01";
    [SerializeField] string hit_Right_Medium_02 = "Hit_Right_Medium_02";
    [SerializeField] string hit_Left_Medium_01 = "Hit_Left_Medium_01";
    [SerializeField] string hit_Left_Medium_02 = "Hit_Left_Medium_02";

    public List<string> forward_Medium_Damage = new List<string>();
    public List<string> backward_Medium_Damage = new List<string>();
    public List<string> right_Medium_Damage = new List<string>();
    public List<string> left_Medium_Damage = new List<string>();


    protected virtual void Start()
    {
        //Ping Hit Animations
        forward_Ping_Damage.AddRange(new List<string>() { hit_Forward_Ping_01, hit_Forward_Ping_02 });
        backward_Ping_Damage.AddRange(new List<string>() { hit_Backward_Ping_01, hit_Backward_Ping_02 });
        left_Ping_Damage.AddRange(new List<string>() { hit_Left_Ping_01, hit_Left_Ping_02 });
        right_Ping_Damage.AddRange(new List<string>() { hit_Right_Ping_01, hit_Right_Ping_02 });

        //Medium Hit Animations
        forward_Medium_Damage.AddRange(new List<string>() { hit_Forward_Medium_01, hit_Forward_Medium_02 });
        backward_Medium_Damage.AddRange(new List<string>() { hit_Backward_Medium_01, hit_Backward_Medium_02 });
        left_Medium_Damage.AddRange(new List<string>() { hit_Left_Medium_01, hit_Left_Medium_02 });
        right_Medium_Damage.AddRange(new List<string>() { hit_Right_Medium_01, hit_Right_Medium_02 });
    }

    public string GetRandomAnimationFromList(List<string> animationList)
    {
        //Copy list created to avoid editing actual list
        List<string> finalList = new List<string>();

        //Populate copy list
        foreach (var item in animationList)
        {
            finalList.Add(item);
        }

        //Check if we have already played this damage animation so it doesn't repeat
        finalList.Remove(lastDamageAnimationPlayed);

        //Check the list for null entries, then remove them
        for (int i = finalList.Count - 1; i > -1; i--)
        {
            if (finalList[i] == null)
            {
                finalList.RemoveAt(i);
            }
        }

        return finalList[Random.Range(0, finalList.Count)];
    }
    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();

        //Memory saving trick
        vertical = Animator.StringToHash("Vertical");
        horizontal = Animator.StringToHash("Horizontal");
    }

    public void UpdateAnimatorMovementParameters(float horizontalValue, float verticalValue, bool isSprinting)
    {
        //If you want to snap the animations to values of 0, 0.5, or 1 only, add clamped version. This is good when your animations don't blend well.

        float snappedHorizontalAmount = horizontalValue;
        float snappedVerticalAmount = verticalValue;

        //Snap movement values
        if (horizontalValue > 0f && horizontalValue < 0.5f)
        {
            snappedHorizontalAmount = 0.5f;
        }
        else if (horizontalValue < 0f && horizontalValue > -0.5f)
        {
            snappedHorizontalAmount = -0.5f;
        }

        if (verticalValue > 0f && verticalValue < 0.5f)
        {
            snappedVerticalAmount = 0.5f;
        }
        else if (verticalValue < 0f && verticalValue > -0.5f)
        {
            snappedVerticalAmount = -0.5f;
        }


        if (isSprinting)
        {
            snappedVerticalAmount = 2;
        }

        character.animator.SetFloat(horizontal, snappedHorizontalAmount, 0.1f, Time.deltaTime);
        character.animator.SetFloat(vertical, snappedVerticalAmount, 0.1f, Time.deltaTime);

    }

    //Can be used for non-WeaponManager based enemies
    public virtual void PlayTargetActionAnimation(
        string targetAnimation,
        bool isPerformingAction,
        bool applyRootMotion = true,
        bool canRotate = false,
        bool canMove = false)
    {
        character.animator.applyRootMotion = applyRootMotion;
        character.animator.CrossFade(targetAnimation, 0.2f);

        //Debug.Log("Playing Animation: " + targetAnimation);

        //Can be used to stop character from attempting new actions
        //For example, if you get damaged, and begin performing a damage animation, this will stop from doing anything else.
        //This flag will turn true if you are stunned
        //We can then check for this before attempting new actions
        character.isPerformingAction = isPerformingAction;
        character.canRotate = canRotate;
        character.canMove = canMove;
    }

    //Only use with WeaponManager based enemies
    public virtual void PlayTargetAttackActionAnimation(
        AttackType attackType,
        string targetAnimation,
        bool isPerformingAction,
        bool applyRootMotion = true,
        bool canRotate = false,
        bool canMove = false)
    {

        //Keep track of last attack performed (For Combos)
        //Keep track of current attack type (Light, Heavy, etc.)
        character.characterWeaponManager.currentAttackType = attackType;
        character.characterCombatManager.lastAttackAnimationPerformed = targetAnimation;


        //Update Animation Set to Current Weapon's Animations
        UpdateAnimatorControllerByWeapon(character.characterWeaponManager.ownedWeapons[character.characterWeaponManager.indexOfEquippedWeapon].GetComponent<WeaponScript>());
        character.animator.applyRootMotion = applyRootMotion;
        character.animator.CrossFade(targetAnimation, 0.2f);
        character.isPerformingAction = isPerformingAction;
        character.canRotate = canRotate;
        character.canMove = canMove;

        //Set Character isAttacking Flag
        character.isAttacking = true;

        //Decide if our attack can be parried

    }

    public void UpdateAnimatorControllerByWeapon(WeaponScript weaponScript)
    {
        character.animator.runtimeAnimatorController = weaponScript.weaponAnimatorOverride;
    }

}
