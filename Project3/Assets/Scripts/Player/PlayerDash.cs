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
    private bool isDashing = false;        // True if the player is currently dashing
    private float dashTime;                // Time left in the current dash

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
    }

    /// <summary>
    /// Checks for dash input and moves the player if dashing.
    /// </summary>
    void Dash()
    {
        // Start dash if the dash button was just pressed and not already dashing
        if (dashAction.WasPressedThisFrame() && !isDashing)
        {
            dashDirection = transform.forward; // Dash in the direction the player is facing
            isDashing = true;
            dashTime = dashDuration;
            suplexhitbox.SetActive(true);      // Enable hitbox for the dash
            Debug.Log("Dash initiated!");
        }

        // If currently dashing, move the player and count down the dash timer
        if (isDashing)
        {
            if (dashTime > 0)
            {
                // Move the player in the dash direction at dashSpeed
                controller.Move(dashDirection * dashSpeed * Time.deltaTime);
                dashTime -= Time.deltaTime;
            }
            else
            {
                // Dash finished: reset state and disable hitbox
                isDashing = false;
                suplexhitbox.SetActive(false);
                Debug.Log("Dash ended!");
            }
        }
    }
}
