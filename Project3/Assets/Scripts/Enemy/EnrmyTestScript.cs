using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnrmyTestScript : MonoBehaviour
{
    public GameObject Target;
    private NavMeshAgent m_EnemyAgent;
    Rigidbody rb;

    public Transform groundCheck;
    public LayerMask groundMask;
    public float groundDistance;

    public float chaseRange;
    private float m_Distance;
    bool isGrounded;
    bool isPushed;
    public bool isGrabbed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_EnemyAgent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        isPushed = false;
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && !isGrabbed && !isPushed)
        {
            // Debug.Log("Enemy just landed!");
            rb.isKinematic = true;
            m_EnemyAgent.enabled = true;
        }

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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Shockwave"))
        {
            isPushed = true;
            m_EnemyAgent.enabled = false;
            rb.isKinematic = false;
        }
    }
}