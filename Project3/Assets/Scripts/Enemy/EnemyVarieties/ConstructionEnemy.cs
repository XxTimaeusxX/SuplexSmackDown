using UnityEngine;
using UnityEngine.AI;

public class ConstructionEnemy : EnemyBase
{
 //   private bool _WorkerInChaseRange = false;
    // Edge trigger to play damage sound once when push starts
  //  private bool _wasPushed = false;
    [Header("Animation")]
    public Animator WorkerAnimator;
    private string CurrentWorkerAnimation = "";
    void OnValidate()
    {
        // 1) Target: assign Player by tag if not set
        if (Target == null)
        {
            var player = GameObject.FindWithTag("Player");
            if (player != null) Target = player;

        }
        // 2) Core components on this GameObject
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        if (rb == null) rb = GetComponent<Rigidbody>();
        // Optional: ensure sane defaults (won’t override if already configured)
        if (agent != null)
        {
            if (agent.stoppingDistance < 0.5f) agent.stoppingDistance = 0.75f;
            if (agent.radius < 0.1f) agent.radius = 0.3f;
            if (agent.acceleration < 8f) agent.acceleration = 12f;
        }
        // 3) Ground check: find child named "GroundCheck"
        if (groundCheck == null)
        {
            var existing = transform.Find("GroundCheck");
            if (existing != null) groundCheck = existing;
            else Debug.LogWarning($"[{name}] Missing child 'GroundCheck'. Create one or assign 'groundCheck'.", this);
        }
        // Prefer a small ground distance if unset
        if (groundDistance <= 0f) groundDistance = 0.2f;

        // 4) Hitbox slapbox: find child trigger named "AttackHitBox"
        if (slapbox == null)
        {
            var hitbox = transform.Find("SlapHitBox");
            if (hitbox != null)
            {
                var col = hitbox.GetComponentInChildren<GameObject>();
                if (col != null) slapbox = col;
                else Debug.LogWarning($"[{name}] 'SlapHitBox' found but has no Collider.", this);
            }
        }
        // 5) UI: try find in children (optional)
        if (chargeSlider == null)
        {
            chargeSlider = GetComponentInChildren<UnityEngine.UI.Slider>(includeInactive: true);
         
        }
        // 6) Ground mask: if unset, try to infer "Ground" layer
        if (groundMask.value == 0)
        {
            int groundLayer = LayerMask.NameToLayer("Ground");
            if (groundLayer >= 0) groundMask = 1 << groundLayer;
            else Debug.LogWarning($"[{name}] Layer 'Ground' not found. Set 'groundMask' in Inspector.", this);
        }
    }
    public override void Update()
    {
        base.Update();  // Let base class handle everything
        CheckAnimation();
    }

    /*   public override void ChasePlayer()
       {
           // Call base implementation (handles all pathfinding)
           base.ChasePlayer();

           // ONLY Construction-specific: Audio edge trigger
           if (Target != null)
           {
               bool inChaseRange = m_Distance <= chaseRange;

               if (inChaseRange && !_WorkerInChaseRange)
               {
                   AudioManager.PlayConstructionSeenOne();
               }

               _WorkerInChaseRange = inChaseRange;
           }
           else
           {
               _WorkerInChaseRange = false;
           }
       }*/
 
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

        // Check if moving (has a path and is actively navigating)
        // call grab animation if grabbed
        if (isGrabbed)
        {
            ChangeAnimation("WorkerGrabbed");
            return;
        }
        //Walk Animation call
        if (agent.enabled && agent.isOnNavMesh && agent.hasPath && agent.remainingDistance > agent.stoppingDistance)
        {
            ChangeAnimation("WorkerWalk");
            return;
        }

        // Default to idle
        ChangeAnimation("WorkerIdle");

       
    }
    private void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);
        if (collision.gameObject.tag == "Shockwave")
        {
            AudioManager.PlayConstructionFalling();
        }
    }
}
