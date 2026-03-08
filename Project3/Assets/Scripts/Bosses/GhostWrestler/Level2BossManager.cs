using System.Collections;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

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
    public float maxAttacks;
    private float attackCounter = 0;
    public float maxBreakCooldown;
    private float breakCooldownTimer;
    private float suplexTimer;
    public float maxSuplexTimer;

    [Header("References")]
    public Transform player;
    public Transform boss;
    public NavMeshAgent agent;
    public Rigidbody rb;
    public LayerMask groundLayer, playerLayer, bossLayer; // -
    public Collider bossCollider;
    public GameObject body;
    [SerializeField] private PlayerSuplex playerSuplex;
    public Transform groundCheck;
    public Material solidMaterial;
    public Material transparentMaterial;
    public Renderer objectRenderer;
    public TravelToLocation travel;
    public GameObject shockwave;
    public GameObject grabBox;
    public Transform holdPoint;
    public GameObject playerBody;
    public PlayerMovement playerMovement;
    public PlayerDash playerDash;
    public GameObject healthBar;
    public Slider bossHealthSlider;

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
    private bool jumpCooldown;
    private bool breakCooldown;
    public bool finalArea;
    public bool slow;
    public bool grabBoxGrab;
    private bool suplex;

    private void Start()
    {
        grabbed = false;
        stunnedTimer = maxStunnedTimer;
        triggerTimer = maxTriggerTimer;
        grabbedTimer = maxGrabbedTimer;
        jumpTime = maxJumpTime;
        attackCooldown = maxAttackCooldown;
        jumpCooldownTimer = maxJumpCooldownTimer;
        breakCooldownTimer = maxBreakCooldown;
        suplexTimer = maxSuplexTimer;
    }

    private void Update()
    {
        if (travel.moveToLocation == false)
        {
            if (playerSuplex.bossDropped == true)
            {
                grabbed = false;
            }
            TriggerOn();
            agent.speed = moveSpeed;
            playerInSightRange = Physics.CheckSphere(transform.position, sightRange, playerLayer);
            playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerLayer);
            if (isDashing) return;
            if (!breakCooldown)
            {
                States();
            }
            Stunned();
            Grounded();
            HighJump();
            AttackCooldown();
            JumpCooldown();
            AttackBreak();
            SuplexPlayer();
        }
        if (bossHealthSlider.value <= 0)
        {
            healthBar.SetActive(false);
            gameObject.SetActive(false);
        }
    }

    public void AttackBreak()
    {
        if (attackCounter == maxAttacks)
        {
            breakCooldown = true;
        }
        if (breakCooldown)
        {
            breakCooldownTimer -= Time.deltaTime;
        }
        if (breakCooldownTimer <= 0)
        {
            breakCooldown = false;
            breakCooldownTimer = maxBreakCooldown;
        }
    }

    private void Patroling() // -
    {
        if (!stunned)
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
        if (!stunned)
        {
            if (agent.enabled == true)
            {
                agent.SetDestination(player.position);
            }
        }
    }

    private void AttackPlayer()
    {
        if (!stunned)
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
                        attackCounter++;
                    }
                }
            }
        }
    }

    private void GrabPlayer()
    {
        if (finalArea && slow && !grabBoxGrab)
        {
            Debug.Log("Activate");
            grabBox.SetActive(true);
            agent.SetDestination(transform.position);
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            Vector3 targetPosition = transform.position + directionToPlayer * (Vector3.Distance(transform.position, player.position) * dashDistanceMultiplier);
            targetPosition.y = transform.position.y;
            Vector3 targetLookAt = new Vector3(player.position.x, transform.position.y, player.position.z);
            transform.LookAt(targetLookAt);
            if (!alreadyAttacked)
            {
                StartCoroutine(DashCoroutine(targetPosition));
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
            yield return new WaitForSeconds(0.5f);
            GrabPlayer();
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
            if (playerInSightRange && playerInAttackRange && !grabbed && !alreadyAttacked && !jump && !grabBoxGrab && travel.moveToLocation == false)
            {
                int randomAttackIndex = Random.Range(0, 6);
                switch (randomAttackIndex)
                {
                    case 0:
                        AttackPlayer();
                        break;
                    case 1:
                        Jump();
                        break;
                    case 2:
                        AttackPlayer();
                        break;
                    case 3:
                        AttackPlayer();
                        break;
                    case 4:
                        AttackPlayer();
                        break;
                    case 5:
                        AttackPlayer();
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
        if (!stunned)
        {
            if (jump)
            {
                agent.enabled = false;
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                jumpTime -= Time.deltaTime;
            }
            if (jumpTime <= 0)
            {
                jump = false;
                if (isGrounded)
                {
                    Instantiate(shockwave, boss.position, boss.rotation, boss);
                    jumpTime = maxJumpTime;
                    attackCounter++;
                    jumpCooldown = true;
                }
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
        objectRenderer.material = solidMaterial;
    }

    public void TurnTransparent()
    {
        objectRenderer.material = transparentMaterial;
    }

    public void SuplexPlayer()
    {
        if (grabBoxGrab)
        {
            playerMovement.enabled = false;
            playerDash.enabled = false;
            playerBody.transform.SetParent(holdPoint);
            playerBody.transform.localPosition = Vector3.zero;
            suplex = true;
        }
        if (suplex)
        {
            agent.enabled = false;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            suplexTimer -= Time.deltaTime;
        }
        if (suplexTimer <= 0)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, -fallSpeed, rb.linearVelocity.z);
            suplex = false;
            if (isGrounded)
            {
                jumpCooldown = true;
                playerMovement.enabled = true;
                playerDash.enabled = true;
                playerBody.transform.SetParent(null);
                grabBoxGrab = false;
                suplexTimer = maxSuplexTimer;
                Instantiate(shockwave, boss.position, boss.rotation, boss);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Shockwave"))
        {
            if (movingBoss)
            {
                travel.moveToLocation = true;
                bossHealthSlider.value -= 1;
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

    void OnCollisionStay(Collision collisionInfo)
    {
        if ((groundLayer.value & (1 << collisionInfo.gameObject.layer)) > 0)
        {
            isGrounded = true;
        }
    }

    void OnCollisionExit(Collision collisionInfo)
    {
        if ((groundLayer.value & (1 << collisionInfo.gameObject.layer)) > 0)
        {
            isGrounded = false;
        }
    }
}
