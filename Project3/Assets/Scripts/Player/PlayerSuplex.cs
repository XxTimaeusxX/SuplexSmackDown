using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine.UI;

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
    public LineRenderer trajectoryRenderer; // Visualize the suplex arc

    [Header("Suplex Configurations")]
    public List<SuplexConfig> suplexConfigs; // List of all possible suplex types
    public AnimationCurve GravityControl; // line graph to control gravity during suplex
    public AnimationCurve CameraOffsetCurve; // line graph to control camera offset during suplex

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
            }

            if (rb != null)
            {
                grabbedEnemy.SetParent(null);
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

    // Map each suplex type to its input action
    var suplexInputs = new (SuplexAbilities ability, InputAction action)[]
    {
        (SuplexAbilities.Super, SuperSuplexAction),
        (SuplexAbilities.Rainbow, RainbowSuplexAction),
        (SuplexAbilities.Long, LongJumpSuplexAction)
    };

    SuplexAbilities previewing = SuplexAbilities.None;
    // Get reference to camera orbital follow and store default offset
    var orbital = playerMovement.CinemachineCamera.GetCinemachineComponent(CinemachineCore.Stage.Body) as CinemachineOrbitalFollow;
    Vector3 DefaultCameraOffset = orbital.TargetOffset;
    bool offsetApplied = false;

    while (currentSuplex == SuplexAbilities.None)
    {
        bool anyHeld = false;
        foreach (var (ability, action) in suplexInputs)
    {
        if (action != null)
        {
            if (action.IsPressed())
            {
                ShowTrajectory(suplexConfigs.Find(cfg => cfg.ability == ability));
                previewing = ability;
                anyHeld = true;
            }
            // Check for release regardless of IsPressed
            if (previewing == ability && action.WasReleasedThisFrame())
            {
                currentSuplex = ability;
                break;
            }
        }
    }
        if (anyHeld)
        {
            if (!offsetApplied)
            {  
                offsetApplied = true;
            }
        }
        if (!anyHeld)
        {
            trajectoryRenderer.positionCount = 0;
            previewing = SuplexAbilities.None;
                if (offsetApplied)
                {
                    orbital.TargetOffset = DefaultCameraOffset;
                    offsetApplied = false;
                }
            }

        yield return null;
    }

        trajectoryRenderer.positionCount = 0;
        orbital.TargetOffset = DefaultCameraOffset; // reset camera offset after suplex selection
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

        /*  // Get the absolute value of gravity (usually 9.81)
          float gravity = Mathf.Abs(Physics.gravity.y);

          // How high the player/enemy will be lifted
          float height = config.LiftHeight;

          // How far forward the suplex will travel
          float distance = config.FowardDistance;

          // Calculate the vertical velocity needed to reach the desired height
          float vy = Mathf.Sqrt(2 * gravity * height);

          // Calculate the time to travel the forward distance at the given speed
          float totalTime = distance / Mathf.Max(config.LaunchSpeed, 0.2f);

          // Calculate the horizontal velocity needed to cover the distance in the given time
          float vx = distance / totalTime;

          // Combine forward and upward velocities for the launch
          Vector3 Launchvelocity = transform.forward * vx + Vector3.up * vy;

          // Set the player's velocity to launch them
          playerMovement.velocity = Launchvelocity;*/

        float gravity = Mathf.Abs(playerMovement.gravity);
        float height = config.LiftHeight;
        float distance = config.FowardDistance;

        // Minimum time to reach the desired height
        float minTimeToPeak = Mathf.Sqrt(2f * height / gravity);
        float minTotalTime = minTimeToPeak * 2f;

        // Use the greater of LaunchSpeed or minTotalTime
        float totalTime = Mathf.Max(config.LaunchSpeed, minTotalTime);
        float timeToPeak = totalTime / 2f;

        // Calculate vertical velocity to reach desired height in timeToPeak
        float vy = (2f * height) / timeToPeak;

        // Calculate horizontal velocity to cover the distance in totalTime
        float vx = distance / totalTime;

        // Combine velocities
        Vector3 launchVelocity = transform.forward * vx + Vector3.up * vy;
        playerMovement.velocity = launchVelocity;

        // gravity settings for suplex arc control during descent
        float t = 0;
        bool jumpedOff = false;
        float minAirTime = 0.2f; // Prevents instant landing
        float originalGravity = playerMovement.gravity;
        float minGravity = originalGravity * 0.2f; // Start with low gravity
        float maxGravity = originalGravity;         // End with normal gravity
        float gravityIncreaseDuration = 3f;       // Time to reach max gravity (tweak as needed)
        float gravityLerpTime = 0f;

        // camera settings
        Vector3 targetOffset = new Vector3(0f, 13f, 0f); // target offset for cinematic effect during suplex
        float cameraLerpDuration = 0.5f;
        float cameraLerpTime = 0f;

        // reference to the cinemachine orbital follow component to adjust camera during suplex
        var orbital = playerMovement.CinemachineCamera.GetCinemachineComponent(CinemachineCore.Stage.Body) as CinemachineOrbitalFollow; 
        Vector3 DefaultCameraOffset =  orbital.TargetOffset;
        bool cameratilted = false;

        // Wait for the player to land or jump off
        while (true)
        {
            t += Time.deltaTime;
            // if player is falling down
            if (playerMovement.velocity.y < 0)
            {
                if(currentSuplex == SuplexAbilities.Super)
                {
                    // add moon gravity effect during descent
                    gravityLerpTime += Time.deltaTime;
                    float lerpFactor = Mathf.Clamp01(gravityLerpTime / gravityIncreaseDuration);
                    playerMovement.gravity = Mathf.Lerp(minGravity, maxGravity, GravityControl.Evaluate(lerpFactor));
                    if (!cameratilted)
                    {
                        // tilt camera downwards during descent and have more control on flowing down 
                        cameratilted = true;
                        playerMovement.velocity.x = 0f;
                        playerMovement.velocity.z = 0f;
                    }
                    cameraLerpTime += Time.deltaTime;
                    float cameraLerpFactor = Mathf.Clamp01(cameraLerpTime / cameraLerpDuration);
                    orbital.TargetOffset = Vector3.Lerp(DefaultCameraOffset, targetOffset,CameraOffsetCurve.Evaluate(cameraLerpFactor));
                }     
            }
            else
            {
                Debug.Log("not super suplex no moon gravity effect");
            }

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
            // Stop all horizontal movement and snap to ground or else player bounces like a ball and slide forever
            playerMovement.velocity.x = 0f;
            playerMovement.velocity.z = 0f;
            playerMovement.velocity.y = -2f;
        }

        isSuplexing = false;
        //reset gravity to normal and reset camera offset position
        playerMovement.gravity = originalGravity;
        orbital.TargetOffset = DefaultCameraOffset;
    }

    /// <summary>
    /// Checks if the player is currently on the ground.
    /// </summary>
    bool IsGrounded()
    {
        return controller != null && controller.isGrounded;
    }

    /// <summary>
    /// Displays the predicted trajectory of the suplex using a LineRenderer.
    /// just copy and pasted logic from suplex routine into here for visualization
    /// </summary>
    /// <param name="config"></param>
    void ShowTrajectory(SuplexConfig config)
    {
        int steps = 60;
        Vector3[] points = new Vector3[steps + 1];
        Vector3 startPos = heldEnemy.position;
        float gravity = Mathf.Abs(playerMovement.gravity); // Use the same gravity as the player

        float height = config.LiftHeight;
        float distance = config.FowardDistance;
        float minTimeToPeak = Mathf.Sqrt(2f * height / gravity);
        float minTotalTime = minTimeToPeak * 2f;
        float totalTime = Mathf.Max(config.LaunchSpeed, minTotalTime);
        float timeToPeak = totalTime / 2f;
        float vy = (2f * height) / timeToPeak;
        float vx = distance / totalTime;

        Vector3 forward = heldEnemy.forward.normalized;
        Vector3 launchVelocity = forward * vx + Vector3.up * vy;

        Vector3 pos = startPos;
        Vector3 velocity = launchVelocity;
        float dt = totalTime / steps;
        int lastPoint = 0;

        Collider[] enemyColliders = heldEnemy.GetComponentsInChildren<Collider>();
        foreach (var col in enemyColliders)
            col.enabled = false;

        points[0] = pos;
        for (int i = 1; i <= steps; i++)
        {
            // Apply gravity using the same value as the player
            velocity += Vector3.down * gravity * dt;
            Vector3 nextPos = pos + velocity * dt;

            if (Physics.Linecast(pos, nextPos, out RaycastHit hit))
            {
                points[i] = hit.point;
                lastPoint = i;
                break;
            }

            if (nextPos.y <= startPos.y)
            {
                nextPos.y = startPos.y;
                points[i] = nextPos;
                lastPoint = i;
                break;
            }

            points[i] = nextPos;
            pos = nextPos;
            lastPoint = i;
        }

        foreach (var col in enemyColliders)
        col.enabled = true;
        trajectoryRenderer.positionCount = lastPoint+ 1;
        trajectoryRenderer.SetPositions(points);
    }
}
