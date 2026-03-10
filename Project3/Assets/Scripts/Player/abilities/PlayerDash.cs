using UnityEngine;
using UnityEngine.InputSystem;


// Last Edited: 1 / 19 / 2026 by Istvan W.

/// <summary>
/// Handles the player's dash ability, including input, movement, and hitbox activation.
/// </summary>
public class PlayerDash : MonoBehaviour
{
    [Header("References")]
    private CharacterController controller;
    private MovementConfig movementConfig;
    public SuplexHitboxCaller suplexHitboxCaller;
    private SuplexController suplexController;
    PlayerAnimationController playerAnimationController;
    //private MovementController movementController;
    //private PlayerInput playerInput;        // Reference to the player's input system

    [Header("Dash Settings")]
    //private Vector3 dashDirection;         // Direction the player will dash in
    private float dashTime;                // Time left in the current dash
    private float dashCooldown;
    private int airDashCount;           // Number of dashes a player can perform in air
    public int maxAirDashCount;           // Maximum number of air dashes allowed
    public float dashStartTime;



    [Header("Boolean States")]
    public bool isDashing = false;         // True if the player is currently dashing

    void Start()
    {
        // Reference instances
        //movementController = GetComponent<MovementController>();
        controller = GetComponent<CharacterController>();
        movementConfig = GetComponent<MovementConfig>();
        suplexController = GetComponent<SuplexController>();
        playerAnimationController = GetComponent<PlayerAnimationController>();

        // Setup
        movementConfig.grabHitbox.SetActive(false); 
    }

    void Update()
    {
        if (controller.isGrounded  && !isDashing)              
        { 
            dashCooldown = 0f;  // Reset cooldown on ground
            airDashCount = maxAirDashCount; 
        } 

        if (dashCooldown > 0f && !isDashing)    { dashCooldown -= 0.1f; }
        if (dashCooldown < 0f)                  { dashCooldown = 0f; }
        if (dashTime > 0f && !suplexHitboxCaller.hitTarget)                      { dashTime -= Time.deltaTime; }
        if (dashTime <= 0f || suplexHitboxCaller.hitTarget == true)
        {
            CancelDash();
        }
    }

    public void Dash()
    {

        // Start dash if the dash button was just pressed and not already dashing
        //Debug.Log($"Attempting to dash. isDashing: {isDashing}, dashCooldown: {dashCooldown}, airDashCount: {airDashCount}");
        if (!isDashing && dashCooldown <= 0f && airDashCount > 0)
        {
            //Debug.Log("Dash conditions met. Initiating dash.");
            isDashing = true;
            dashStartTime = Time.time;

            dashCooldown = movementConfig.targetDashCooldown; // Reset cooldown
            dashTime = movementConfig.dashDuration;

            //homingDashActive = false; // input dash by default is not a homing dash

            //Debug.Log($"Movement config: {suplexController.carriedObject}");
            if (movementConfig.grabHitbox != null && suplexController.carriedObject == null)
                movementConfig.grabHitbox.SetActive(true);    // Enable hitbox for the dash

            playerAnimationController.CheckAnimation();


            if (!controller.isGrounded) airDashCount--; // Consume an air dash if not grounded
            //Debug.Log("Dash initiated!");
        }
    }
    public void CancelDash()
    {
        isDashing = false;
        dashTime = 0f;
        dashCooldown = 0f;
        suplexHitboxCaller.hitTarget = false;
        if (movementConfig.grabHitbox != null) movementConfig.grabHitbox.SetActive(false);
    }
}
