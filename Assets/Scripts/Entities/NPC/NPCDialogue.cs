using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;

public class NPCDialogue : MonoBehaviour
{
    [Header("Speaker's name")]
    public string speakerName = "Default Name";
    [Header("Speaker's lines")]
    public string[] lines;
    public Line[] lines2;
    //public bool firstTime = true;
}
[Serializable]
public class Line
{
    public string line;
    //public UnityEngine.Events.UnityEvent condition;
    public string conditionKey;
}
