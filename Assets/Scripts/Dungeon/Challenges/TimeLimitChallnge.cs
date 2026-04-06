using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TimeLimitChallnge : DungeonChallengeData
{
    public float timeLimit;

    public override bool IsFailed()
    {
        return DungeonManager.elapsedTime > timeLimit;
    }
    public override void Initialize()
    {
        description = "Beat the level in less than " + timeLimit + " seconds";
        DungeonManager.elapsedTime = 0;
    }
}
