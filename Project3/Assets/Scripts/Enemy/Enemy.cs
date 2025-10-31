using System.Collections;
using UnityEngine;
using UnityEngine.AI;
public class Enemy : MonoBehaviour
{
    public GameObject Target;
    private NavMeshAgent m_EnemyAgent;
    Rigidbody rb;

    public Transform groundCheck;
    public LayerMask groundMask;
    public float groundDistance;

    
    private float m_Distance;
    private bool wasGrounded = false;
    public bool isGrabbed;
    bool isPushed = false;
    public float pushCooldown;

    [Header("Patrol Settings")]
    public float chaseRange;
    public float patrolWalkSpeed;
    public float patrolWaitTime;
    public float patrolRunSpeed;
    private float patrolWaitDefault;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    GameManager gameManager;
    void Start()
    {
       
        m_EnemyAgent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
       
    }

    // Update is called once per frame
    void Update()
    {
        bool grounded = IsEnemyGrounded();
        if (isPushed)
        {
            pushCooldown -= Time.deltaTime;
        }
        if (pushCooldown < 0)
        {
            pushCooldown = 0;
            isPushed = false;
            m_EnemyAgent.enabled = true;
            rb.isKinematic = true;
        }
       
        if (grounded && wasGrounded && !isGrabbed && !isPushed)
        {
            // Debug.Log("Enemy just landed!");
           rb.isKinematic = true;
            m_EnemyAgent.enabled = true;
        } 
        wasGrounded = grounded;
        if (m_EnemyAgent.enabled && m_EnemyAgent.isOnNavMesh)
        {
            ChasePlayer();

        }
    }
    public void  RandomPatrolDestination()
    {
        if (!m_EnemyAgent.enabled || !m_EnemyAgent.isOnNavMesh) return;

        // Pick points around current floor height (not y=0) to stay on the same NavMesh island
        const float patrolRadius = 20f;
        const int maxTries = 6;
        Vector3 origin = transform.position;

        for (int i = 0; i < maxTries; i++)
        {
            Vector2 r = Random.insideUnitCircle * patrolRadius;
            Vector3 candidate = new Vector3(origin.x + r.x, origin.y, origin.z + r.y);

            if (NavMesh.SamplePosition(candidate, out var hit, 2f, m_EnemyAgent.areaMask))
            {
                var path = new NavMeshPath();
                if (m_EnemyAgent.CalculatePath(hit.position, path) && path.status == NavMeshPathStatus.PathComplete)
                {
                    m_EnemyAgent.isStopped = false;
                    m_EnemyAgent.speed = patrolWalkSpeed;
                    m_EnemyAgent.destination = hit.position;
                    return;
                }
            }
        }

    }
    public void ChasePlayer()
    {
        m_Distance = Vector3.Distance(Target.transform.position, transform.position);
        float arrivalThreshold = Mathf.Max(0.5f, m_EnemyAgent.stoppingDistance);
        if (m_EnemyAgent.isOnNavMesh)
        {
            if (m_Distance <= chaseRange)
            {
                patrolWaitDefault = 0f;
                m_EnemyAgent.isStopped = false;
                m_EnemyAgent.speed = patrolRunSpeed; // set chase speed
                m_EnemyAgent.destination = Target.transform.position;
            }
            // Out of chase range -> patrol
            if (patrolWaitDefault > 0f)
            {
                // Idle thinking
                m_EnemyAgent.isStopped = true;
                patrolWaitDefault -= Time.deltaTime;
                if (patrolWaitDefault <= 0f)
                {
                    m_EnemyAgent.isStopped = false;
                    RandomPatrolDestination();
                }
            }
            else  if(!m_EnemyAgent.hasPath || m_EnemyAgent.remainingDistance <= arrivalThreshold) RandomPatrolDestination();   
        }
    }
    public void SetGrabbed(bool grabbed)
    {
        isGrabbed = grabbed;
        if (grabbed)
        {
            // Optionally disable agent here if needed
            m_EnemyAgent.enabled = false;
        }
        
    }
    bool IsEnemyGrounded()
    {
        // Use a raycast or other method to check if the enemy is on the ground
        Debug.DrawRay(transform.position, Vector3.down * 4.0f, Color.red, 0.1f);
        return Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

    }
    /// <summary>
    /// Gizmo to visualize the ground check sphere in the editor
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Vector3 center = groundCheck != null ? groundCheck.position : transform.position;
        float radius = Mathf.Max(groundDistance, 0f);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(center, radius);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Shockwave"))
        {
            pushCooldown = 3;
            isPushed = true;
            m_EnemyAgent.enabled = false;
            rb.isKinematic = false;
        }
    }
}
