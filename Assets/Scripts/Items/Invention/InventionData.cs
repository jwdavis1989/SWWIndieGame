using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Items/Invention Data")]
public class InventionData : ScriptableObject
{
    [Header("InventionScript is used to mark an object as an Invention")]
    public string inventionId;
    public string inventionName;
    public IdeaType[] neededIdeas;
    public string[] ideas = new string[3];
    [Header("Invent UI icon")]
    public Sprite icon;
    [TextArea] public string description;
    [Header("Used by InventionManager - CAN I DO THIS IN A S.O.? Guess Ill find out")]
    public bool hasObtained = false;
    public DateTime createTime;
}
