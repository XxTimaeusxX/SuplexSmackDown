using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyBase : MonoBehaviour
{
    [Header("References")]
    public GameObject Target;
    public NavMeshAgent agent;
    public Rigidbody rb;
    public InGameMenuManager menuManager;
    public PowerGauge powerGuage;
    public Transform groundCheck;
    public LayerMask groundMask;
    public float groundDistance;
    public Slider rageBar;

    [Header("Behavior Toggles ")]
    public bool canPatrol = true;
    public bool canChase = true;
    public bool canAttack = true;
    public virtual bool AutoKinematic => true; 
    [Header("Ground Settings")]
    public float m_Distance;
    public bool wasGrounded = false;
    public bool IgnoreGroundCheck = false;
    public bool isGrabbed;
    public bool isPushed = false;
    public float pushCooldown;

    [Header("UI")]
    public Slider chargeSlider;

    public Slider enemyHealth;
    public GameObject enemyHealthScreen;

    [Header("Patrol Settings")]
    public float chaseRange;
    public float patrolWalkSpeed;
    public float patrolWaitTime;
    public float patrolRunSpeed;
    public float patrolWaitDefault;

    [Header("Combat")]
    public float meleeRange = 1.75f;
    public float attackCooldown = 0.8f;
    public float _nextAttackTime = 0f;
  //public bool IsDead = false;

    [Header("Hitbox")]
    public GameObject slapbox;          // child trigger collider with AttackHitBox
    [SerializeField] private float slapActiveTime = 0.1f;
    private bool grounded;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    GameManager gameManager;

    [Header("Ground Check Optimizations")]
    private float groundCheckInterval = 3f; // Check every 3 seconds
    private float groundCheckTimer;
    private Coroutine activeGroundCheck;
    private static readonly WaitForSeconds SlapWait = new WaitForSeconds(0.5f); //caching to reduce GC
    private static readonly WaitForSeconds SlapActiveWait = new WaitForSeconds(0.09f);

    [Header("Chase Optimizations")]
    private Vector3 _LastTargetPosition;
    private float _nextPathUpdateTime;
    private float pathUpdateInterval = 2f; // Update path every 0.5 seconds
    private float TargetMoveThreshold = 2f; // Only update if target moved more than this distance

    public void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        if (chargeSlider != null)
        {
            chargeSlider.minValue = 0f;
            chargeSlider.maxValue = attackCooldown;
            chargeSlider.value = 0f;
            chargeSlider.gameObject.SetActive(false);
        }
     
        if (slapbox != null)
        {
            slapbox.SetActive(false);
        }
    }

    // Update is called once per frame
    public virtual void Update()
    {
        // Throttle ground check: only check every groundCheckInterval, and only if not grabbed/pushed/agent enabled
        groundCheckTimer -= Time.deltaTime;
        if (groundCheckTimer <= 0f && !isGrabbed && !isPushed && !agent.enabled)
        {
            grounded = IsEnemyGrounded();
            groundCheckTimer = groundCheckInterval;
        }
        // If agent is enabled, assume grounded (optional: set grounded = true)
        if (agent.enabled)
        {
            grounded = true;
        }

        if (isPushed)
        {
            pushCooldown -= Time.deltaTime;
        }
        if (pushCooldown < 0)
        {
            if (!isGrabbed && !CompareTag("DontRespawn"))
            {
                pushCooldown = 0;
                isPushed = false;
                agent.enabled = true;
                rb.isKinematic = true;
            }
        }
        if (!grounded)
        {
            ResetSlapState();
        }
        if (grounded && wasGrounded && !isGrabbed && !isPushed && AutoKinematic)
        {
            rb.isKinematic = true;
            agent.enabled = true;
        }
        wasGrounded = grounded;
        if (agent.enabled && agent.isOnNavMesh)
        {
            if (canChase)
            {
                ChasePlayer();
            }
            else if (canPatrol)
            {
                // Patrol-only mode: request a new patrol destination when there's no path or we've arrived.
                float arrivalThreshold = Mathf.Max(0.5f, agent.stoppingDistance);
                if (!agent.hasPath || agent.remainingDistance <= arrivalThreshold)
                    RandomPatrolDestination();
            }
        }
    }



    public void ResetSlapState()
    {
        _nextAttackTime = 0f;
        if (slapbox != null)
        {
            slapbox.SetActive(false);
        }
        StopCoroutine(SlapattackDuration());
        ResetChargeUI();
    }

    public virtual void RandomPatrolDestination()
    {
        if (!canPatrol) return;
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
                    agent.speed = patrolWalkSpeed;
                    agent.destination = hit.position;
                    return;
                }
            }
        }
    }

    public void FaceTarget()
    {
        Vector3 direction = Target.transform.position - transform.position;
        direction.y = 0;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    public virtual void ChasePlayer()
    {
        // Behavior guard: only chase when allowed
        if (!canChase) return;
        if (Target == null)
        {
            RandomPatrolDestination();
            return;
        }
        //  m_Distance = Vector3.Distance(Target.transform.position, transform.position);
        float SqrChaseRange = chaseRange * chaseRange;
        float SqrMeleeRange = meleeRange * meleeRange;
        m_Distance = (Target.transform.position - transform.position).sqrMagnitude;
        float arrivalThreshold = Mathf.Max(0.5f, agent.stoppingDistance);

        if (agent.isOnNavMesh)
        {
            if (m_Distance <= SqrChaseRange)
            {
                patrolWaitDefault = 0f;
                agent.speed = patrolRunSpeed;
                FaceTarget();
                if (m_Distance < SqrMeleeRange)
                {
                    agent.isStopped = true;
                    BaseAttack();
                }
                else
                {
                    if (agent.isStopped) agent.isStopped = false;
                    if(Time.time >= _nextPathUpdateTime || Vector3.SqrMagnitude(Target.transform.position - _LastTargetPosition) > TargetMoveThreshold * TargetMoveThreshold)
                    {
                        agent.destination = Target.transform.position;
                        _LastTargetPosition = Target.transform.position;
                        _nextPathUpdateTime = Time.time + pathUpdateInterval;
                    }

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
                RandomPatrolDestination();
            }
        }
    }

    public virtual void BaseAttack()
    {
        // Behavior guard: only attack when allowed
        if (!canAttack) { ResetSlapState(); return; }// ensure state/UI is cleared if attack disabled mid-charge
       

        // Charge up while in melee
        if (_nextAttackTime < attackCooldown)
        {
            _nextAttackTime += Time.deltaTime;
            UpdateChargeUI(_nextAttackTime, attackCooldown, show: true);
            // Debug.Log($"charge: {_nextAttackTime:F2}/{attackCooldown:F2}");
            return;
        }
        CustomAttack();
        // Fully charged -> attack, then reset charge for the next swing

        _nextAttackTime = 0f; // restart charge
        UpdateChargeUI(_nextAttackTime, attackCooldown, show: true);
        
    }
    protected virtual void CustomAttack()
    {
        Debug.Log($"[{name}] Melee attack!");
        AudioManager.PlayEnemySlap();
        StartCoroutine(SlapattackDuration());
    }    


public IEnumerator SlapattackDuration()
{
    if (slapbox == null) yield break;
    yield return SlapWait;
    slapbox.SetActive(true);
    yield return SlapActiveWait;
    slapbox.SetActive(false);
}
    // Add these helpers inside Enemy class
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
    public virtual void SetGrabbed(bool grabbed)
    {
        isGrabbed = grabbed;
        if (grabbed)
        {
            
            agent.enabled = false;
        }

    }
    public bool IsEnemyGrounded()
    {
        if (IgnoreGroundCheck) return false;
        // Use a raycast or other method to check if the enemy is on the ground
      //    Debug.DrawRay(transform.position, Vector3.down * 4.0f, Color.red, 0.1f);
       return Physics.Raycast(transform.position, Vector3.down, groundDistance, groundMask);

    }
    /// <summary>
    /// Gizmo to visualize the ground check sphere in the editor
    /// </summary>
  /*  public void OnDrawGizmosSelected()
    {
        Vector3 center = groundCheck != null ? groundCheck.position : transform.position;
        float radius = Mathf.Max(groundDistance, 0f);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(center, radius);
        // Chase range sphere (yellow)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);

        // Melee range sphere (red/orange)
        Gizmos.color = new Color(1f, 0.5f, 0f); // orange
        Gizmos.DrawWireSphere(transform.position, meleeRange);
    }*/
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Shockwave")
        {
            pushCooldown = 3;
            isPushed = true;
            agent.enabled = false;
            rb.isKinematic = false;
            if (powerGuage.rageIncrease == true)
            {
                rageBar.value += 0.01f;
            }
        }
    }
}
