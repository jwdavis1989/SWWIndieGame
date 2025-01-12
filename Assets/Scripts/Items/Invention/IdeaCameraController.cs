using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class IdeaCameraController : MonoBehaviour
{
    public Camera ideaCamera;
    
    [Header("Preview UI shown with Save/Discard options")]
    public GameObject photoPreviewFrame;
    public TextMeshProUGUI ideaPhotoText;
    public TextMeshProUGUI previewControlsText;
    public GameObject previewPhoto;
    [Header("Graphic in center of camera view")]
    public GameObject cameraLensCrosshair;
    [Header("Black border")]
    public GameObject border;
    [Header("Flash Graphic")]
    public GameObject flashGraphic;
    public Canvas canvas;
    PlayerManager player;
    PlayerControls playerControls;
    [SerializeField] LayerMask ideaLayers;
    private bool takingPhoto = false;
    public static IdeaCameraController instance;
    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
            AttachCameraToPlayer();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void Start()
    {
        canvas.gameObject.SetActive(false);
        ideaCamera.gameObject.SetActive(false);
        cameraLensCrosshair.SetActive(false);
        border.SetActive(false);
        photoPreviewFrame.SetActive(false);
    }

    public void TakeScreenshotInput()
    {
        if (ideaCamera.gameObject.activeSelf)
        {
            // Idea Camera is active
            if (photoPreviewFrame.activeSelf)
            {   // Exit photo preview
                photoPreviewFrame.SetActive(false);
                cameraLensCrosshair.SetActive(true);
                takingPhoto = false;
            }
            else
            {
                if (!takingPhoto)
                {   // Take photo
                    cameraLensCrosshair.SetActive(false);
                    takingPhoto = true;
                    StartCoroutine(TakeScreenshot());
                }
            }
        }

    }
    IEnumerator TakeScreenshot()
    {
        yield return new WaitForSeconds(0.4f);//delay for crosshair
        //ScreenCapture.CaptureScreenshot("SomeLevel.png");

        int width = Screen.width * 65 / 100;
        int height = Screen.height * 65 / 100;
        //Debug.Log("w="+width+" h="+height);//astest
        Texture2D screenshot = new Texture2D(width, height, TextureFormat.RGB24, false);
        Rect rect = new Rect(width/6, height/4, width, height);
        screenshot.ReadPixels(rect, 0, 0);
        screenshot.Apply();
        byte[] bytes = screenshot.EncodeToPNG();
        System.IO.File.WriteAllBytes(Application.dataPath + "/screenshot.png", bytes);
        Debug.Log("Screenshot saved at: " + Application.dataPath + "/screenshot.png");
        //TODO - Probably will save last taken screenshot like this then save to up to 1 of each idea type
        StartCoroutine(FlashThenPreview());
    }
    /** Activate Prieview Frame. If idea is present then  */
    void ShowPreview(IdeaScript idea)
    {
        photoPreviewFrame.SetActive(true);
        flashGraphic.SetActive(false);
        if (idea == null) {
            ideaPhotoText.text = "No idea here!";
        }
        else{
            if (InventionManager.instance.CheckHasIdea(idea.type)){
                ideaPhotoText.text = "Idea " + idea.ToString();
                previewControlsText.text = "Return - [Space] / (X)\r\nExit Camera - [ 1 ] / (Y)";
                previewControlsText.text += "\n<s> Replace Photo - [Enter] / (A)</s>";
            }
            else{
                InventionManager.instance.SetHasIdea(idea.type);
                ideaPhotoText.text = "New idea! - " + idea.ToString();
                previewControlsText.text = "Return - [Space] / (X)\r\nExit Camera - [ 1 ] / (Y)";
            }
        }
            
        StartCoroutine(LoadScreenshot());
    }
    IEnumerator FlashThenPreview()
    {
        //check for idea
        IdeaScript idea = LocateIdeaTarget();
        //activate graphic
        flashGraphic.SetActive(true);
        Image image = flashGraphic.GetComponent<Image>();
        Color curColor = image.color;
        curColor.a = 1;
        //fade out
        while (Mathf.Abs(curColor.a) > 0.0001f)
        {
            curColor.a -= 0.05f; //Mathf.Lerp(curColor.a, targetAlpha, FadeRate * Time.deltaTime);
            image.color = curColor;
            yield return new WaitForSeconds(0.05f);
        }
        flashGraphic.SetActive(false);

        //Open Photo Preview
        ShowPreview(idea);
        yield return null;
    }
    IEnumerator LoadScreenshot()
    {
        string filePath = Application.dataPath + "/screenshot.png";
        if (System.IO.File.Exists(filePath))
        {
            byte[] bytes = System.IO.File.ReadAllBytes(filePath);
            Texture2D texture = new Texture2D(0, 0);
            texture.LoadImage(bytes);
            //previewPhoto.GetComponent<Renderer>().material.mainTexture = texture;
            previewPhoto.GetComponent<RawImage>().texture = texture;
        }
        yield return null;
    }


    void AttachCameraToPlayer()
    {
        player = GameObject.Find("Player").GetComponent<PlayerManager>();
        Transform p = player.transform;
        ideaCamera.transform.parent = p;
        ideaCamera.transform.position = new Vector3(p.position.x, p.position.y+2, p.position.z);
    }
    public void ActivateDeactiveCameraView()
    {
        if (canvas.gameObject.activeSelf)
        {
            DeactivateIdeaCameraView();
        }
        else
        {
            ActivateIdeaCameraView();
        }
    }
    public void ActivateIdeaCameraView()
    {
        player.canMove = false;
        PlayerUIManager.instance.gameObject.SetActive(false);
        canvas.gameObject.SetActive(true);
        ideaCamera.gameObject.SetActive(true);
        cameraLensCrosshair.SetActive(true);
        border.SetActive(true);
        photoPreviewFrame.SetActive(false);
        PlayerCamera.instance.cameraObject.enabled = false;
    }
    public void DeactivateIdeaCameraView()
    {
        canvas.gameObject.SetActive(false);
        ideaCamera.gameObject.SetActive(false);
        cameraLensCrosshair.SetActive(false);
        border.SetActive(false);
        photoPreviewFrame.SetActive(false);
        PlayerCamera.instance.cameraObject.enabled = true;
        PlayerUIManager.instance.gameObject.SetActive(true);
        player.canMove = true;
        takingPhoto = false;
    }
    static public bool isBusy()
    {
        return instance != null && instance.canvas.isActiveAndEnabled;
    }
    public IdeaScript LocateIdeaTarget()
    {
        //declarations
        List<IdeaScript> availableTargets = new List<IdeaScript>();
        IdeaScript nearestTarget = null;
        float lockOnRadius = 12f;
        float minimumViewableAngle = -50f;
        float maximumViewableAngle = 50f;

        //Will be used to determine the target closest to us
        float shortestDistance = Mathf.Infinity;

        //Uses a Character Layermask to improve efficiency
        Collider[] colliders = Physics.OverlapSphere(player.transform.position, lockOnRadius, ideaLayers);

        for (int i = 0; i < colliders.Length; i++)
        {
            IdeaScript lockOnTarget = colliders[i].GetComponent<IdeaScript>();

            if (lockOnTarget != null)
            {
                //Check if they are within our Field of View
                Vector3 lockOnTargetsDirection = lockOnTarget.transform.position - player.transform.position;
                float distanceFromTarget = Vector3.Distance(player.transform.position, lockOnTarget.transform.position);
                float viewableAngle = Vector3.Angle(lockOnTargetsDirection, ideaCamera.transform.forward);


                //If the target is outside of the field of view or blocked by environment, check the next potential target
                if (viewableAngle > minimumViewableAngle && viewableAngle < maximumViewableAngle)
                {
                    RaycastHit hit;

                    //Check Line of sight by environment layers
                    if (Physics.Linecast(player.playerCombatManager.LockOnTransform.position, lockOnTarget.transform.position, out hit, WorldUtilityManager.instance.GetEnvironmentLayers()))
                    {
                        //We hit something in the environment, blocking line of sight
                        continue;
                    }
                    else
                    {
                        //Add the target to the available targets list since it's within Line of Sight
                        availableTargets.Add(lockOnTarget);
                    }
                }

            }
        }

        //Sort through potential targets to see which one we lock onto first
        for (int i = 0; i < availableTargets.Count; i++)
        {
            if (availableTargets[i] != null)
            {
                float distanceFromTarget = Vector3.Distance(player.transform.position, availableTargets[i].transform.position);

                if (distanceFromTarget < shortestDistance)
                {
                    shortestDistance = distanceFromTarget;
                    nearestTarget = availableTargets[i];
                }
            }
        }
        return nearestTarget;
    }
}
