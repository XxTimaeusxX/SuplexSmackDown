using UnityEngine;
using UnityEngine.TextCore.Text;

public class PlayerMovementManager : MonoBehaviour
{
    PlayerManager player;
    [SerializeField] Transform groundCheck;

    [HideInInspector] public float verticalMovement;
    [HideInInspector] public float horizontalMovement;
    [HideInInspector] public float moveAmount;

    [Header("Ground Check & Jumping")]
    [SerializeField] float gravityForce;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float groundCheckSphereRadius;
    [SerializeField] Vector3 yVelocity;
    [SerializeField] float groundedYVelocity;
    [SerializeField] float fallStartYVelocity;
    bool fallingVelocityHasBeenSet = false;
    float inAirTimer = 0;

    [Header("Movement Settings")]
    [SerializeField] float walkingSpeed;
    [SerializeField] float runningSpeed;
    [SerializeField] float rotationSpeed;
    [HideInInspector] public Vector3 moveDirection;
    private Vector3 targetRotationDirection;

    [Header("Dash")]
    public float dashSpeed;
    [HideInInspector] public Vector3 dashDirection;

    [Header("Jump")]
    [SerializeField] float jumpHeight;
    private Vector3 jumpDirection;
    [SerializeField] float jumpingSpeed;
    [SerializeField] float freeFallSpeed;

    private void Awake()
    {
        player = GetComponent<PlayerManager>();
    }

    private void Update()
    {
        HandleGroundCheck();

        if (player.isGrounded)
        {
            if (yVelocity.y < 0)
            {
                inAirTimer = 0;
                fallingVelocityHasBeenSet = false;
                yVelocity.y = groundedYVelocity;
            }
        }
        else
        {
            if (!player.isJumping && !fallingVelocityHasBeenSet)
            {
                fallingVelocityHasBeenSet = true;
                yVelocity.y = fallStartYVelocity;
            }

            inAirTimer = inAirTimer + Time.deltaTime;
            player.animator.SetFloat("InAirTimer", inAirTimer);

            yVelocity.y += gravityForce * Time.deltaTime;
        }

        player.characterController.Move(yVelocity * Time.deltaTime);
    }

    public void HandleAllMovement()
    {
        HandleGroundedMovement();
        HandleRotation();
        HandleJumpingMovement();
        HandleFreeFallMovement();
    }

    private void HandleGroundCheck()
    {
        player.isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckSphereRadius, groundLayer);
    }

    private void GetVerticalAndHorizontalInputs()
    {
        verticalMovement = PlayerInputManager.instance.verticalInput;
        horizontalMovement = PlayerInputManager.instance.horizontalInput;
        moveAmount = PlayerInputManager.instance.moveAmount;
    }

    private void HandleGroundedMovement()
    {
        if (!player.canMove)
        {
            return;
        }

        GetVerticalAndHorizontalInputs();

        moveDirection = PlayerCamera.instance.transform.forward * verticalMovement;
        moveDirection = moveDirection + PlayerCamera.instance.transform.right * horizontalMovement;
        moveDirection.Normalize();
        moveDirection.y = 0;

        //Running Speed
        if (PlayerInputManager.instance.moveAmount > 0.5f)
        {
            player.characterController.Move(moveDirection * runningSpeed * Time.deltaTime);
        }
        //Walking Speed
        else if (PlayerInputManager.instance.moveAmount <= 0.5f)
        {
            player.characterController.Move(moveDirection * walkingSpeed * Time.deltaTime);
        }
    }

    private void HandleJumpingMovement()
    {
        if (player.isJumping)
        {
            player.characterController.Move(jumpDirection * jumpingSpeed * Time.deltaTime);
        }
    }

    private void HandleFreeFallMovement()
    {
        if (!player.isGrounded)
        {
            Vector3 freeFallDirection;

            freeFallDirection = PlayerCamera.instance.transform.forward * PlayerInputManager.instance.verticalInput;
            freeFallDirection = freeFallDirection + PlayerCamera.instance.transform.right * PlayerInputManager.instance.horizontalInput;
            freeFallDirection.y = 0;

            player.characterController.Move(freeFallDirection * freeFallSpeed * Time.deltaTime);
        }
    }

    private void HandleRotation()
    {
        if (!player.canRotate)
        {
            return;
        }

        targetRotationDirection = Vector3.zero;
        targetRotationDirection = PlayerCamera.instance.cameraObject.transform.forward * verticalMovement;
        targetRotationDirection = targetRotationDirection + PlayerCamera.instance.cameraObject.transform.right * horizontalMovement;
        targetRotationDirection.Normalize();
        targetRotationDirection.y = 0;

        if (targetRotationDirection == Vector3.zero)
        {
            targetRotationDirection = transform.forward;
        }

        Quaternion newRotation = Quaternion.LookRotation(targetRotationDirection);
        Quaternion targetRotation = Quaternion.Slerp(transform.rotation, newRotation, rotationSpeed * Time.deltaTime);
        transform.rotation = targetRotation;
    }

    public void AttemptToPerformDash()
    {
        if (player.isPerformingAction)
        {
            return;
        }

        dashDirection = PlayerCamera.instance.cameraObject.transform.forward * verticalMovement;
        dashDirection += PlayerCamera.instance.cameraObject.transform.right * horizontalMovement;
        dashDirection.y = 0;
        dashDirection.Normalize();

        Quaternion playerRotation = Quaternion.LookRotation(dashDirection);
        player.transform.rotation = playerRotation;

        player.playerAnimatorManager.PlayTargetActionAniamtion("Dash", true);
    }

    public void AttemptToPerformJump()
    {
        if (player.isPerformingAction)
        {
            return;
        }
        if (player.isJumping)
        {
            return;
        }
        if (!player.isGrounded)
        {
            return;
        }

        player.playerAnimatorManager.PlayTargetActionAniamtion("Jump", false, false);

        player.isJumping = true;

        jumpDirection = PlayerCamera.instance.cameraObject.transform.forward * PlayerInputManager.instance.verticalInput;
        jumpDirection += PlayerCamera.instance.cameraObject.transform.right * PlayerInputManager.instance.horizontalInput;
        jumpDirection.y = 0;

        if (jumpDirection != Vector3.zero)
        {
            if (PlayerInputManager.instance.moveAmount > 0.5)
            {
                jumpDirection *= 0.5f;
            }
            else if (PlayerInputManager.instance.moveAmount <= 0.5)
            {
                jumpDirection *= 0.25f;
            }
        }
    }

    public void ApplyJumpingVelocity()
    {
        yVelocity.y = Mathf.Sqrt(jumpHeight * -2 * gravityForce);
    }
}
