using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICombatManager : CharacterCombatManager
{
    [Header("Aggro")]
    public bool isAggro = false;
    public float aggroRange = 5.0f;
    public float minDetectAngle = -35;
    public float maxDetectAngle = 35;
    public void FindATargetWitLineOSight(AICharacterManager aiCharacter)
    {
        if(currentTarget != null) return;
        Collider[] colliders = Physics.OverlapSphere(aiCharacter.transform.position, aggroRange, WorldUtilityManager.instance.GetCharacterLayers());
        for(int i = 0; i < colliders.Length; i++)
        {
            CharacterManager target = colliders[i].transform.GetComponent<CharacterManager> ();
            // no target, self, dead target
            if(target == null || target == aiCharacter || target.isDead) continue;
            //check factions
            if(WorldUtilityManager.instance.CanIAtkThisTarget(aiCharacter.faction, aiCharacter.faction)){
                Vector3 tarDir = target.transform.position - aiCharacter.transform.position;
                float viewAngle = Vector3.Angle(tarDir, aiCharacter.transform.forward);
                if(viewAngle > minDetectAngle && viewAngle < maxDetectAngle)
                {
                    if(Physics.Linecast(aiCharacter.characterCombatManager.LockOnTransform.position, target.characterCombatManager.LockOnTransform.position))
                    {
                        //blocked
                        Debug.Log("BLOCKED");
                        Debug.DrawLine(aiCharacter.characterCombatManager.LockOnTransform.position, target.characterCombatManager.LockOnTransform.position);
                    }
                    else
                    {
                        aiCharacter.characterCombatManager.SetTarget(target);
                    }
                }
            }
        }
    }
    public void AggroPlayer(GameObject player)
    {
        character.isLockedOn = true;
        LockOnTransform = player.transform;
        SetTarget(player.GetComponent<CharacterManager>());
    }
}
public enum Faction
{
    TeamPlayer, //Player, Pets, Summons
    TeamGreen, //Player cannot attack
    TeamYellow, //Non-Hostile, Attackable by player
    TeamHostile //Hostile to player
}