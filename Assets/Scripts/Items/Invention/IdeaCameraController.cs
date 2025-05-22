using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class IdeaCameraController : MonoBehaviour
{
    public static IdeaCameraController instance;
    public Camera ideaCamera;
    
    [Header("Preview UI shown with Save/Discard options")]
    public GameObject photoPreviewFrame;
    public TextMeshProUGUI ideaPhotoText;
    public TextMeshProUGUI previewControlsText;
    public GameObject previewPhoto;
    public GameObject oldPhoto;//previous photo if replacing image for previously captured idea
    public GameObject oldPhotoFrame;

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
    [Header("Rotation")]
    float leftAndRightLookAngle = 0;
    float leftAndRightRotationSpeed = 220f;
    float upAndDownLookAngle = 0;
    float upAndDownRotationSpeed = 220f;
    float minimumPivot = -30f;
    float maximumPivot = 60f;
    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
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
    /** returns true if the player is in idea camera mode */
    static public bool isBusy()
    {
        return instance != null && instance.canvas.isActiveAndEnabled;
    }

    public void TakeScreenshotInput()
    {
        // make sure idea camera mode is active
        if (ideaCamera.gameObject.activeSelf)
        {
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
    //string lastCaptureFilepath = "";
    byte[] lastCapturePng = null;
    WaitForEndOfFrame frameEnd = new WaitForEndOfFrame();
    WaitForSeconds waitTime = new WaitForSeconds(0.4f);
    IEnumerator TakeScreenshot()
    {
        yield return waitTime;//delay for crosshair
        yield return frameEnd; //wait for end of frame
        //ScreenCapture.CaptureScreenshot("SomeLevel.png");

        int width = Screen.width * 65 / 100;
        int height = Screen.height * 65 / 100;
        //Debug.Log("w="+width+" h="+height);//astest
        Texture2D screenshot = new Texture2D(width, height, TextureFormat.RGB24, false);
        Rect rect = new Rect(width/6, height/4, width, height);
        screenshot.ReadPixels(rect, 0, 0);
        screenshot.Apply();
        lastCapturePng = screenshot.EncodeToPNG();
        //lastCaptureFilepath = Application.persistentDataPath + "/" + player.playerStatsManager.characterName + WorldSaveGameManager.instance.currentCharacterSlotBeingUsed + "LastIdeaCapture.png";
        //System.IO.File.WriteAllBytes(lastCaptureFilepath, bytes);
        //Debug.Log("Capture saved at: " + lastCaptureFilepath);
        StartCoroutine(FlashThenPreview());
    }
    /** Activate Prieview Frame. If idea is present then  */
    void ShowPreview(IdeaScript idea)
    {
        //activate photo ui
        photoPreviewFrame.SetActive(true);
        //end flash
        flashGraphic.SetActive(false);
        // if idea was in the capture set text
        if (idea == null) {
            ideaPhotoText.text = "No idea here!";
        }
        else{
            if (InventionManager.instance.CheckHasIdea(idea.type)){
                ideaPhotoText.text = "Idea " + idea.ToString();
                previewControlsText.text = "Return - [Space] / (X)\r\nExit Camera - [ 1 ] / (Y)";
                previewControlsText.text += "\n<s> Replace Photo - [Enter] / (A)</s>";
                oldPhotoFrame.SetActive(true);
                byte[] bytes = InventionManager.instance.GetIdeaPicture(idea.type);
                Texture2D texture = new Texture2D(0, 0);
                texture.LoadImage(bytes);
                oldPhoto.GetComponent<RawImage>().texture = texture;
            }
            else{
                InventionManager.instance.SetHasIdea(idea.type);
                ideaPhotoText.text = "New idea! - " + idea.ToString();
                previewControlsText.text = "Return - [Space] / (X)\r\nExit Camera - [ 1 ] / (Y)";
                ReplacePhoto(idea.type);
                oldPhotoFrame.SetActive(false);
            }
        }
        //load the picture we just took
        StartCoroutine(LoadCaptureToScreen());
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
    IEnumerator LoadCaptureToScreen()
    {
        //string filePath = Application.persistentDataPath + "/" + player.playerStatsManager.characterName + "Screenshot.png";
        //if (System.IO.File.Exists(lastCaptureFilepath))
        if (lastCapturePng != null)
        {
            //byte[] bytes = System.IO.File.ReadAllBytes(lastCaptureFilepath);
            Texture2D texture = new Texture2D(0, 0);
            texture.LoadImage(lastCapturePng);
            //previewPhoto.GetComponent<Renderer>().material.mainTexture = texture;
            previewPhoto.GetComponent<RawImage>().texture = texture;
        }
        yield return null;
    }

    public void ReplacePhoto(IdeaType idea)
    {
        //if (System.IO.File.Exists(lastCaptureFilepath))
        if (lastCapturePng != null)
        {
            //load last picture
            //byte[] bytes = System.IO.File.ReadAllBytes(lastCaptureFilepath);
            InventionManager.instance.SetIdeaPicture(lastCapturePng, idea);
            
            //save
            //string saveFileName = Application.persistentDataPath + "/" + player.playerStatsManager.characterName + WorldSaveGameManager.instance.currentCharacterSlotBeingUsed + idea + ".png";//save file for idea
            //Debug.Log("Idea photo saved to "+ saveFileName);//astest
            //TODO - Only save locally on object then do this code as part of save/load system...
            //System.IO.File.WriteAllBytes( saveFileName, bytes);
        }
        
    }
    void AttachCameraToPlayer()
    {
        if (player == null)
        {
            player = GameObject.Find("Player").GetComponent<PlayerManager>();
        }
        ideaCamera.transform.parent = player.transform;
        Vector3 playerPosition = player.transform.position;
        ideaCamera.transform.position = new Vector3(playerPosition.x, playerPosition.y + 1.6f, playerPosition.z) + (player.transform.forward * 1.5f);
        //leftAndRightLookAngle = player.transform.rotation.y;
        //upAndDownLookAngle = 0;
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
        leftAndRightLookAngle = PlayerCamera.instance.leftAndRightLookAngle;
        upAndDownLookAngle = PlayerCamera.instance.upAndDownLookAngle;
        //deactivate player
        player.canMove = false;
        PlayerUIManager.instance.gameObject.SetActive(false);
        //deactivate player camera
        
        PlayerCamera.instance.cameraObject.enabled = false;
        //activate camera ui
        canvas.gameObject.SetActive(true);
        cameraLensCrosshair.SetActive(true);
        border.SetActive(true);
        photoPreviewFrame.SetActive(false);
        //activate camera
        ideaCamera.gameObject.SetActive(true);
        
    }
    public void DeactivateIdeaCameraView()
    {
        //deactivate camera ui
        canvas.gameObject.SetActive(false);
        cameraLensCrosshair.SetActive(false);
        border.SetActive(false);
        photoPreviewFrame.SetActive(false);
        ideaCamera.gameObject.SetActive(false);
        //activate player
        PlayerCamera.instance.cameraObject.enabled = true;
        PlayerUIManager.instance.gameObject.SetActive(true);
        player.canMove = true;
        takingPhoto = false;
    }
    public void HandleRotations()
    {
        if (!isBusy())
        {
            return;
        }
        //Normal Rotations
        //Rotate left and right based on horizontal movement on the right joystick
        leftAndRightLookAngle += (PlayerInputManager.instance.cameraHorizontalInput * leftAndRightRotationSpeed) * Time.deltaTime;
        //Rotate up and down based on the vertical movement on the right Joystick
        upAndDownLookAngle -= (PlayerInputManager.instance.cameraVerticalInput * upAndDownRotationSpeed) * Time.deltaTime;
        //Clamp the up and down look angle between min/max values
        upAndDownLookAngle = Mathf.Clamp(upAndDownLookAngle, minimumPivot, maximumPivot);

        //Temp variables used for the below assignments
        Vector3 cameraRotation = Vector3.zero;
        Quaternion targetRotation;

        //rotate camera left and right
        cameraRotation.y = leftAndRightLookAngle;
        targetRotation = Quaternion.Euler(cameraRotation);
        player.transform.rotation = targetRotation;

        //Rotate the camera up and down
        cameraRotation = Vector3.zero;
        if (PlayerCamera.instance.isCameraInverted)
            cameraRotation.x = -upAndDownLookAngle;
        else
            cameraRotation.x = upAndDownLookAngle;
        targetRotation = Quaternion.Euler(cameraRotation);
        ideaCamera.transform.localRotation = targetRotation;
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
