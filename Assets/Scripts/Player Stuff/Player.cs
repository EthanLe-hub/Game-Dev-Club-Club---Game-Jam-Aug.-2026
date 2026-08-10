// Ethan Le (8/7/2026):
using UnityEngine;
using UnityEngine.InputSystem; 
using Unity.Cinemachine;

/** 
 * Script for player movement and controls.
**/
public class Player : MonoBehaviour
{
    InputSystem_Actions controls; // Variable to hold the controls script for the player. 

    Vector2 moveInput; // The direction and magnitude of where the player is currently moving. 
    CinemachineCamera cinemachineCam; // Variable holding the [Cinemachine] camera that follows the player. 
    CinemachineInputAxisController camInputController; // The component that manipulates the actual [Cinemachine] camera. 
    string cineCamTag = "Camera"; 
    
    Camera playerCamera; // Needed for referencing the front, back, and sides in respective to where the camera is facing. 
    Vector3 camForward; // Which way is forward of the current camera angle. 
    Vector3 camRight; // Which way is right of the current camera angle. 

    float moveSpeed = 6f; // Speed of the player.
    float rotationSpeed = 2f; // Lower values slow down rotation. 
    float gravity = 10f;  
    Vector3 desiredMoveDirection; // Movement calculated in relation to where the camera is facing. 
    Vector3 officialMoveDirection; // Final calculated movement direction with speed incorporated. 
    bool isTalking = false; // Flag used when talking to other characters. 

    Animator animator; // Likely being attached in the child of the GameObject holding this script. 
    CharacterController playerController; // The component that moves the actual player entity. 
    Transform playerTransform; // Position and rotation of the player. 

    void Awake()
    {
        controls = new InputSystem_Actions(); // Assign controls script for the player. 

        // Expected control type for "Move" action is a Vector2 (read its value and assign it into moveInput):
        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>(); 
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero; 

        // animator = GetComponentInChildren<Animator>(); 
        playerController = GetComponent<CharacterController>(); 
        playerTransform = this.transform; 
    }

    public void OnEnable() => controls.Enable(); // Turns on player controls. 
    public void OnDisable() => controls.Disable(); // Turns off player controls. 

    void Start()
    {
        playerCamera = Camera.main; // Get the main camera. 
/*
        Cursor.lockState = CursorLockMode.None; // Use cursor to move the camera angle. 
        Cursor.visible = true; // Can see your cursor. 
*/
        SetCursorState(true); 
        
        GameObject camObj = GameObject.FindGameObjectWithTag(cineCamTag); 
        if (camObj != null)
        {
            camInputController = camObj.GetComponent<CinemachineInputAxisController>(); 
            cinemachineCam = camObj.GetComponent<CinemachineCamera>(); 
            if (camInputController != null)
            {
                camInputController.enabled = true; 
            }
        }
    }

    void Update()
    {
        // Keep cursor locked if user clicks back into the window: 
        if (Cursor.lockState != CursorLockMode.Locked && !GameManager.Instance.isCutsceneActive)
        {
            SetCursorState(true); 
        }

        // Disable movement during cutscene, re-enable movement when cutscene is over:
        if (GameManager.Instance.isCutsceneActive && controls.asset.enabled)
        {
            OnDisable(); // Turn off player movement if cutscene is playing. 
        }
        else if (!GameManager.Instance.isCutsceneActive && !controls.asset.enabled)
        {
            OnEnable(); // Turn on player movement again if cutscene is no longer playing. 
        }
        
        // Disable movement during accusation, re-enable movement when cutscene is over:
        if (GameManager.Instance.isAccusing && controls.asset.enabled)
        {
            SetCursorState(false); // Allow player to use cursor to make accusation. 
            OnDisable(); // Turn off player movement while accusing. 
        }
        else if (!GameManager.Instance.isAccusing && !controls.asset.enabled)
        {
            SetCursorState(true); // Lock and hide the cursor when accusation is over. 
            OnEnable(); // Turn on player movement again if no longer accusing for the night. 
        }

        // Logic to move on from the death cutscene when it plays:
        if (GameManager.Instance.isCutsceneActive && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            GameManager.Instance.HelperCloseCutscene(); 
        }

        // (1) Capture camera angles:
        camForward = playerCamera.transform.forward; 

        // (2) Ensure player stays on the ground:
        camForward.y = 0; 
        camForward.Normalize(); 

        // (3) Rotate player transform immediately to face camera's horizontal angle: 
        if (camForward != Vector3.zero)
        {
            playerTransform.forward = camForward; 
        }

        // Steps 3-6 for player movement are in FixedUpdate(). 

/*
        // Mouse look (requires right-mouse-button hold to look around):
        if (camInputController != null)
        {
            if (Mouse.current.rightButton.isPressed)
            {
                // Lock cursor and hide it while dragging:
                Cursor.lockState = CursorLockMode.Locked; // Lock cursor in place.
                Cursor.visible = false; // Hide the cursor at that place. 

                camInputController.enabled = true; // Use cam controller to rotate the camera angle when Right Mouse Button is held down. 
            }
            else
            {
                // Release the mouse and make it visible when no longer dragging:
                Cursor.lockState = CursorLockMode.None; // Release the cursor.
                Cursor.visible = true; // Cursor shows again. 

                camInputController.enabled = false; // Disable cam controller so camera angle does not keep rotating. 
            }
        }
*/
    }

    void FixedUpdate()
    {
        // (4) Calculate movement direction based on captured camera angles:
        // Move forward/backward and stay grounded (don't start escalating upwards), and move left/right: 
        desiredMoveDirection = (playerTransform.forward * moveInput.y) + (playerTransform.right * moveInput.x); 

/*
        // Slowly rotate the player to match the camera angle:
        if (desiredMoveDirection != Vector3.zero)
        {
            // Shift the player's transform values slowly in the desired direction: 
            playerTransform.forward = Vector3.Slerp(playerTransform.forward, desiredMoveDirection, Time.deltaTime * 15f); 
        }
*/

        // (5) Grab y-axis movement direction (needed in case the player is not grounded):
        float movementY = officialMoveDirection.y; 

        // (6) Finally, create the official movement direction vector (with speed included):
        officialMoveDirection = desiredMoveDirection * moveSpeed; 

        // Check to ensure player is grounded (check its CharacterController's value "isGrounded"):
        if (playerController.isGrounded)
        {
            officialMoveDirection.y = -1f; // Keep player stable on the ground if already on the ground. 
        }
        else // If player not on the ground, have them slowly fall based on gravity over time:
        {
            officialMoveDirection.y = movementY - gravity * Time.deltaTime; 
        }

        // (7) Now, actually move the player over time:
        playerController.Move(officialMoveDirection * Time.deltaTime); 
    }

    private void SetCursorState(bool isLocked)
    {
        Cursor.lockState = isLocked ? CursorLockMode.Locked : CursorLockMode.None; // First-person control locks cursor. 
        Cursor.visible = !isLocked; // Hide cursor in first-person. 
    }
}