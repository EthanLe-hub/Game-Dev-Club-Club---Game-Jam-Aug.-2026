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
    [SerializeField] float inputSmoothTime = 0.08f; // Seconds for movement input to catch up to the sticks/keys (higher = softer acceleration).
    [SerializeField] float mouseSensitivity = 1f; // Multiplies the Cinemachine look gains at startup (1 = as authored in the Inspector).
    Vector2 smoothedMoveInput; // moveInput eased over time, so movement (and the camera following it) does not snap from 0 to full speed.
    Vector2 moveInputVelocity; // Internal state for SmoothDamp.
    Vector3 desiredMoveDirection; // Movement calculated in relation to where the camera is facing. 
    Vector3 officialMoveDirection; // Final calculated movement direction with speed incorporated. 
    public bool isTalking = false; // Flag used when talking to other characters. 

    Animator animator; // Likely being attached in the child of the GameObject holding this script. 
    public CharacterController playerController; // The component that moves the actual player entity. 
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

                // Scale the authored look gains by mouseSensitivity (sign is preserved, so Y-invert stays as authored):
                foreach (var controller in camInputController.Controllers)
                {
                    controller.Input.Gain *= mouseSensitivity;
                }
            }
        }
    }

    void Update()
    {
        
        bool uiNeedsCursor = GameManager.Instance.isAccusing || GameManager.Instance.isAtDoor;
        bool blockControls = GameManager.Instance.isCutsceneActive || uiNeedsCursor;

        if (blockControls && controls.asset.enabled)
        {
            OnDisable(); // Freeze player movement while a cutscene or mouse-driven UI is up.
        }
        else if (!blockControls && !controls.asset.enabled)
        {
            OnEnable(); // Restore player movement once nothing is blocking it.
        }

        if (uiNeedsCursor)
        {
            if (Cursor.lockState == CursorLockMode.Locked) SetCursorState(false); // Free the cursor for UI buttons.
            if (camInputController != null && camInputController.enabled)
            {
                camInputController.enabled = false; // No camera rotation while clicking through UI.
            }
        }
        else
        {
            // Keep cursor locked for first-person control (also re-locks when the user clicks back into the window):
            if (Cursor.lockState != CursorLockMode.Locked && !GameManager.Instance.isCutsceneActive)
            {
                SetCursorState(true);
            }
            if (camInputController != null && !camInputController.enabled)
            {
                camInputController.enabled = true;
            }
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

        // (4) Smooth the raw input so movement accelerates/decelerates gently instead of snapping:
        smoothedMoveInput = Vector2.SmoothDamp(smoothedMoveInput, moveInput, ref moveInputVelocity, inputSmoothTime);

        // (5) Calculate movement direction based on captured camera angles:
        // Move forward/backward and stay grounded (don't start escalating upwards), and move left/right:
        desiredMoveDirection = (playerTransform.forward * smoothedMoveInput.y) + (playerTransform.right * smoothedMoveInput.x);

        // (6) Grab y-axis movement direction (needed in case the player is not grounded):
        float movementY = officialMoveDirection.y;

        // (7) Finally, create the official movement direction vector (with speed included):
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

        playerController.Move(officialMoveDirection * Time.deltaTime);

        // Footstep sounds while actually walking on the ground (SoundManager rate-limits the steps).
        // smoothedMoveInput is checked (not raw input) so steps stop as movement eases out:
        if (playerController.isGrounded && smoothedMoveInput.magnitude > 0.1f && SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayFootstep();
        }

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

    private void SetCursorState(bool isLocked)
    {
        Cursor.lockState = isLocked ? CursorLockMode.Locked : CursorLockMode.None; // First-person control locks cursor. 
        Cursor.visible = !isLocked; // Hide cursor in first-person. 
    }
}