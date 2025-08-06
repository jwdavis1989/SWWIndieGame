using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character Actions/Weapon Actions/Cast Magic Action")]
public class CastMagicAction : WeaponItemAction
{
    public override void AttemptToPerformAction(CharacterManager characterPerformingAction)
    {
        base.AttemptToPerformAction(characterPerformingAction);

        //Check for Stops:
        //Out of Stamina?
        if (characterPerformingAction.characterStatsManager.currentStamina <= 0)
        {
            return;
        }

        //Not grounded?
        if (!characterPerformingAction.isGrounded)
        {
            return;
        }

        //Magic Special Weapon Attack
        PerformSpellAttack(characterPerformingAction);
    }

    private void PerformSpellAttack(CharacterManager characterPerformingAction)
    {
        characterPerformingAction.characterWeaponManager.GetEquippedWeapon(true).GetComponent<WeaponScript>().AttemptToCastSpell(characterPerformingAction);
    }

}
