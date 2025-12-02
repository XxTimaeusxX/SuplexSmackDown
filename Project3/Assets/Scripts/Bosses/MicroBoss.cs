using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[DisallowMultipleComponent]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class MicroBoss : EnemyBase
{
    [Header("Boss Throw (Simple)")]
    [SerializeField] private BoxCollider throwHitBox; // hitbox to detect when to throw Macro
    [SerializeField] private GameObject MacroPrefab;   // prefab For MicroBoss
    [SerializeField] private Transform throwOrigin;    // optional; defaults to boss position
    [SerializeField] private float throwInterval = 5f;
    [SerializeField] private float throwForce = 12f;
    private NavMeshAgent MacroAgent;
    private Rigidbody MacrosRb;
    private MacroBoss MacroEnemy;

    private void Awake()
    {
        canAttack = false; // Disable basic attack for MicroBoss "big guy"
        canChase = true;
        canPatrol = true;
     

        // ----- get macros components ----- //
         MacroAgent = MacroPrefab.GetComponent<NavMeshAgent>();
         MacrosRb = MacroPrefab.GetComponent<Rigidbody>();
        MacroEnemy = MacroPrefab.GetComponent<MacroBoss>();
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
            if (Target == null || MacroPrefab == null)
            {
                yield return poll;
                continue;
            }

            float dist = Vector3.Distance(Target.transform.position, transform.position);
            // Only throw when within chaseRange and boss currently allowed to chase
            if (dist <= meleeRange && canChase)
            {

              // StartCoroutine(ThrowMicro());

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
   
    public IEnumerator ThrowMicro()
    {
        // ----- Position macro prefab at throw origin ----- //
        Vector3 origin = throwOrigin.position;
        MacroPrefab.transform.position = origin;
        MacroPrefab.transform.rotation = Quaternion.identity;
        //---- Hold Macro for 5 seconds--//
      //  yield return new WaitForSeconds(5f);

        // ----- Disabling navmesh & kinematics  ----- //
        MacroAgent.enabled = false; // disable navmesh agent to allow physics throw
        MacrosRb.isKinematic = false; // ensure rigidbody is non-kinematic to allow physics throw

       //---- Disable enemy AI behaviors on thrown MacroEnemy--//
       MacroEnemy.canAttack = false;
       MacroEnemy.canPatrol = false;
       MacroEnemy.canChase = false;
       MacroEnemy.SetGrabbed(true);

     

        // ----- Calculate throw direction and apply force ----- //
        Vector3 dir = (Target.transform.position - origin).normalized;
        MacrosRb.AddForce(dir * throwForce, ForceMode.VelocityChange);

        yield return new WaitForSeconds(3f); // wait for macro to land
        // ----- Re-enable navmesh & kinematics ----- //
        
        StartCoroutine(MacroEnemy.ResumeSequence());
       
    }

}
