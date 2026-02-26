using UnityEngine;
using UnityEngine.AI;
using System.Collections; // Required for Coroutine

public class EnemyAI : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    public LayerMask groundLayer, playerLayer;

    // Patrolling/Chase variables
    public float moveSpeed = 5f;
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;
    public Vector3 walkPoint;
    public bool walkPointSet;
    public float walkPointRange;

    // Attack/Dash variables
    public bool alreadyAttacked;
    public float timeBetweenAttacks;
    public float dashSpeed = 20f;
    public float dashDuration = 0.5f;
    private bool isDashing;

    private void Update()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, playerLayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerLayer);

        if (isDashing) return; // Prevent movement interference while dashing

        if (!playerInSightRange && !playerInAttackRange) Patroling();
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        if (playerInSightRange && playerInAttackRange) AttackPlayer();
    }

    private void Patroling()
    {
        agent.speed = moveSpeed;
        if (!walkPointSet) SearchWalkPoint();
        if (walkPointSet) agent.SetDestination(walkPoint);
        Vector3 distanceToWalkPoint = transform.position - walkPoint;
        if (distanceToWalkPoint.magnitude < 1f) walkPointSet = false;
    }

    private void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);
        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
        if (Physics.Raycast(walkPoint, -transform.up, 2f, groundLayer)) walkPointSet = true;
    }

    private void ChasePlayer()
    {
        agent.speed = moveSpeed;
        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        // Stop moving normally
        agent.SetDestination(transform.position);

        Vector3 targetPosition = new Vector3(player.position.x, transform.position.y, player.position.z);
        transform.LookAt(targetPosition);

        if (!alreadyAttacked)
        {
            // Trigger Dash Coroutine
            StartCoroutine(DashCoroutine(targetPosition));
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private IEnumerator DashCoroutine(Vector3 target)
    {
        isDashing = true;
        agent.enabled = false; // Disable NavMesh for direct control

        float startTime = Time.time;
        Vector3 startPos = transform.position;

        while (Time.time < startTime + dashDuration)
        {
            // Move forward toward target
            transform.position = Vector3.Lerp(startPos, target, (Time.time - startTime) / dashDuration);
            yield return null;
        }

        agent.enabled = true;
        isDashing = false;
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }
}