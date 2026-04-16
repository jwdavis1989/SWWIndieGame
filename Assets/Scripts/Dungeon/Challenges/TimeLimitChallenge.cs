using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Dungeon/Dungeon Challenges/Time Limit")]
public class TimeLimitChallnge : DungeonChallengeData
{
    public float timeLimit;

    public override bool IsFailed()
    {
        return DungeonManager.elapsedTime > timeLimit;
    }
    public override void Initialize()
    {
        if(description == null || description.Length == 0) 
            description = "Beat the level in less than " + timeLimit + " seconds";
        DungeonManager.elapsedTime = 0;
    }
}
