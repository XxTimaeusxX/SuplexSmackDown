using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.ProBuilder.MeshOperations;

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
    public bool isGrounded;
    public LayerMask groundMask;
    private Vector2 direction; // variable to store movement direction 
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
    [Header("Player health")]
    private PlayerHealth _playerhealth;
    private int lastHealth = int.MinValue;
    private bool IsHurt = false;

    [Header("Animation")]
    public Animator CoheteAnimator;
    private string CurrentAnimation = "";

    // --- Freefall timing ---
    [Header("Free fall settings")]
    private float airTime = 0f;
    public float freefallDelay = 1.5f;
    // minimum downward velocity required to be considered freefall
    public float freefallVelocityThreshold = -0.5f;

    private bool isPlayingHurt = false;
    private static readonly int HurtHash = Animator.StringToHash("HURT");

    private void Start()
    {
        _playerhealth = GetComponent<PlayerHealth>();
        CoheteAnimator = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions.FindAction("Move");
        jumpAction = playerInput.actions.FindAction("Jump");
        isMoving = false;
        playerSuplex = GetComponent<PlayerSuplex>();
        playerDash = GetComponent<PlayerDash>();
        moveSpeed = startingMoveSpeed;
        ChangeAnimtion("IDLE");
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

        if (controller != null)
        {
            controller.Move(velocity * Time.deltaTime);
        }
        
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
          //  ChangeAnimtion("JUMP");
            // resetting air timer when initiating a jump so FREEFALL won't trigger too soon
            airTime = 0f;
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
        // update air-time tracking
        if (!isGrounded)
            airTime += Time.deltaTime;
        else
            airTime = 0f;

        // Detect health decrease to request HURT animation
        if (_playerhealth != null)
        {
            int currenthealth = _playerhealth.currentHealth;
            if (lastHealth != int.MinValue && currenthealth < lastHealth)
            {
                IsHurt = true;
            }
            lastHealth = currenthealth;
        }
        CheckAnimation();
    }

    void MovePlayer()
    {
        direction = moveAction.ReadValue<Vector2>();
        //Vector2 direction = moveAction.ReadValue<Vector2>();

        if (direction.magnitude >= 0.1f)
        {
            // Only fire once when movement starts
            isMoving = true;
            float targetAngle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg + thirdPersonCamera.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            controller.Move(moveDirection.normalized * moveSpeed * Time.deltaTime);
        }
        else
        {
            isMoving = false;
        }

    }

    void Jump()
    {

        bool isHoldingEnemy = playerSuplex.grabHandler.IsHoldingEnemy();
        if (isGrounded && !isHoldingEnemy)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            isGrounded = false;
            airTime = 0f; // start counting air time from zero
            AudioManager.PlayJumping();
            // Debug.Log("Jumped!");
        }
      
        else if (isHoldingEnemy && !playerSuplex.isSuplexing)
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
      //  CoheteAnimator.SetTrigger("jump");
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

    //---------------- Animation ---------------------------//
    public void ChangeAnimtion(string animation, float crossfade = 0.2f)
    {
        if(CurrentAnimation!= animation)
        {
            CurrentAnimation = animation;
            CoheteAnimator.CrossFade(animation, crossfade);
            
        }
    }
    private void CheckAnimation()
    {
        //--------- HURT handling -----------//
        if (isPlayingHurt)// If we are playing HURT (no transitions), don't switch to anything else until the clip finishes.
        {
            var currentstate = CoheteAnimator.GetCurrentAnimatorStateInfo(0);
            // If Animator is blending, let it settle
            if (CoheteAnimator.IsInTransition(0))
                return;

            // Keep HURT locked until its clip finishes
            if (currentstate.shortNameHash == HurtHash && currentstate.normalizedTime < 0.99f)
                return;

            // Clip finished, unlock and fall through to normal selection
            isPlayingHurt = false;
            CurrentAnimation = ""; // allow next state change this frame
        }

        // Enter HURT when health drops; set lock so nothing overrides it
        if (IsHurt)
        {
            ChangeAnimtion("HURT");
            isPlayingHurt = true;
            IsHurt = false; // consume request
            return;
        }

        // ----- GRAB / GRABAIR / GRABWALK -----
        if (playerSuplex.grabHandler.IsHoldingEnemy())
        {
           
            if (!isGrounded)
            {
                ChangeAnimtion("GRABAIR");
                return;
            }
            // Make GRABWALK behave like WALK: every time movement resumes, switch to GRABWALK.
            if (direction.magnitude >= 0.1f)
            {
              //  Debug.Log("Changing to GRABWALK animation");
                ChangeAnimtion("GRABWALK");
                return;
            }

            return;
        }
        // ----- DASHING -----//
        if (playerDash.isDashing)
        {
            ChangeAnimtion("GRABAIR");
            return;
        }
        //----- jump / freefall / walk / idle settiings -----//
        if (!isGrounded)// Jumping takes priority if not grounded
        {
            if(velocity.y > 0.01f) ChangeAnimtion("JUMP");
            else
            {
                // only switch to FREEFALL after the configured delay AND a sufficient downward velocity
                if (airTime >= freefallDelay && velocity.y <= freefallVelocityThreshold)
                    ChangeAnimtion("FREEFALL");
                else
                    ChangeAnimtion("JUMP"); // still considered jump/rise or early fall
            }
            return;
        }

        // ----- Grounded movement -----
        if (direction.magnitude >= 0.1f) ChangeAnimtion("WALK");
        else ChangeAnimtion("IDLE");



    }
}
