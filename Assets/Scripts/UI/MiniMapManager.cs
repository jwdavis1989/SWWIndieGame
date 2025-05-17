using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniMapManager : MonoBehaviour
{
    public Transform player;
    public static MiniMapManager instance;
    public GameObject hudMiniMap;
    private RectTransform hudMiniMapRect;
    private RawImage MiniMapImage;
    private Camera miniMapCamera;
    private Image miniMapBackground;

    [Header("Minimap Zoom Levels")]
    public float[] miniMapZoomArray = { 20f, 60f, 60f };

    [Header("Minimap Opacity Levels")]
    public float[] miniMapAlphaArray = { 0.5f, 0.25f, 1f };

    [Header("Minimap Aspect Ratio Levels")]
    public float[] miniMapScaleYArray = { 1f, 1.78f, 1.78f };
    private int miniMapStateIndex = 0;

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

            if (hudMiniMap == null)
            {
                hudMiniMap = GameObject.Find("MiniMapBackGround");
            }
            hudMiniMapRect = hudMiniMap.GetComponent<RectTransform>();
            MiniMapImage = hudMiniMapRect.GetChild(0).GetComponent<RawImage>();
            miniMapBackground = hudMiniMap.GetComponent<Image>();
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
        //Change the minimap zoom
        miniMapCamera.orthographicSize = miniMapZoomArray[miniMapStateIndex];

        //Change the minimap alpha
        Color newColorAlpha = MiniMapImage.color;
        newColorAlpha.a = miniMapAlphaArray[miniMapStateIndex];
        MiniMapImage.color = newColorAlpha;

        //Change the minimap Aspect Ratio
        miniMapBackground.transform.localScale = new Vector3(1, miniMapScaleYArray[miniMapStateIndex], 1);

        //Hide Minimap Background whenever in states beyond the default 0
        miniMapBackground.enabled = miniMapStateIndex < 1;

        //Update Minimap Hud anchor and scale
        switch (miniMapStateIndex)
        {
            case 0:
                miniMapStateIndex++;
                hudMiniMapRect.pivot = new Vector2(1f, 1f);
                hudMiniMapRect.anchorMin = new Vector2(1f, 1f);
                hudMiniMapRect.anchorMax = new Vector2(1f, 1f);
                hudMiniMapRect.offsetMin = new Vector2(-475, -475);
                hudMiniMapRect.offsetMax = new Vector2(-175, -175);
                hudMiniMapRect.pivot = new Vector2(0.5f, 0.5f);
                break;
            case 1:
                miniMapStateIndex++;
                hudMiniMapRect.pivot = new Vector2(0.5f, 0.5f);
                hudMiniMapRect.anchorMin = new Vector2(0f, 0f);
                hudMiniMapRect.anchorMax = new Vector2(1f, 1f);
                hudMiniMapRect.offsetMin = new Vector2(175, 175);
                hudMiniMapRect.offsetMax = new Vector2(-175, -175);
                hudMiniMapRect.pivot = new Vector2(0.5f, 0.5f);
                break;
            case 2:
                miniMapStateIndex = 0;
                hudMiniMapRect.pivot = new Vector2(0.5f, 0.5f);
                hudMiniMapRect.anchorMin = new Vector2(0f, 0f);
                hudMiniMapRect.anchorMax = new Vector2(1f, 1f);
                hudMiniMapRect.offsetMin = new Vector2(175, 175);
                hudMiniMapRect.offsetMax = new Vector2(-175, -175);
                hudMiniMapRect.pivot = new Vector2(0.5f, 0.5f);
                break;
        }

    }

    private void HandleMiniMapPositionUpdate()
    {
        Vector3 newPosition = player.position;
        newPosition.y = transform.position.y;
        transform.position = newPosition;
    }

}
