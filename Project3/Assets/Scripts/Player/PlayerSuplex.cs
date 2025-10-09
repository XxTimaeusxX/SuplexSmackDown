using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using System.Collections.Generic;

/// <summary>
/// Stores configuration for each type of suplex (height, distance, speed, etc). 
/// </summary>
[System.Serializable]
public class SuplexConfig
{
    public SuplexAbilities ability;   // The type of suplex this config is for
    public float LiftHeight;          // How high the player/enemy is lifted
    public float FowardDistance;      // How far forward the suplex launches
    public float LaunchSpeed;         // How fast the launch happens
}
/// <summary>
/// Enum for different suplex types.
/// </summary>
public enum SuplexAbilities
{
    None,
    Long,
    Rainbow,
    Super
}
/// <summary>
/// Handles all logic for grabbing, holding, and suplexing enemies.
/// </summary>
public class PlayerSuplex : MonoBehaviour
{
    [Header("References")]
    public Transform heldEnemy;      // Where the grabbed enemy is held
    public PlayerInput playerInput;  // Reference to the player's input system

    [Header("Suplex Configurations")]
    public List<SuplexConfig> suplexConfigs; // List of all possible suplex types

    // Internal references to other player scripts/components
    private PlayerMovement playerMovement;
    private PlayerDash playerDash;
    private Transform grabbedEnemy;          // The enemy currently grabbed
    private CharacterController controller;  // For ground checks

    // Input actions for different suplexes and jumping
    private InputAction SuperSuplexAction;
    private InputAction RainbowSuplexAction;
    private InputAction LongJumpSuplexAction;
    private InputAction jumpAction;
 
    public bool isSuplexing = false;         // True if a suplex is in progress

    private SuplexAbilities currentSuplex = SuplexAbilities.None; // Which suplex is being performed

    /// <summary>
    /// Called when the script instance is being loaded.
    /// Sets up references to other components and input actions.
    /// </summary>
    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (playerDash == null)
            playerDash = GetComponentInParent<PlayerDash>();
        playerMovement = GetComponentInParent<PlayerMovement>();
        jumpAction = playerInput.actions.FindAction("Jump");
        SuperSuplexAction = playerInput.actions.FindAction("SuperSuplex");
        RainbowSuplexAction = playerInput.actions.FindAction("RainbowSuplex");
        LongJumpSuplexAction = playerInput.actions.FindAction("LongjumpSuplex");
    }

    /// <summary>
    /// Starts the suplex process by grabbing the enemy.
    /// </summary>
    public void StartSuplex(Collider enemy)
    {
        if (isSuplexing) return; // Prevent double suplexing
        isSuplexing = true;
        HoldEnemy(enemy);
    }

    /// <summary>
    /// Attaches the enemy to the player and disables its physics.
    /// </summary>
    void HoldEnemy(Collider enemy)
    {
        grabbedEnemy = enemy.transform;
        grabbedEnemy.SetParent(heldEnemy);
        grabbedEnemy.localPosition = Vector3.zero;

        // Optionally disable enemy AI here
        var agent = enemy.GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent != null)
        {
            agent.enabled = false; // Disable AI while held
            Debug.Log("NavMeshAgent disabled.");
        }
        var rb = enemy.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true; // Prevent physics while held
        }
        var enemyScript = enemy.GetComponent<Enemy>();
        if (enemyScript != null)
        {
            enemyScript.SetGrabbed(true); // Disable ground detection
            Debug.Log("set grab = true.");
        }
        // Wait for player to choose which suplex to perform
        StartCoroutine(WaitForSuplexInput());
    }

    /// <summary>
    /// Releases the enemy, optionally slamming them down.
    /// </summary>
    void ReleaseEnemy(bool slam, SuplexConfig config = null)
    {
        if (grabbedEnemy != null)
        {
            var rb = grabbedEnemy.GetComponent<Rigidbody>();
            var enemyScript = grabbedEnemy.GetComponent<Enemy>();
            if (enemyScript != null)
            { // Enable ground detection
                enemyScript.SetGrabbed(false);
                Debug.Log("set grab = false.");
            }

            if (rb != null)
            {
                grabbedEnemy.SetParent(null);
                Debug.Log("Kinematic set to false.");
                rb.isKinematic = false; // Re-enable physics
                if (slam && config != null)
                {
                    // Launch the enemy downward if slamming
                    rb.linearVelocity = Vector3.down * (config.LaunchSpeed * 0.5f);
                } 
            }
                grabbedEnemy = null;
        }
    }

    /// <summary>
    /// Waits for the player to press a suplex input, then starts the chosen suplex.
    /// </summary>
    IEnumerator WaitForSuplexInput()
    {
        currentSuplex = SuplexAbilities.None;
        while (currentSuplex == SuplexAbilities.None)
        {
            // Check which suplex button was pressed
            if (SuperSuplexAction != null && SuperSuplexAction.WasPressedThisFrame())
                currentSuplex = SuplexAbilities.Super;
            else if (RainbowSuplexAction != null && RainbowSuplexAction.WasPressedThisFrame())
                currentSuplex = SuplexAbilities.Rainbow;
            else if (LongJumpSuplexAction != null && LongJumpSuplexAction.WasPressedThisFrame())
                currentSuplex = SuplexAbilities.Long;
            yield return null;
        }
        PerformSuplex(currentSuplex);
    }

    /// <summary>
    /// Starts the coroutine for the selected suplex type.
    /// </summary>
    void PerformSuplex(SuplexAbilities type)
    {
        var config = suplexConfigs.Find(cfg => cfg.ability == type);
        if (config != null)
        {
            StartCoroutine(SuplexRoutine(config));
        }
        else
        {
            isSuplexing = false;
        }
    }

    /// <summary>
    /// Handles the actual launch and arc of the suplex, including jump-off and landing logic.
    /// </summary>
    IEnumerator SuplexRoutine(SuplexConfig config)
    {
        // --- Calculate launch velocity for the suplex arc ---

        // Get the absolute value of gravity (usually 9.81)
        float gravity = Mathf.Abs(Physics.gravity.y);

        // How high the player/enemy will be lifted
        float height = config.LiftHeight;

        // How far forward the suplex will travel
        float distance = config.FowardDistance;

        // Calculate the vertical velocity needed to reach the desired height
        float vy = Mathf.Sqrt(2 * gravity * height);

        // Calculate the time to travel the forward distance at the given speed
        float totalTime = distance / Mathf.Max(config.LaunchSpeed, 0.01f);

        // Calculate the horizontal velocity needed to cover the distance in the given time
        float vx = distance / totalTime;

        // Combine forward and upward velocities for the launch
        Vector3 Launchvelocity = transform.forward * vx + Vector3.up * vy;

        // Set the player's velocity to launch them
        playerMovement.velocity = Launchvelocity;

        float t = 0;
        bool jumpedOff = false;
        float minAirTime = 0.2f; // Prevents instant landing

        // Wait for the player to land or jump off
        while (true)
        {
            t += Time.deltaTime;

            // Allow player to jump off during the arc
            if (!jumpedOff && jumpAction != null && jumpAction.WasPressedThisFrame())
            {
                playerMovement.ForceJump();
                jumpedOff = true;
                ReleaseEnemy(true, config); // apply downward force and enable enemy ground detection
                Debug.Log("Player jumped off enemy!");
                // Stop all horizontal movement and snap to ground or else player bounces like a ball and slide forever
                playerMovement.velocity.x = 0f;
                playerMovement.velocity.z = 0f;
               
                break;
            }

            // End the suplex when the player lands (after a minimum airtime)
            if (t > minAirTime && IsGrounded())
                break;

            yield return null;
        }

        // If the player didn't jump off, finish the suplex and stop movement
        if (!jumpedOff)
        {
            ReleaseEnemy(false, config); // dont apply downward force but still release enemy and enable enemy ground detection
            Debug.Log("Suplex landed!");

            // Stop all horizontal movement and snap to ground or else player bounces like a ball and slide forever
            playerMovement.velocity.x = 0f;
            playerMovement.velocity.z = 0f;
            playerMovement.velocity.y = -2f;
        }

        isSuplexing = false;
    }

    /// <summary>
    /// Checks if the player is currently on the ground.
    /// </summary>
    bool IsGrounded()
    {
        return controller != null && controller.isGrounded;
    }
}
