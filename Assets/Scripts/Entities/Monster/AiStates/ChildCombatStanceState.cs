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
                finished = true;
                GameObject explosion = aiCharacter.gameObject.GetComponent<SpawningBehavior>().Spawn(aiCharacter.transform);
                aiCharacter.transform.SetParent(explosion.transform, true);
                aiCharacter.ProcessDeathEvent(true);
            }
            
        }
        return this;
    }

}
