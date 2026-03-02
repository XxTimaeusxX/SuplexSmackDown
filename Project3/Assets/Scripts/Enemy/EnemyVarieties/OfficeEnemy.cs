using System.Collections;
using UnityEngine;
using UnityEngine.AI;
[DisallowMultipleComponent]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]

//TODO: look into the attack hitbox, seems it always remains active
public class OfficeEnemy : EnemyBase
{
    [Header("Hitbox")]
    public GameObject slapbox;          // child trigger collider with AttackHitBox
    public float slapActiveTime = 0.1f;

    private bool _OfficeShoalWasInChaseRange = false;

    private void Start()
    {
        slapbox.SetActive(false);
    }

    public override void Update()
    {
        base.Update();

        if (target != null)
        {
            m_Distance = Vector3.Distance(target.transform.position, transform.position);
            bool inChaseRange = m_Distance <= chaseRange;
            if (inChaseRange && !_OfficeShoalWasInChaseRange)
            {
                AudioManager.PlayShoalIdle(); // Detected sound (replace per-enemy)
            }
            _OfficeShoalWasInChaseRange = inChaseRange;
        }
        else
        {
            _OfficeShoalWasInChaseRange = false;
        }
        if (health <= 0)
        {
            Death();
        }
    }
    public override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);
        if (collision.gameObject.CompareTag("Shockwave"))
        {       
            AudioManager.PlayShoalFalling();
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
    public override void Death()
    {
        StartCoroutine(DeathRoutine());
    }
    private IEnumerator DeathRoutine()
    {
        gameObject.tag = "Untagged";  // prevents grabbing a 'dead' enemy
        yield return new WaitForSeconds(timeTillDeath);   //Time till object disappears
        this.gameObject.SetActive(false);
    }
    public IEnumerator SlapAttackDuration()
    {
        yield return new WaitForSeconds(.5f); // wait a frame to sync with animation
        slapbox.SetActive(true);
        yield return new WaitForSeconds(slapActiveTime);
        slapbox.SetActive(false);
    }
}
