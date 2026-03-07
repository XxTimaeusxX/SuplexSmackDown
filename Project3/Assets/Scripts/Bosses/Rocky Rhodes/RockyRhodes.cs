using JetBrains.Annotations;
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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
  new  void Start()
    {
        // default orientation of mesh, used to reset rotation after abilities
        originalMeshRotation = RockyRhodesMesh != null ? RockyRhodesMesh.localRotation : Quaternion.identity;
        CheckState(RockyRhodesStates.Regular);
    }

    // Update is called once per frame
    public override void  Update()
    {
        base.Update();
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
            Debug.Log("Boulder Eruption -in motion.");
            // Rotate mesh continuously
            RockyRhodesMesh.Rotate(Vector3.forward, 1000f * Time.deltaTime, Space.World);
            yield return null;
        }
      
        RockyRhodesMesh.localRotation = originalMeshRotation; // Reset mesh rotation after ability
        yield return new WaitForSeconds(AbilityCooldown);


        if (isGrabbed || isPushed) yield break;

        // Re-enable everything
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
            Debug.Log("Bull rock -in motion.");
            // Rotate mesh continuously
            RockyRhodesMesh.Rotate(Vector3.up *1400f * Time.deltaTime, Space.World);
            yield return null;
        }

        RockyRhodesMesh.localRotation = originalMeshRotation; // Reset mesh rotation after ability
        yield return new WaitForSeconds(AbilityCooldown);
        if (isGrabbed || isPushed) yield break;

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
        canAttack = IsEnabled;
        canChase = IsEnabled;
        canPatrol = IsEnabled;

        // Disable NavMesh
        agent.enabled = IsEnabled;
        rb.isKinematic = IsEnabled;
    }
}
