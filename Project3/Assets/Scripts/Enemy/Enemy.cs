using UnityEngine;
using UnityEngine.AI;
public class Enemy : MonoBehaviour
{
    public GameObject Target;
    private NavMeshAgent m_EnemyAgent;

    public Transform groundCheck;
    public LayerMask groundMask;
    public float groundDistance;

    public float chaseRange;
    private float m_Distance;
    private bool wasGrounded = false;
    public bool isGrabbed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_EnemyAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        bool grounded = IsEnemyGrounded();
       
        var rb = GetComponent<Rigidbody>();
        if (grounded && wasGrounded && !isGrabbed)
        {
            Debug.Log("Enemy just landed!");
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
}
