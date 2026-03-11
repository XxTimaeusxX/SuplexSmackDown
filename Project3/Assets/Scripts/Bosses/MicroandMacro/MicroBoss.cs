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
    [Header("-------------------------Micro Settings ------------------------------")]
    public Boss1Arena bossArena;
    [Header("Boss Throw")]
    //[SerializeField] private BoxCollider throwHitBox; // hitbox to detect when to throw Macro
    [SerializeField] private GameObject macroPrefab;   // prefab For MicroBoss
    [SerializeField] private Transform macrosmesh;
    [SerializeField] private Transform throwOrigin;    // optional; defaults to boss position
    [SerializeField] private float throwInterval = 3f;
    [SerializeField] private float throwForce = 12f;
   [SerializeField] private PowerGauge _powerGauge;
    private NavMeshAgent _MacroAgent;
    private Rigidbody _MacrosRb;
    private MacroBoss _MacroEnemy;
    private float _throwTimer;
   
    [SerializeField] private LowerRoom lowerRoom;

    private GlowMesh _glowMesh;
    [Header("Voice Line Settings")]
    private bool hasPlayed3HealthLine = false;
    private bool hasPlayed2HealthLine = false;
    private bool hasPlayed1HealthLine = false;
    private bool isPlayingVoiceLine = false;
    private bool wasInChaseRange = false;

    [Header("Animation")]
    public Animator WorkerAnimator;
    private string CurrentWorkerAnimation = "";
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

        // ----- call glowmesh script on prefab----- //
           _glowMesh = GetComponent<GlowMesh>();
        if (_glowMesh == null)
            {
                Debug.LogError("GlowMesh component not found on MacroPrefab or its children.");
        }
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
        if (canChase && !isPlayingVoiceLine)
        {
            StartCoroutine(PlayHealthBasedVoiceLine());
        }
     
        if (enemyHealth.value <= 0)
        {
            // Disable this boss functionality
            bossArena.moveDown = true;
            canAttack = false;
            canChase = false;
            canPatrol = false;
            agent.enabled = false;
            _glowMesh.SetGlowColor(); // trigger glow effect on death
            lowerRoom.EnableArrows();// enable arrows to show path to next area
            this.gameObject.tag ="Enemy";
            enemyHealthScreen.SetActive(false);
            Destroy(MacroPrefab);
            _powerGauge.EnableInfiniteMeter();
            lowerRoom.MoveDown();
        }
   //    if (!_MacroEnemy.IsGrabbedByMicro){ CheckAnimation(); }
        
    }

    private IEnumerator PlayHealthBasedVoiceLine()
    {
        isPlayingVoiceLine = true;
        int currentHealth = (int)enemyHealth.value;
        // Play voice line based on current health (only once per health threshold)
        if (currentHealth == 3 && !hasPlayed3HealthLine)
        {
             yield return new WaitForSeconds(3f); // slight delay before first line
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
        isPlayingVoiceLine = false;
        yield return null;
    }

    public IEnumerator ThrowMacro()
    {
        ChangeAnimation("MicroThrow"); // play throw animation
        _MacroEnemy.IsGrabbedByMicro = true;
        // Store original mesh rotation to restore later
        Quaternion originalMeshRotation = macrosmesh != null ? macrosmesh.localRotation : Quaternion.identity;
        AudioManager.PlayMicroPrepareAttack();
        // ----- Position macro prefab at throw origin ----- //
        var origin = (throwOrigin != null) ? throwOrigin : this.transform;
        MacroPrefab.transform.position = origin.position;
      //  MacroPrefab.transform.rotation = Quaternion.Euler(90f, origin.rotation.eulerAngles.y, 0f); // orient collider to face like a torpedo
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
       ResumeAnimation(); // resume throw animation after hold
        MacroPrefab.transform.SetParent(null); // unparent macro before throw
        _MacroEnemy.IsGrabbedByMicro = false;// set out of grab ball state
        _MacrosRb.isKinematic = false; // re-enable physics


        // ----- Calculate throw direction and apply force ----- //

     //  float hieght = 0f;
     //   float foward = 18f;
        Vector3 dir = (Target.transform.position - MacroPrefab.transform.position).normalized;
      //  Vector3 orientThrow = new Vector3(dir.x, 0f, dir.z).normalized;
      //  Vector3 Upwardforce = hieght *  Vector3.up; // total power to apply to macro
      //  Vector3 FowardForce = foward * orientThrow; // forward force to apply to macro*/

    
        _MacrosRb.AddForce(dir*throwForce , ForceMode.Impulse);
     
        _MacroEnemy.IsThrownByMicro = true; // flag macro as thrown
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
        /*  while(!_MacroEnemy.IsEnemyGrounded())
          {
              Debug.Log("Macro is grounded but not resuming - waiting to resume");
              macrosmesh.Rotate(Vector3.left, 1000f * Time.deltaTime, Space.World);
              yield return null;
          }*/
        // ----- Re-enable navmesh & kinematics ----- //
        //  Debug.Log("Macro resumed after throw - not grabbed");
        // macrosmesh.localRotation = originalMeshRotation; // restore original mesh rotation
     //   _MacroEnemy.IsThrownByMicro = false;
        _MacroEnemy.ResumeSequence();
        
    }
    public IEnumerator TakeDamage()
    {
        enemyHealth.value -= 1;
        ChangeAnimation("MicroHurt");
        yield return new WaitForSeconds(2f); // wait for hurt animation to play
        CheckAnimation();
    }

    //---------------- Animation ---------------------------//
    public void ChangeAnimation(string animation, float crossfade = 0.2f)
    {
        if (CurrentWorkerAnimation != animation)
        {
            CurrentWorkerAnimation = animation;
            WorkerAnimator.CrossFade(animation, crossfade);

        }
    }
    // Pauses animation frame, but frame is set to paused at  0.5f set in the animation controller event system.
    public void PauseAnimation()
    {
        WorkerAnimator.speed = 0f;
    }
    public void ResumeAnimation()
    {
        WorkerAnimator.speed = 1f;
    }
    private void CheckAnimation()
    {
        // Attack animation call
        /* if (_nextAttackTime > 0f && _nextAttackTime < attackCooldown)
         {
             ChangeAnimation("");
             return;
         }*/

        // Check if moving (has a path and is actively navigating)
        // call grab animation if grabbed
        
        //Walk Animation call
        if (agent.enabled && agent.isOnNavMesh && agent.hasPath && agent.remainingDistance > agent.stoppingDistance)
        {
            ChangeAnimation("MicroRun");
            return;
        }

        // Default to idle
        ChangeAnimation("MicroIdle");


    }
}
