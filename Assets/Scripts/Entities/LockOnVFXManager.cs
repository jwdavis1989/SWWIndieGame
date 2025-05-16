using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockOnVFXManager : MonoBehaviour
{
    public Transform targetObject;
    private Vector3 targetsDirection;
    // Start is called before the first frame update
    void Start() {
        targetObject = GameObject.Find("Main Camera").transform;
    }

    // Update is called once per frame
    void Update() {
        transform.LookAt(targetObject);
    }
}
