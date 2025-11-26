using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[DisallowMultipleComponent]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class MicroBoss : EnemyBase
{

    private void Awake()
    {
        canAttack = false; // Disable basic attack for MicroBoss "big guy"
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


    [Header("Boss Throw (Simple)")]
    [SerializeField] private GameObject MicroPrefab;   // prefab For MicroBoss
    [SerializeField] private Transform throwOrigin;    // optional; defaults to boss position
    [SerializeField] private float throwInterval = 5f;
    [SerializeField] private float throwForce = 12f;
    private void OnEnable()
    {
        StartCoroutine(Throwload());
    }
    private IEnumerator Throwload()
    {
        var waitInterval = new WaitForSeconds(throwInterval); // cooldown between throws when in-range
        var poll = new WaitForSeconds(0.2f);                  // how often we check range when out-of-range

        while (true)
        {
            // Basic safety checks
            if (Target == null || MicroPrefab == null)
            {
                yield return poll;
                continue;
            }

            float dist = Vector3.Distance(Target.transform.position, transform.position);
            // Only throw when within chaseRange and boss currently allowed to chase
            if (dist <= meleeRange && canChase)
            {

                ThrowMicro();

                // wait the full throw cooldown before attempting another throw
                yield return waitInterval;
            }
            else
            {

                // not in range yet — poll again shortly
                yield return poll;
            }
        }
    }
    public void ThrowMicro()
    {
        Vector3 origin = throwOrigin.position;
        var go = Instantiate(MicroPrefab, origin, Quaternion.identity);
        var rb = go.GetComponent<Rigidbody>();
        Vector3 dir = (Target.transform.position - origin).normalized;
        rb.AddForce(dir * throwForce, ForceMode.VelocityChange);
    }

}
