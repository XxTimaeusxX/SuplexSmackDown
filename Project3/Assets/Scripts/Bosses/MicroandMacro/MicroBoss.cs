using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

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
   [SerializeField] private PowerGauge _powerGauge;
    private NavMeshAgent _MacroAgent;
    private Rigidbody _MacrosRb;
    private MacroBoss _MacroEnemy;
    private float _throwTimer;

    [SerializeField] private LowerRoom lowerRoom;

    [Header("Voice Line Settings")]
    private bool hasPlayed3HealthLine = false;
    private bool hasPlayed2HealthLine = false;
    private bool hasPlayed1HealthLine = false;
    private bool wasInChaseRange = false;

    public GameObject MacroPrefab => macroPrefab;
    private void Awake()
    {
        canAttack = false; // Disable basic attack for MicroBoss "big guy"
        canChase = true;
        canPatrol = true;
     

        // ----- get macros components ----- //
         _MacroAgent = MacroPrefab.GetComponent<NavMeshAgent>();
         _MacrosRb = MacroPrefab.GetComponent<Rigidbody>();
        _MacroEnemy = MacroPrefab.GetComponent<MacroBoss>();

        if (_powerGauge == null)
            _powerGauge = GetComponent<PowerGauge>();

        lowerRoom = FindFirstObjectByType<LowerRoom>();
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

        // Check if player is in chase range
        if(canChase)
        {
            PlayHealthBasedVoiceLine();
        }
        if (enemyHealth.value <= 0)
        {
            // Disable this boss functionality
            canAttack = false;
            canChase = false;
            canPatrol = false;
            agent.enabled = false;
           this.gameObject.tag ="Enemy";
            enemyHealthScreen.SetActive(false);
            Destroy(MacroPrefab);
            _powerGauge.EnableInfiniteMeter();
            lowerRoom.MoveDown();
        }
    }

    private void PlayHealthBasedVoiceLine()
    {
        int currentHealth = (int)enemyHealth.value;

        // Play voice line based on current health (only once per health threshold)
        if (currentHealth == 3 && !hasPlayed3HealthLine)
        {
            AudioManager.PlayMicroEncounterOne();
            hasPlayed3HealthLine = true;
        }
        else if (currentHealth == 2 && !hasPlayed2HealthLine)
        {
            AudioManager.PlayMicroTwoHealth();
            hasPlayed2HealthLine = true;
        }
        else if (currentHealth == 1 && !hasPlayed1HealthLine)
        {
            AudioManager.PlayMicroOneHealth();
            hasPlayed1HealthLine = true;
        }
    }

    public IEnumerator ThrowMacro()
    {

        AudioManager.PlayMicroPrepareAttack();
        // ----- Position macro prefab at throw origin ----- //
        var origin = (throwOrigin != null) ? throwOrigin : this.transform;
        MacroPrefab.transform.position = origin.position;
        MacroPrefab.transform.rotation = Quaternion.Euler(90f, origin.rotation.eulerAngles.y, 0f);
        MacroPrefab.transform.SetParent(origin);
        
        // ----- Disabling navmesh & kinematics  ----- //
        _MacrosRb.isKinematic = true;

        //---- Disable enemy AI behaviors on thrown MacroEnemy--//
        _MacroEnemy.canAttack = false;
        _MacroEnemy.canPatrol = false;
        _MacroEnemy.canChase = false;
        _MacroEnemy.SetGrabbed(true);
        if (_MacroEnemy.CompareTag("Macro"))
        {
            _MacroEnemy.gameObject.tag = "DamagePlayer";
        }

        //---- Hold Macro for x seconds--//
        _throwTimer = 0f;
        while (_throwTimer < throwInterval)
        {
            _throwTimer += Time.deltaTime;
            yield return null;
        }

        MacroPrefab.transform.SetParent(null); // unparent macro before throw
        _MacrosRb.isKinematic = false; // re-enable physics


        // ----- Calculate throw direction and apply force ----- //

     //  float hieght = 0f;
     //   float foward = 18f;
        Vector3 dir = (Target.transform.position - MacroPrefab.transform.position).normalized;
      //  Vector3 orientThrow = new Vector3(dir.x, 0f, dir.z).normalized;
      //  Vector3 Upwardforce = hieght *  Vector3.up; // total power to apply to macro
      //  Vector3 FowardForce = foward * orientThrow; // forward force to apply to macro*/

        _MacrosRb.AddForce(dir*throwForce , ForceMode.Impulse);
        _MacroEnemy.wasThrown = true; // flag macro as thrown
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
      
        _MacroEnemy.ResumeSequence();
        
    }
}
