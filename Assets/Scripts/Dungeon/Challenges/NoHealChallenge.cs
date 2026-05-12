using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Dungeon/Dungeon Challenges/No Heal")]

public class NoHealChallenge : DungeonChallengeData
{
    public override bool IsFailed()
    {
        return DungeonManager.healingItemUsed;
    }
    public override void Initialize()
    {
        DungeonManager.healingItemUsed = false;
    }
}
