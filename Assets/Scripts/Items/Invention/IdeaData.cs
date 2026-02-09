using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Idea Data")]
public class IdeaData : ScriptableObject
{
    [Header("Statistical idea data")]
    public string ideaId;
    public string ideaName;
    [Header("Optional")]
    public Sprite presetImage = null;
}
