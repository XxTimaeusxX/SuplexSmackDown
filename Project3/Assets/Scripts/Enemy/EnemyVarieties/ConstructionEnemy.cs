using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ConstructionEnemy : EnemyBase
{
    // Edge trigger to play damage sound once when push starts

    public GameObject slapbox;

    [Header("Animation")]
    private string CurrentWorkerAnimation = string.Empty;
    public float slapActiveTime = 0.2f;

    public override void Update()
    {
        base.Update();  // Let base class handle everything
        CheckAnimation();
        if (health <= 0)
        {
            Death();
        }
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
            health -= 1;
        }
    }

    public override void Attack()
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
        UpdateChargeUI(_nextAttackTime, attackCooldown, show: true);
        StartCoroutine(SlapAttackDuration());
        ResetChargeUI();
    }
    public IEnumerator SlapAttackDuration()
    {
        yield return new WaitForSeconds(.5f); // wait a frame to sync with animation
        slapbox.SetActive(true);
        yield return new WaitForSeconds(slapActiveTime);
        slapbox.SetActive(false);
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
