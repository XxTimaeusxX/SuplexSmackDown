using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class ShoalEnemy : EnemyBase
{
    public DoorManager doorManager;

    public bool bossShoal;
    public GameObject timer;
    public GameObject player;
    public GameObject bossTrigger2;
    public Cinema_final CinemaScript;
    [Header("Animation")]
    public Animator ShoalAnimator;
    private string CurrentShoalAnimation = string.Empty;

    bool IsAlive = true;
    public GameObject slapbox;          // child trigger collider with AttackHitBox
    public float slapActiveTime = 0.2f;
    public Slider enemyHealth;
    public GameObject enemyHealthScreen;

    private static bool waveStarted = false;
    private static int totalShoalCount = 0;

    void Start()
    {
        if (!bossShoal)
        {        
            if (!waveStarted)
            {
                waveStarted = true;
                enemyHealth.value = enemyHealth.maxValue;
            }
            totalShoalCount++;
        }
        else
        {
            enemyHealth.value += 1; // Boss health starts at 100%
        }

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
    public override void Update()
    {
        if (agent.enabled)
        {
            ChasePlayer();
        }

        bool grounded = IsEnemyGrounded();

        if (target != null)
        {
            m_Distance = Vector3.Distance(target.transform.position, transform.position);
            
            bool inChaseRange = m_Distance <= chaseRange;

            if (inChaseRange)
            {
                // Entered chase range: play Shoal "detected" sound
                AudioManager.PlayShoalIdle();
            }
        }
        if (!grounded)
        {
            ResetSlapState();
        }
        if (enemyHealth.value <= 0)
        {
            enemyHealthScreen.SetActive(false);
            if (bossShoal == false)
            {
                doorManager.open = true;
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
        if (health <= 0 && IsAlive)
        {
            enemyHealth.value -= 1;  // 100 shoal in first stage, so 1 damage = 1% health
            IsAlive = false;    // prevents rapid depeletion of boss bar
            Death();
        }
        CheckAnimation();

        if (totalShoalCount >= 100)
            {
            totalShoalCount = 0;
            waveStarted = false;
        }
    }

    public void ResetSlapState()
    {
        _nextAttackTime = 0f;
        if (slapbox != null)
        {
            slapbox.SetActive(false);
        }
        StopCoroutine(SlapAttackDuration());
        ResetChargeUI();
    }
    public IEnumerator SlapAttackDuration()
    {
        yield return new WaitForSeconds(.5f); // wait a frame to sync with animation
        slapbox.SetActive(true);
        yield return new WaitForSeconds(slapActiveTime);
        slapbox.SetActive(false);
    }
    public override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);
        if (collision.gameObject.CompareTag("Shockwave"))
        {
            health -= 1;
            AudioManager.PlayShoalDamageHit();
        }
    }

    public override void ChasePlayer()
    {
        target = PLAYER; // Set target to player (can be modified for other targets)

        distanceToTarget = Vector3.Distance(target.transform.position, transform.position);
        float arrivalThreshold = Mathf.Max(0.5f, agent.stoppingDistance);

        if (agent.isOnNavMesh)
        {
            bool inChaseRange = (distanceToTarget <= chaseRange);

            if (inChaseRange)
            {
                patrolWaitDefault = 0f;
                agent.speed = enemySprintSpeed;
                agent.destination = target.transform.position;

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

                    agent.destination = target.transform.position;
                    _nextAttackTime = 0f;
                    ResetChargeUI();
                }
            }
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
      //  ChangeAnimation("IdleShoal");
    }
}
