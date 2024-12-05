using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyManager : CharacterManager
{
    public bool isAggro = false;
    public float aggroRange = 1.0f;
    public AggroCollider aggroCollider;

    public float atkRange = 1.0f;
    public CharacterCombatManager combatManager;
    [Header("Tell which type of exp to give")]
    public bool lastHitByMainHand = true;
    protected override void Awake()
    {
        base.Awake();
        if (combatManager == null) combatManager = GetComponent<CharacterCombatManager>();
        if(aggroCollider) aggroCollider.SetRange(aggroRange);
    }
    public void AggroPlayer(GameObject player)
    {
        isLockedOn = true;
        if (combatManager != null)
        {
            combatManager.LockOnTransform = player.transform;
            combatManager.currentTarget = player.GetComponent<CharacterManager>();
        }
        else Debug.Log("Combat manager is null");
    }
    //public void BeginMeleeSwing()
    //{

    //}
    //public void LungeForward()
    //{
    //}
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
            GetComponent<EnemyStatsManager>().DoAllDrops(lastHitByMainHand);
        }
        
        yield return new WaitForSeconds(5);

    }
}
