using UnityEngine;
using UnityEngine.AI;
[DisallowMultipleComponent]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class OfficeEnemy : EnemyBase
{
    private bool _OfficeShoalWasInChaseRange = false;
    // Edge trigger to play damage sound once when push starts
    private bool _wasPushed = false;
    [Header("OfficeShoal")]
    public bool IsKillable = false; // Set false for invulnerable variants   
    [Header("Animation")]
    public Animator ShoalAnimator;
    private string CurrentShoalAnimation = "";
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
            // It may find enemyHealth slider; that’s OK—assign explicitly if needed
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

        // ONLY Office-specific: Death behavior when pushed
        if (isPushed && pushCooldown < 0 && IsKillable)
        {
            gameObject.SetActive(false);
        }

        // ONLY Office-specific: Animations
        CheckAnimation();
    }
    
 /*   public override void ChasePlayer()
    {
        // Call base implementation (handles all pathfinding)
        base.ChasePlayer();
        
        // ONLY Office-specific: Audio edge trigger
      /*  if (Target != null)
        {
            bool inChaseRange = m_Distance <= chaseRange;
            
            if (inChaseRange && !_OfficeShoalWasInChaseRange)
            {
                AudioManager.PlayShoalIdle();
            }
            
            _OfficeShoalWasInChaseRange = inChaseRange;
        }
        else
        {
            _OfficeShoalWasInChaseRange = false;
        }
    } */

    private void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);
        if (collision.gameObject.CompareTag("Shockwave"))
        {
            // EnemyBase sets isPushed=true here; play immediately
           
            AudioManager.PlayShoalFalling();
         //   Debug.Log("construction");
        }
    }

    //---------------- Animation ---------------------------//
    public void ChangeAnimation(string animation, float crossfade = 0.2f)
    {
        if (CurrentShoalAnimation != animation)
        {
            CurrentShoalAnimation = animation;
            ShoalAnimator.CrossFade(animation, crossfade);

        }
    }
    private void CheckAnimation()
    {
        // Attack state takes priority
        if (_nextAttackTime > 0f && _nextAttackTime < attackCooldown)
        {
            ChangeAnimation("ShoalSlap");
            return;
        }

        // Check if moving (has a path and is actively navigating)
        if (agent.enabled && agent.isOnNavMesh && agent.hasPath && agent.remainingDistance > agent.stoppingDistance)
        {
            ChangeAnimation("ShoalWalk");
            return;
        }

        // Default to idle
        ChangeAnimation("IdleShoal");
    }
}
