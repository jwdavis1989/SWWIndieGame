using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Items/Item Effects/Camera Item Effect")]

public class CameraItemEffect : ItemEffect
{
    public override void ProcessEffect(CharacterManager character)
    {
        IdeaCameraController.instance.ActivateDeactiveCameraView();
    }
}
