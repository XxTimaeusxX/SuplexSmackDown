using System.Collections;
using UnityEngine;
using UnityEngine.AI;

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
public class MacroBoss : EnemyBase
{
    [Header("---------------Macro Settings------------------------")]

    [Header("Boss Return settings")]
    [SerializeField] private MicroBoss MicroBossScript;
    [SerializeField] private Transform MicroPosition;
    [SerializeField] private float returnDelay = 6f;
    public Collider damageHitbox;
    public Collider MacrosCollider;
    public bool wasThrown = false;
    public bool IsReturningToMicro = false;
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
   public override void Update()
    {
       base.Update();
           if(IsThrownByMicro && !isGrabbed && IsEnemyGrounded())
        {
           IsThrownByMicro = false;
         
            ResumeSequence(); 
            if (CompareTag("DamagePlayer"))
            {
                this.gameObject.tag = "Macro";
            }
            return; 
        }
        
        if (returnDelay >0)
        {
            // If boss is being pushed/grabbed, pause progress until stable
            if (isPushed || isGrabbed)
            {
                CheckAnimation();
                return;
      
            }
            if (!isGrabbed && !isPushed)
            {
                returnDelay -= Time.deltaTime;
                if (returnDelay < 0)
                {
                    returnDelay = 0;
                    StartCoroutine(ReturnToMicroPosition());
                }
            }
            
        }
        CheckAnimation();
    }

    public void ResumeSequence()
    {
        //Resume normal behavior: EnemyBases
        MacrosCollider.enabled = true;
        agent.enabled = true;
        canChase = true;
        canAttack = true;
        canPatrol = true;
        SetGrabbed(false);
        returnDelay = 6f;
       
    }
    private IEnumerator ReturnToMicroPosition()
    {
        MicroBossScript.GrabCollider.enabled =true; // disable micro's grab collider while macro is returning to micro position
        Debug.Log("Returning to micro position");
        AudioManager.PlayMacroRetreatTwo();
        var wait = new WaitForSeconds(.5f);
        MacrosCollider.enabled = false; // disable macro collider while returning to micro position
        canChase = false; // disable chasing while returning to micro position
        canAttack = false; // disable attacking while returning to micro position
        canPatrol = false; // disable patrolling while returning to micro position
        agent.isStopped = false; // ensure agent is not stopped
        agent.enabled = true; // ensure agent is enabled
   
     
        while (true)
        {
            CurrentMacroState = MacroState.Returning;
            agent.SetDestination(MicroPosition.position);
           agent.speed = 20f; // increase speed while returning to micro position
            if (agent.pathPending)
            {
                yield return null;
                continue;
            }
          

            if (agent.remainingDistance <= Mathf.Max(agent.stoppingDistance, 5f))// get near the destination but not exactly on it
            {
              Debug.Log("Reached micro position");
                MacrosCollider.enabled = true; // re-enable macro collider so it can be grabbed again
                damageHitbox.enabled = false; // disable damage hitbox
     
                yield break; // exit coroutine
            }
            yield return null;
        }
    }
    public void LaunchToTarget(Transform targetPlayer)
    {
        Debug.Log("Launching Macro towards target");
        Vector3 TargetPos = targetPlayer.position - transform.position;
        float gravity = Physics.gravity.magnitude;
        float horizontalDist = new Vector3(TargetPos.x, 0f, TargetPos.z).magnitude;
        float verticalDist = TargetPos.y;

        float flightTime = Mathf.Clamp(horizontalDist / throwTimeDistanceDivisor, throwMinFlightTime, throwMaxFlightTime);
        float velocityY = (verticalDist + 0.5f * gravity * flightTime * flightTime) / flightTime;

        Vector3 horizontalDir = new Vector3(TargetPos.x, 0f, TargetPos.z).normalized;
        float velocityXZ = horizontalDist / flightTime;

        Vector3 launchVelocity = (horizontalDir * velocityXZ) + (Vector3.up * velocityY);

        rb.linearVelocity = Vector3.zero;
        rb.AddForce(launchVelocity, ForceMode.VelocityChange);
        StartCoroutine(DiveOnDescent());
    }
    private IEnumerator DiveOnDescent()
    {
        while (!wasGrounded)
        {
            if (rb.linearVelocity.y < 0f)
            {
                float baseSpeed = throwTimeDistanceDivisor / Mathf.Max(throwMinFlightTime, throwMaxFlightTime);
                float diveSpeed = baseSpeed * diveDownMultiplier;

                float newY = -diveSpeed;
                Vector3 horizontal = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
                Vector3 toTarget = Target.transform.position - transform.position;
                toTarget.y = 0f;

                if (toTarget.sqrMagnitude > 0.01f)
                {
                    Vector3 desiredHorizontal = toTarget.normalized * diveSpeed;
                    horizontal = Vector3.MoveTowards(horizontal, desiredHorizontal, diveSpeed * Time.deltaTime);
                }

                rb.linearVelocity = new Vector3(horizontal.x, newY, horizontal.z);
            }

            yield return null;
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
