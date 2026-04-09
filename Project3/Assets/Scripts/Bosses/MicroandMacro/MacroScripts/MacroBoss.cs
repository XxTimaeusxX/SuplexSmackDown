using System;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public enum MacroState
{
    Idle,
    Chasing,
    Attacking,
    Grabbed,
    Thrown,
    Returning
}

[DisallowMultipleComponent]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]

// MARK - Currently broken
public class MacroBoss : MonoBehaviour, ICarriable
{
    [Header("References")]
    public NavMeshAgent agent;
    public Rigidbody Rigidbody => rb;
    private Rigidbody rb;
    [SerializeField] private Animator animator;
    public MicroBoss microBoss;
    private Coroutine enableAgentRoutine;

    private Transform originalParent;
    private RigidbodyConstraints originalConstraints;
    private GroundChecker groundChecker;

    [SerializeField] protected CarryWeightProfile carryWeightProfile;
    public CarryWeightProfile CarryWeightProfile => carryWeightProfile;

    public GameObject RespawnPoint;

    [Header("Combat")]
    public float attackRange = 2f;
    public float attackCooldown = 0.8f;   // Time between attacks
    public float _nextAttackTime = 0f;
    public float distanceToTarget;
    public GameObject damageHitbox;          // child trigger collider with AttackHitBox
    public GameObject slapHitbox;
    public float slapActiveTime = 0.2f;
    public float knockOutTime = 5f; // Time the enemy stays knocked out
    public float defaultSpeed;

    [Header("Carrying")]
    private Collider mainCollider;
    public Collider carryProxy;

    [Header("Bools")]
    public bool wasThrown = false;
    private bool isReturning = false;
    public bool isChasing = false;
    public bool invulnerable = false;
    public bool isGrabbed = false;

    [Header("Throw Launch Settings")]
    [SerializeField] private float throwMinFlightTime = 0.6f;
    [SerializeField] private float throwMaxFlightTime = 2f;
    [SerializeField] private float throwTimeDistanceDivisor = 10f;
    [SerializeField] private float diveDownMultiplier = 1.5f;
    
    [Header("Animation")]
    public Animator MacroAnimator;
    private string CurrentMacroAnimation = "";
    public bool IsGrabbedByMicro;
    public bool IsThrownByMicro;
    public MacroState CurrentMacroState;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        mainCollider = GetComponent<CapsuleCollider>();
        groundChecker = GetComponentInChildren<GroundChecker>();

        originalConstraints = rb.constraints;
        damageHitbox.SetActive(false);
        slapHitbox.SetActive(false);
        isChasing = true;

        defaultSpeed = agent.speed;
    }

    public void Attack()
    {
        if (_nextAttackTime < attackCooldown)
        {
            _nextAttackTime += Time.deltaTime;
            // Debug.Log($"charge: {_nextAttackTime:F2}/{attackCooldown:F2}");
            return;
        }

        //Debug.Log($"[{name}] Melee attack!");

        animator.SetTrigger("EnemySlap");
        AudioManager.PlayEnemySlap();
        _nextAttackTime = 0f;
        StartCoroutine(SlapAttackDuration());
    }
    public IEnumerator SlapAttackDuration()
    {
        yield return new WaitForSeconds(.5f); // wait a frame to sync with animation
        slapHitbox.SetActive(true);
        yield return new WaitForSeconds(slapActiveTime);
        slapHitbox.SetActive(false);
    }
    public void Update()
    {
        //Debug.Log($"MacroBoss Update - throwingMacro: {microBoss.throwingMacro}, agent enabled: {agent.enabled}");
        if (!microBoss.throwingMacro && isChasing && agent.enabled)
        {
            Debug.Log("Chasing player");
            ChasePlayer();
        }
        if (microBoss.throwingMacro && !wasThrown && agent.enabled && !isReturning)
        {
            isReturning = true;
            isChasing = false;
            StartCoroutine(ReturnToMicroPosition());
        }

        if (Vector3.Distance(microBoss.transform.position, transform.position) > 100)
        {
            transform.position = RespawnPoint.transform.position;
        }
    }
    public void ChasePlayer()
    {

        distanceToTarget = Vector3.Distance(microBoss.PLAYER.transform.position, transform.position);
        float arrivalThreshold = Mathf.Max(0.5f, agent.stoppingDistance);

        agent.destination = microBoss.PLAYER.transform.position;

        if (distanceToTarget < attackRange)  // When within melee range -> Face -> Attack
        {
            agent.isStopped = true;
            FaceTarget();
            WaitForSeconds wait = new WaitForSeconds(0.5f);
            Attack();
        }
        else
        {
            if (agent.isStopped)
                agent.isStopped = false;

            agent.destination = microBoss.PLAYER.transform.position;
            _nextAttackTime = 0f;
        }
    }

    private IEnumerator ReturnToMicroPosition()
    {
        Debug.Log("Returning to micro position");
        AudioManager.PlayMacroRetreatTwo();

        while (true)
        {
            CurrentMacroState = MacroState.Returning;
            agent.SetDestination(microBoss.transform.position);
            agent.speed = 20f; // increase speed while returning to micro position
            if (agent.pathPending)
            {
                yield return null;
                continue;
            }
          

            if (agent.remainingDistance <= Mathf.Max(agent.stoppingDistance, 5f))// get near the destination but not exactly on it
            {
                Debug.Log("Reached micro position");
                damageHitbox.SetActive(false); // disable damage hitbox
                slapHitbox.SetActive(false); // disable slap hitbox
                break; // exit coroutine
            }
            yield return null;
        }
        isReturning = false;
        agent.speed = defaultSpeed;
        yield return new WaitUntil(() => wasThrown);
    }
    public void FaceTarget()
    {
        var TurnToTarget = agent.steeringTarget;
        Vector3 direction = (TurnToTarget - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    public void EnterCarriedState(Transform carryPoint)
    {
        //Debug.Log("Enemy picked up");

        originalParent = transform.parent;  // Store original parent

        if (enableAgentRoutine != null)
        {
            StopCoroutine(enableAgentRoutine); // Stop any pending re-enabling of the agent if we're picked up again mid-air
            enableAgentRoutine = null;
        }


        // Disable physics
        rb.isKinematic = true;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        //mainCollider.enabled = false;   // Disable colliders that should not interact

        // Disable AI/NavMeshAgent
        agent.enabled = false;

        if (carryProxy != null) carryProxy.enabled = true;  // Enable proxy collider (prevents clipping)

        // Parent to carry point
        transform.SetParent(carryPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        isGrabbed = true;
        //Debug.Log("EnterCarriedState called on " + gameObject.name);
    }

    public void ExitCarriedState(Vector3 throwForce)
    {

        transform.SetParent(originalParent);    // Unparent

        rb.isKinematic = false;     // Re-enable physics

        rb.constraints = RigidbodyConstraints.None;

        //mainCollider.enabled = true;    //  Re-enable colliders

        if (carryProxy != null) carryProxy.enabled = false;     //  Disable proxy collider

        if (throwForce != Vector3.zero)
            rb.AddForce(throwForce, ForceMode.Impulse);    // Apply throw force

        enableAgentRoutine = StartCoroutine(EnableAgentAfterLanding());

        isGrabbed = false;
        //Debug.Log("ExitCarriedState called on " + gameObject.name);
        //Debug.Log($"Main collider enabled: {mainCollider.enabled}, Proxy: {carryProxy.enabled}");
    }

    private IEnumerator EnableAgentAfterLanding()
    {
        yield return new WaitUntil(()=> groundChecker.IsGrounded() == true);
        damageHitbox.SetActive(false); // Ensure hitbox is off while in the air
        if (!invulnerable)
            tag = "canGrab";
        yield return new WaitForSeconds(knockOutTime); // Small delay to ensure physics has settled after landing
        tag = "Macro";
        rb.constraints = originalConstraints;
        agent.enabled = true;
        enableAgentRoutine = null;
        isChasing = true;
        ChasePlayer();
        //Debug.Log($"{microBoss.throwingMacro} {agent.enabled}");
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Shockwave"))
        {
            tag = "Macro";
            invulnerable = true;
        }
    }
    //---------------- Animation ---------------------------//
    public void ChangeAnimation(string animation, float crossfade = 0.2f)
    {
        if (CurrentMacroAnimation != animation)
        {
            CurrentMacroAnimation = animation;
            MacroAnimator.CrossFade(animation, crossfade);

        }
    }
    private void CheckAnimation()
    {
        // Attack animation call
        /* if (_nextAttackTime > 0f && _nextAttackTime < attackCooldown)
         {
             ChangeAnimation("");
             return;
         }*/

        // Checks if its currently grabbed by Micro or Player
        // call grab animation if grabbed
        if (IsThrownByMicro) // checks when thrown by micro
        {
            CurrentMacroState = MacroState.Thrown;
            ChangeAnimation("MacroLaunched");
            return;
        }
         if (isGrabbed && !agent.enabled)
        {
            if (!IsGrabbedByMicro)
            {
                CurrentMacroState = MacroState.Grabbed;
            }
            ChangeAnimation(IsGrabbedByMicro ? "MacroBall" : "MacroGrabbed");
            return;
        }
     
        //Walk Animation call
        if (agent.enabled && agent.isOnNavMesh && agent.hasPath && agent.remainingDistance > agent.stoppingDistance)
        {
            // Only set to Chasing if we aren't returning
            if (CurrentMacroState != MacroState.Returning)
            {
                CurrentMacroState = MacroState.Chasing;
            }
            ChangeAnimation("MacroRun");
            return;
        }

        // Default to idle
        ChangeAnimation("MacroIdle");
        if (CurrentMacroState != MacroState.Returning)
        {
            CurrentMacroState = MacroState.Idle;
        }

    }

}
