using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[DisallowMultipleComponent]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class MacroBoss : Enemy
{
    private void Awake()
    {
        canAttack = false; // Disable basic attack for MacroBoss
        canChase = true;
        canPatrol = true;
    }
    // Auto-provision basic Enemy fields so chase/slap work without manual setup.
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

        // 3) Defaults for movement/combat so it actually chases and swings
        if (chaseRange <= 0f) chaseRange = 20f;
        if (meleeRange <= 0f) meleeRange = 20f;
        if (patrolRunSpeed <= 0f) patrolRunSpeed = 3.5f;
        if (patrolWalkSpeed <= 0f) patrolWalkSpeed = 1.5f;
        if (attackCooldown <= 0f) attackCooldown = 0.8f;

        // 4) Grounding mask and distance
        if (groundDistance <= 0f) groundDistance = 0.2f;
        if (groundMask.value == 0) groundMask = ~0; // everything, fine for a flat test plane
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
