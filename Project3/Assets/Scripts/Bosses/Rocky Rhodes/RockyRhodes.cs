using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
// ~ Ovi. 



// TODO: Develop the principle mechnics

// TODO: jump slam attack, dash knock up,
//TODO: make rocky jump to next platform after depleting certain amount of health and adding more health when on new platform.

public enum RockyRhodesStates
{
    Idle,
    //-- base attack states--//
    BullRush,
    Haymaker,
    Chestbump,
    HeelTaunt,
    //-- arena 1 states--//
    RopeRush,
    //-- arena 2 states--//
    CannonBall,
    //-- arena 3 states--//
    EnhancedRopeRush,
    Deadlift,
    DesperationFlurry,
    QTEMode,
    Dead,
}

[DisallowMultipleComponent]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class RockyRhodes : EnemyBase
{
    [Header("Rocky Rhodes Settings")]
    // runtime toggles
    [SerializeField] private InGameMenuManager inGameMenuManager;
    [SerializeField] RhockyHealth rhockyHealth;
    public bool abilitiesEnabled = false; // when true the state machine runs
    private RhockyAbilities _abilities;

    [Header("Visual References")]
    public Transform RockyRhodesMesh;
    public Quaternion originalMeshRotation;
    public GameObject RockyRhodesHealthBarUI;
   

 

    [Header("Jump Patrol Settings")]
    public float jumpPatrolForce = 40f;
    public float jumpPatrolHorizontalForce = 15f;
    public float jumpPatrolCooldown = 3f;
    private int _currentJumpIndex = 0;
    public bool isJumping = false;
    public List<Transform> JumpPoints = new List<Transform>();
    public Transform Recoverypoint;

    [Header("QTE trigger")]
    public  QTESystem QTESystemScript;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    new  void Start()
    {
        base.Start();
        if (QTESystemScript == null) QTESystemScript = GetComponent<QTESystem>();
        if (_abilities == null) _abilities = GetComponent<RhockyAbilities>();
        rhockyHealth = GetComponent<RhockyHealth>();
        originalMeshRotation = RockyRhodesMesh != null ? RockyRhodesMesh.localRotation : Quaternion.identity;
     if(inGameMenuManager==null) GetComponent<InGameMenuManager>();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }
    public override void SetGrabbed(bool grabbed) // custom grab condition for Rhocky :enemybase
    {
        base.SetGrabbed(grabbed);
        
        RockyRhodesManager manager = GetComponent<RockyRhodesManager>();

        if(grabbed)
        {
            ToggleBehaviors(false);
            _abilities.InterruptAbility(true);
            rb.isKinematic = true;
        }
        else
        {
            rhockyHealth.TakeDamage();
            ToggleBehaviors(true);
            this.gameObject.tag="Untagged"; // untag so player can't accidentally re-grab while recovering

            if (_abilities != null && _abilities.CurrentRockyState != RockyRhodesStates.QTEMode)
            {
                _abilities.CheckState(RockyRhodesStates.Idle);
            }
        }
    }
    protected override void CustomAttack() // Rhocky's custom attack behavior :enemybase
    {
        if (!abilitiesEnabled) return;

        // If the QTE started, force him into the QTE state
        if (QTESystemScript.EnableQuickTimeEvent)
        {
            if (_abilities.CurrentRockyState != RockyRhodesStates.QTEMode)
            {
                Debug.Log("Forcing Rocky into QTE Mode state.");
                _abilities.IsPerformingAbility = false;
                _abilities.CheckState(RockyRhodesStates.QTEMode);
            }
        }
        // Otherwise, just let him casually run his normal state
        else
        {
            if (_abilities.CurrentRockyState != RockyRhodesStates.QTEMode)
            {
                _abilities.CheckState(_abilities.CurrentRockyState);
            }
        }
    }
    public void Dead()
    {
               Debug.Log("Rocky Rhodes is Dead");
        RockyRhodesHealthBarUI.SetActive(false);
        this.gameObject.SetActive(false);
        inGameMenuManager.WinScreen();
    }
    public void ToggleBehaviors( bool IsEnabled) // disabling rocky States switch
    {
        Debug.Log("Toggling Rocky's behaviors: " + (IsEnabled ? "ENABLED" : "DISABLED"));
        // Disable AI behaviors
        canAttack = IsEnabled;
        canChase = IsEnabled;
       // canPatrol = IsEnabled;

        // Disable NavMesh
        agent.enabled = IsEnabled;
        rb.isKinematic = IsEnabled;
     //   rb.useGravity = IsEnabled;
    }

  /*  public IEnumerator JumpToPlatform() 
    {
        isJumping = true;

        // 1. CLEAR CURRENT ACTIONS
        ToggleBehaviors(false);
        if (_abilities != null)
        {
            _abilities.InterruptAbility(true); // Stop any rogue attacks
        }

        // 2. BOUNCE TO THE ROOF (Recoverypoint) FIRST
        if (Recoverypoint != null)
        {
            Debug.Log("Jumping to the ROOF!");
            yield return StartCoroutine(PerformJump(Recoverypoint));
            
            // Wait on the roof for a moment to catch his breath
            yield return new WaitForSeconds(1.5f);
        }

        // 3. PICK THE NEXT STAGE AND JUMP DOWN TO IT
        if (JumpPoints != null && JumpPoints.Count > 0)
        {
            Transform targetPoint = JumpPoints[_currentJumpIndex];
            _currentJumpIndex = (_currentJumpIndex + 1) % JumpPoints.Count;

            Debug.Log("Jumping down to the NEXT STAGE!");
            yield return StartCoroutine(PerformJump(targetPoint));
        }

        // 4. WAKE UP ON THE NEW STAGE
        rb.isKinematic = true;
        agent.enabled = true;
        ToggleBehaviors(true);
        isJumping = false;

        // Give the AI back control
        if (_abilities != null && _abilities.CurrentRockyState != RockyRhodesStates.QTEMode)
        {
            _abilities.CheckState(RockyRhodesStates.Idle);
        }
    }
    */
    // Helper Coroutine: Actually just calculates the math and moves him!
 /*   private IEnumerator PerformJump(Transform targetPoint)
    {
        // Disable NavMesh so physics can freely drive movement
        agent.enabled = false;
        rb.isKinematic = false;
        IgnoreGroundCheck = true;

        Vector3 displacement = targetPoint.position - transform.position;
        float gravity = Physics.gravity.magnitude;
        float horizontalDist = new Vector3(displacement.x, 0f, displacement.z).magnitude;
        float verticalDist = displacement.y;

        float flightTime = Mathf.Clamp(horizontalDist / 10f, 0.6f, 2f);

        float velocityY = (verticalDist + 0.5f * gravity * flightTime * flightTime) / flightTime;
        Vector3 horizontalDir = new Vector3(displacement.x, 0f, displacement.z).normalized;
        float velocityXZ = horizontalDist / flightTime;

        Vector3 launchVelocity = (horizontalDir * velocityXZ) + (Vector3.up * velocityY);

        rb.linearVelocity = Vector3.zero; // clear any existing velocity
        rb.AddForce(launchVelocity, ForceMode.VelocityChange);

        yield return new WaitForSeconds(0.5f); // let Rocky leave the ground
        IgnoreGroundCheck = false;

        // Wait until landed
        while (!IsEnemyGrounded())
        {
            // Spin through the air
            if (!isGrabbed && !isPushed)
            {
                RockyRhodesMesh.Rotate(Vector3.forward, 1000f * Time.deltaTime, Space.World);
            }
            yield return null;
        }

        // Snap rotation back to normal upon landing
        RockyRhodesMesh.localRotation = originalMeshRotation;
    } */
    // rocky rhodes begines its jumping here
  /*  public override void RandomPatrolDestination()
    {
        if (isJumping) return; // already mid-jump or in cooldown, wait
        if (JumpPoints == null || JumpPoints.Count == 0) return;
        if (QTESystemScript.EnableQuickTimeEvent) return; // QTE is active, don't jump away
        StartCoroutine(JumpToPlatform());    
    }*/

 /*   public void JumpAway()
    {
        isJumping = false;
        StopCoroutine(nameof(JumpToPlatform)); // kill any lingering jump coroutine
        StartCoroutine(JumpToPlatform());
    }*/

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the boss just hit the player while performing the Chestbump ability
        if (collision.gameObject.CompareTag("Player") && _abilities != null)
        {
            if (_abilities.CurrentRockyState == RockyRhodesStates.Chestbump)
            {
                PlayerMovement playerMovement = collision.gameObject.GetComponent<PlayerMovement>();
                if (playerMovement != null)
                {
                    Debug.Log("Chest bump hit! Applying damage AND knockback.");

                    // Normal horizontal push direction away from Rocky
                    Vector3 knockbackDir = (collision.transform.position - transform.position).normalized;
                    knockbackDir.y = 0f;

                    float knockbackForce = 15f;
                    float upwardForce = 15f;

                    // Apply knockback to the player's velocity
                    playerMovement.velocity = (knockbackDir * knockbackForce) + (Vector3.up * upwardForce);

                    // (Optional) Stop Rocky instantly so he doesn't slide past the player
                    rb.linearVelocity = Vector3.zero;
                }
            }
        }
    }
    }
