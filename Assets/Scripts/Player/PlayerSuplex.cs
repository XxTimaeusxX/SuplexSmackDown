using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
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
    public GameObject shockwave;
    public Transform player;

    [Header("Suplex Configurations")]
    public List<SuplexConfig> suplexConfigs; // List of all possible suplex types
    public AnimationCurve GravityControl; // line graph to control gravity during suplex
    public AnimationCurve CameraOffsetCurve; // line graph to control camera offset during suplex

    // homing setting
    [Header("Homing Attack")]
    [SerializeField] private float homingSearchRadius = 12f;
    private bool canHomeChain = false;
    private Transform lastReleasedEnemy = null;
    private InputAction homingAction;

    [Header("Enemy heavyness")]
    [SerializeField] private string bigEnemyLayerName = "BigEnemy";
    [SerializeField] private float bigEnemyGravityScale = 2.0f; // -40 -> -80
    [SerializeField] private float bigEnemyMoveSpeedScale = 1;
    private float _savedMoveSpeed;
    private float _defaultGravity;
    private float currentGravityScale = 1f;
    private float currentMoveSpeedScale = 1f;

    // Internal references to other player scripts/components
    private PowerGauge powerGauge;
    private PlayerMovement playerMovement;
    private PlayerDash playerDash;
    public Transform grabbedEnemy;          // The enemy currently grabbed
    private CharacterController controller;  // For ground checks

    // Input actions for different suplexes and jumping
    private InputAction SuperSuplexAction;
    private InputAction RainbowSuplexAction;
    private InputAction LongJumpSuplexAction;
    private InputAction jumpAction;
 
    public bool isSuplexing = false;         // True if a suplex is in progress
    private SuplexAbilities currentSuplex = SuplexAbilities.None; // Which suplex is being performed

    [Header("Trajectory Materials")]
    public Material longSuplexMaterial;     // Yellow version
    public Material rainbowSuplexMaterial;  // Blue/default
    public Material superSuplexMaterial;    // Red version

    [Header("Trajectory Target")]
    // Prefab asset you assign in inspector
    public GameObject RainbowArcLandPrefab;
    public GameObject LongArcLandPrefab;
    public GameObject SuperLandPrefab;
    // Slight lift so it doesn't z-fight or clip under ground
    [SerializeField] private Vector3 targetLandOffset = new Vector3(0f, 0.02f, 0f);

    // Runtime-only references (do not assign in inspector)
    private GameObject _targetLandInstance;
    private SpriteRenderer _targetLandSpriteRenderer; // cached from the instance

    [Header("Landing Icon Colors")]
    public Color longIconColor = new Color(1f, 0.92f, 0.2f, 1f);   // Yellow
    public Color rainbowIconColor = new Color(0.25f, 0.5f, 1f, 1f); // Blue
    public Color superIconColor = new Color(1f, 0.15f, 0.15f, 1f);  // Red
                                                                    // Tracks which ability the current marker instance represents
    private SuplexAbilities _landingMarkerAbility = SuplexAbilities.None;

    /// <summary>
    /// Called when the script instance is being loaded.
    /// Sets up references to other components and input actions.
    /// </summary>
    private void Awake()
    {
       
        if (playerDash == null)
            playerDash = GetComponentInParent<PlayerDash>();
        playerMovement = GetComponentInParent<PlayerMovement>();
        controller = GetComponentInParent<CharacterController>();
        powerGauge = GetComponentInParent<PowerGauge>();
        if (playerMovement != null) { _savedMoveSpeed = playerMovement.moveSpeed;
                                      _defaultGravity = playerMovement.gravity;}// cache the default move speed and gravity
        jumpAction = playerInput.actions.FindAction("Jump");
        SuperSuplexAction = playerInput.actions.FindAction("SuperSuplex");
        RainbowSuplexAction = playerInput.actions.FindAction("RainbowSuplex");
        LongJumpSuplexAction = playerInput.actions.FindAction("LongjumpSuplex");
        homingAction = playerInput.actions.FindAction("Homing");
    }

    private void Update()
    {
        // Disarm when grounded
        if (IsGrounded())
        {
            canHomeChain = false;
            lastReleasedEnemy = null;
            return;
        }

        // Only Homing action triggers lock-on; no reuse of Dash here
        if (canHomeChain && !isSuplexing && playerDash != null && !playerDash.isDashing)
        {
            bool homingPressed = (homingAction != null && homingAction.WasPressedThisFrame());

            if (homingPressed)
            {
                Transform target = FindNearestEnemy(transform.position, homingSearchRadius, lastReleasedEnemy);
                if (target != null)
                {
                    if (playerDash.TryDashTowards(target)) // steer to live enemy position (vertical included)
                    {
                        canHomeChain = false; // consume the window
                    }
                }
            }
        }
    }
    /// <summary>
    /// Starts the suplex process by grabbing the enemy.
    /// </summary>
    public void StartSuplex(Collider enemy)
    {
        if (isSuplexing) return; // Prevent double suplexing
                                 
        if (playerDash != null && playerDash.isDashing) // Ensure dash movement stops before we grab and start the next suplex
        playerDash.CancelDash();
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

        var root = enemy.GetComponentInParent<Enemy>()?.transform ?? enemy.transform;
        int bigLayer = LayerMask.NameToLayer(bigEnemyLayerName);
        currentGravityScale = (root.gameObject.layer == bigLayer) ? bigEnemyGravityScale : 1f;
        currentMoveSpeedScale = (root.gameObject.layer == bigLayer) ? bigEnemyMoveSpeedScale : 1f;

        if (root.gameObject.layer == bigLayer)
        {
            _savedMoveSpeed = playerMovement.moveSpeed;
            playerMovement.moveSpeed = playerMovement.moveSpeed * currentMoveSpeedScale;// slow down player move speed for big enemy suplex
           
        }
        var agent = enemy.GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent != null) agent.enabled = false; // disable navmesh agent while held

        var rb = enemy.GetComponent<Rigidbody>();
        if (rb != null)rb.isKinematic = true;// Prevent physics while held

        var enemyScript = enemy.GetComponent<Enemy>();
        if (enemyScript != null) enemyScript.SetGrabbed(true); // Disable ground detection

        // starting a new suplex breaks any homing window
        canHomeChain = false;
        lastReleasedEnemy = null;

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
                   rb.AddForce(Vector3.down * 5f, ForceMode.VelocityChange);            
                } 
            }
           
                grabbedEnemy = null;
                playerMovement.moveSpeed = _savedMoveSpeed;
                playerMovement.gravity = _defaultGravity;
        }
    }

    /// <summary>
    /// Waits for the player to press a suplex input, then starts the chosen suplex.
    /// </summary>
    public IEnumerator WaitForSuplexInput()
    {
        currentSuplex = SuplexAbilities.None;

        var suplexInputs = new (SuplexAbilities ability, InputAction action)[] 
        {
            (SuplexAbilities.Super, SuperSuplexAction),
            (SuplexAbilities.Rainbow, RainbowSuplexAction),
            (SuplexAbilities.Long, LongJumpSuplexAction)
        };

        // Track which ability is currently being previewed/held
        SuplexAbilities previewing = SuplexAbilities.None;

        while (currentSuplex == SuplexAbilities.None)
        {
            bool anyHeldThisFrame = false;

            foreach (var (ability, action) in suplexInputs)
            {
                if (action == null) continue;

                // While held: show that ability's trajectory and mark it as previewing
                if (action.IsPressed())
                {
                    SetTrajectoryMaterial(ability);// change trajectory color based on ability
                    var cfg = suplexConfigs.Find(cfg => cfg.ability == ability);
                    if (cfg != null)
                        ShowTrajectory(cfg); // Show trajectory preview

                    previewing = ability;
                    anyHeldThisFrame = true;
                }

                // On release of the currently previewed ability: commit selection
                if (previewing == ability && action.WasReleasedThisFrame())
                {
                    if (ability == SuplexAbilities.Super && !CanPerformSuperSuplex())
                    {
                        Debug.Log("Not enough power for Super Suplex!");
                        continue; // Ignore the attempt
                    }
                    currentSuplex = ability;
                    break;
                }
            }

            // If nothing is held this frame, clear preview visuals
            if (!anyHeldThisFrame)
            {
                previewing = SuplexAbilities.None;
                if (trajectoryRenderer != null)
                    trajectoryRenderer.positionCount = 0;
                SetTargetLandActive(false);
            }

            yield return null;
        }

        // Clear preview and perform the chosen suplex
        if (trajectoryRenderer != null)
            trajectoryRenderer.positionCount = 0;
        SetTargetLandActive(false);
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
            powerGauge.AddMeter(10f);

            if (type == SuplexAbilities.Super && powerGauge != null)
                powerGauge.SpendMeter();
            AudioManager.PlaySuplexStart();
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
        // Apply per-target scaling first
        float originalGravity = playerMovement.gravity;
        playerMovement.gravity = originalGravity * currentGravityScale;     
        float originalMoveSpeed = playerMovement.moveSpeed;
        // Use the scaled gravity for the throw math
        float gravity = Mathf.Abs(playerMovement.gravity);
        float height = config.LiftHeight;
        float distance = config.FowardDistance;

        float minTimeToPeak = Mathf.Sqrt(2f * height / gravity);
        float minTotalTime = minTimeToPeak * 2f;
        float totalTime = Mathf.Max(config.LaunchSpeed, minTotalTime);
        float timeToPeak = totalTime / 2f;

        float vy = (2f * height) / timeToPeak;
        float vx = distance / totalTime;

        Vector3 launchVelocity = transform.forward * vx + Vector3.up * vy;
        playerMovement.velocity = launchVelocity;

        float t = 0f;
        bool jumpedOff = false;
        float minAirTime = 0.2f;

        // Super-suplex descent control based on scaled gravity
        float minGravity = playerMovement.gravity * 0.2f;
        float maxGravity = playerMovement.gravity;
        float gravityIncreaseDuration = 2f;
        float gravityLerpTime = 1f;

        // Camera settings
        Vector3 targetOffset = new Vector3(0f, 13f, 0f);
        float cameraLerpDuration = 0.5f;
        float cameraLerpTime = 0f;

        var orbital = playerMovement.CinemachineCamera.GetCinemachineComponent(CinemachineCore.Stage.Body) as CinemachineOrbitalFollow;
        Vector3 DefaultCameraOffset = orbital.TargetOffset;
        bool cameratilted = false;

        // Wait for the player to land or jump off
        while (true)
        {
            t += Time.deltaTime;
            // Cancel suplex if we hit the ceiling or any object above, this frame
            if (controller != null && (controller.collisionFlags & CollisionFlags.Above) != 0)
            {
                // Stop upward motion and gently start descending
                playerMovement.velocity.x = 0f;
                playerMovement.velocity.z = 0f;
                playerMovement.velocity.y = -1f;
                break;
            }
            // if player is falling down with the super suplex performed
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
              //  Debug.Log("not super suplex no moon gravity effect");
            }

            // Allow player to jump off during the arc
            if (!jumpedOff && jumpAction != null && jumpAction.WasPressedThisFrame())
            {
                Transform justReleased = grabbedEnemy;
                playerMovement.ForceJump();
                jumpedOff = true;
                ReleaseEnemy(true, config); // apply downward force and enable enemy ground detection 
                
                // Arm homing window until grounded or next suplex
                canHomeChain = true;
                lastReleasedEnemy = justReleased;

               
                // if dash didn't start, zero horizontal so we don't slide forever
                playerMovement.velocity.x = 0f;
                playerMovement.velocity.z = 0f;   
                break;
            }
            
            // End the suplex when the player lands (after a minimum airtime)
            if (t > minAirTime && IsGrounded())
            {
                if(shockwave != null)// checks if there is a shockwave prefab assigned ,optional check if player !=null
                    Instantiate(shockwave, player.position, player.rotation, player);
                AudioManager.PlaySuplexSlam();
                break;
            }
                

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
        orbital.TargetOffset = DefaultCameraOffset;
    }

    /// <summary>
    /// Checks if the player is currently on the ground.
    /// </summary>
    bool IsGrounded()
    {
        return controller != null && controller.isGrounded;
    }
    ///------------------------------- Homing Attack ---------------------------------///
    //find nearest enemy within radius, ignoring a specific enemy (e.g., the just released one)
    private Transform FindNearestEnemy(Vector3 origin, float radius, Transform ignore = null)
    {
        Collider[] hits = Physics.OverlapSphere(origin, radius, ~0, QueryTriggerInteraction.Collide);

        Transform best = null;
        float bestSqr = float.MaxValue;

        for (int i = 0; i < hits.Length; i++)
        {
            var col = hits[i];

            if (ignore != null && (col.transform == ignore || col.transform.IsChildOf(ignore)))
                continue;

            var enemy = col.GetComponentInParent<Enemy>();
            if (enemy == null || !enemy.gameObject.activeInHierarchy)
                continue;

            float sqr = (enemy.transform.position - origin).sqrMagnitude;
            if (sqr < bestSqr)
            {
                bestSqr = sqr;
                best = enemy.transform;
            }
        }

        return best;
    }

    ///------------------------------- Trajectory Visualization ---------------------------------///
    /// <summary>
    /// Displays the predicted trajectory of the suplex using a LineRenderer.
    /// just copy and pasted logic from suplex routine into here for visualization
    /// </summary>
    /// <param name="config"></param>
    void ShowTrajectory(SuplexConfig config)
    {
         if (trajectoryRenderer == null || heldEnemy == null || playerMovement == null)
             return;

         int steps = 60;
         Vector3[] points = new Vector3[steps + 1];
         Vector3 startPos = heldEnemy.position;
         float gravity = Mathf.Abs(playerMovement.gravity)* currentGravityScale; // Use the same gravity as the player

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
        trajectoryRenderer.alignment = LineAlignment.View;
        trajectoryRenderer.enabled = true;
        
        // Disable enemy colliders to prevent interference with trajectory calculation
        Collider[] enemyColliders = heldEnemy.GetComponentsInChildren<Collider>();
         foreach (var col in enemyColliders)
             col.enabled = false;
        // track landing (position + normal) for the marker
        Vector3 landingNormal = Vector3.up;
        points[0] = pos;
         for (int i = 1; i <= steps; i++)
         {
             // Apply gravity using the same value as the player
             velocity += Vector3.down * gravity * dt;
             Vector3 nextPos = pos + velocity * dt;

            
            // Ignore triggers to reduce false hits
            if (Physics.Linecast(pos, nextPos, out RaycastHit hit, ~0, QueryTriggerInteraction.Ignore))
             {
                 points[i] = hit.point;
                 landingNormal = hit.normal;
                 lastPoint = i;
                 break;
             }

             if (nextPos.y <= startPos.y)
             {
                 nextPos.y = startPos.y;
                 points[i] = nextPos;

                // try to get a ground normal under the clamped point
                 if (Physics.Raycast(nextPos + Vector3.up * 0.25f, Vector3.down, out RaycastHit groundHit, 5f, ~0, QueryTriggerInteraction.Ignore))
                    landingNormal = groundHit.normal;
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
          for (int i = 0; i <= lastPoint; i++)
              trajectoryRenderer.SetPosition(i, points[i]);
        // place/update landing marker
        Vector3 landingPos = points[lastPoint];
        UpdateTargetLand(landingPos, landingNormal);
    }
    private void SetTrajectoryMaterial(SuplexAbilities ability)
    {
        // Pick trajectory material, icon color, and prefab in one place
        Material rendererMaterial = null;
        Color iconColor = Color.white;
        GameObject prefab = null;

        switch (ability)
        {
            case SuplexAbilities.Long:
                rendererMaterial = longSuplexMaterial;
                iconColor = longIconColor;
                prefab = LongArcLandPrefab;
                break;
            case SuplexAbilities.Rainbow:
                rendererMaterial = rainbowSuplexMaterial;
                iconColor = rainbowIconColor;
                prefab = RainbowArcLandPrefab;
                break;
            case SuplexAbilities.Super:
                rendererMaterial = superSuplexMaterial;
                iconColor = superIconColor;
                prefab = SuperLandPrefab;
                break;
            default:
                rendererMaterial = null;
                prefab = null;
                iconColor = Color.white;
                break;
        }

        // Apply trajectory material if available; otherwise leave existing material
        if (trajectoryRenderer != null && rendererMaterial != null)
        {
            trajectoryRenderer.material = rendererMaterial;
        }

        // If no prefab assigned for this ability, destroy any existing marker and exit
        if (prefab == null)
        {
            if (_targetLandInstance != null)
            {
                Destroy(_targetLandInstance);
                _targetLandInstance = null;
                _targetLandSpriteRenderer = null;
                _landingMarkerAbility = SuplexAbilities.None;
            }
            return;
        }

        // Ensure/Swap landing marker instance for this ability
        if (_targetLandInstance == null || _landingMarkerAbility != ability)
        {
            if (_targetLandInstance != null) Destroy(_targetLandInstance);
            _targetLandInstance = Instantiate(prefab);
            if (_targetLandInstance != null)
            {
                _targetLandInstance.SetActive(false);
                _targetLandSpriteRenderer = _targetLandInstance.GetComponentInChildren<SpriteRenderer>();
                _landingMarkerAbility = ability;
            }
            else
            {
                _targetLandSpriteRenderer = null;
                _landingMarkerAbility = SuplexAbilities.None;
            }
        }

        // Tint icon if available
        if (_targetLandSpriteRenderer != null)
            _targetLandSpriteRenderer.color = iconColor;
    }


    private void UpdateTargetLand(Vector3 position, Vector3 normal)
    {
        if (_targetLandInstance == null) return;
       
        var t = _targetLandInstance.transform;
        t.position = position + targetLandOffset;
        t.rotation = Quaternion.FromToRotation(Vector3.forward, normal);
        if (!_targetLandInstance.activeSelf) _targetLandInstance.SetActive(true);
    }

    private void SetTargetLandActive(bool active)
    { 
        if (_targetLandInstance == null) return;
        if (_targetLandInstance.activeSelf != active)
            _targetLandInstance.SetActive(active);
    }
    private bool CanPerformSuperSuplex()
    {
        return powerGauge != null && powerGauge.currentMeter >= powerGauge.maxMeter;
    }

}


