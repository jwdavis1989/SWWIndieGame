using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "A.I./States/ParentPursueStanceState")]

public class ParentPursueStanceState : PursueTargetState
{
    //[Header("ParentPursueStanceState is a child of pursue target state")]
    //float spawnInterval = 1.5f;
    //float currentSpawnTimer = 0f;
    public void Update()
    {
        //currentSpawnTimer += Time.deltaTime;
    }
    public override AIState Tick(AICharacterManager aiCharacter)
    {
        //if (currentSpawnTimer < spawnInterval)
        //{
        //    //currentSpawnTimer += Time.deltaTime;
        //}
        //else
        //{
        //    //spawner will handle max spawns
        //    SpawningBehavior spawner = aiCharacter.gameObject.GetComponent<SpawningBehavior>();
        //    //Instantiate the child monster
        //    GameObject child = spawner.Spawn();
        //    if (child != null)
        //    {
        //        child.transform.position = aiCharacter.transform.position + (aiCharacter.transform.forward * 1.25f);
        //    }
        //    currentSpawnTimer = 0f;
        //}
        //copied from old
        //Check if we're performing an action. If so, then do nothing until the action is finshed
        if (aiCharacter.isPerformingAction)
        {
            return this;
        }
        //If we have no target, then return to the Idle State
        if (aiCharacter.aiCharacterCombatManager.currentTarget == null)
        {
            return SwitchState(aiCharacter, aiCharacter.idleState);
        }
        //Make sure our navmesh is active. if not, then enable it
        if (!aiCharacter.navMeshAgent.enabled)
        {
            aiCharacter.navMeshAgent.enabled = true;
        }
        //  Will parent rotate?
        //If our target is outside of our field of view, pivot to face them
        if (aiCharacter.aiCharacterCombatManager.viewableAngle < aiCharacter.aiCharacterCombatManager.minimumDetectionAngle
         || aiCharacter.aiCharacterCombatManager.viewableAngle > aiCharacter.aiCharacterCombatManager.maximumDetectionAngle)
        {
            aiCharacter.aiCharacterCombatManager.PivotTowardsTarget(aiCharacter);
        }
        aiCharacter.aiCharacterLocomotionManager.RotateTowardsAgent(aiCharacter);

        return this;
    }
}
