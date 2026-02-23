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
    [SerializeField] private float slapActiveTime = 0.1f;

    private bool _OfficeShoalWasInChaseRange = false;

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
    }
    public override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);
        if (collision.gameObject.CompareTag("Shockwave"))
        {       
            AudioManager.PlayShoalFalling();
        }
    }

    public override void Attack()
    {
        // Behavior guard: only attack when allowed
        if (!canAttack) { ResetSlapState(); return; }// ensure state/UI is cleared if attack disabled mid-charge


        // Charge up while in melee
        if (_nextAttackTime < attackCooldown)
        {
            _nextAttackTime += Time.deltaTime;
            UpdateChargeUI(_nextAttackTime, attackCooldown, show: true);
            // Debug.Log($"charge: {_nextAttackTime:F2}/{attackCooldown:F2}");
            return;
        }

        // Fully charged -> attack, then reset charge for the next swing
        Debug.Log($"[{name}] Melee attack!");
        animator.SetTrigger("EnemySlap");
        AudioManager.PlayEnemySlap();
        _nextAttackTime = 0f; // restart charge
        UpdateChargeUI(_nextAttackTime, attackCooldown, show: true);
        StartCoroutine(SlapattackDuration());
    }
    public IEnumerator SlapattackDuration()
    {
        if (slapbox == null) yield break;
        yield return new WaitForSeconds(.5f); // wait a frame to sync with animation
        slapbox.SetActive(true);
        yield return new WaitForSeconds(.09f);
        slapbox.SetActive(false);
    }
    public void ResetSlapState()
    {
        _nextAttackTime = 0f;
        if (slapbox != null)
        {
            slapbox.SetActive(false);
        }
        StopCoroutine(SlapattackDuration());
        ResetChargeUI();
    }
}
