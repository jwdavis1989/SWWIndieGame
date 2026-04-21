using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Dungeon/Dungeon Challenges/One Hand")]
public class OneHandChallenge : DungeonChallengeData
{
    public override bool IsFailed()
    {
        return DungeonManager.offHandUsed;
    }
    public override void Initialize()
    {
        //DungeonManager.elapsedTime = 0;
    }
}
