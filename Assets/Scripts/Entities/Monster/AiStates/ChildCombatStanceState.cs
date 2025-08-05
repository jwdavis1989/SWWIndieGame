using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
[CreateAssetMenu(menuName = "A.I./States/ChildCombatStanceState")]
public class ChildCombatStanceState : CombatStanceState
{
    float timeUntilExplosion = 1.5f;
    float currentTimeUntilExplosion = 0f;
    bool finished = false;
    public void Update()
    {
        currentTimeUntilExplosion += Time.deltaTime;
    }

    public override AIState Tick(AICharacterManager aiCharacter)
    {
        if (!aiCharacter.navMeshAgent.enabled)
        {
            aiCharacter.navMeshAgent.enabled = true;
        }
        //Rotate to face our target
        aiCharacter.aiCharacterCombatManager.RotateTowardsAgent(aiCharacter);

        //If Target is no longer present, return to the Idle State
        if (aiCharacter.aiCharacterCombatManager.currentTarget == null)
        {
            return SwitchState(aiCharacter, aiCharacter.idleState);
        }
        if (currentTimeUntilExplosion < timeUntilExplosion)
        {
            currentTimeUntilExplosion += Time.deltaTime;
        } 
        else
        {
            //Instantiate the explosion DamageCollider prefab, that includes the ExplosionPuff particle VFX
            //You might need to check if the player is locked onto the child, to unlock it
            if (!finished)
            {
                GameObject explosion = aiCharacter.gameObject.GetComponent<SpawningBehavior>().Spawn();
                explosion.transform.position = aiCharacter.transform.position;
                //aiCharacter.transform.SetParent(explosion.transform, true);
                aiCharacter.statsManager.currentHealth = 0;
                aiCharacter.ProcessDeathEvent(true);
                finished = true;
            }
            
        }
        return this;
    }

}
