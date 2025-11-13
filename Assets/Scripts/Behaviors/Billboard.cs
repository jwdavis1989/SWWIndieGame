using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    void LateUpdate()
    {
        // Make the sprite face the camera
        transform.forward = cam.transform.forward;
    }
}
