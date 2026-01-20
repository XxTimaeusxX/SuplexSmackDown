using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles the player's dash ability, including input, movement, and hitbox activation.
/// </summary>
public class PlayerDash : MonoBehaviour
{
    [Header("References")]
    public CharacterController controller; // Controls player movement and collision
    InputAction dashAction;                // Input action for dashing
    public PlayerInput PlayerInput;        // Reference to the player's input system
    public GameObject suplexhitbox;        // Hitbox used during dash (for attacks/collisions)

    [Header("Dash Settings")]
    public float dashSpeed = 5f;           // How fast the player dashes
    public float dashDuration = 0.2f;      // How long the dash lasts (in seconds)
    private Vector3 dashDirection;         // Direction the player will dash in
    public bool isDashing = false;         // True if the player is currently dashing
    private float dashTime;                // Time left in the current dash

    [SerializeField]
    private float targetDashCoolDown = 5f;
    [SerializeField]
    private float dashCoolDown;

    public int airDashCount = 0;           // Number of dashes a player can perform in air

    [Header("Homing Dash Settings")]
    public bool homingDashActive = false; //  identify dashes started by homing attack (to only auto-grab then)
    public float homingSpeedMultiplier = 10f;
    public bool enableHitboxOnNormalDash = false;
    private Transform _homingTarget;

    /// <summary>
    /// Called when the script starts. Sets up input and disables the hitbox.
    /// </summary>
    void Start()
    {
        // Find the dash action from the player's input actions
        dashAction = PlayerInput.actions.FindAction("Dash");
        // Make sure the hitbox is off at the start
        suplexhitbox.SetActive(false);
    }

    /// <summary>
    /// Called every frame. Handles dash logic.
    /// </summary>
    void Update()
    {
            Dash();

        if (dashCoolDown > 0f && !isDashing) 
        {
            dashCoolDown -= 0.1f;
        }

        if (dashCoolDown < 0f)
        {
            // Debug.Log("Dash is ready!");
            dashCoolDown = 0f;
        }
        
    }

    /// <summary>
    /// Checks for dash input and moves the player if dashing.
    /// </summary>
    void Dash()
    {
        // Start dash if the dash button was just pressed and not already dashing
        if (dashAction.WasPressedThisFrame() && !isDashing && dashCoolDown == 0f && airDashCount > 0)
        { 
            dashCoolDown = targetDashCoolDown; // Reset cooldown
            airDashCount--;                    // Iterate the amount of air dashes by 1

            dashDirection = transform.forward; // Dash in the direction the player is facing
            isDashing = true;
            homingDashActive = false; // input dash by default is not a homing dash
            dashTime = dashDuration;
            if (suplexhitbox != null)
                suplexhitbox.SetActive(true);    // Enable hitbox for the dash
            // Debug.Log("Dash initiated!");
        }
       
        // If currently dashing, move the player and count down the dash timer
         if (isDashing)
         {
            // Re-aim while homing
            if (homingDashActive && _homingTarget != null)
            {
                Vector3 toTarget = _homingTarget.position - transform.position; // include vertical
                if (toTarget.sqrMagnitude > 0.0001f)
                {
                    dashDirection = toTarget.normalized;

                    Vector3 face = new Vector3(dashDirection.x, dashDirection.y, dashDirection.z);
                    if (face.sqrMagnitude > 0.000001f)
                        transform.forward = face.normalized;
                }
            }

            float speed = homingDashActive ? dashSpeed * homingSpeedMultiplier : dashSpeed;

            // single Move per frame
            controller.Move(dashDirection * speed * Time.deltaTime);

            dashTime -= Time.deltaTime;
            if (dashTime <= 0f)
            {
                isDashing = false;
                homingDashActive = false;
                if (suplexhitbox != null) suplexhitbox.SetActive(false);
            }
        }
    }


    // Simple homing entry: steer toward the current enemy position each frame
    public bool TryDashTowards(Transform target)
    {
        if (isDashing || target == null) return false;

        _homingTarget = target;                 // steer to live position
        Vector3 toTarget = target.position - transform.position;
        if (toTarget.sqrMagnitude < 0.0001f) return false;

        dashDirection = toTarget.normalized;    // initial heading
        // face target (optional)
        Vector3 face = new Vector3(dashDirection.x, dashDirection.y, dashDirection.z);
        if (face.sqrMagnitude > 0.0001f) transform.forward = face;

        isDashing = true;
        homingDashActive = true;
        // Ensure enough time to reach target at homing speed
        float speed = Mathf.Max(dashSpeed * homingSpeedMultiplier, 0.05f);
        float dist = toTarget.magnitude;
        dashTime = Mathf.Max(dashDuration, (dist / speed) + 0.1f); // small buffer

        if (suplexhitbox != null) suplexhitbox.SetActive(true);
        return true;
    }

    // NEW: cancel the current dash (used when auto-grabbing on hit)
    public void CancelDash()
    {
        Debug.Log("Dash cancelled.");
        if (!isDashing) return;
        isDashing = false;
        homingDashActive = false;
        dashTime = 0f;
        suplexhitbox.SetActive(false);
    }
}
