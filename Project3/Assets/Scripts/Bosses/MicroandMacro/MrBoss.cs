using UnityEngine;
using UnityEngine.AI;

[DisallowMultipleComponent]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class MrBoss : OGEnemyBase
{
    private void Awake()
    {
        // Patrol-only: disable chasing and attacking, keep patrol enabled.
        canAttack = false;
        canChase = false;
        canPatrol = true;
    }

    

    void OnValidate()
    {
        // 1) target: find and assign player as target if not assigned
        if (Target == null)
        {
            var player = GameObject.FindWithTag("Player");
            if (player != null) Target = player;
        }

        // 2) Ground check: find and assign ground check transform if not assigned
        if (groundCheck == null)
        {
            var existing = transform.Find("GroundCheck");
            if (existing != null) groundCheck = existing;
        }
    }
}
