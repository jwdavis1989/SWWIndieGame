using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraItem : UsableItem 
{
    public override void Use(GameObject player)
    {
        IdeaCameraController.instance.ActivateDeactiveCameraView();
    }
}
