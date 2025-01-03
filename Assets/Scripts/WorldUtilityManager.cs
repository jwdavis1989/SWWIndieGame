using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class WorldUtilityManager : MonoBehaviour
{
    public static WorldUtilityManager instance;

    [Header("Layers")]
    [SerializeField] LayerMask characterLayers;
    [SerializeField] LayerMask environmentLayers;

    private void Awake() {
        if (instance == null) {
            instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }

    public LayerMask GetCharacterLayers() {
        return characterLayers;
    }

    public LayerMask GetEnvironmentLayers() {
        return environmentLayers;
    }
    public bool CanIAttackThisTarget(CharacterFaction attackingCharacterFaction, CharacterFaction targetCharacterFaction) {
        if (attackingCharacterFaction == targetCharacterFaction) {
            return false;
        }

        //Player Team's targets: Yellow (Non-hostile but attackable), Hostile 1, Hostile 2, and Hostile 3
        if (attackingCharacterFaction == CharacterFaction.TeamPlayer) {
            switch (targetCharacterFaction) {
                case CharacterFaction.TeamYellow: 
                    return true;
                case CharacterFaction.TeamHostile01: 
                    return true;
                case CharacterFaction.TeamHostile02: 
                    return true;
                case CharacterFaction.TeamHostile03: 
                    return true;
                default: 
                    return false;
            }
        }
        //Hostile Team 01's targets: Player, Hostile 2, and Hostile 3
        else if (attackingCharacterFaction == CharacterFaction.TeamHostile01) {
            switch (targetCharacterFaction) {
                case CharacterFaction.TeamPlayer: 
                    return true;
                case CharacterFaction.TeamHostile02: 
                    return true;
                case CharacterFaction.TeamHostile03: 
                    return true;
                default: 
                    return false;
            }
        }
        //Hostile Team 02's targets: Player, Yellow (Non-hostile but attackable), Hostile 2, and Hostile 3
        else if (attackingCharacterFaction == CharacterFaction.TeamHostile02) {
            switch (targetCharacterFaction) {
                case CharacterFaction.TeamYellow: 
                    return true;
                case CharacterFaction.TeamPlayer: 
                    return true;
                case CharacterFaction.TeamHostile01: 
                    return true;
                case CharacterFaction.TeamHostile02: 
                    return true;
                default: 
                    return false;
            }
        }
        //Hostile Team 03's targets: Player, Yellow (Non-hostile but attackable), Hostile 2, and Hostile 3
        else if (attackingCharacterFaction == CharacterFaction.TeamHostile03) {
            switch (targetCharacterFaction) {
                case CharacterFaction.TeamYellow: 
                    return true;
                case CharacterFaction.TeamPlayer: 
                    return true;
                case CharacterFaction.TeamHostile01: 
                    return true;
                case CharacterFaction.TeamHostile02: 
                    return true;
                default: 
                    return false;
            }
        }

        return false;
    }

    public float GetAngleOfTarget(Transform characterTransform, Vector3 targetCharactersDirection) {
        targetCharactersDirection.y = 0;
        float viewableAngle = Vector3.Angle(characterTransform.forward, targetCharactersDirection);
        Vector3 cross = Vector3.Cross(characterTransform.forward, targetCharactersDirection);

        if (cross.y < 0) {
            viewableAngle = -viewableAngle;
        }

        return viewableAngle;
    }

}
