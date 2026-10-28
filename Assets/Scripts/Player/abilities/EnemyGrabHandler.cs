using System.Collections;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Handles grabbing, holding, and releasing enemies during suplex operations.
/// Manages enemy physics, AI, and special properties like weight/size.
/// </summary>
public class EnemyGrabHandler : MonoBehaviour
{
    [Header("Grab Settings")]
    public Transform heldEnemyTransform;
    public Transform TrajectoryTransform;

    [Header("Enemy Weight/Size")]
    [SerializeField] private string bigEnemyLayerName = "BigEnemy";
    [SerializeField] private float bigEnemyGravityScale = 2.0f;
    [SerializeField] private float bigEnemyMoveSpeedScale = 1f;

    // Current grabbed enemy state
    public Transform GrabbedEnemy { get; private set; }
    public float CurrentGravityScale { get; private set; } = 1f;
    public float CurrentMoveSpeedScale { get; private set; } = 1f;

    // References to components
    private PlayerMovement _playerMovement;
    private MacroBoss _currentMacroBoss;
    private float _savedMoveSpeed;
    private float _defaultGravity;

 
    public void Initialize(PlayerMovement playerMovement)
    {
        
        _playerMovement = playerMovement;
        if (_playerMovement != null) // caching default values
        {
            _savedMoveSpeed = _playerMovement.moveSpeed;
            _defaultGravity = _playerMovement.gravity;
        }
    }

    public void GrabEnemy(Collider enemy)
    {
       
        if (enemy == null || heldEnemyTransform == null)
            return;
        //  Grab Enemy & play animation 
        GrabbedEnemy = enemy.transform;
        GrabbedEnemy.SetParent(heldEnemyTransform);
        StartCoroutine(GrabAnimationSequence());
        //  _playerMovement.ChangeAnimtion("GRAB");

        //  Debug.Log("Grabbed enemy: " + GrabbedEnemy.name);
        GrabbedEnemy.localPosition = Vector3.zero;

        //  Check for MacroBoss component 
        _currentMacroBoss = GrabbedEnemy.GetComponent<MacroBoss>();

        // Determine enemy size/weight based on enemy type
        var root = enemy.GetComponentInParent<EnemyBase>()?.transform ?? enemy.transform;
        int bigLayer = LayerMask.NameToLayer(bigEnemyLayerName);
        bool isBigEnemy = root.gameObject.layer == bigLayer;

        CurrentGravityScale = isBigEnemy ? bigEnemyGravityScale : 1f;
        CurrentMoveSpeedScale = isBigEnemy ? bigEnemyMoveSpeedScale : 1f;

        // Alter movement speed changes for big enemies
        if (isBigEnemy && _playerMovement != null)
        {
            _savedMoveSpeed = _playerMovement.moveSpeed;
            _playerMovement.moveSpeed = _savedMoveSpeed * CurrentMoveSpeedScale;
        }

        // Disable enemy AI and physics
        DisableEnemyComponents(enemy);
    }
    public IEnumerator GrabAnimationSequence()
    {
        _playerMovement.IsPlayingGrabAnimation = true;
        if (_playerMovement.isGrounded)
            _playerMovement.SetState(PlayerState.Grab, "GRAB", 0f, true);

        else
            _playerMovement.SetState(PlayerState.LeapGrab, "LeapGrab", 0f, true);
        yield return new WaitForSeconds(1f); // Wait for grab animation to play (adjust timing as needed)
        _playerMovement.IsPlayingGrabAnimation = false;
        //  _playerMovement.ChangeAnimtion("GrabIDLE");
    }
    public void ReleaseEnemy(bool applyDownwardForce)
    {
        if (GrabbedEnemy == null)
            return;

        StopCoroutine(GrabAnimationSequence());
        _playerMovement.IsPlayingGrabAnimation = false;

        var rb = GrabbedEnemy.GetComponent<Rigidbody>();
        var enemyScript = GrabbedEnemy.GetComponent<EnemyBase>();
        var ghostBoss = GrabbedEnemy.GetComponent<Level2BossManager>();
        
        // Re-enable enemy ground detection FIRST
        if (enemyScript != null)
        {
            enemyScript.SetGrabbed(false);
        }
        if (ghostBoss != null)
        {
            ghostBoss.grabbed = false;
        }

        // Unparent and re-enable physics
        GrabbedEnemy.SetParent(null);
        
        if (rb != null)
        {
            rb.isKinematic = false;

            if (applyDownwardForce)
            {
                rb.AddForce(Vector3.down * 5f, ForceMode.VelocityChange);
            }
        }

        // Re-enable NavMeshAgent AFTER unparenting
        if (enemyScript != null && enemyScript.agent != null&& enemyScript.IsEnemyGrounded())
        {
            enemyScript.agent.enabled = true;
        }

        // Restore player movement settings
        if (_playerMovement != null)
        {
            _playerMovement.moveSpeed = _savedMoveSpeed;
            _playerMovement.gravity = _defaultGravity;
        }

        // Disable MacroBoss damage hitbox
        if (_currentMacroBoss != null && _currentMacroBoss.damageHitbox != null)
        {
            _currentMacroBoss.damageHitbox.enabled = false;
        }

        _currentMacroBoss = null;
        GrabbedEnemy = null;
        CurrentGravityScale = 1f;
        CurrentMoveSpeedScale = 1f;
    }


    public void SetMacroBossHitbox(bool enabled)
    {
        if (_currentMacroBoss != null && _currentMacroBoss.damageHitbox != null)
        {
            _currentMacroBoss.damageHitbox.enabled = enabled;
        }
    }


    private void DisableEnemyComponents(Collider enemy)
    {
        // Disable NavMeshAgent
        var agent = enemy.GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent != null)
            agent.enabled = false;

        // Make rigidbody kinematic
        var rb = enemy.GetComponent<Rigidbody>();
        if (rb != null)
            rb.isKinematic = true;

        // Disable enemy ground detection
        var enemyScript = enemy.GetComponent<EnemyBase>();
        if (enemyScript != null)
            enemyScript.SetGrabbed(true);
    }


    public bool IsHoldingEnemy()
    {
        return GrabbedEnemy != null;
    }
}
