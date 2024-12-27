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
    public bool CanIAtkThisTarget(Faction attacker, Faction target)
    {
        if (attacker == target) return false;
        if (attacker == Faction.TeamPlayer)
        {
            switch (target)
            {
                case Faction.TeamHostile: return true;
                default: return false;
            }
        }
        else if (attacker == Faction.TeamHostile)
        {
            if(target == Faction.TeamPlayer) return true;
        }
        return false;
    }
}
