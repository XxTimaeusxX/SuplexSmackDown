using UnityEngine;
using UnityEngine.AI;

// Last Edited: 1/19/2026 by Istvan W.
public class ConstructionEnemy : EnemyBase
{
    public override void Attack()
    {

    }
    /*
 private bool _WorkerInChaseRange = false;
    // Edge trigger to play damage sound once when push starts
    private bool _wasPushed = false;
    void OnValidate()
    {
        // 1) target: assign Player by tag if not set
        if (target == null)
        {
            var player = GameObject.FindWithTag("Player");
            if (player != null) target = player;

        }
        // 2) Core components on this GameObject
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        if (rb == null) rb = GetComponent<Rigidbody>();
        if (animator == null) animator = GetComponent<Animator>();
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
        base.Update();
      
       
        if (target != null)
        {
            m_Distance = Vector3.Distance(target.transform.position, transform.position);
            bool inChaseRange = m_Distance <= chaseRange;
            if (inChaseRange && !_WorkerInChaseRange)
            {
                AudioManager.PlayConstructionSeenOne(); // Detected sound (replace per-enemy)
            }
            _WorkerInChaseRange = inChaseRange;
        }
        else
        {
            _WorkerInChaseRange = false;
        }
        
    }
    // Alternatively, if you prefer tying directly to the collision event:
    private void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);
        if (collision.gameObject.CompareTag("Shockwave"))
        {
            // EnemyBase sets isPushed=true here; play immediately
            // AudioManager.PlayConstructionDamageHitTwo();
            AudioManager.PlayConstructionFalling();
         //   Debug.Log("construction");
        }
    }
    */
}
