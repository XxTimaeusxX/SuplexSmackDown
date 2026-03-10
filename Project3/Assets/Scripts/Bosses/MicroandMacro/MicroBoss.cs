using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]

// MARK - Currently broken
public class MicroBoss : MonoBehaviour
{
    [SerializeField] private float stateTimer;
    public float chasePeriodMean; // Time Macro will chase before thrown
    private float chasePeriod;

    [Header("Boss Throw")]
    public bool throwingMacro = false;

    [SerializeField] GameObject macro;
    public GameObject Macro => macro; // Public getter for macro prefab

    [SerializeField] private Transform macrosmesh;
    [SerializeField] private Transform throwOrigin;    // optional; defaults to boss position
    [SerializeField] private float throwInterval = 3f;
    [SerializeField] private float throwForce = 5f;
    private SuplexController suplexController;
    private NavMeshAgent _MacroAgent;
    private Rigidbody _MacrosRb;
    private MacroBoss _MacroEnemy;
    public float grabRange = 2f;
    //public GameObject grabHitbox;

    [SerializeField] private LowerRoom lowerRoom;

    [Header("References")]
    public GameObject PLAYER; // Reference to the player (Currently set as so due to no enemy vs enemy interactions)
    public Rigidbody Rigidbody => rb; // Public getter for Rigidbody (for ICarriable interface)
    private Rigidbody rb;
    public NavMeshAgent agent;

    [SerializeField] protected CarryWeightProfile carryWeightProfile; // Used to determine how the enemy behaves when being carried (e.g. how much it slows the player down, whether it can be thrown, etc.)
    public CarryWeightProfile CarryWeightProfile => carryWeightProfile; // Public getter for carry weight profile

    [Header("Colliders")]
    public Collider mainCollider;   // Main collider for the enemy
    public Collider carryProxy;     // Collider used when being carried to prevent clipping

    private GlowMesh _glowMesh;

    [Header("Voice Line Settings")]
    private bool hasPlayed3HealthLine = false;
    private bool hasPlayed2HealthLine = false;
    private bool hasPlayed1HealthLine = false;
    private bool isPlayingVoiceLine = false;
    private bool wasInChaseRange = false;

    [Header("UI")]
    public Slider enemyHealth;
    public GameObject enemyHealthScreen;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        PLAYER = GameObject.FindGameObjectWithTag("Player");
        suplexController = PLAYER.GetComponent<SuplexController>();

        //grabHitbox.SetActive(false);

        // ----- get macros components ----- //
        _MacroAgent = macro.GetComponent<NavMeshAgent>();
        _MacrosRb = macro.GetComponent<Rigidbody>();
        _MacroEnemy = macro.GetComponent<MacroBoss>();

        lowerRoom = FindFirstObjectByType<LowerRoom>();

        // ----- call glowmesh script on prefab----- //
        _glowMesh = GetComponent<GlowMesh>();
        if (_glowMesh == null)
        {
                Debug.LogError("GlowMesh component not found on MacroPrefab or its children.");
        }

        chasePeriod = UnityEngine.Random.Range(chasePeriodMean -5f , chasePeriodMean + 5f);
        stateTimer = chasePeriod;
    }
    public void Update()
    {
        if (agent.enabled)
        {
            if (!throwingMacro && _MacroEnemy.agent.enabled == true)
                stateTimer -= Time.deltaTime;

            if (stateTimer <= 0f && !throwingMacro)
            {
                //Debug.Log("State timer expired, throwing macro");
                //grabHitbox.SetActive(true);
                throwingMacro = true;
                _MacroEnemy.wasThrown = false;
                chasePeriod = UnityEngine.Random.Range(chasePeriodMean - 5f, chasePeriodMean + 5f);
                StartCoroutine(ThrowMacro());
            }
            FaceTarget();
            if (!isPlayingVoiceLine)
            {
                StartCoroutine(PlayHealthBasedVoiceLine());
            }
        }
        if (enemyHealth.value <= 0)
        {
            Death();
        }
    }
    public void FaceTarget()
    {
        var TurnToTarget = agent.steeringTarget;
        Vector3 direction = (TurnToTarget - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
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
        //Debug.Log("Initiating ThrowMacro sequence");
        // Store original mesh rotation to restore later
        Quaternion originalMeshRotation = macrosmesh != null ? macrosmesh.localRotation : Quaternion.identity;
        AudioManager.PlayMicroPrepareAttack();

        // ----- Position macro prefab at throw origin ----- //
        yield return new WaitUntil(() => Vector3.Distance(Macro.transform.position, transform.position) <= grabRange);
        Debug.Log("Macro is within grab range, proceeding with throw");


        stateTimer = chasePeriod;

        var origin = (throwOrigin != null) ? throwOrigin : this.transform;
        _MacroEnemy.EnterCarriedState(origin); // disable macro's NavMeshAgent to prevent interference with throw
        //macro.transform.position = origin.position;
        macro.transform.rotation = Quaternion.Euler(90f, origin.rotation.eulerAngles.y, 0f);
        //macro.transform.SetParent(origin);


        //---- Hold macro for x seconds--//
        float throwTimer = 0f;
        while (throwTimer < throwInterval)
        {
            throwTimer += Time.deltaTime;
            yield return null;
        }

        //macro.transform.SetParent(null); // unparent macro before throw


        // ----- Calculate throw direction and apply force ----- //
        Vector3 dir = (PLAYER.transform.position - macro.transform.position).normalized;
        //_MacrosRb.AddForce(dir * throwForce , ForceMode.Impulse);
        _MacroEnemy.ExitCarriedState(dir * throwForce); // re-enable macro's NavMeshAgent after throw
        _MacroEnemy.wasThrown = true; // flag macro as thrown
        throwingMacro = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Shockwave") && suplexController.suplexedObject.name == "Macro")
        {
            //enemyHealth.value -= 1;
            Debug.Log("Macro hit by shockwave, applying damage to Micro");
        }
    }
    private void Death()
    {
        // Disable this boss functionality
        agent.enabled = false;
        rb.isKinematic = false;

        _glowMesh.SetGlowColor(); // trigger glow effect on death
        lowerRoom.EnableArrows();// enable arrows to show path to next area
        lowerRoom.MoveDown();

        this.gameObject.tag = "Enemy";

        enemyHealthScreen.SetActive(false);
        Destroy(macro);
    }
}
