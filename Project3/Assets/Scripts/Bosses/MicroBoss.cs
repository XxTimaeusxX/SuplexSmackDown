using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[DisallowMultipleComponent]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class MicroBoss : EnemyBase
{
    [Header("Boss Throw")]
    //[SerializeField] private BoxCollider throwHitBox; // hitbox to detect when to throw Macro
    [SerializeField] private GameObject macroPrefab;   // prefab For MicroBoss
    [SerializeField] private Transform throwOrigin;    // optional; defaults to boss position
    [SerializeField] private float throwInterval = 3f;
    [SerializeField] private float throwForce = 12f;
    private NavMeshAgent MacroAgent;
    private Rigidbody MacrosRb;
    private MacroBoss MacroEnemy;
    private float throwTimer;
    public GameObject MacroPrefab => macroPrefab;
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
    public override void Update()
    {
       base.Update();
        if (enemyHealth.value <= 0)
        {
            // Disable this boss functionality
            canAttack = false;
            canChase = false;
            canPatrol = false;
            agent.enabled = false;
           
            enemyHealthScreen.SetActive(false);
            Destroy(MacroPrefab);
        }
    }

    public IEnumerator ThrowMacro()
    {
        AudioManager.PlayMicroPrepareAttack();
        // ----- Position macro prefab at throw origin ----- //
        var origin = (throwOrigin != null) ? throwOrigin : this.transform;
        MacroPrefab.transform.position = origin.position;
        MacroPrefab.transform.rotation = Quaternion.identity;
        MacroPrefab.transform.SetParent(origin);
        
        // ----- Disabling navmesh & kinematics  ----- //
        MacrosRb.isKinematic = true;

        //---- Disable enemy AI behaviors on thrown MacroEnemy--//
        MacroEnemy.canAttack = false;
        MacroEnemy.canPatrol = false;
        MacroEnemy.canChase = false;
        MacroEnemy.SetGrabbed(true);

        //---- Hold Macro for x seconds--//
        throwTimer = 0f;
        while (throwTimer < throwInterval)
        {
            throwTimer += Time.deltaTime;
            yield return null;
        }

        MacroPrefab.transform.SetParent(null); // unparent macro before throw
        MacrosRb.isKinematic = false; // re-enable physics
        
      
        // ----- Calculate throw direction and apply force ----- //
        Vector3 dir = (Target.transform.position - MacroPrefab.transform.position).normalized;
        MacrosRb.AddForce(dir * throwForce, ForceMode.VelocityChange);
        MacroEnemy.wasThrown = true; // flag macro as thrown
        float enableMacroTimer = 0f;
        while(enableMacroTimer < 3f)
        {
            // prevents enabling ai agent if player grabs macro mid-air
            if (macroPrefab.transform.parent !=null)
            {
             //   Debug.Log("Player grabbed Macro mid-air, aborting resume sequence");

            yield break;
            }
            enableMacroTimer += Time.deltaTime;
            yield return null;
        }
        // ----- Re-enable navmesh & kinematics ----- //
      //  Debug.Log("Macro resumed after throw - not grabbed");
      
        MacroEnemy.ResumeSequence();
        
    }
}
