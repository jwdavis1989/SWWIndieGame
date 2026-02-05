using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicDoorframeController : MonoBehaviour
{
    public GameObject walls;
    public GameObject corners; 

    public void CreateDoorframe()
    {
        walls.SetActive(false);
        corners.SetActive(true);
    }
}
