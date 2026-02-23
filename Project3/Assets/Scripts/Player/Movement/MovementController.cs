using UnityEngine;
using UnityEngine.InputSystem;

// ~Istvan W


// TODO: Add rotation locking logic using the isTesting boolean
// TODO: Make sure carry proxy collider works properly

[RequireComponent(typeof(CharacterController))]
public class MovementController : MonoBehaviour
{
    [Header("References")]
    public CharacterController controller;
    private PlayerAnimationController playerAnimationController;
    private MovementConfig movementConfig;
    private PlayerInput playerInput;
    private PlayerDash playerDash;
    private SuplexController suplexController;
    private PlayerThrow playerThrow;

    public Transform cameraTransform;

    // private Vector3 velocity;

    [Header("Boolean States")]
    public bool overrideVerticalMotion = false;
    public bool isGrounded;
    public bool isTemp;

    [Header("Movement Actions")]
    public InputAction moveAction;
    public InputAction dashAction;
    public InputAction jumpAction;
    public InputAction dropAction;

    // Super Suplex Buttons
    public InputAction leftBumper;
    public InputAction rightBumper;

    // Aim/Throw
    public InputAction throwAction;
    public InputAction aimAction;

    // Testing
    public InputAction testButton;

    public Vector2 InputDirection => moveAction.ReadValue<Vector2>();
    public Vector3 velocity;          // Player's current velocity
    public Vector3 move;              // Horizontal movement vector
    private float rotationVelocity;     // Keeps track of rotation velocity for smooth damping


    private void Awake()
    {
        /// Reference instances
        movementConfig = GetComponent<MovementConfig>();
        playerInput = GetComponent<PlayerInput>();
        controller = GetComponent<CharacterController>();
        playerDash = GetComponent<PlayerDash>();
        suplexController = GetComponent<SuplexController>();
        playerThrow = GetComponent<PlayerThrow>();
        playerAnimationController = GetComponent<PlayerAnimationController>();

        /// Find input actions
        moveAction = playerInput.actions.FindAction("Move");
        dashAction = playerInput.actions.FindAction("Dash");
        jumpAction = playerInput.actions.FindAction("Jump");
        dropAction = playerInput.actions.FindAction("Drop");
        // Super Suplex Buttons
        leftBumper = playerInput.actions.FindAction("LB");
        rightBumper = playerInput.actions.FindAction("RB");
        // Aim/Throw
        throwAction = playerInput.actions.FindAction("Throw");
        aimAction = playerInput.actions.FindAction("Aim");
        // Test
        testButton = playerInput.actions.FindAction("Test");
    }

    private void Update()
    {
        isGrounded = controller.isGrounded;
        if (isGrounded)
        {
            if (velocity.y < 0)
                velocity.y = -2f;

            velocity.x = 0f;
            velocity.z = 0f;
        }

        /// Dash input
        if (dashAction.WasPressedThisFrame())   
        { 
            //Debug.Log("DASH ATTEMPT"); 
            playerDash.Dash(); 
        }

        /// Drop input
        if (dropAction.WasPressedThisFrame())
        {
            //Debug.Log("Drop pressed.");
            suplexController.ReleaseEnemy(Vector3.zero);
        }

        /// Handle movement
        if (playerDash.isDashing)
        {
            Vector3 forward = transform.forward; forward.y = 0f;
            move = forward.normalized * movementConfig.dashSpeed;

        }
        else { HandleMovement(); }


        /// Jump input
        if (jumpAction.WasPressedThisFrame() && isGrounded && !suplexController.isSuplexing)
        { 
            Jump(); 
        }
        else if (jumpAction.WasPressedThisFrame() && suplexController.isSuplexing && !playerDash.isDashing && !suplexController.suplexInputLocked)
        {
            ForceJump();
            Debug.Log("Forced jump during suplex!");
        }

        /// Aim input
        if (aimAction.IsPressed() && suplexController.carriedObject != null)
        {
            // TODO: Add trajectory prediction
            // TODO: Add trajectory prediction for suplex throws
            //Debug.Log("Aiming with object");
            transform.forward = CamForward; // Face the same direction as the camera while aiming
        }

        /// Throw input
        if (throwAction.WasPressedThisFrame() && suplexController.carriedObject != null)
        {
            playerThrow.Throw();
        }

        /// Testing
        if (testButton.WasPressedThisFrame())
        {
            //Debug.Log("Test button pressed");

        }

        /// Gravity
        if (!overrideVerticalMotion) // Use this to override vertical motion during suplex testing
        {
            if (playerDash.isDashing == false)
                velocity.y += movementConfig.customGravity * Time.deltaTime;
            else
                velocity.y = 0;                                                 // No vertical movement while dashing
        }


        /// Move char: Hor + Vert seperately
        Vector3 finalMove = move * Time.deltaTime;

        if (!overrideVerticalMotion)
        {
            finalMove = (move + new Vector3(velocity.x, 0, velocity.z)) * Time.deltaTime;
            finalMove.y = velocity.y * Time.deltaTime;
        }
        else
        {
            finalMove = move * Time.deltaTime;
            finalMove.y = 0f;
        }

        controller.Move(finalMove);
        playerAnimationController.CheckAnimation();
    }

    private void HandleMovement()
    {
        /// Implement movement handling using movementConfig values
        Vector2 input = moveAction.ReadValue<Vector2>();

        if (input.magnitude >= 0.1f)
        {

            /// Camera-relative directions (ignore vertical tilt)
            Vector3 camRight = cameraTransform.right;

            /// Convert input into world-space movement
            Vector3 moveDir = (CamForward * input.y + camRight * input.x).normalized;

            /// target angle for rotation
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
        isGrounded = false;
        //Debug.Log("Jumped");
    }
    private void ForceJump()
    {
        suplexController.JumpOff();
        velocity.y = Mathf.Sqrt(movementConfig.jumpHeight * 5f * -2f * movementConfig.customGravity);
        isGrounded = false;
    }
    public void SetVelocity(Vector3 newVelocity)
    {
        velocity = newVelocity;
    }

    public void AddImpulse(Vector3 impulse)
    {
        velocity += impulse;
    }
    public Vector3 CamForward // Helper property to get camera-relative forward direction on the horizontal plane
    {
        get
        {
            return Vector3.Scale(cameraTransform.forward, new Vector3(1, 0, 1)).normalized;
        }
    }
}
