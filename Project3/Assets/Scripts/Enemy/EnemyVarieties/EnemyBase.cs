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
    public Transform groundCheck;
    public LayerMask groundMask;
    public float groundDistance;

    [Header("Behavior Toggles ")]
    public bool canPatrol = true;
    public bool canChase = true;
    public bool canAttack = true;


    [Header("Ground Settings")]
    public float m_Distance;
    public bool wasGrounded = false;
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
    [Tooltip("Distance at which this enemy will attempt a melee attack.")]
    public float meleeRange = 1.75f;
    [Tooltip("Seconds between melee attack attempts (prevents spam).")]
    public float attackCooldown = 0.8f;
    public float _nextAttackTime = 0f;

    [Header("Hitbox")]
    public Collider slapbox;          // child trigger collider with AttackHitBox
    [SerializeField] private float slapActiveTime = 0.1f;

    [Header("Animation")]
    public Animator animator;



    // Start is called once before the first execution of Update after the MonoBehaviour is created

    GameManager gameManager;
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
        if (animator == null) animator = GetComponent<Animator>();
        slapbox.enabled = false;

    }

    // Update is called once per frame
    public void Update()
    {
        bool grounded = IsEnemyGrounded();
        if (isPushed)
        {
            pushCooldown -= Time.deltaTime;
        }
        if (pushCooldown < 0)
        {
            if (!isGrabbed)
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
        if (grounded && wasGrounded && !isGrabbed && !isPushed)
        {
            // Debug.Log("Enemy just landed!");
            rb.isKinematic = true;
            agent.enabled = true;
        }
        wasGrounded = grounded;
        if (agent.enabled && agent.isOnNavMesh)
        {
            ChasePlayer();
        }
    }
    public void ResetSlapState()
    {
        _nextAttackTime = 0f;
        slapbox.enabled = false;
        StopCoroutine(SlapattackDuration());
        ResetChargeUI();
    }
    public void RandomPatrolDestination()
    {
        // Behavior guard: only patrol when allowed
        if (!canPatrol) return;
        if (!agent.enabled || !agent.isOnNavMesh) return;

        // Pick points around current floor height (not y=0) to stay on the same NavMesh island
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
    public void ChasePlayer()
    {
        // Behavior guard: only chase when allowed
        if (!canChase) return;
        if (Target == null)
        {
            RandomPatrolDestination();
            return;
        }
        m_Distance = Vector3.Distance(Target.transform.position, transform.position);
        float arrivalThreshold = Mathf.Max(0.5f, agent.stoppingDistance);
        if (agent.isOnNavMesh)
        {
            if (m_Distance <= chaseRange)
            {
                patrolWaitDefault = 0f;
                agent.speed = patrolRunSpeed; // set chase speed
                agent.destination = Target.transform.position;

                if (m_Distance < meleeRange)
                {
                    agent.isStopped = true;
                    SlapAttack();
                }
                else
                {
                    // Out of melee: resume chase and clear timer so next entry arms again
                    if (agent.isStopped) agent.isStopped = false;
                    agent.destination = Target.transform.position;
                    _nextAttackTime = 0f;
                    ResetChargeUI();
                }
            }


            // Out of chase range -> patrol
            if (patrolWaitDefault > 0f)
            {
                // Idle thinking
                // m_EnemyAgent.isStopped = true;
                patrolWaitDefault -= Time.deltaTime;
                if (patrolWaitDefault <= 0f)
                {
                    agent.isStopped = false;
                    //  RandomPatrolDestination();

                }
            }
            else if (!agent.hasPath || agent.remainingDistance <= arrivalThreshold) RandomPatrolDestination();
        }
    }

    public void SlapAttack()
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

        // Fully charged -> attack, then reset charge for the next swing
        Debug.Log($"[{name}] Melee attack!");
        animator.SetTrigger("EnemySlap");
        AudioManager.PlayEnemySlap();
        _nextAttackTime = 0f; // restart charge
        UpdateChargeUI(_nextAttackTime, attackCooldown, show: true);
        StartCoroutine(SlapattackDuration());
    }
    public IEnumerator SlapattackDuration()
    {
        if (slapbox == null) yield break;
        yield return new WaitForSeconds(.5f); // wait a frame to sync with animation
        slapbox.enabled = true;
        yield return new WaitForSeconds(.09f);
        slapbox.enabled = false;
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
    public void SetGrabbed(bool grabbed)
    {
        isGrabbed = grabbed;
        if (grabbed)
        {
            // Optionally disable agent here if needed
            agent.enabled = false;
        }

    }
    public bool IsEnemyGrounded()
    {
        // Use a raycast or other method to check if the enemy is on the ground
        Debug.DrawRay(transform.position, Vector3.down * 4.0f, Color.red, 0.1f);
        return Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

    }
    /// <summary>
    /// Gizmo to visualize the ground check sphere in the editor
    /// </summary>
    public void OnDrawGizmosSelected()
    {
        Vector3 center = groundCheck != null ? groundCheck.position : transform.position;
        float radius = Mathf.Max(groundDistance, 0f);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(center, radius);
    }
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Shockwave"))
        {
            pushCooldown = 3;
            isPushed = true;
            agent.enabled = false;
            rb.isKinematic = false;
        }
    }
}
