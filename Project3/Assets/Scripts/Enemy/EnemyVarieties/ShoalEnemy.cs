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
    private string CurrentShoalAnimation = "";

    public GameObject slapbox;          // child trigger collider with AttackHitBox
    public float slapActiveTime = 0.1f;
    public Slider enemyHealth;
    public GameObject enemyHealthScreen;

    void Start()
    {
        enemyHealth.value += 1;
    }
    public override void Death()
    {
        StartCoroutine(DeathRoutine());
    }
    private IEnumerator DeathRoutine()
    {
        yield return new WaitForSeconds(timeTillDeath);   //Time till object disappears
        this.gameObject.SetActive(false);
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
        if (isPushed)
        {
            pushCooldown -= Time.deltaTime;
        }

        if (pushCooldown < 0 && isPushed)
        {
            pushCooldown = 0;
            isPushed = false;
            
            enemyHealth.value -= 1;
            AudioManager.PlayShoalDamageHit();
            
            
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
            Destroy(gameObject);
        }
        if (!grounded)
        {
            ResetSlapState();
        }
        if (agent.enabled && agent.isOnNavMesh)
        {
            ChasePlayer();
        }

        CheckAnimation();
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
