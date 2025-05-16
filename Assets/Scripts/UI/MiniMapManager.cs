using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapManager : MonoBehaviour
{
    public Transform player;
    public static MiniMapManager instance;
    private Camera miniMapCamera;
    public float[] miniMapZoomArray = { 20f, 40f, 100f };
    private int miniMapZoomIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Awake() {
        if (instance == null)
        {
            instance = this;
            miniMapCamera = GetComponent<Camera>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        HandleMiniMapPositionUpdate();

        // transform.rotation = Quaternion.Euler(90f, player.eularAngles.y, 0f);
    }

    public void UpdateMiniMapZoom()
    {
        if (miniMapZoomIndex < 2)
        {
            miniMapZoomIndex++;
        }
        else
        {
            miniMapZoomIndex = 0;
        }
        miniMapCamera.orthographicSize = miniMapZoomArray[miniMapZoomIndex];
    }

    private void HandleMiniMapPositionUpdate()
    {
        Vector3 newPosition = player.position;
        newPosition.y = transform.position.y;
        transform.position = newPosition;
    }

}
