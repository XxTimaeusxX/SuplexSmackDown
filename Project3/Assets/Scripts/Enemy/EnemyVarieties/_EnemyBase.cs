using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

// Contributers: Istvan W.

// TODO: Make sure to make a grounded check before enabling NavMeshAgent after being thrown, to prevent weird mid-air pathfinding behavior
// TODO: Put a cooldown on enemy so they can't immediatly attack on release.
// TODO: Add a check to the exit state to make sure the character isn't grabbed when re-enabling AI, etc.
/// <summary>
/// Abstract base class for enemy characters, providing core behavior such as patrolling, chasing, attacking, and
/// handling carried state interactions.
/// </summary>
public abstract class EnemyBase : MonoBehaviour, ICarriable
{
    [Header("References")]
    private GameObject PLAYER; // Reference to the player (Currenly set as so due to no enemy vs enemy interactions)
    public Rigidbody Rigidbody => rb; // Public getter for Rigidbody (for ICarriable interface)
    private Rigidbody rb;
    public NavMeshAgent agent;
    //public MonoBehaviour enemyAI; // whatever AI script used
    private Transform originalParent; // To store original parent when carried (e.g. After picking up an enemy that's parented to another object, once released it should go back to that object)
    //private GroundChecker groundChecker;
    private RigidbodyConstraints originalConstraints;
    private GroundChecker groundChecker;

    [Header("Stats")]
    public int health;
    public float enemyWalkSpeed;
    public float enemySprintSpeed;

    public float rageXP; // Amount of rage you get on defeat
    public float rageTime; // Amount of rage time you get on defeat
    public float timeTillDeath; // Time from when the enemy is knocked out to when it dies (for enemies that can be knocked out but not killed immediately, e.g. construction workers)

    [SerializeField] protected CarryWeightProfile carryWeightProfile; // Used to determine how the enemy behaves when being carried (e.g. how much it slows the player down, whether it can be thrown, etc.)
    public CarryWeightProfile CarryWeightProfile => carryWeightProfile; // Public getter for carry weight profile

    [Header("Combat")]
    public float attackRange = 2f;
    public float attackCooldown = 0.8f;   // Time between attacks
    public float _nextAttackTime = 0f;
    public float knockOutTime = 5f; // Time the enemy stays knocked out


    [Header("Colliders")]
    public Collider mainCollider;   // Main collider for the enemy
    public Collider carryProxy;     // Collider used when being carried to prevent clipping

    private bool hasLanded = false;

    [Header("Patrol Settings")]
    public GameObject target; // Current target (e.g. player or patrol point)
    public float distanceToTarget;
    public float chaseRange;
    public float patrolWaitDefault;

    [Header("Ground Settings")]
    public float m_Distance;    // Distance to the target
    public bool isGrabbed;
    public bool isPushed = false;
    public float pushCooldown;

    [Header("Behavior Toggles ")]
    //public bool canPatrol = true;
    //public bool canChase = true;
    //public bool canAttack = true;
    //NOTE: make sure behavior toggles are properly working.

    [Header("Animation")]
    public Animator animator;

    // Testing purposes
    [Header("UI")]
    public Slider chargeSlider;
    public Slider rageBar;

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
        animator = GetComponent<Animator>();
        originalConstraints = rb.constraints; // Store original Rigidbody constraints
        groundChecker = GetComponent<GroundChecker>();
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

    public virtual void Update()
    {
        if (isPushed)
        {
            pushCooldown -= Time.deltaTime;
        }
        if (agent.enabled && agent.isOnNavMesh)
        {
            if (Vector3.Distance(PLAYER.transform.position, transform.position) <= chaseRange)
            {
                ChasePlayer();
            }
            else 
            {
                // Patrol-only mode: request a new patrol destination when there's no path or we've arrived.
                float arrivalThreshold = Mathf.Max(0.5f, agent.stoppingDistance);
                if (!agent.hasPath || agent.remainingDistance <= arrivalThreshold)
                    Patrol();
            }
        }
    }
    // Default = Random Roaming Patrol
    public virtual void Patrol()
    {
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
        target = PLAYER; // Set target to player (can be modified for other targets)

        distanceToTarget = Vector3.Distance(target.transform.position, transform.position);
        float arrivalThreshold = Mathf.Max(0.5f, agent.stoppingDistance);

        // MARK - Chasing Logic
        // NOTE: Look into changing this into vision cone logic later on
        if (agent.isOnNavMesh)
        {
            bool inChaseRange = (distanceToTarget <= chaseRange);

            if (inChaseRange)
            {
                patrolWaitDefault = 0f;
                agent.speed = enemySprintSpeed;
                agent.destination = target.transform.position;

                if (distanceToTarget < attackRange)  // When within melee range -> Face -> Attack
                {
                    agent.isStopped = true;
                    FaceTarget();
                    WaitForSeconds wait = new WaitForSeconds(0.5f);
                    Attack();
                }
                else
                {
                    if (agent.isStopped) 
                        agent.isStopped = false;

                    agent.destination = target.transform.position;
                    _nextAttackTime = 0f;
                    ResetChargeUI();
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
        hasLanded = false; // Reset landing state for when the enemy is thrown

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

        rb.constraints = RigidbodyConstraints.None;

        mainCollider.enabled = true;    //  Re-enable colliders
     
        if (carryProxy != null) carryProxy.enabled = false;     //  Disable proxy collider

        if (throwForce != Vector3.zero)
            rb.AddForce(throwForce, ForceMode.Impulse);    // Apply throw force

        //Debug.Log("ExitCarriedState called on " + gameObject.name);
        //Debug.Log($"Main collider enabled: {mainCollider.enabled}, Proxy: {carryProxy.enabled}");
    }

    public virtual void OnCollisionEnter(Collision collision)
    {
        if (hasLanded) return;

        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            StartCoroutine(EnableAgentAfterLanding());
        }

        if (collision.gameObject.CompareTag("Shockwave"))
        {
            pushCooldown = 3;
            isPushed = true;
            agent.enabled = false;
            rb.isKinematic = false;
            rageBar.value += rageXP;
            RageMeter rageMeter = PLAYER.GetComponent<RageMeter>();
            rageMeter.IsEnraged(rageTime);

        }
    }

    /// ------------------------------- ///

    /// Unchanging methods for enemy behaviour
    private IEnumerator EnableAgentAfterLanding()
    {
        yield return new WaitForSeconds(knockOutTime); // Small delay to ensure physics has settled after landing
        hasLanded = true;
        rb.constraints = originalConstraints;
        agent.enabled = true;
        //Debug.Log("Agent has landed and is now enabled for " + gameObject.name);
    }
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

    public bool IsEnemyGrounded()
    {
        return groundChecker = GetComponent<GroundChecker>();
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
