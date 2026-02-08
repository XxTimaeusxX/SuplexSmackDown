using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

// Contributers: Istvan W.

// Last Modified: 1/19/2026 by Istvan W.

/// <summary>
/// Abstract base class for enemy characters, providing core behavior such as patrolling, chasing, attacking, and
/// handling carried state interactions.
/// </summary>
public abstract class EnemyBase : MonoBehaviour
{
    [Header("References")]
    public GameObject PLAYER; // Reference to the player (Currenly set as so due to no enemy vs enemy interactions)
    private Rigidbody rb;
    private NavMeshAgent agent;
    //public MonoBehaviour enemyAI; // whatever AI script used
    private Transform originalParent; // To store original parent when carried (e.g. After picking up an enemy that's parented to another object, once released it should go back to that object)
    private MonoBehaviour groundChecker;

    [Header("Stats")]
    public float health;
    public float enemyWalkSpeed;
    public float enemySprintSpeed;

    [Header("Combat")]
    public float attackRange = 2f;
    public float attackCooldown = 0.8f;   // Time between attacks
    private float _nextAttackTime = 0f;

    [Header("Colliders")]
    public Collider mainCollider;   // Main collider for the enemy
    public Collider carryProxy;     // Collider used when being carried to prevent clipping

    [Header("Patrol Settings")]
    public float distanceToTarget;
    public float chaseRange;
    public float patrolWaitDefault;

    // Testing purposes
    [Header("UI")]
    public Slider chargeSlider;

    /// Abstract methods for enemy behavior (to be implemented by derived classes)
    public abstract void Attack();
    public abstract void Death();

    /// ------------------------------- ///

    /// Virtual methods for enemy behaviour (can be overridden by derived classes)

    public virtual void Awake()
    {
        /// Initialize references
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        groundChecker = GetComponent<MonoBehaviour>(); // Replace with actual ground checker script type if available

        // Test purposes
        if (chargeSlider != null)
        {
            chargeSlider.minValue = 0f;
            chargeSlider.maxValue = attackCooldown;
            chargeSlider.value = 0f;
            chargeSlider.gameObject.SetActive(false);
        }

        // Assign PLAYER if not set
        if (PLAYER == null)
        {
            if (GameObject.FindWithTag("Player") != null) 
                PLAYER = GameObject.FindWithTag("Player");

        }
    }


    // Default = Random Roaming Patrol
    public virtual void Patrol()
    {
        //if (!canPatrol) return;
        if (!agent.enabled || !agent.isOnNavMesh) return;

        const float patrolRadius = 20f;
        const int maxTries = 6;
        Vector3 origin = transform.position;

        for (int i = 0; i < maxTries; i++)
        {
            Vector2 r = Random.insideUnitCircle * patrolRadius;
            Vector3 candidate = new Vector3(origin.x + r.x, origin.y, origin.z + r.y);

            if (NavMesh.SamplePosition(candidate, out var hit, 2f, agent.areaMask))
            {
                var path = new NavMeshPath();
                if (agent.CalculatePath(hit.position, path) && path.status == NavMeshPathStatus.PathComplete)
                {
                    agent.isStopped = false;
                    agent.speed = enemyWalkSpeed;
                    agent.destination = hit.position;
                    return;
                }
            }
        }
    }
    public virtual void ChasePlayer()
    {
        GameObject Target = PLAYER; // Set target to player (can be modified for other targets)

        // Behavior guard: only chase when allowed
        //if (!canChase) return;
        if (Target == null)
        {
            Patrol();
            return;
        }

        distanceToTarget = Vector3.Distance(Target.transform.position, transform.position);
        float arrivalThreshold = Mathf.Max(0.5f, agent.stoppingDistance);

        // MARK - Chasing Logic
        // NOTE: Look into changing this into vision cone logic later on
        if (agent.isOnNavMesh)
        {
            bool inChaseRange = distanceToTarget <= chaseRange;

            if (inChaseRange)
            {
                patrolWaitDefault = 0f;
                agent.speed = enemySprintSpeed;
                agent.destination = Target.transform.position;

                if (distanceToTarget < attackRange)  // When within melee range -> Face -> Attack
                {
                    agent.isStopped = true;
                    FaceTarget();
                    Attack();
                }
                else
                {
                    if (agent.isStopped) agent.isStopped = false;
                    agent.destination = Target.transform.position;
                    _nextAttackTime = 0f;
                    //ResetChargeUI();
                }
            }

            if (patrolWaitDefault > 0f)
            {
                patrolWaitDefault -= Time.deltaTime;
                if (patrolWaitDefault <= 0f)
                {
                    agent.isStopped = false;
                }
            }
            else if (!agent.hasPath || agent.remainingDistance <= arrivalThreshold)
            {
                Patrol();
            }
        }
    }

    //// Logic for when the enemy is picked up by the player

    /// Called when the player picks up the enemy
    public virtual void EnterCarriedState(Transform carryPoint)
    {
        //Debug.Log("Enemy picked up");

        originalParent = transform.parent;  // Store original parent

        // Disable physics
        rb.isKinematic = true;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        mainCollider.enabled = false;   // Disable colliders that should not interact

        // Disable AI/NavMeshAgent
        //if (enemyAI != null) enemyAI.enabled = false;
        agent.enabled = false;

        if (carryProxy != null) carryProxy.enabled = true;  // Enable proxy collider (prevents clipping)

        // Parent to player carry point
        transform.SetParent(carryPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        //Debug.Log("EnterCarriedState called on " + gameObject.name);
    }

    /// Called when the player throws or drops the enemy
    public virtual void ExitCarriedState(Vector3 throwForce)
    {
    
        transform.SetParent(originalParent);    // Unparent
  
        rb.isKinematic = false;     // Re-enable physics

        mainCollider.enabled = true;    //  Re-enable colliders
     
        if (carryProxy != null) carryProxy.enabled = false;     //  Disable proxy collider

        // Re-enable AI/NavMeshAgent
        //if (enemyAI != null) enemyAI.enabled = true;
        agent.enabled = true;

        //rb.AddForce(throwForce, ForceMode.VelocityChange);    // Apply throw force

        //Debug.Log("ExitCarriedState called on " + gameObject.name);
    }

    /// ------------------------------- ///

    /// Unchanging methods for enemy behaviour

    public void FaceTarget()
    {
        var TurnToTarget = agent.steeringTarget;
        Vector3 direction = (TurnToTarget - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    public static float GetHeight(Collider col)
    {
        return col.bounds.size.y;
    }

    public virtual void ApplyDownwardForce(float force)
    {
        if (rb == null)
            return;

        rb.AddForce(Vector3.down * force, ForceMode.Impulse);
    }
    /// ------------------------------- ///

    /// Testing purposes

    public void UpdateChargeUI(float current, float max, bool show)
    {
        if (chargeSlider == null) return;

        if (!chargeSlider.gameObject.activeSelf && show)
            chargeSlider.gameObject.SetActive(true);
        else if (chargeSlider.gameObject.activeSelf && !show)
            chargeSlider.gameObject.SetActive(false);

        if (!Mathf.Approximately(chargeSlider.maxValue, max))
            chargeSlider.maxValue = max;

        chargeSlider.value = Mathf.Clamp(current, 0f, max);
    }
    public void ResetChargeUI()
    {
        if (chargeSlider == null) return;
        chargeSlider.value = 0f;
        if (chargeSlider.gameObject.activeSelf)
            chargeSlider.gameObject.SetActive(false);
    }

}
