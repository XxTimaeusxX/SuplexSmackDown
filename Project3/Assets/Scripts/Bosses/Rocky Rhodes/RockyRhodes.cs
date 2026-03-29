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
    Regular,
    CannonBall,
    BullRush,
    Haymaker,
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

    [Header("QTE trigger")]
    public  QTESystem QTESystemScript;
    private string _playerTag = "Player";
    private int _PlayerOverLapCount = 0; // track how many colliders of the player are overlapping the QTE trigger
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    new  void Start()
    {
        base.Start();
        if (QTESystemScript == null) QTESystemScript = GetComponent<QTESystem>();
        if (_abilities == null) _abilities = GetComponent<RhockyAbilities>();
        originalMeshRotation = RockyRhodesMesh != null ? RockyRhodesMesh.localRotation : Quaternion.identity;
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        if (_abilities != null && _abilities.IsPerformingAbility && (isGrabbed || isPushed))
        {
            ToggleBehaviors(false);
           // _abilities.InterruptAbility();
        }
    }

    protected override void CustomAttack()
    {
        if (abilitiesEnabled && !QTESystemScript.EnableQuickTimeEvent)
        {
            Debug.Log("sssssssssssssssssssssssss");
            // Only enforce the current state if he's NOT in QTE mode
          
                _abilities.CheckState(_abilities.CurrentRockyState);
            
        }
        else if (QTESystemScript.EnableQuickTimeEvent) // Make sure this else-if is here!
        {
            if (_abilities.CurrentRockyState != RockyRhodesStates.QTEMode)
            {
                Debug.Log("Forcing Rocky into QTE Mode state.");
                _abilities.IsPerformingAbility = false;
                _abilities.CheckState(RockyRhodesStates.QTEMode);
            }
        }
    }
    public void Dead()
    {
               Debug.Log("Rocky Rhodes is Dead");
        RockyRhodesHealthBarUI.SetActive(false);
        this.gameObject.SetActive(false);
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
        isJumping = true;
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

        // v_y = (y + 0.5 * g * t²) / t  — needed vertical speed to arrive at target height
        float velocityY = (verticalDist + 0.5f * gravity * flightTime * flightTime) / flightTime;

        // v_xz = xz / t — needed horizontal speed to cover the ground distance
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
      //  yield return new WaitForSeconds(jumpPatrolCooldown);

        isJumping = false;
        
    }

    // rocky rhodes begines its jumping here
  /*  public override void RandomPatrolDestination()
    {
        if (isJumping) return; // already mid-jump or in cooldown, wait
        if (JumpPoints == null || JumpPoints.Count == 0) return;
        if (QTESystemScript.EnableQuickTimeEvent) return; // QTE is active, don't jump away
        StartCoroutine(JumpToPlatform());    
    }*/

    public void JumpAway()
    {
        isJumping = false;
        StopCoroutine(nameof(JumpToPlatform)); // kill any lingering jump coroutine
        StartCoroutine(JumpToPlatform());
    }
    private void OnTriggerEnter(Collider other)
    {
       //if doing abilities player take damage
    }
    private void OnTriggerExit(Collider other)
    {

    }
   
}
