using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public static PlayerCamera instance;
    public PlayerManager player;
    public Camera cameraObject;

    [Header("Camera Settings")]
    private Vector3 cameraVelocity;

    //Larger cameraSmoothSpeed values equal longer time for the camera to reach its position during movement
    private float cameraSmoothSpeed = 1f;
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Awake() {
        if (instance == null) {
            instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }

    public void HandleAllCameraActions() {
        if (player != null) {
            FollowTarget();
            //2. Rotate around the Player
            //3. Collide with objects
        }
    }

    private void FollowTarget() {
        Vector3 targetCameraPosition = Vector3.SmoothDamp(transform.position, player.transform.position, ref cameraVelocity, cameraSmoothSpeed * Time.deltaTime);
        transform.position = targetCameraPosition;
    }
}
