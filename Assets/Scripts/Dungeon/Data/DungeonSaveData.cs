using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DungeonSaveData
{
    public string dungeonId;
    public List<DungeonNodeSaveData> savedNodes = new List<DungeonNodeSaveData>();
}
