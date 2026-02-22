
using UnityEngine;
using UnityEngine.UI;

public class ShoalEnemy : EnemyBase
{
    public DoorManager doorManager;
    // Shoal-specific audio edge trigger state
    private bool _shoalWasInChaseRange = false;
    public bool bossShoal;
    public GameObject timer;
    public GameObject player;
    public GameObject bossTrigger2;
    public GameObject areaTwoEnemies;
    public Cinema_final CinemaScript;
    [Header("Animation")]
    public Animator ShoalAnimator;
    private string CurrentShoalAnimation = "";

    public override void Update()
    {
        base.Update();  
        bool grounded = IsEnemyGrounded();
        if (Target != null)
        {
            m_Distance = Vector3.Distance(Target.transform.position, transform.position);
            bool inChaseRange = m_Distance <= chaseRange;

            if (inChaseRange && !_shoalWasInChaseRange)
            {
                // Entered chase range: play Shoal "detected" sound
                AudioManager.PlayShoalIdle();
            }
      

            _shoalWasInChaseRange = inChaseRange;
        }
        else _shoalWasInChaseRange = false;



        if (isPushed)
        {
            pushCooldown -= Time.deltaTime;
        }
        if (pushCooldown < 0 && isPushed)
        {
            pushCooldown = 0;
            isPushed = false;
            gameObject.SetActive(false);
            enemyHealth.value -= 1;
            AudioManager.PlayShoalDamageHit();
            
            
            if (enemyHealth.value <= 0)
            {
                enemyHealthScreen.SetActive(false);
                if (bossShoal == false)
                {
                    doorManager.open = true;
                    areaTwoEnemies.SetActive(true);
                }
                if (bossShoal == true)
                {
                    if (bossTrigger2 != null)
                    {
                        bossTrigger2.SetActive(true);
                    }  
                    timer.SetActive(true);
                    if (CinemaScript != null)
                    {
                        CinemaScript.isPhase2Intro = true;
                    }
                }
            }
        }
        if (!grounded)
        {
            ResetSlapState();
        }
        if (grounded && wasGrounded && !isGrabbed && !isPushed)
        {
            // Debug.Log("Enemy just landed!");
            rb.isKinematic = true;
            agent.enabled = true;
        }
        wasGrounded = grounded;
        if (agent.enabled && agent.isOnNavMesh)
        {
            ChasePlayer();
        }

        CheckAnimation();
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
      //  ChangeAnimation("IdleShoal");
    }
}
