using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputManager instance;
    PlayerControls playerControls;
    [SerializeField] Vector2 movementInput;
    public float horizontalInput;
    public float verticalInput;
    public float moveAmount;


    //Start is called before the first frame update
    void Start() {
        //Has to happen before we disable the instance
        DontDestroyOnLoad(gameObject);

        //When the scene changes, run this logic
        //This is to do with subscribing and might require research
        SceneManager.activeSceneChanged += OnSceneChange;
        
        instance.enabled = false;
    }

    //Update is called once per frame
    void Update()
    {
        HandleMovementInput();
        //Debug.Log("PlayerInputManager Update");
    }

    //Goals:
    //1. Read joystick values
    //2. Move Character using those values
    private void OnEnable() {
        if (playerControls == null) {
            playerControls = new PlayerControls();
            playerControls.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
        }

        playerControls.Enable();
    }

    private void Awake() {

        //Establish Singleton Instance
        if (instance == null) {
            instance = this;
        }
        else {
            Destroy(gameObject);
        }
    }

    private void OnSceneChange(Scene oldScene, Scene newScene) {
        //If we are loading into our world scene, enable our player controls
        if (newScene.buildIndex == WorldSaveGameManager.instance.GetWorldSceneIndex()) {
            //Script being enabled, not the game object
            instance.enabled = true;
        }
        //Otherwise, we're in a menu so disable player controls
        //This is so our player can't move around if we enter things like a character creation menu, etc
        else {
            instance.enabled = false;
        }
    }

    private void OnDestroy() {
        //If we destroy this object, we unsubcribe from this event
        //This is to do with subscribing and might require research
        SceneManager.activeSceneChanged -= OnSceneChange;
    }

    private void HandleMovementInput() {
        verticalInput = movementInput.y;
        horizontalInput = movementInput.x;

        //Returns the absolute value of both movement inputs
        moveAmount = Mathf.Clamp01(Mathf.Abs(verticalInput) + Mathf.Abs(horizontalInput));

        //Clamp moveAmount's values to improve smoothness. Values will be 0, 0.5, or 1 (Optional)
        if (moveAmount <= 0.5 && moveAmount > 0) {
            moveAmount = 0.5f;
        }
        else if (moveAmount > 0.5 && moveAmount <= 1) {
            moveAmount = 1;
        }
    }

    private void OnApplicationFocus(bool focus) {
        if (enabled) {
            if (focus) {
                playerControls.Enable();
            }
            else {
                playerControls.Disable();
            }
        }
    }
}
