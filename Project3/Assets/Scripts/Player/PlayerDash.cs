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
    public PlayerSuplex playerSuplex;
    //private MovementController movementController;
    //private PlayerInput playerInput;        // Reference to the player's input system

    [Header("Dash Settings")]
    //private Vector3 dashDirection;         // Direction the player will dash in
    private float dashTime;                // Time left in the current dash
    [SerializeField]    private float dashCooldown;
    [SerializeField]    private int airDashCount = 0;           // Number of dashes a player can perform in air

    [Header("Boolean States")]
    public bool isDashing = false;         // True if the player is currently dashing

    //[Header("Homing Dash Settings")]
    //public bool homingDashActive = false; //  identify dashes started by homing attack (to only auto-grab then)
    //public float homingSpeedMultiplier = 10f;
    //public bool enableHitboxOnNormalDash = false;
    //private Transform _homingTarget;

    void Start()
    {
        // Reference instances
        //movementController = GetComponent<MovementController>();
        controller = GetComponent<CharacterController>();
        movementConfig = GetComponent<MovementConfig>();
        playerSuplex = GetComponent<PlayerSuplex>();

        // Setup
        movementConfig.grabHitbox.SetActive(false); 
    }

    void Update()
    {
        if (controller.isGrounded  && !isDashing)              { dashCooldown = 0f; } // Reset cooldown on ground

        if (dashCooldown > 0f && !isDashing)    { dashCooldown -= 0.1f; }
        if (dashCooldown < 0f)                  { dashCooldown = 0f; }
        if (dashTime > 0f && !suplexHitboxCaller.hitTarget)                      { dashTime -= Time.deltaTime; }
        if (dashTime <= 0f || suplexHitboxCaller.hitTarget == true)
        {
            isDashing = false;
            suplexHitboxCaller.hitTarget = false;
            //homingDashActive = false;
            if (movementConfig.grabHitbox != null) movementConfig.grabHitbox.SetActive(false);
        }
    }

    /// <summary>
    /// Checks for dash input and moves the player if dashing.
    /// </summary>
    public void Dash()
    {

        // Start dash if the dash button was just pressed and not already dashing
        if (!isDashing && dashCooldown <= 0f)
        { 
            isDashing = true;

            dashCooldown = movementConfig.targetDashCooldown; // Reset cooldown
            dashTime = movementConfig.dashDuration;

            //homingDashActive = false; // input dash by default is not a homing dash

            if (movementConfig.grabHitbox != null && playerSuplex.carriedEnemyBase == null)
                movementConfig.grabHitbox.SetActive(true);    // Enable hitbox for the dash
            Debug.Log("Dash initiated!");
        }
       
        //// If currently dashing, move the player and count down the dash timer
        // if (isDashing)
        // {
        //    // Re-aim while homing
        //    if (homingDashActive && _homingTarget != null)
        //    {
        //        Vector3 toTarget = _homingTarget.position - transform.position; // include vertical
        //        if (toTarget.sqrMagnitude > 0.0001f)
        //        {
        //            dashDirection = toTarget.normalized;

        //            Vector3 face = new Vector3(dashDirection.x, dashDirection.y, dashDirection.z);
        //            if (face.sqrMagnitude > 0.000001f)
        //                transform.forward = face.normalized;
        //        }
        //    }

        //    float speed = homingDashActive ? movementConfig.dashSpeed * homingSpeedMultiplier : movementConfig.dashSpeed;

        //    // single Move per frame
        //    controller.Move(dashDirection * speed * Time.deltaTime);
        //
        //  }
    }


    // Simple homing entry: steer toward the current enemy position each frame
    //public bool TryDashTowards(Transform target)
    //{
    //    if (isDashing || target == null) return false;

    //    _homingTarget = target;                 // steer to live position
    //    Vector3 toTarget = target.position - transform.position;
    //    if (toTarget.sqrMagnitude < 0.0001f) return false;

    //    dashDirection = toTarget.normalized;    // initial heading
    //    // face target (optional)
    //    Vector3 face = new Vector3(dashDirection.x, dashDirection.y, dashDirection.z);
    //    if (face.sqrMagnitude > 0.0001f) transform.forward = face;

    //    isDashing = true;
    //    homingDashActive = true;
    //    // Ensure enough time to reach target at homing speed
    //    float speed = Mathf.Max(movementConfig.dashSpeed * homingSpeedMultiplier, 0.05f);
    //    float dist = toTarget.magnitude;
    //    dashTime = Mathf.Max(movementConfig.dashDuration, (dist / speed) + 0.1f); // small buffer

    //    if (movementConfig.grabHitbox != null) movementConfig.grabHitbox.SetActive(true);
    //    return true;
    //}

    //// NEW: cancel the current dash (used when auto-grabbing on hit)
    //public void CancelDash()
    //{
    //    Debug.Log("Dash cancelled.");
    //    if (!isDashing) return;
    //    isDashing = false;
    //    homingDashActive = false;
    //    dashTime = 0f;
    //    movementConfig.grabHitbox.SetActive(false);
    //}
}
