using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles homing attack functionality after releasing an enemy during a suplex.
/// Allows chaining attacks by dashing to nearby enemies while airborne.
/// activates grab mechanic once player dashes into an enemy.
/// </summary>
public class HomingAttack : MonoBehaviour
{
    [Header("Homing Settings")]
    [SerializeField] private float homingSearchRadius = 12f;

    // State tracking
    private bool canHomeChain = false;
    private Transform lastReleasedEnemy = null;

    // References
    private PlayerDash playerDash;
    private CharacterController controller;
    private InputAction homingAction;

    public void Initialize(PlayerDash dash, CharacterController characterController, InputAction homingInputAction) // Initialize references 
    {
        playerDash = dash;
        controller = characterController;
        homingAction = homingInputAction;
    }

    public void UpdateHoming(bool isSuplexing)
    {
        // Disarm when grounded
        if (IsGrounded())
        {
            canHomeChain = false;
            lastReleasedEnemy = null;
            return;
        }

        // Only allow homing when airborne, not suplexing, and not already dashing
        if (canHomeChain && !isSuplexing && playerDash != null && !playerDash.isDashing)
        {
            bool homingPressed = (homingAction != null && homingAction.WasPressedThisFrame());

            if (homingPressed)
            {
                Transform target = FindNearestEnemy(transform.position, homingSearchRadius, lastReleasedEnemy);
                if (target != null)
                {
                    if (playerDash.TryDashTowards(target))
                    {
                        canHomeChain = false; // Consume the homing window
                    }
                }
            }
        }
    }

    /// <summary>
    /// Arms the homing attack window when releasing an enemy mid-air.
    /// </summary>
    public void ArmHomingWindow(Transform releasedEnemy)
    {
        canHomeChain = true;
        lastReleasedEnemy = releasedEnemy;
    }

    /// <summary>
    /// Disarms the homing attack window when starting a new suplex.
    /// </summary>
    public void DisarmHomingWindow()
    {
        canHomeChain = false;
        lastReleasedEnemy = null;
    }

    private Transform FindNearestEnemy(Vector3 origin, float radius, Transform lastReleasedEnemyignore = null) // player position, search radius, new enemy to dash towards
    {
        Collider[] hits = Physics.OverlapSphere(origin, radius, ~0, QueryTriggerInteraction.Collide); // creates a sphere radius to detect nearby colliders

        Transform best = null; // empty transform to store the closest enemy found
        float bestSqr = float.MaxValue; // 3.40282347E+38 big number as an initial value starting point, example:  if (sqr < bestSqr)  = if (5enemy units< 3.40282347E+38)

        for (int i = 0; i < hits.Length; i++) // loops through all enemy colliders found in the radius
        {
            var col = hits[i];

            if (lastReleasedEnemyignore != null && (col.transform == lastReleasedEnemyignore || col.transform.IsChildOf(lastReleasedEnemyignore)))// checks to ensure player does not home attacks same enemy
                continue;

            var enemy = col.GetComponentInParent<EnemyBase>();
            if (enemy == null || !enemy.gameObject.activeInHierarchy)
                continue;

            float sqr = (enemy.transform.position - origin).sqrMagnitude; // squared distance between player and enemy
            if (sqr < bestSqr) //loop checks if this enemy is closer to player than previous closest
            {
                bestSqr = sqr; // update new closest distance
                best = enemy.transform; // assign the closest enemy transform to home attack towards
            }
        }

        return best; // return the closest enemy found
    }

    private bool IsGrounded()
    {
        return controller != null && controller.isGrounded;
    }
}