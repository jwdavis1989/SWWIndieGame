using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class InventionScript
{
    [Header("InventionScript is used to mark an object as an Invention")]
    public InventionType type;
    public IdeaType[] neededIdeas;
    [Header("Invent UI icon")]
    public Sprite icon;
    public string description;
    [Header("Used by InventionManager")]
    public bool hasObtained = false;
    public DateTime createTime;

    public override string ToString()
    {
        string name = "" + type;
        string formatted = "";
        foreach (char letter in name)
        {
            if (char.IsUpper(letter))
            {
                formatted += " " + letter;
            }
            else
            {
                formatted += letter;
            }
        }
        formatted = formatted.Trim();//remove extra space
        return formatted;
    }
    public string GetMysteryString()
    {
        string name = ToString();
        string formatted = "";
        foreach (char letter in name)
        {
            if (letter != ' ')
            {
                formatted += '?';
            }else
            {
                formatted += ' ';
            }
        }
        formatted = formatted.Trim();//remove extra space
        return formatted;
    }
}
public enum InventionType
{
    QuickChargeCapacitory,
    PredictiveNeuralLink,
    IcarausBoosters,
    TreasureScanner,
    GolemEndoplating,
    Alternator,
    RollerJoints,
    EnemyRadar,
    DaedalusNanoMaterials
}