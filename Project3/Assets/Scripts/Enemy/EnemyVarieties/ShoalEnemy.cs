using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class ShoalEnemy : EnemyBase
{
    new void Update()
    {
        bool grounded = IsEnemyGrounded();
        if (isPushed)
        {
            pushCooldown -= Time.deltaTime;
        }
        if (pushCooldown < 0)
        {
            if (!isGrabbed)
            {
                pushCooldown = 0;
                isPushed = false;
                agent.enabled = true;
                rb.isKinematic = true;
            }
            enemyHealth.value -= 1;
            if (enemyHealth.value <= 0)
            {
                enemyHealthScreen.SetActive(false);
            }
            Destroy(gameObject);
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
    }
}
