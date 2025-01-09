using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventionScript : MonoBehaviour
{
    [Header("InventionScript is used to mark an object as an Invention")]
    public InventionType type;
    public IdeaType[] neededIdeas;
    [Header("Used by InventionManager")]
    public bool hasObtained = false;
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