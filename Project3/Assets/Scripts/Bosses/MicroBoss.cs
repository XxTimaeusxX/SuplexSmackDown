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
   /* new void Update()
    {
       
        if(enemyHealth.value == 1)
            Debug.Log("Macro Health is 1");
        if (enemyHealth.value <= 0)
        {
            Destroy(MacroPrefab);

        }
    }*/

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
   
    public IEnumerator ThrowMacro()
    {
        
        // ----- Position macro prefab at throw origin ----- //
        var origin = (throwOrigin != null) ? throwOrigin : this.transform;
        MacroPrefab.transform.position = origin.position;
        MacroPrefab.transform.rotation = Quaternion.identity;
        MacroPrefab.transform.SetParent(origin);
        MacroPrefab.transform.localPosition = Vector3.zero;
        
        // ----- Disabling navmesh & kinematics  ----- //
        MacrosRb.isKinematic = true;

        //---- Disable enemy AI behaviors on thrown MacroEnemy--//
        MacroEnemy.canAttack = false;
        MacroEnemy.canPatrol = false;
        MacroEnemy.canChase = false;
        MacroEnemy.SetGrabbed(true);

        //---- Hold Macro for x seconds--//
        yield return new WaitForSeconds(1f);

       
        MacroPrefab.transform.SetParent(null); // unparent macro before throw
        MacrosRb.isKinematic = false; // re-enable physics

        // ----- Calculate throw direction and apply force ----- //
        Vector3 dir = (Target.transform.position - MacroPrefab.transform.position).normalized;
        MacrosRb.AddForce(dir * throwForce, ForceMode.VelocityChange);

        yield return new WaitForSeconds(3f); // wait for macro to land
        // ----- Re-enable navmesh & kinematics ----- //
        MacroEnemy.SetGrabbed(false);
        StartCoroutine(MacroEnemy.ResumeSequence());
       
    }

}
