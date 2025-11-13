using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public Transform thirdPersonCamera;
    public Transform groundCheck;
    public PlayerInput playerInput;
    InputAction moveAction;
    public InputAction jumpAction;
    public Vector3 velocity;
    public float velocityCap = -20f;
    bool isGrounded;
    public LayerMask groundMask;
    public float moveSpeed;
    public float startingMoveSpeed = 5f;
    public float moveAcceleration;
    public float maxAcceleration;
    public float gravity = -9.81f; // Set to Unity's default gravity and change Unity's gravity to -50f
    public float groundDistance;
    public float jumpHeight;
    public float turnSmoothTime;
    float turnSmoothVelocity;
    public CinemachineCamera CinemachineCamera;
    [SerializeField] private bool isMoving;

    PlayerSuplex playerSuplex;
    PlayerDash playerDash;

    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions.FindAction("Move");
        jumpAction = playerInput.actions.FindAction("Jump");
        isMoving = false;
        playerSuplex = GetComponent<PlayerSuplex>();
        playerDash = GetComponent<PlayerDash>();
        moveSpeed = startingMoveSpeed;
    }

    private void Update()
    {
        if (!playerDash.isDashing)
        {
            velocity.y += gravity * Time.deltaTime;
        }
        else if (playerDash.isDashing)
        {
            velocity.y = 0; 
        }
 
        controller.Move(velocity * Time.deltaTime);
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        //  Reset horizontal momentum
            velocity.x = 0f; 
            velocity.z = 0f;
            playerDash.airDashCount = 2;
        }

        if (velocity.y < velocityCap)
            velocity.y = Mathf.Clamp(velocity.y, velocityCap, 100);   


        MovePlayer();

        if (jumpAction.WasPressedThisFrame())
        {
            Jump();
        }
        HandleCeilingHit();

        if (isMoving)
        {
            moveSpeed += moveAcceleration * Time.deltaTime;
        }
        else
        {
            moveSpeed = startingMoveSpeed;
        }
        if (moveSpeed >= maxAcceleration)
        {
            moveSpeed = maxAcceleration;
        }
    }

    void MovePlayer()
    {
        Vector2 direction = moveAction.ReadValue<Vector2>();

        if (direction.magnitude >= 0.1f)
        {
            isMoving = true;
            float targetAngle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg + thirdPersonCamera.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            controller.Move(moveDirection.normalized * moveSpeed * Time.deltaTime);
        }
        if (direction.magnitude == 0)
        {
            isMoving = false;
        }
    }

    void Jump()
    {
        if (isGrounded && playerSuplex.grabbedEnemy == null)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            isGrounded = false;
            AudioManager.PlayJumping();
            // Debug.Log("Jumped!");
        }
        else if (playerSuplex.grabbedEnemy != null && !playerSuplex.isSuplexing)
        {
            StartCoroutine(playerSuplex.WaitForSuplexInput());
            // Debug.Log("Waiting for suplex input!");
        }
    }

    /// <summary>
    /// Forces the player to jump, used when jumping off an enemy during a suplex.
    /// </summary>
    public void ForceJump()
    {
        velocity.y = Mathf.Sqrt(jumpHeight*5f * -2f * gravity);
        isGrounded = false;
        AudioManager.PlayJumping();
        // Debug.Log("jumping off enemy");
    }

    public void HandleCeilingHit()
    {
        if (controller != null && (controller.collisionFlags & CollisionFlags.Above) != 0 && velocity.y > 0f)
        {
            velocity.y = -2f; // cancel upward momentum if we hit ceiling
            Debug.Log("Hit ceiling while jumping off enemy, cancelling upward momentum.");
        }
    }
}
