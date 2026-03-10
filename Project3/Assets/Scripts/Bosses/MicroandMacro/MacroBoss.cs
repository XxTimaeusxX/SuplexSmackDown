using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[DisallowMultipleComponent]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class MacroBoss : EnemyBase
{
    [Header("---------------Macro Settings------------------------")]
    [SerializeField] private Transform MicroPosition;
    [SerializeField] private float returnDelay = 6f;
    public Collider damageHitbox;
    public Collider MacrosCollider;
    public bool wasThrown = false;

    [Header("Animation")]
    public Animator WorkerAnimator;
    private string CurrentWorkerAnimation = "";
    public bool IsGrabbedByMicro;
    public bool IsThrownByMicro;
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
            agent.SetDestination(MicroPosition.position);

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

    //---------------- Animation ---------------------------//
    public void ChangeAnimation(string animation, float crossfade = 0.2f)
    {
        if (CurrentWorkerAnimation != animation)
        {
            CurrentWorkerAnimation = animation;
            WorkerAnimator.CrossFade(animation, crossfade);

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
     /*   if (IsThrownByMicro) // checks when thrown by micro
        {
            ChangeAnimation("MacroLaunched");
            return;
        }*/
         if (isGrabbed && !agent.enabled)
        {
           
           ChangeAnimation(IsGrabbedByMicro ? "MacroBall" : "MacroGrabbed");
            return;
        }
     
        //Walk Animation call
        if (agent.enabled && agent.isOnNavMesh && agent.hasPath && agent.remainingDistance > agent.stoppingDistance)
        {
            ChangeAnimation("MacroRun");
            return;
        }

        // Default to idle
        ChangeAnimation("MacroIdle");


    }

}
