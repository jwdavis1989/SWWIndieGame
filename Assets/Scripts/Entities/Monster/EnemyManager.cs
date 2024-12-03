using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : CharacterManager
{
    public override IEnumerator ProcessDeathEvent(bool manuallySelectDeathAnimation = false)
    {
        characterStatsManager.currentHealth = 0;
        canMove = false;
        isDead = true;

        //Reset any Flags here that need to be reset
        //Todo: Add these later

        //If not grounded, play an aerial death animation

        if (!manuallySelectDeathAnimation)
        {
            //Could change this to choose a random death animation in the future if we wanted to.
            characterAnimatorManager.PlayTargetActionAnimation("Dead_01", true);
        }

        //Play Death SFX
        //characterSoundFXManager.audioSource.PlayOneShot(WorldSoundFXManager.instance.deathSFX);

        if (!isPlayer)
        {
            //If monster: Award players with Gold or items
            GetComponent<EnemyStatsManager>().DoAllDrops();
        }
        
        yield return new WaitForSeconds(5);

    }
}
