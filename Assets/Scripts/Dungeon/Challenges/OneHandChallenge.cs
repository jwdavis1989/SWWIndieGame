using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneHandChallenge : DungeonChallengeData
{
    public bool isMainHandOnly;

    public override bool IsFailed()
    {
        if (isMainHandOnly)
            return DungeonManager.offHandUsed;
        else 
            return DungeonManager.mainHandUsed;
    }
    public override void Initialize()
    {
        description = "Beat the level using only your " + (isMainHandOnly ? "main hand":"off hand") + " weapon";
        DungeonManager.elapsedTime = 0;
    }
}
