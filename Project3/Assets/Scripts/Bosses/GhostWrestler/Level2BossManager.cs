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
    private Vector3 walkPoint;
    public float dashSpeed;
    public float dashDuration;
    public float dashDistanceMultiplier;
    private float stunnedTimer;
    public float maxStunnedTimer;
    public float groundDistance;
    private float triggerTimer;
    public float maxTriggerTimer;
    public float fallSpeed;
    private float grabbedTimer;
    public float maxGrabbedTimer;
    public float jumpForce;
    private float jumpTime;
    public float maxJumpTime;
    private float attackCooldown;
    public float maxAttackCooldown;
    private float jumpCooldownTimer;
    public float maxJumpCooldownTimer;

    [Header("References")]
    public Transform player;
    public Transform boss;
    public NavMeshAgent agent;
    public Rigidbody rb;
    public LayerMask groundLayer, playerLayer, bossLayer;
    public Collider bossCollider;
    public GameObject body;
    [SerializeField] private PlayerSuplex playerSuplex;
    public Transform groundCheck;
    public LayerMask groundMask;
    public Material opaqueMaterial;
    public Material transparentMaterial;
    public Renderer objectRenderer;
    public TravelToLocation travel;
    public GameObject shockwave;

    [Header("Bools")]
    private bool alreadyAttacked;
    private bool walkPointSet;
    private bool playerInSightRange;
    private bool playerInAttackRange;
    public bool isDashing;
    private bool triggerOn;
    public bool stunned;
    public bool grabbed;
    public bool isGrounded;
    private bool grabbedCooldown;
    private bool jump;
    public bool movingBoss;
    public bool jumpCooldown;

    private void Start()
    {
        grabbed = false;
        stunnedTimer = maxStunnedTimer;
        triggerTimer = maxTriggerTimer;
        grabbedTimer = maxGrabbedTimer;
        jumpTime = maxJumpTime;
        attackCooldown = maxAttackCooldown;
        jumpCooldownTimer = maxJumpCooldownTimer;
    }

    private void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (playerSuplex.bossDropped == true)
        {
            grabbed = false;
        }
        TriggerOn();
        agent.speed = moveSpeed;
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, playerLayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerLayer);
        if (isDashing) return;
        States();
        Stunned();
        Grounded();
        HighJump();
        AttackCooldown();
        JumpCooldown();

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
        if (agent.enabled == true)
        {
            agent.SetDestination(player.position);
        }
    }

    private void AttackPlayer()
    {
        if (!alreadyAttacked)
        {
            if (agent.enabled == true)
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
                }
            }
        }
    }

    private IEnumerator DashCoroutine(Vector3 target)
    {
        if (!grabbedCooldown)
        {
            isDashing = true;
            agent.enabled = false;
            triggerOn = true;
            float startTime = Time.time;
            Vector3 startPos = transform.position;
            rb.constraints = RigidbodyConstraints.FreezePosition;
            while (Time.time < startTime + dashDuration)
            {
                transform.position = Vector3.MoveTowards(transform.position, target, dashSpeed * Time.deltaTime);
                yield return null;
            }
            rb.constraints = ~RigidbodyConstraints.FreezePosition;
            agent.enabled = true;
            isDashing = false;
        }
    }

    private void AttackCooldown()
    {
        if (alreadyAttacked)
        {
            attackCooldown -= Time.deltaTime;
        }
        if (attackCooldown <= 0)
        {
            alreadyAttacked = false;
            attackCooldown = maxAttackCooldown;
        }
    }

    private void Stunned()
    {
        if (stunned)
        {
            agent.enabled = false;
            stunnedTimer -= Time.deltaTime;
            body.tag = "Solid";
            TurnSolid();
        }  
        if (!grabbed && !grabbedCooldown)
        {
            if (stunnedTimer <= 0)
            {
                stunned = false;
                agent.enabled = true;
                body.tag = "Boss";
                stunnedTimer = maxStunnedTimer;
                TurnTransparent();
            }
        }
    }

    public void Grounded()
    {
        if (grabbed && stunnedTimer != maxStunnedTimer)
        {
            grabbedCooldown = true;
        }
        if (grabbedCooldown)
        {
            grabbedTimer -= Time.deltaTime;
        }
        if (grabbedTimer <= 0)
        {
            grabbedCooldown = false;
            grabbedTimer = maxGrabbedTimer;
        }
        if (isGrounded && !grabbed && !grabbedCooldown)
        {
            agent.enabled = true;
        }
        if (!isGrounded && !grabbed && travel.moveToLocation == false)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, -fallSpeed, rb.linearVelocity.z);
        }
    }

    public void TriggerOn()
    {
        if (triggerOn)
        {
            bossCollider.isTrigger = true;
            triggerTimer -= Time.deltaTime;
        }
        if (triggerTimer <= 0)
        {
            triggerOn = false;
            triggerTimer = maxTriggerTimer;
        }
        if (!triggerOn && travel.moveToLocation == false)
        {
            bossCollider.isTrigger = false;
        }
    }

    public void States()
    {
        if (!jumpCooldown)
        {
            if (!playerInSightRange && !playerInAttackRange && !grabbed && !jump && travel.moveToLocation == false)
            {
                Patroling();
            }
            if (playerInSightRange && !playerInAttackRange && !grabbed && !jump && travel.moveToLocation == false)
            {
                ChasePlayer();
            }
            if (playerInSightRange && playerInAttackRange && !grabbed && !alreadyAttacked && !jump && travel.moveToLocation == false)
            {
                int randomAttackIndex = Random.Range(0, 2);
                switch (randomAttackIndex)
                {
                    case 0:
                        AttackPlayer();
                        break;
                    case 1:
                        Jump();
                        break;
                }
            }
        }
    }

    private void Jump()
    {
        if (!alreadyAttacked)
        {
            jump = true;
            alreadyAttacked = true;
        }
    }

    public void HighJump()
    {
        if (jump)
        {
            agent.enabled = false;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isGrounded = false;
            jumpTime -= Time.deltaTime;
        }
        if (jumpTime <= 0)
        {
            jump = false;
            if (isGrounded)
            {
                Instantiate(shockwave, boss.position, boss.rotation, boss);
                jumpTime = maxJumpTime;
                jumpCooldown = true;
            }
        }
    }

    private void JumpCooldown()
    {
        if (jumpCooldown)
        {
            jumpCooldownTimer -= Time.deltaTime;
        }
        if (jumpCooldownTimer <= 0)
        {
            jumpCooldown = false;
            jumpCooldownTimer = maxJumpCooldownTimer;
        }
    }

    public void TurnSolid()
    {
        objectRenderer.material = opaqueMaterial;
    }

    public void TurnTransparent()
    {
        objectRenderer.material = transparentMaterial;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Shockwave"))
        {
            if (movingBoss)
            {
                travel.moveToLocation = true;
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Flower"))
        {
            if(isDashing)
            {
                stunned = true;
                Debug.Log("Stunned");
            }
        }
    }
}
