using System.Collections;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class Level2BossManager : MonoBehaviour
{
    [Header("Settings")]
    public float moveSpeed;
    public float timeBetweenAttacks;
    public float walkPointRange;
    public float sightRange;
    public float attackRange;
    public Vector3 walkPoint;
    public float dashSpeed;
    public float dashDuration;
    public float dashDistanceMultiplier;
    private float stunnedTimer;
    public float maxStunnedTimer;

    [Header("References")]
    public Transform player;
    public NavMeshAgent agent;
    public Rigidbody rb;
    public LayerMask groundLayer, playerLayer, bossLayer;
    public Collider bossCollider;
    public GameObject body;

    [Header("Bools")]
    public bool alreadyAttacked;
    public bool walkPointSet;
    public bool playerInSightRange;
    public bool playerInAttackRange;
    public bool isDashing;
    public bool triggerOn;
    public bool stunned;

    private void Start()
    {
        stunnedTimer = maxStunnedTimer;
    }

    private void Update()
    {
        if (triggerOn)
        {
            bossCollider.isTrigger = true;
        }
        if (!triggerOn)
        {
            bossCollider.isTrigger = false;
        }
        agent.speed = moveSpeed;
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, playerLayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerLayer);
        if (isDashing) return;
        if (!playerInSightRange && !playerInAttackRange)
        {
            Patroling();
        }
        if (playerInSightRange && !playerInAttackRange)
        {
            ChasePlayer();
        }
        if (playerInSightRange && playerInAttackRange)
        {
            AttackPlayer();
        }
        Stunned();
    }

    private void Patroling()
    {
        if (!walkPointSet)
        {
            SearchWalkPoint();
        }
        if (walkPointSet)
        {
            agent.SetDestination(walkPoint);
        }
        Vector3 distanceToWalkPoint = transform.position - walkPoint;
        if (distanceToWalkPoint.magnitude < 1f)
        {
            walkPointSet = false;
        }
    }

    private void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);
        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
        if (Physics.Raycast(walkPoint, -transform.up, 2f, groundLayer))
        {
            walkPointSet = true;
        }
    }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        agent.SetDestination(transform.position);
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        Vector3 targetPosition = transform.position + directionToPlayer * (Vector3.Distance(transform.position, player.position) * dashDistanceMultiplier);
        targetPosition.y = transform.position.y;
        Vector3 targetLookAt = new Vector3(player.position.x, transform.position.y, player.position.z);
        transform.LookAt(targetLookAt);
        if (!alreadyAttacked)
        {
            StartCoroutine(DashCoroutine(targetPosition));
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private IEnumerator DashCoroutine(Vector3 target)
    {
        isDashing = true;
        agent.enabled = false;
        triggerOn = true;
        float startTime = Time.time;
        Vector3 startPos = transform.position;
        while (Time.time < startTime + dashDuration)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, dashSpeed * Time.deltaTime);
            yield return null;
        }
        agent.enabled = true;
        isDashing = false;
        triggerOn = false;
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    private void Stunned()
    {
        if (stunned)
        {
            agent.enabled = false;
            stunnedTimer -= Time.deltaTime;
            body.tag = "Solid";
        }  
        if (stunnedTimer <= 0)
        {
            stunned = false;
            agent.enabled = true;
            body.tag = "Boss";
            stunnedTimer = maxStunnedTimer;
        }
    }
}
