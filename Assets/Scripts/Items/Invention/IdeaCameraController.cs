using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
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
    [Header("Post Processing Effect")]
    public PostProcessLayer postProcessLayer;
    [Header("Idea cam ui")]
    public Canvas canvas;

    PlayerManager player;

    [Header("Layers to search for ideas")]
    [SerializeField] LayerMask ideaLayers;
    private bool takingPhoto = false;
    [Header("Controls")]
    PlayerControls playerControls;
    [SerializeField] bool capturePhotoInput = false;
    [SerializeField] bool deactivateCameraViewInput = false;
    [Header("Rotation")]
    float leftAndRightLookAngle = 0;
    float leftAndRightRotationSpeed = 220f;
    float upAndDownLookAngle = 0;
    float upAndDownRotationSpeed = 220f;
    float minimumPivot = -60f;
    float maximumPivot = 60f;

    [Header("Steve Audio")]
    public AudioClip[] steveAudioClipAffirmative;
    public AudioClip[] steveAudioClipNegative;
    public AudioClip[] steveAudioClipScan;

    [Header("Camera Attributes")]
    float ideaCaptureRange = 12f;
    float ideaCaptureRadius = 0.5f;
    float cameraPositionHeightModifier = 1.6f;
    float cameraPositionDepthModifier = 0.5f;

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
        postProcessLayer.enabled = false;
        canvas.gameObject.SetActive(false);
        ideaCamera.gameObject.SetActive(false);
        cameraLensCrosshair.SetActive(false);
        border.SetActive(false);
        photoPreviewFrame.SetActive(false);
        if (playerControls == null)
        {
            playerControls = new PlayerControls();
            playerControls.IdeaCameraView.CaptureIdeaPhotoBtn.performed += i => capturePhotoInput = true;
            playerControls.IdeaCameraView.DeactivateCameraView.performed += i => deactivateCameraViewInput = true;
            playerControls.Enable();
            playerControls.IdeaCameraView.Disable();
        }
    }
    public void LateUpdate()
    {
        HandleCapturePhotoInput();
        HandleDeactivateCameraViewInput();
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
        postProcessLayer.enabled = true;
        yield return frameEnd; //wait for end of frame
        //ScreenCapture.CaptureScreenshot("SomeLevel.png");

        int height = Screen.height * 75 / 100;
        int width = (int)(height * (4.0 / 3.0));
        //Debug.Log("Width:"+width+" \nHeight:"+height);
        Texture2D screenshot = new Texture2D(width, height, TextureFormat.RGB24, false);
        int x = Screen.width / 2 - (width / 2);
        int y = Screen.height / 2 - (height / 2);
        Rect rect = new Rect(x, y, width, height);
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
        if (idea == null)
        {
            ideaPhotoText.text = "No idea here!";
            oldPhotoFrame.SetActive(false);

            //Play Steve audio - Negative
            WorldSoundFXManager.instance.PlayAdvancedSoundFX(player.characterSoundFXManager.audioSource, WorldSoundFXManager.instance.ChooseRandomSFXFromArray(steveAudioClipNegative));
        }
        else if (InventionManager.instance.CheckHasIdea(idea.ideaId))
        { 
            ideaPhotoText.text = "Idea " + idea.ToString();
            previewControlsText.text = "Return - [Space] / (X)\r\nExit Camera - [ 1 ] / (Y)";
            previewControlsText.text += "\n<s> Replace Photo - [Enter] / (A)</s>";
            oldPhotoFrame.SetActive(true);
            byte[] bytes = InventionManager.instance.GetIdeaPicture(idea.ideaId);
            Texture2D texture = new Texture2D(0, 0);
            texture.LoadImage(bytes);
            oldPhoto.GetComponent<RawImage>().texture = texture;

            //Play Steve audio - Scan
            WorldSoundFXManager.instance.PlayAdvancedSoundFX(player.characterSoundFXManager.audioSource, WorldSoundFXManager.instance.ChooseRandomSFXFromArray(steveAudioClipScan));
        }
        else 
        {
            InventionManager.instance.SetHasIdea(idea);
            ideaPhotoText.text = "New idea! - " + InventionManager.instance.ideaDatabase.GetIdea(idea.ideaId).ideaName; 
            previewControlsText.text = "Return - [Space] / (X)\r\nExit Camera - [ 1 ] / (Y)";
            ReplacePhoto(idea.ideaId);
            oldPhotoFrame.SetActive(false);

            //Play Steve audio - Affirmative
            WorldSoundFXManager.instance.PlayAdvancedSoundFX(player.characterSoundFXManager.audioSource, WorldSoundFXManager.instance.ChooseRandomSFXFromArray(steveAudioClipAffirmative));
        }
        //load the picture we just took
        StartCoroutine(LoadCaptureToScreen());
    }
    IEnumerator FlashThenPreview()
    {
        //check for idea
        IdeaScript idea = LocateIdeaTarget();
        //activate graphic
        postProcessLayer.enabled = false;
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

    public void ReplacePhoto(string ideaID)
    {
        //if (System.IO.File.Exists(lastCaptureFilepath))
        if (lastCapturePng != null)
        {
            //load last picture
            //byte[] bytes = System.IO.File.ReadAllBytes(lastCaptureFilepath);
            InventionManager.instance.SetIdeaPicture(lastCapturePng, ideaID);
            //save
            //string saveFileName = Application.persistentDataPath + "/" + player.playerStatsManager.characterName + WorldSaveGameManager.instance.currentCharacterSlotBeingUsed + idea + ".png";//save file for idea
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
        ideaCamera.transform.position = new Vector3(playerPosition.x, playerPosition.y + cameraPositionHeightModifier, playerPosition.z) + (player.transform.forward * cameraPositionDepthModifier);
        //leftAndRightLookAngle = player.transform.rotation.y;
        //upAndDownLookAngle = 0;
    }
    //public void ActivateDeactiveCameraView()
    //{
    //    if (canvas.gameObject.activeSelf)
    //    {
    //        DeactivateIdeaCameraView();
    //    }
    //    else
    //    {
    //        ActivateIdeaCameraView();
    //    }
    //}
    public void ActivateCameraViewInput()
    {
        StartCoroutine(WaitThenActivateCameraView());
    }
    public IEnumerator WaitThenActivateCameraView()
    {
        yield return frameEnd; //wait for end of frame to avoid both paused/unpaused input triggering
        ActivateIdeaCameraView();
    }
    public void ActivateIdeaCameraView()
    {
        //Set bool so the Interactable system understands a Menu window has opened
        PlayerUIManager.instance.menuWindowIsOpen = true;

        leftAndRightLookAngle = PlayerCamera.instance.leftAndRightLookAngle;
        upAndDownLookAngle = PlayerCamera.instance.upAndDownLookAngle;
        //deactivate player
        player.canMove = false;
        player.isMoving = false;
        PlayerUIManager.instance.gameObject.SetActive(false);
        //Disable Controls
        PlayerInputManager.instance.SafeDisable(false, true);
        //deactivate player camera

        PlayerCamera.instance.cameraObject.enabled = false;
        //activate camera ui
        playerControls.IdeaCameraView.Enable();
        canvas.gameObject.SetActive(true);
        cameraLensCrosshair.SetActive(true);
        border.SetActive(true);
        photoPreviewFrame.SetActive(false);
        //activate camera
        ideaCamera.gameObject.SetActive(true);

        //Disable Weapon Graphics
        if(PlayerWeaponManager.instance.GetMainHand() != null)
            PlayerWeaponManager.instance.GetMainHand().gameObject.SetActive(false);
        if (PlayerWeaponManager.instance.GetOffHand() != null)
            PlayerWeaponManager.instance.GetOffHand().gameObject.SetActive(false);

    }
    public void DeactivateIdeaCameraView()
    {
        //Set bool so the Interactable system understands a Menu window has closed
        PlayerUIManager.instance.menuWindowIsOpen = false;

        //deactivate camera controls
        playerControls.IdeaCameraView.Disable();
        //deactivate camera ui - TODO pretty sure this should be simpler
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
        //Enable Controls
        PlayerInputManager.instance.SafeEnable();

        //Re-enable Weapon Graphics
        PlayerWeaponManager.instance.GetMainHand().gameObject.SetActive(true);
        PlayerWeaponManager.instance.GetOffHand().gameObject.SetActive(true);
    }
    IEnumerator WaitToEndOfFrameThenExit()
    {
        capturePhotoInput = false;
        deactivateCameraViewInput = false;
        yield return frameEnd; //wait for end of frame to avoid both paused/unpaused input triggering
        DeactivateIdeaCameraView();
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
        IdeaScript nearestTarget = null;
        RaycastHit hit;
        Vector3 origin = ideaCamera.transform.position;
        Vector3 direction = ideaCamera.transform.forward;

        if (Physics.SphereCast(origin, ideaCaptureRadius, direction, out hit, ideaCaptureRange, ideaLayers))
        {
            nearestTarget = hit.collider.GetComponent<IdeaScript>();
        }

        return nearestTarget;
    }
    //Idea Capture button
    void HandleCapturePhotoInput()
    {
        if (capturePhotoInput) // [Space], (X)
        {
            capturePhotoInput = false;
            TakeScreenshotInput();
        }
    }
    void HandleDeactivateCameraViewInput()
    {
        if (deactivateCameraViewInput) // [Esc], (Y), D-Pad Up
        {
            deactivateCameraViewInput = false;
            StartCoroutine(WaitToEndOfFrameThenExit());
        }
    }
}
