using UnityEngine;
using UnityEngine.InputSystem;

// ~Istvan W

// Last Edited: 1/19/2026 by Istvan W.

[RequireComponent(typeof(CharacterController))]
public class MovementController : MonoBehaviour
{
    [Header("References")]
    private MovementConfig movementConfig;
    private PlayerInput playerInput; 
    private CharacterController controller;
    private PlayerDash playerDash;
    private PlayerSuplex playerSuplex;

    public Transform cameraTransform;

    // private Vector3 velocity;

    [Header("Boolean States")]
    private bool isGrounded;
    //private bool isDashing;
    public bool isTemp;

    [Header("Movement Actions")]
    public InputAction moveAction;
    public InputAction dashAction;
    public InputAction jumpAction;
    public InputAction dropAction;

    // Super Suplex Buttons
    public InputAction leftBumper;
    public InputAction rightBumper;

    // Test
    public InputAction testButton;

    private Vector3 velocity;          // Player's current velocity
    public Vector3 move;              // Horizontal movement vector
    private float rotationVelocity;     // Keeps track of rotation velocity for smooth damping


    private void Awake()
    {

        /// Reference instances
        movementConfig = GetComponent<MovementConfig>();
        playerInput = GetComponent<PlayerInput>();
        controller = GetComponent<CharacterController>();
        playerDash = GetComponent<PlayerDash>();
        playerSuplex = GetComponent<PlayerSuplex>();

        /// Find input actions
        moveAction = playerInput.actions.FindAction("Move");
        dashAction = playerInput.actions.FindAction("Dash");
        jumpAction = playerInput.actions.FindAction("Jump");
        dropAction = playerInput.actions.FindAction("Drop");
        // Super Suplex Buttons
        leftBumper = playerInput.actions.FindAction("LB");
        rightBumper = playerInput.actions.FindAction("RB");
        // Test
        testButton = playerInput.actions.FindAction("Test");
    }

    private void Update()
    {
        isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;                                               // Small negative value to keep grounded
        }

        /// Dash input
        if (dashAction.WasPressedThisFrame())   { playerDash.Dash(); }

        /// Drop input
        if (dropAction.WasPressedThisFrame())
        {
            Debug.Log("Drop pressed. carriedEnemyBase = " + playerSuplex.carriedEnemyBase);

            if (playerSuplex.carriedEnemyBase != null)
            {
                playerSuplex.carriedEnemyBase.ExitCarriedState(Vector3.zero);
                playerSuplex.carriedEnemyBase = null;   // Clear reference
                playerSuplex.isSuplexing = false;
            }
            else
            {
                Debug.Log("No carried object to drop.");
            }
        }

        /// Handle movement
        if (playerDash.isDashing)
        {
            Vector3 forward = transform.forward; forward.y = 0f;
            move = forward.normalized * movementConfig.dashSpeed;

        }
        else { HandleMovement(); }


        /// Jump input
        if (jumpAction.WasPressedThisFrame() && isGrounded) { Jump(); }

        /// Test
        if (testButton.WasPressedThisFrame())
        {
            Debug.Log("Test button pressed");
        }

        /// Gravity
        if (playerDash.isDashing == false)
            velocity.y += movementConfig.customGravity * Time.deltaTime;
        else
            velocity.y = 0;                                                 // No vertical movement while dashing

        /// Move char: Hor + Vert seperately
        Vector3 finalMove = move * Time.deltaTime;
        finalMove.y = velocity.y * Time.deltaTime;

        controller.Move(finalMove);
    }

    private void HandleMovement()
    {
        /// Implement movement handling using movementConfig values
        Vector2 input = moveAction.ReadValue<Vector2>();

        if (input.magnitude >= 0.1f)
        {
            /// Camera-relative directions (ignore vertical tilt)
            Vector3 camForward = Vector3.Scale(cameraTransform.forward, new Vector3(1, 0, 1)).normalized;
            Vector3 camRight = cameraTransform.right;

            /// Convert input into world-space movement
            Vector3 moveDir = (camForward * input.y + camRight * input.x).normalized;

            /// Target angle for rotation
            float targetAngle = Mathf.Atan2(moveDir.x, moveDir.z) * Mathf.Rad2Deg;

            /// Smooth rotation
            float rotationSpeedModifier = movementConfig.rotationSmoothTime;

            /// Apply carry weight modifiers
            if (isTemp)
                rotationSpeedModifier *= 5f;                             // Placeholder for carry weight modifier

            float angle = Mathf.SmoothDampAngle(
                transform.eulerAngles.y,
                targetAngle,
                ref rotationVelocity,
                rotationSpeedModifier
            );

            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            //// TODO: Finish implementing sprinting logic
            /// Apply movement speed (walk vs sprint)
            float speed = movementConfig.walkSpeed;

            /// Apply carry weight modifiers
            if (isTemp)
                speed *= 1f;                                                    // Placeholder for carry weight modifier
            ////
            move = moveDir * speed;
        }
        else
        {
            move = Vector3.zero;                                            // no input, no horizontal move
        }

    }
    private void Jump()
    {
        velocity.y = Mathf.Sqrt(movementConfig.jumpHeight * -2f * movementConfig.customGravity);
        Debug.Log("Jumped");

    }
}
