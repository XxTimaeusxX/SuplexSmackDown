using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


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
    public RageMeter rageMeter;
    
    [Header("Component References")]
    public SuplexTrajectoryVisualizer trajectoryVisualizer;
    public EnemyGrabHandler grabHandler;
    public HomingAttack homingAttack;
    
    [Header("Suplex Configurations")]
    public List<SuplexConfig> suplexConfigs;
    public AnimationCurve GravityControl;
    public AnimationCurve CameraOffsetCurve;

    // Internal references to other player scripts/components
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
        playerDash = GetComponentInParent<PlayerDash>();
        playerMovement = GetComponentInParent<PlayerMovement>();
        controller = GetComponentInParent<CharacterController>();
        trajectoryVisualizer = GetComponent<SuplexTrajectoryVisualizer>();
        grabHandler = GetComponent<EnemyGrabHandler>();
        homingAttack = GetComponent<HomingAttack>();
        
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
            //trajectoryVisualizer.Initialize(playerMovement, grabHandler.heldEnemyTransform);
        
        if (homingAttack != null)
            homingAttack.Initialize(playerDash, controller, homingAction);
    }

  
}


