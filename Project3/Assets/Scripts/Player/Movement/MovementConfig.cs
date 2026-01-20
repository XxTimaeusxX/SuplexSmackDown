using UnityEngine;

/// <summary>
/// This script only handles the settings related to movement
/// </summary>

// ~Istvan W

// Last Edited: 1/19/2026 by Istvan W.

public class MovementConfig : MonoBehaviour
{
    [Header("Basic Movement Values")]
    public float walkSpeed;
    public float sprintSpeed;

    public float maxSlopeAngle;

    [Header("Rotation Values")]
    public float rotationSmoothTime;                    // How long it takes to rotate towards movement direction

    [Header("Jump/Air Movement Values")]
    public float jumpHeight;
    public float customGravity = -9.81f;                // Set to Unity's default gravity and change Unity's gravity to -50f
    public float airControl;                            // How much control the player has over movement in air (0 = none, 1 = full)
    public float terminalVelocity;                      // Max downward speed

    [Header("Dash Settings")]
    public GameObject grabHitbox;                       // Hitbox used during dash (for attacks/collisions)

    public float dashSpeed;                             // How fast the player dashes
    public float dashDuration;                          // How long the dash lasts (in seconds)
    public float targetDashCooldown;                    // Cooldown time between dashes
    public int maxAirDashes;                            // Number of dashes a player can perform in air

    /// NOTE: Suplex arcs only define the height of the suplex over time and how they slightly come down after the peak.
    /// After that, the player is affected by a downward force to bring them down faster
    [Header("Suplex Arcs")]             
    public AnimationCurve rainbowSuplexHeight;
    public AnimationCurve longSuplexHeight;
    public AnimationCurve superSuplexHeight;

    [Header("Throwing/Carrying")]
    public float aimingWalkSpeed;                       // Speed while aiming to throw

    /// NOTE: Carry Weight Profiles determine how much the player is slowed down based on what they are carrying
    public CarryWeightProfile defaultCarryProfile;      // Default carry weight profile when not carrying anything


}
