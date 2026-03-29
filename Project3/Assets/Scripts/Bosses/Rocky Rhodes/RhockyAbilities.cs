using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(RockyRhodes))]
public class RhockyAbilities : MonoBehaviour
{
    [Header("Ability Settings")]
    public float AbilityCooldown = 2f;

    [Header("Launch Settings")]
    public float jumpForce = 55f;
    private Vector3 dashForce;
    [Header("Bull Rush Settings")]
    [SerializeField] private float bullRushSpeed = 20f;
    [SerializeField] private float bullRushDuration = 0.6f;

    public bool IsPerformingAbility = false;
    private float _abilityTimer = 0f;
    public Transform PlayerTarget;


    private RockyRhodes _rockyRhodes;
    private Coroutine _currentStateCoroutine;

    public RockyRhodesStates CurrentRockyState;

    public List<RockyRhodesStates> _randomSelection = new List<RockyRhodesStates>
    {
        RockyRhodesStates.CannonBall,
        RockyRhodesStates.BullRush,
        RockyRhodesStates.Haymaker,
    };
   [SerializeField] private QteCollideSensor qteCollideSensorScript;
    private void Awake()
    {
        _rockyRhodes = GetComponent<RockyRhodes>();
       if(qteCollideSensorScript ==null) qteCollideSensorScript = GetComponent<QteCollideSensor>();
        //  CurrentRockyState = RockyRhodesStates.Regular;
    }

    public void InterruptAbility(bool PauseAbility)
    {
        //  if (!IsPerformingAbility) return;
        if (PauseAbility && _currentStateCoroutine != null)
        {
            StopAllCoroutines();
            _currentStateCoroutine = null;
            IsPerformingAbility = false;
            //   CheckState(RockyRhodesStates.Regular);
            Debug.Log("Ability interrupted by grab/push. Stopping ability and waiting for release.");
        }
      
    }

    public void CheckState(RockyRhodesStates states)
    {
        if (CurrentRockyState == states && _currentStateCoroutine != null)
        {
            return;
        }

        if (_currentStateCoroutine != null)
        {
            StopCoroutine(_currentStateCoroutine);
            _currentStateCoroutine = null;
        }
        CurrentRockyState = states;
        switch (states)
        {
            case RockyRhodesStates.Regular:
                _currentStateCoroutine = StartCoroutine(AbilityChoose());
                break;
            case RockyRhodesStates.CannonBall:
                _currentStateCoroutine = StartCoroutine(CannonBall());
                break;
            case RockyRhodesStates.BullRush:
                _currentStateCoroutine = StartCoroutine(BullRush());
                break;
             case RockyRhodesStates.Haymaker:
                _currentStateCoroutine = StartCoroutine(Haymaker());
                break;
            case RockyRhodesStates.QTEMode:
                _currentStateCoroutine = StartCoroutine(QTE());
                break;
   
        }
    }
    public IEnumerator QTE()
    {
        InterruptAbility(true); // Interrupt any ongoing ability when entering QTE mode
        qteCollideSensorScript.QTETriggerCollider.enabled = false;
        // Wait until QTE finishes
        while (_rockyRhodes.QTESystemScript != null &&
               _rockyRhodes.QTESystemScript.EnableQuickTimeEvent)
        {
            Debug.Log("QTE State Active");
            yield return null;
        }

        // Resume normal AI AFTER QTE
        InterruptAbility(false); // Resume normal AI after QTE ends
        CheckState(RockyRhodesStates.Regular);
    }
    // --------------------------------------- Main  abilities ------------------------------------//
    public IEnumerator AbilityChoose()
    {
        Debug.Log("Regular State Active");
        yield return new WaitForSeconds(AbilityCooldown);
        int randomIndex = Random.Range(0, _randomSelection.Count);
        //   CurrentRockyState = _randomSelection[randomIndex];
        CheckState(_randomSelection[randomIndex]);
        yield return null;
    }
    public IEnumerator BullRush()
    {
        Debug.Log("Bull Rush State Active");
        if (PlayerTarget == null) yield break;

        IsPerformingAbility = true;
        _rockyRhodes.ToggleBehaviors(false);
        _rockyRhodes.IgnoreGroundCheck = true;

        qteCollideSensorScript.QTETriggerCollider.enabled = true; // Enable the QTE trigger collider for the Bull Rush attack

         if ( _rockyRhodes.QTESystemScript.EnableQuickTimeEvent)
        {
            Debug.Log("Player successfully triggered QTE during Bull Rush! Transitioning to QTE mode.");
            _rockyRhodes.ToggleBehaviors(false);
            IsPerformingAbility = false;
       qteCollideSensorScript.QTETriggerCollider.enabled = false;
            yield break;
        }
        yield return new WaitForSeconds(5f); // charge-up delay
    qteCollideSensorScript.QTETriggerCollider.enabled = false; // Enable the QTE trigger collider for the Bull Rush attack
        Vector3 toTarget = PlayerTarget.position - transform.position;
        toTarget.y = 0f;
        toTarget.Normalize();

        float timer = 0f;
        while (timer < bullRushDuration)
        {
            if (!_rockyRhodes.isGrabbed && !_rockyRhodes.isPushed)
            {
                _rockyRhodes.rb.linearVelocity = toTarget * bullRushSpeed;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        _rockyRhodes.rb.linearVelocity = Vector3.zero;
        _rockyRhodes.IgnoreGroundCheck = false;
        _rockyRhodes.ToggleBehaviors(true);
        IsPerformingAbility = false;

        CheckState(RockyRhodesStates.Regular);
    }
    public IEnumerator Haymaker()
    {

        Debug.Log("Bull Rush State Active");
        if (PlayerTarget == null) yield break;

        IsPerformingAbility = true;
        _rockyRhodes.ToggleBehaviors(false);
        _rockyRhodes.IgnoreGroundCheck = true;

        Vector3 toTarget = PlayerTarget.position - transform.position;
        toTarget.y = 0f;
        toTarget.Normalize();

        float timer = 0f;
        while (timer < bullRushDuration)
        {
            if (!_rockyRhodes.isGrabbed && !_rockyRhodes.isPushed)
            {
                _rockyRhodes.rb.linearVelocity = toTarget * bullRushSpeed;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        _rockyRhodes.rb.linearVelocity = Vector3.zero;
        _rockyRhodes.IgnoreGroundCheck = false;
        _rockyRhodes.ToggleBehaviors(true);
        IsPerformingAbility = false;

        CheckState(RockyRhodesStates.Regular);
    }
    public IEnumerator ChestBump()
    {
        yield return new WaitForSeconds(AbilityCooldown);
    }
    public IEnumerator HeelTaunt()
    {
        yield return new WaitForSeconds(AbilityCooldown);
    }

    // --------------------------------------- Arena 1 abilities ------------------------------------//
    public IEnumerator RopeRush()
    {
        yield return new WaitForSeconds(AbilityCooldown);
    }
    // --------------------------------------- Arena 2 abilities ------------------------------------//
    public IEnumerator CannonBall()
    {
        Debug.Log("Cannon Ball State Active");
        if (PlayerTarget == null) yield break;

        IsPerformingAbility = true;
        _rockyRhodes.ToggleBehaviors(false);
        _rockyRhodes.IgnoreGroundCheck = true;

        Vector3 toTarget = PlayerTarget.position - transform.position;
        float gravity = Mathf.Abs(Physics.gravity.y);

        Vector3 toTargetXZ = new Vector3(toTarget.x, 0f, toTarget.z);
        float horizontalDist = toTargetXZ.magnitude;

        float apex = Mathf.Max(jumpForce, toTarget.y + 0.5f);

        float timeUp = Mathf.Sqrt(2f * apex / gravity);
        float timeDown = Mathf.Sqrt(2f * Mathf.Max(0.01f, apex - toTarget.y) / gravity);
        float totalTime = timeUp + timeDown;

        float vy = gravity * timeUp;
        Vector3 vxz = toTargetXZ / Mathf.Max(0.01f, totalTime);

        Vector3 launchVelocity = vxz + Vector3.up * vy;

        _rockyRhodes.rb.linearVelocity = Vector3.zero;
        _rockyRhodes.rb.AddForce(launchVelocity, ForceMode.VelocityChange);

        yield return new WaitForSeconds(0.5f);
        _rockyRhodes.IgnoreGroundCheck = false;

        while (!_rockyRhodes.IsEnemyGrounded())
        {
            if (_rockyRhodes.rb.linearVelocity.y < 0f)
            {
                float diveSpeed = Mathf.Max(jumpForce, 1f);
                float NewY = -diveSpeed;
                Vector3 horizontal = new Vector3(_rockyRhodes.rb.linearVelocity.x, 0f, _rockyRhodes.rb.linearVelocity.z);

                Vector3 seek = PlayerTarget.position - transform.position;
                seek.y = 0f;

                if (seek.sqrMagnitude > 0.01f)
                {
                    Vector3 desired = seek.normalized * diveSpeed;
                    horizontal = Vector3.MoveTowards(horizontal, desired, diveSpeed * Time.deltaTime);
                }

                _rockyRhodes.rb.linearVelocity = new Vector3(horizontal.x, _rockyRhodes.rb.linearVelocity.y, horizontal.z);
            }

            yield return null;
        }

        _rockyRhodes.RockyRhodesMesh.localRotation = _rockyRhodes.originalMeshRotation;
        _rockyRhodes.ToggleBehaviors(true);
        IsPerformingAbility = false;
        CheckState(RockyRhodesStates.Regular);
    }
    // --------------------------------------- Arena 3 abilities ------------------------------------//
    public IEnumerator DeadLiftSuplex()
    {
        yield return new WaitForSeconds(AbilityCooldown);
    }
    public IEnumerator DesperationFlurry()
    {
        yield return new WaitForSeconds(AbilityCooldown);
    }






}
