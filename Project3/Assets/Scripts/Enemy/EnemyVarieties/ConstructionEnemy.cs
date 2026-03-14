using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ConstructionEnemy : EnemyBase
{
    // Edge trigger to play damage sound once when push starts

    public GameObject slapbox;

    [Header("Animation")]
    private string CurrentWorkerAnimation = string.Empty;

    void OnValidate()
    {
        // 1) Target: assign Player by tag if not set
        if (target == null)
        {
            target = PLAYER;
        }
        // Optional: ensure sane defaults (won’t override if already configured)
        if (agent != null)
        {
            if (agent.stoppingDistance < 0.5f) agent.stoppingDistance = 0.75f;
            if (agent.radius < 0.1f) agent.radius = 0.3f;
            if (agent.acceleration < 8f) agent.acceleration = 12f;
        }

        // Hitbox slapbox: find child trigger named "AttackHitBox"
        if (slapbox == null)
        {
            var hitbox = transform.Find("SlapHitBox");
            if (hitbox != null)
            {
                //var col = hitbox.GetComponentInChildren<GameObject>();
                //if (col != null) slapbox = col;
                //else Debug.LogWarning($"[{name}] 'SlapHitBox' found but has no Collider.", this);
            }
        }
        // 5) UI: try find in children (optional)
        if (chargeSlider == null)
        {
            chargeSlider = GetComponentInChildren<UnityEngine.UI.Slider>(includeInactive: true);
         
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
            animator.CrossFade(animation, crossfade);

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
    public override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);
        if (collision.gameObject.CompareTag("Shockwave"))
        {
            AudioManager.PlayConstructionFalling();
        }
    }

    public override void Attack()
    {
    }
    public override void Death()
    {
        StartCoroutine(DeathRoutine());
    }
    private IEnumerator DeathRoutine()
    {
        gameObject.tag = "Untagged";  // prevents grabbing a 'dead' enemy
        yield return new WaitForSeconds(timeTillDeath);   //Time till object disappears
        Destroy(this.gameObject);
    }
}
