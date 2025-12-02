using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[DisallowMultipleComponent]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class MacroBoss : EnemyBase
{
    [Header("Macro Settings")]
    [SerializeField] private Transform MicroPosition;
    [SerializeField] private float returnDelay = 6f;
    public Collider damageHitbox;
    private void Awake()
    {
        canAttack = true; // little guy can attack
        canChase = true;
        canPatrol = true;
        damageHitbox.enabled = false;
    }
    // ------------ auto assign references -------------- //
    void OnValidate()
    {
        // 1) Target: find and assign player as target if not assigned
        if (Target == null)
        {
            var player = GameObject.FindWithTag("Player");
            if (player != null) Target = player;
        }

        // 2) Ground check: find and assign ground check transform if not assigned
        if (groundCheck == null)
        {
            var existing = transform.Find("GroundCheck");
            if (existing != null) groundCheck = existing;
           
        }
    }
   
   
    private void OnEnable()
    {
        StartCoroutine(ResumeSequence());
    }
    public IEnumerator ResumeSequence()
    {
        //Resume normal behavior: EnemyBase
        canChase = true;
        canAttack = true;
        canPatrol = true;
        SetGrabbed(false);
        yield return new WaitForSeconds(returnDelay);
        StartCoroutine(ReturnToMicroPosition());
    }
    private IEnumerator ReturnToMicroPosition()
    {
        var wait = new WaitForSeconds(.5f);
        canChase = false; // disable chasing while returning to micro position
        canAttack = false; // disable attacking while returning to micro position
        canPatrol = false; // disable patrolling while returning to micro position
        agent.isStopped = false; // ensure agent is not stopped
        agent.SetDestination(MicroPosition.position);
        while (true)
        {
            // If boss is being pushed/grabbed, pause progress until stable
            if (isPushed || isGrabbed)
            {
                yield return null;
                continue;
            }
            if(agent.remainingDistance <= Mathf.Max(agent.stoppingDistance, .5f))
            {
                Debug.Log("Reached micro position");
               StartCoroutine(ResumeSequence());
                yield break; // exit coroutine
            }
            yield return wait;
        }
    }
  
   
}
