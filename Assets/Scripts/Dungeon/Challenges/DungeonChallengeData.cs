using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DungeonChallengeData : ScriptableObject
{
    public string challengeId;
    public string description;

    public abstract void Initialize();
    public abstract bool IsFailed();
}
