using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[CreateAssetMenu(menuName = "Items/Invention Data")]
public class InventionData : ScriptableObject
{
    [Header("Statistical invention data")]
    public string inventionId;
    public string inventionName;
    public string[] ideas = new string[3];
    public Sprite icon;
    [TextArea] public string description;
}
