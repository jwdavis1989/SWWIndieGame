using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
[CreateAssetMenu(menuName = "A.I./States/ChildCombatStanceState")]
public class ChildCombatStanceState : CombatStanceState
{
    //float timeUntilExplosion = 1.5f;
    //float currentTimeUntilExplosion = 0f;
    //bool finished = false;
    //public void Update()
    //{
    //    currentTimeUntilExplosion += Time.deltaTime;
    //}

    public override AIState Tick(AICharacterManager aiCharacter)
    {
        SpawningBehavior explosionSpawner = aiCharacter.gameObject.GetComponent<SpawningBehavior>();
        FlashingBehavior flashingLight = aiCharacter.gameObject.GetComponentInChildren<FlashingBehavior>();
        explosionSpawner.auto = true; // spawn explosion after interval
        flashingLight.ActivateFlashing(); //flashing light effect
        aiCharacter.animator.speed = 3; // speed up
        if (explosionSpawner.spawnList.Count > 0)//exploded
            aiCharacter.statsManager.currentHealth = 0;
        if (aiCharacter.statsManager.currentHealth <= 0)
        {//don't flash and explode if killed
            explosionSpawner.auto = false;
            flashingLight.DeactivateFlashing();
        }
        if (!aiCharacter.navMeshAgent.enabled)
        {
            aiCharacter.navMeshAgent.enabled = true;
        }
        //Rotate to face our target
        aiCharacter.aiCharacterCombatManager.RotateTowardsAgent(aiCharacter);
        //movement
        NavMeshPath path = new NavMeshPath();
        aiCharacter.navMeshAgent.CalculatePath(aiCharacter.aiCharacterCombatManager.currentTarget.transform.position, path);
        aiCharacter.navMeshAgent.SetPath(path);

        //If Target is no longer present, return to the Idle State
        if (aiCharacter.aiCharacterCombatManager.currentTarget == null)
        {
            return SwitchState(aiCharacter, aiCharacter.idleState);
        }
        //If outside combat engagement range, switch to pursue target state
        //if (aiCharacter.aiCharacterCombatManager.distanceFromTarget > maximumEngagementDistance)
        //{
        //    return SwitchState(aiCharacter, aiCharacter.pursueTargetState);
        //}
        //if (currentTimeUntilExplosion < timeUntilExplosion)
        //{
        //    currentTimeUntilExplosion += Time.deltaTime;
        //}
        //else
        //{
        //    //Instantiate the explosion DamageCollider prefab, that includes the ExplosionPuff particle VFX
        //    //You might need to check if the player is locked onto the child, to unlock it
        //    UnityEngine.Debug.Log("EXPLODING");
        //    GameObject explosion = aiCharacter.gameObject.GetComponent<SpawningBehavior>().Spawn();
        //    explosion.transform.position = aiCharacter.transform.position;
        //    //aiCharacter.transform.SetParent(explosion.transform, true);
        //    aiCharacter.statsManager.currentHealth = 0;
        //    Destroy(aiCharacter.gameObject);
        //aiCharacter.ProcessDeathEvent(true);

        //}
        return this;
    }

}
