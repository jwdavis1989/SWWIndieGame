using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Invention : MonoBehaviour
{
    public InventionType type;
    public bool hasObtained = false;
    public IdeaType[] neededIdeas;
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
public enum IdeaType
{
    Rock,
    Brick,
    Screw,
    Window,
    Potato
}