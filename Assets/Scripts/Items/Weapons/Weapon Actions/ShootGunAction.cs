using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character Actions/Weapon Actions/Shoot Gun Action")]
public class ShootGunAction : WeaponItemAction
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

        //Gun Special Weapon Attack
        PerformGunShootAttack(characterPerformingAction);
    }

    private void PerformGunShootAttack(CharacterManager characterPerformingAction)
    {
        characterPerformingAction.characterWeaponManager.GetEquippedWeapon(true).GetComponent<WeaponScript>().AttemptToShootGun(characterPerformingAction);
    }

}
