using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
// ~ Ovi. 



// TODO: Develop the principle mechnics

// TODO: jump slam attack, dash knock up,
//TODO: change tag to damageplayer when rocket hop jumps to player

public enum RockyRhodesStates
{
    Regular,
    BoulderEruption,
    BullRock,
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
    private bool abilitiesEnabled = false; // when true the state machine runs
    public RockyRhodesStates CurrentRockyState;
    private List<RockyRhodesStates> _RandomSelection= new List<RockyRhodesStates>
    {
        RockyRhodesStates.BoulderEruption,
        RockyRhodesStates.BullRock,
    };

    [Header("Visual References")]
    public Transform RockyRhodesMesh;
    private Quaternion originalMeshRotation;

    [Header ("Launch Settings")]
    public float jumpForce = 55f;
    private Vector3 dashForce;
    public float AbilityCooldown = 5f;
    public bool IsPerformingAbility = false;
    private float _abilityTimer = 0f;
    private Coroutine _currentStateCoroutine;
    [SerializeField]private Transform PlayerTarget;
    public List<Transform> JumpPoints = new List<Transform>();

    [Header("Jump Patrol Settings")]
    public float jumpPatrolForce = 40f;
    public float jumpPatrolHorizontalForce = 15f;
    public float jumpPatrolCooldown = 3f;
    private int _currentJumpIndex = 0;
    private bool _isJumping = false;


    [Header("QTE trigger")]
    [SerializeField] QTESystem QTESystemScript;
    public Collider QTETriggerCollider;
    private string _playerTag = "Player";


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    new  void Start()
    {
        base.Start();
        // default orientation of mesh, used to reset rotation after abilities
        QTETriggerCollider.isTrigger = true; // make QTE a triggerenter type 
        if (QTESystemScript == null) QTESystemScript = GetComponent<QTESystem>();
        originalMeshRotation = RockyRhodesMesh != null ? RockyRhodesMesh.localRotation : Quaternion.identity;
       // CheckState(RockyRhodesStates.Regular);
    }

    // Update is called once per frame
    public override void  Update()
    {
        base.Update();
        if(IsPerformingAbility &&(isGrabbed || isPushed))
        {
            ToggleBehaviors(false); // Ensure behaviors remain disabled while performing ability and being grabbed/pushed
            StopAllCoroutines(); // Stop current ability coroutine to prevent conflicts
             _currentStateCoroutine = null;
             Debug.Log("Ability interrupted by grab/push. Stopping ability and waiting for release.");
        }
       
    }

    public override void BaseAttack()
    {
        if (abilitiesEnabled) {CheckState(CurrentRockyState); }
        
    }
    public void CheckState(RockyRhodesStates states)
    {
        if (_currentStateCoroutine != null)
        {
            StopCoroutine(_currentStateCoroutine);
            _currentStateCoroutine = null;
        }
        CurrentRockyState = states;
        switch (states)
        {
            case RockyRhodesStates.Regular:
             _currentStateCoroutine =  StartCoroutine(Regular());
                break;
            case RockyRhodesStates.BoulderEruption:
             _currentStateCoroutine =  StartCoroutine(BoulderEruption());
                break;
            case RockyRhodesStates.BullRock:
             _currentStateCoroutine = StartCoroutine(BullRock());
                break;
        }
     }

    public IEnumerator Regular()
    {  
        Debug.Log("Regular State Active");
        // Wait for cooldown
        yield return new WaitForSeconds(AbilityCooldown);
        int randomIndex = Random.Range(0, _RandomSelection.Count);
        CurrentRockyState = _RandomSelection[randomIndex];
        CheckState(CurrentRockyState);
        yield return null;
    }
    public IEnumerator BoulderEruption()
    {

        // Vector3 toTarget = _homingTarget.position - transform.position; // include vertical
       //  dashForce = transform.forward; 
        IsPerformingAbility = true; // Prevent EnemyBase from re-enabling agent
        ToggleBehaviors(false); // Disable AI behaviors and NavMesh
        IgnoreGroundCheck = true; // Prevent ground check interference during ability

        // --- lock to player logic -- // 
        Vector3 toTarget = (PlayerTarget.position - transform.position);
         toTarget.y = 0f; // Ignore vertical difference for horizontal movement
        //toTarget.x = 0f;
        toTarget.Normalize();
        rb.AddForce((Vector3.up * jumpForce*1f) + (Vector3.forward+ toTarget*20f ), ForceMode.Impulse);
        // --- lock to player logic -- //

       yield return new WaitForSeconds(0.5f); // Short delay before allowing ground check to prevent early reset
                                              
        IgnoreGroundCheck = false; // enable ground check after initial jump to allow proper landing detection
        while ( !IsEnemyGrounded() && IsPerformingAbility)
        {
            // PAUSE: Wait while grabbed or pushed
            if (!isGrabbed && !isPushed)
            {
                Debug.Log("Boulder Eruption -in motion.");
                RockyRhodesMesh.Rotate(Vector3.forward, 1000f * Time.deltaTime, Space.World);

            }
            
            yield return null;
        }
      
        RockyRhodesMesh.localRotation = originalMeshRotation; // Reset mesh rotation after ability
        while (isGrabbed && isPushed)
        {
            yield return null;
        }
        yield return new WaitForSeconds(AbilityCooldown);


       

        // Re-enable everything
        Debug.Log("TOGGLING ---------------- BEHAVIORS-------------.");
        ToggleBehaviors(true);
        IsPerformingAbility = false; // Allow EnemyBase to control agent again
        CheckState(RockyRhodesStates.Regular);
    }

    public IEnumerator BullRock()
    {
        IsPerformingAbility = true;
        ToggleBehaviors(false);
        IgnoreGroundCheck = true; // Prevent ground check interference during ability
        rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);

        //  rb.angularVelocity = new Vector3(0f, 90f, 0f) * 5f * Time.fixedDeltaTime;
        yield return new WaitForSeconds(0.5f); // Short delay before allowing ground check to prevent early reset

        IgnoreGroundCheck = false; // enable ground check after initial jump to allow proper landing detection

        while (!IsEnemyGrounded() && IsPerformingAbility)
        {
            // PAUSE: Wait while grabbed or pushed
            if (!isGrabbed && !isPushed)
            {
                Debug.Log("Bull rock -in motion.");
                RockyRhodesMesh.Rotate(Vector3.up * 1400f * Time.deltaTime, Space.World);

            }
            
            yield return null;
        }

        RockyRhodesMesh.localRotation = originalMeshRotation; // Reset mesh rotation after ability
        while (isGrabbed && isPushed)
        {
            yield return null;
        }
        yield return new WaitForSeconds(AbilityCooldown);
 
        Debug.Log("TOGGLING ---------------- BEHAVIORS-------------.");
        ToggleBehaviors(true);
        IsPerformingAbility = false;
        CheckState(RockyRhodesStates.Regular);
    }
 
    public void Dead()
    {
               Debug.Log("Rocky Rhodes is Dead");
    }
    public void ToggleBehaviors( bool IsEnabled)
    {
        // Disable AI behaviors
       // canAttack = IsEnabled;
        canChase = IsEnabled;
       // canPatrol = IsEnabled;

        // Disable NavMesh
        agent.enabled = IsEnabled;
        rb.isKinematic = IsEnabled;
    }

    public IEnumerator JumpToPlatform()
    {
        _isJumping = true;

        // Pick the next point in the list, wrapping around
        Transform targetPoint = JumpPoints[_currentJumpIndex];
        _currentJumpIndex = (_currentJumpIndex + 1) % JumpPoints.Count;

        // Disable NavMesh so physics can drive movement
        agent.enabled = false;
        rb.isKinematic = false;
        IgnoreGroundCheck = true;

        // Calculate required velocity to reach target point
        Vector3 displacement = targetPoint.position - transform.position;
        float gravity = Physics.gravity.magnitude;
        float horizontalDist = new Vector3(displacement.x, 0f, displacement.z).magnitude;
        float verticalDist = displacement.y;

        // Scale flight time with distance so short hops are quick and long ones arc higher
        float flightTime = Mathf.Clamp(horizontalDist / 10f, 0.6f, 2f);

        // v_y = (?y + 0.5 * g * t²) / t  — needed vertical speed to arrive at target height
        float velocityY = (verticalDist + 0.5f * gravity * flightTime * flightTime) / flightTime;

        // v_xz = ?xz / t — needed horizontal speed to cover the ground distance
        Vector3 horizontalDir = new Vector3(displacement.x, 0f, displacement.z).normalized;
        float velocityXZ = horizontalDist / flightTime;

        Vector3 launchVelocity = (horizontalDir * velocityXZ) + (Vector3.up * velocityY);

        rb.linearVelocity = Vector3.zero; // clear any existing velocity
        rb.AddForce(launchVelocity, ForceMode.VelocityChange);
        Debug.Log("JumpToPlatform - jumping to point " + (_currentJumpIndex) + " | flight time: " + flightTime);

        yield return new WaitForSeconds(0.5f); // let Rocky leave the ground before checking landing
        IgnoreGroundCheck = false;

        // Wait until landed
        while (!IsEnemyGrounded())
        {
            if (!isGrabbed && !isPushed)
            {
                RockyRhodesMesh.Rotate(Vector3.forward, 1000f * Time.deltaTime, Space.World);
            }
            yield return null;
        }

        RockyRhodesMesh.localRotation = originalMeshRotation;

        // Wait while grabbed or pushed before re-enabling
        while (isGrabbed || isPushed)
        {
            yield return null;
        }

        // Re-enable NavMesh after landing
        rb.isKinematic = true;
        agent.enabled = true;

        // Cooldown before the next jump
        yield return new WaitForSeconds(jumpPatrolCooldown);

        _isJumping = false;
    }
    public override void RandomPatrolDestination()
    {
        if (_isJumping) return; // already mid-jump or in cooldown, wait
        if (JumpPoints == null || JumpPoints.Count == 0) return;
        StartCoroutine(JumpToPlatform());    
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag(_playerTag) )
        {
            Debug.Log("Player hit by Boulder Eruption!");
        //    QTESystemScript.EnableQuickTimeEvent = true;
                QTESystemScript.StartQTE();
            // Implement damage logic here
        }
    }
    private void OnTriggerExit(Collider other) {
        if(other.CompareTag(_playerTag) )
        {
            Debug.Log("Player exited Boulder Eruption area.");
         //   QTESystemScript.EnableQuickTimeEvent = false;
         QTESystemScript.StopQTE();
            // Implement logic for exiting QTE area if needed
        }
    }
}
