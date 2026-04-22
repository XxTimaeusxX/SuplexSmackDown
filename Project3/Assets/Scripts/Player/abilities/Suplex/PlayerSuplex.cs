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
    public SuplexAbilities ability;
    public float LiftHeight;
    public float FowardDistance;
    public float LaunchSpeed;
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
    public PlayerInput playerInput;
    public GameObject shockwave;
    public GameObject rageShockwave;
    public Transform player;
    private CinemachineImpulseSource impulseSource;
    public Slider rageBar;
    public PowerGauge powerGuage;
    public bool droneDropped;
    public bool bossDropped;
    private float droneCooldown;
    private float bossCooldown;
    [SerializeField] Image suplexBar;
    [SerializeField] Sprite suplexImg1;

    [Header("Component References")]
    public SuplexTrajectoryVisualizer trajectoryVisualizer;
    public EnemyGrabHandler grabHandler;
    public HomingAttack homingAttack;
    [SerializeField] PlayerVoiceManager voiceManager;
    [Header("Suplex Configurations")]
    public List<SuplexConfig> suplexConfigs;
    public AnimationCurve GravityControl;
    public AnimationCurve CameraOffsetCurve;

    [Header("Visual References")]
    public Transform playerMesh; // Reference to "el cohette idle" child

    // Internal references to other player scripts/components
    private PowerGauge powerGauge;
    private PlayerMovement playerMovement;
    private PlayerDash playerDash;
    private CharacterController controller;

    // Input actions for different suplexes and jumping
    private InputAction SuperSuplexAction;
    private InputAction RainbowSuplexAction;
    private InputAction LongJumpSuplexAction;
    private InputAction jumpAction;
    private InputAction homingAction;
 
    public bool isSuplexing = false;
    private SuplexAbilities currentSuplex = SuplexAbilities.None;
    
    private void Awake()
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();
        if (playerDash == null)
            playerDash = GetComponentInParent<PlayerDash>();
        playerMovement = GetComponentInParent<PlayerMovement>();
        controller = GetComponentInParent<CharacterController>();
        powerGauge = GetComponentInParent<PowerGauge>();
        
        // Auto-find components if not assigned
        if (trajectoryVisualizer == null)trajectoryVisualizer = GetComponent<SuplexTrajectoryVisualizer>();
        if (grabHandler == null) grabHandler = GetComponent<EnemyGrabHandler>();
        if (homingAttack == null)homingAttack = GetComponent<HomingAttack>();
        if(voiceManager == null)voiceManager = GetComponentInParent<PlayerVoiceManager>();
            
        // Get input actions
        jumpAction = playerInput.actions.FindAction("Jump");
        SuperSuplexAction = playerInput.actions.FindAction("SuperSuplex");
        RainbowSuplexAction = playerInput.actions.FindAction("RainbowSuplex");
        LongJumpSuplexAction = playerInput.actions.FindAction("LongjumpSuplex");
        homingAction = playerInput.actions.FindAction("Homing");
        
        // Initialize components
        if (grabHandler != null)
            grabHandler.Initialize(playerMovement);
        
        if (trajectoryVisualizer != null && grabHandler != null)
            trajectoryVisualizer.Initialize(playerMovement, grabHandler.TrajectoryTransform);
        
        if (homingAttack != null)
            homingAttack.Initialize(playerDash, controller, homingAction);

        droneDropped = false;
        bossDropped = false;
        droneCooldown = 1f;
        bossCooldown = 1f;
    }

    private void Update()
    {
        // Update homing attack logic
        if (homingAttack != null)
            homingAttack.UpdateHoming(isSuplexing);
        if (droneDropped == true)
        {
            droneCooldown -= Time.deltaTime;
        }
        if (droneCooldown <= 0)
        {
            droneDropped = false;
            droneCooldown = 1f;
        }
        if (bossDropped == true)
        {
            bossCooldown -= Time.deltaTime;
        }
        if (bossCooldown <= 0)
        {
            bossDropped = false;
            bossCooldown = 1f;
        }
    }

    /// <summary>
    /// Starts the suplex process by grabbing the enemy.
    /// </summary>
    public void StartSuplex(Collider enemy)
    {
        if (isSuplexing) return;
                                 
        if (playerDash != null && playerDash.isDashing)
            playerDash.CancelDash();
        
        isSuplexing = true;
        
        // Use grab handler to hold enemy
        if (!grabHandler.IsHoldingEnemy())
       {
        //    Debug.Log("Already holding an enemy.");
            grabHandler.GrabEnemy(enemy);
            
            // Update trajectory visualizer with current gravity scale
            if (trajectoryVisualizer != null)
                trajectoryVisualizer.SetGravityScale(grabHandler.CurrentGravityScale);
        }

        // Disarm homing when starting a new suplex
        if (homingAttack != null)
            homingAttack.DisarmHomingWindow();

        // Wait for player to choose which suplex to perform
        StartCoroutine(WaitForSuplexInput());
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
                    if (trajectoryVisualizer != null)
                    {
                        trajectoryVisualizer.SetTrajectoryMaterial(ability);
                        var cfg = suplexConfigs.Find(cfg => cfg.ability == ability);
                        if (cfg != null)
                            trajectoryVisualizer.ShowTrajectory(cfg);
                    }

                    previewing = ability;
                    anyHeldThisFrame = true;
                }

                if (previewing == ability && action.WasReleasedThisFrame())
                {
                    if (ability == SuplexAbilities.Super && !CanPerformSuperSuplex())
                    {
                        Debug.Log("Not enough power for Super Suplex!");
                        continue;
                    }
                    currentSuplex = ability;
                    break;
                }
            }

            if (!anyHeldThisFrame)
            {
                previewing = SuplexAbilities.None;
                if (trajectoryVisualizer != null)
                {
                    trajectoryVisualizer.ClearTrajectory();
                    trajectoryVisualizer.SetTargetLandActive(false);
                }
            }

            yield return null;
        }

        if (trajectoryVisualizer != null)
        {
            trajectoryVisualizer.ClearTrajectory();
            trajectoryVisualizer.SetTargetLandActive(false);
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
            powerGauge.powerSlider.value += 0.01f;

            if (type == SuplexAbilities.Super && powerGauge != null)
                powerGauge.SpendMeter();
           // AudioManager.PlaySuplexStart();
            if (voiceManager != null) voiceManager.PlaySuplexVoicePhrase(type);
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
        // Calculate launch velocity using current gravity scale from grab handler
        float currentGravityScale = grabHandler != null ? grabHandler.CurrentGravityScale : 1f;
        float originalGravity = playerMovement.gravity;
        playerMovement.gravity = originalGravity * currentGravityScale;
        
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

        // Store original mesh rotation to restore later
        Quaternion originalMeshRotation = playerMesh != null ? playerMesh.localRotation : Quaternion.identity;

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

        // Enable MacroBoss hitbox during suplex
        if (grabHandler != null)
            grabHandler.SetMacroBossHitbox(true);
       
        while (true)
        {
            if(currentSuplex == SuplexAbilities.Super) playerMovement.CurrentState = PlayerState.SuperSuplex;
            else playerMovement.CurrentState = PlayerState.Suplex;// Setting suplex state for face change and other state-based logic

            t += Time.deltaTime;

            if (controller != null && (controller.collisionFlags & CollisionFlags.Above) != 0)
            {
                playerMovement.velocity.x = 0f;
                playerMovement.velocity.z = 0f;
                playerMovement.velocity.y = -1f;
                break;
            }
            if (playerMovement.velocity.y > 0f)// when player launches up, rotate player mesh based on suplex type
            {
                switch(currentSuplex)
                {
                    case SuplexAbilities.Rainbow:
                        playerMesh.Rotate(Vector3.left, 100f * Time.deltaTime, Space.World);
                        playerMesh.Rotate(Vector3.forward, 100f * Time.deltaTime, Space.World);
                        break;
                    case SuplexAbilities.Long:
                        playerMesh.Rotate(Vector3.forward, 800f * Time.deltaTime, Space.World);

                        break;
                    case SuplexAbilities.Super:
                        playerMesh.Rotate(Vector3.forward, 1000f * Time.deltaTime, Space.World);
                        break;
                } 
                    
                  
            }
            // if player is falling down with the super suplex performed
            if (playerMovement.velocity.y < 0)
            {
                switch(currentSuplex)
                {

                    case SuplexAbilities.Rainbow:
                        playerMesh.Rotate(Vector3.up, 1000f * Time.deltaTime, Space.World);
                        playerMesh.Rotate(Vector3.forward, 1000f * Time.deltaTime, Space.World);
                        break;
                    case SuplexAbilities.Long:
                        playerMesh.Rotate(Vector3.forward, 1000f * Time.deltaTime, Space.World);
                        playerMesh.Rotate(Vector3.left, 1000f * Time.deltaTime, Space.World);
                        break;
                    case SuplexAbilities.Super:
                    //    playerMesh.Rotate(Vector3.left, 1300f * Time.deltaTime, Space.World);
                        playerMesh.Rotate(Vector3.up, 1300f * Time.deltaTime, Space.World);
                        // add moon gravity effect during descent
                        gravityLerpTime += Time.deltaTime;
                        float lerpFactor = Mathf.Clamp01(gravityLerpTime / gravityIncreaseDuration);
                        playerMovement.gravity = Mathf.Lerp(minGravity, maxGravity, GravityControl.Evaluate(lerpFactor));
                        playerMesh.Rotate(Vector3.right, 1000f * Time.deltaTime, Space.World);
                        if (!cameratilted)
                        {
                            // tilt camera downwards during descent and have more control on flowing down 
                            cameratilted = true;
                            playerMovement.velocity.x = 0f;
                            playerMovement.velocity.z = 0f;
                        }
                        cameraLerpTime += Time.deltaTime;
                        float cameraLerpFactor = Mathf.Clamp01(cameraLerpTime / cameraLerpDuration);
                        orbital.TargetOffset = Vector3.Lerp(DefaultCameraOffset, targetOffset, CameraOffsetCurve.Evaluate(cameraLerpFactor));
                        break;
                }
                
               
               
            }

            if (!jumpedOff && jumpAction != null && jumpAction.WasPressedThisFrame())
            {
                Transform justReleased = grabHandler != null ? grabHandler.GrabbedEnemy : null;
                playerMovement.ForceJump();
                jumpedOff = true;
                
                // Release enemy with slam force
                if (grabHandler != null)
                {
                    droneDropped = true;
                    bossDropped = true;
                    grabHandler.ReleaseEnemy(applyDownwardForce: true);
                }
                    

                
                
                // Arm homing window for chaining attacks
                if (homingAttack != null)
                    homingAttack.ArmHomingWindow(justReleased);
               
                playerMovement.velocity.x = 0f;
                playerMovement.velocity.z = 0f;   
                break;
            }
            
            // End the suplex when the player lands (after a minimum airtime)
            if (t > minAirTime && IsGrounded())
            {
                CameraShakeManager.Instance.SuplexCameraShake(impulseSource);
                if (shockwave != null)// checks if there is a shockwave prefab assigned ,optional check if player !=null
                {
                    if (rageBar.value >= 1)
                    {
                        Instantiate(rageShockwave, player.position, player.rotation, player);
                        powerGuage.rageIncrease = false;
                        rageBar.value = 0;
                        suplexBar.sprite = suplexImg1;
                    }
                    else
                    {
                        Instantiate(shockwave, player.position, player.rotation, player);
                    }
                } 
                AudioManager.PlaySuplexSlam();
                break;
            }

            yield return null;
        }
        playerMesh.localRotation = originalMeshRotation;
        // _RB.angularVelocity = Vector3.zero;
        if (!jumpedOff)
        {
            // Release enemy without slam force
            if (grabHandler != null)
                grabHandler.ReleaseEnemy(applyDownwardForce: false);
            
            playerMovement.velocity.x = 0f;
            playerMovement.velocity.z = 0f;
            playerMovement.velocity.y = -2f;
        }
        
        // Disable MacroBoss hitbox
        if (grabHandler != null)
            grabHandler.SetMacroBossHitbox(false);
        
        isSuplexing = false;
        orbital.TargetOffset = DefaultCameraOffset;
    }

    bool IsGrounded()
    {
        return controller != null && controller.isGrounded;
    }

    private bool CanPerformSuperSuplex()
    {
        return powerGauge != null && powerGauge.powerSlider.value >= 1;
    }
}


