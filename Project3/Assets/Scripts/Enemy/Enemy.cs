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

    public float chaseRange;
    private float m_Distance;
    private bool wasGrounded = false;
    public bool isGrabbed;
    bool isPushed = false;
    public float pushCooldown;
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

    public void ChasePlayer()
    {
        m_Distance = Vector3.Distance(Target.transform.position, transform.position);
        if (m_EnemyAgent.isOnNavMesh)
        {
            if (m_Distance <= chaseRange)
            {
                m_EnemyAgent.isStopped = false;
                m_EnemyAgent.destination = Target.transform.position;
            }
            else
            {
                m_EnemyAgent.isStopped = true;
            }
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
