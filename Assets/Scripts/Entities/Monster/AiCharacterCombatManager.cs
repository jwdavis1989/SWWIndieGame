using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiCharacterCombatManager : CharacterCombatManager
{
    [Header("Aggro")]
    public bool isAggro = false;
    public float aggroRange = 5.0f;
    public float minimumDetectionAngle = -35f;
    public float maximumDetectionAngle = 35f;

    public void FindATargetWithInLineOSight(AICharacterManager aiCharacter) {
        if(currentTarget != null) {
            return;
        }

        Collider[] colliders = Physics.OverlapSphere(aiCharacter.transform.position, aggroRange, WorldUtilityManager.instance.GetCharacterLayers());

        for(int i = 0; i < colliders.Length; i++) {
            CharacterManager targetCharacter = colliders[i].transform.GetComponent<CharacterManager>();

            //Check if not targeting targetCharacter, self, or a dead targetCharacter
            if(targetCharacter == null || targetCharacter == aiCharacter || targetCharacter.isDead) {
                continue;
            }

            //Can I attack this character based on their Faction? If so, make them my target
            if(WorldUtilityManager.instance.CanIAttackThisTarget(aiCharacter.faction, targetCharacter.faction)) {

                //If a potential target is found, it has to be in front of us
                Vector3 targetsDirection = targetCharacter.transform.position - aiCharacter.transform.position;
                float viewableAngle = Vector3.Angle(targetsDirection, aiCharacter.transform.forward);

                if(viewableAngle > minimumDetectionAngle && viewableAngle < maximumDetectionAngle) {

                    //Check if the environment blocks sight to the target
                    if(Physics.Linecast(aiCharacter.characterCombatManager.LockOnTransform.position, 
                     targetCharacter.characterCombatManager.LockOnTransform.position, 
                     WorldUtilityManager.instance.GetEnvironmentLayers())) {
                        //blocked
                        Debug.Log("FindATargetWithInLineOSight(): Line of Sight Blocked");
                        Debug.DrawLine(aiCharacter.characterCombatManager.LockOnTransform.position, targetCharacter.characterCombatManager.LockOnTransform.position);
                    }
                    else {
                        aiCharacter.characterCombatManager.SetTarget(targetCharacter);
                    }
                }
            }
        }
    }

    public void AggroPlayer(GameObject player) {
        character.isLockedOn = true;
        LockOnTransform = player.transform;
        SetTarget(player.GetComponent<CharacterManager>());
    }
}