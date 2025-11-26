using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[DisallowMultipleComponent]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class MacroBoss : EnemyBase
{
    private void Awake()
    {
        canAttack = true; // little guy can attack
        canChase = true;
        canPatrol = true;
    }
    // ------------ auto assign references -------------- //
    void OnValidate()
    {
        // 1) Target: find and assign player as target if not assigned
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

    [Header("Macro Settings")]
    [SerializeField] private Transform MicroPosition;

    private void OnEnable()
    {
        StartCoroutine(ReturnToMicroPosition());
    }

    private IEnumerator ReturnToMicroPosition()
    {
        var wait = new WaitForSeconds(5f);
        while (true)
        {
            if (MicroPosition != null)
            {
                Debug.Log("Returning to micro position");
                agent.destination = MicroPosition.position;
                //agent.SetDestination(MicroPosition.position);
            }
            yield return wait;
        }
    }
}
