using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
// ~ Ovi. 



// TODO: Develop the principle mechnics

// TODO: jump slam attack, dash knock up, 

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

    public float jumpForce = 55f;
    public float AbilityCooldown = 5f;
    public bool IsPerformingAbility = false;
    private float _abilityTimer = 0f;
    private Coroutine _currentStateCoroutine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
  new  void Start()
    {
        CheckState(RockyRhodesStates.Regular);
    }

    // Update is called once per frame
    public override void  Update()
    {
        base.Update();
       if(IsPerformingAbility && agent.enabled) agent.enabled = false;
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
        IsPerformingAbility = true; // Prevent EnemyBase from re-enabling agent
        ToggleBehaviors(false); // Disable AI behaviors and NavMesh
        IgnoreGroundCheck = true; // Prevent ground check interference during ability
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        rb.angularVelocity = new Vector3(90f, 00f, 0f) * 5f * Time.fixedDeltaTime;
        float timer = 0f;
            while(timer < 5f)
        {
            Debug.Log("Boulder Eruption - Agent disabled, waiting 4 seconds...");
            timer += Time.fixedDeltaTime;
            yield return null;
        }
       
        

        IgnoreGroundCheck = false;
        yield return new WaitForSeconds(AbilityCooldown);
        Debug.Log("Boulder Eruption - 4 seconds passed");
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
        rb.angularVelocity = new Vector3(0f, 90f, 0f) * 5f * Time.fixedDeltaTime;
        float timer = 0f;
        while (timer < 5f)
        {
            Debug.Log("Bull Rock - Agent disabled, waiting for cooldown...");
            timer += Time.fixedDeltaTime;
            yield return null;
        }

        IgnoreGroundCheck = false; // Prevent ground check interference during ability
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
