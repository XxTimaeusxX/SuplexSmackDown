using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(RockyRhodes))]
public class RhockyAbilities : MonoBehaviour
{
    [Header("Ability Settings")]
    public float AbilityCooldown = 5f;

    [Header("Launch Settings")]
    public float jumpForce = 55f;
    private Vector3 dashForce;
    [Header("Bull Rush Settings")]
    [SerializeField] private float bullRushSpeed = 20f;
    [SerializeField] private float bullRushDuration = 0.6f;

    [Header("Heel Taunt Settings")]
    public float tauntChargeDuration = 5f;
    public float speedMultiplierLevel = 5f;
    public bool isEnraged = false;
    public GameObject auraPlaceholder;

    public bool IsPerformingAbility = false;
    private float _abilityTimer = 0f;
    public Transform PlayerTarget;


    private RockyRhodes _rockyRhodes;
    private RockyAnimations _rockyAnimations;
    private Coroutine _currentStateCoroutine;

    public RockyRhodesStates CurrentRockyState;

    public List<RockyRhodesStates> _randomSelection = new List<RockyRhodesStates>
    {
       // RockyRhodesStates.CannonBall,
        RockyRhodesStates.BullRush,
        RockyRhodesStates.Haymaker,
        RockyRhodesStates.Chestbump,
    };
   [SerializeField] private QteCollideSensor qteCollideSensorScript;
    private void Awake()
    {
        _rockyRhodes = GetComponent<RockyRhodes>();
        _rockyAnimations = GetComponent<RockyAnimations>();
        if (qteCollideSensorScript ==null) qteCollideSensorScript = GetComponent<QteCollideSensor>();
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
        if(_rockyRhodes.gameObject.tag!= "Enemy")// Only reset tag if it's not set to "Enemy" to avoid unnecessary changes
        {
            _rockyRhodes.gameObject.tag = "Untagged"; // Reset tag to default at the start of any state change
        }
       
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
            case RockyRhodesStates.Idle:
                _currentStateCoroutine = StartCoroutine(AbilityChoose());
                break;
          /*  case RockyRhodesStates.CannonBall:
                _currentStateCoroutine = StartCoroutine(CannonBall());
                break;*/
            case RockyRhodesStates.BullRush:
                _currentStateCoroutine = StartCoroutine(BullRush());
                break;
             case RockyRhodesStates.Haymaker:
                _currentStateCoroutine = StartCoroutine(Haymaker());
                break;
            case RockyRhodesStates.Chestbump:   
                _currentStateCoroutine = StartCoroutine(ChestBump());
                break;
            case RockyRhodesStates.HeelTaunt:
                    _currentStateCoroutine = StartCoroutine(HeelTaunt());
                break;
            case RockyRhodesStates.QTEMode:
                _currentStateCoroutine = StartCoroutine(QTE());
                break;
   
        }
    }
    public IEnumerator QTE()
    {
      //  InterruptAbility(true); // Interrupt any ongoing ability when entering QTE mode
        qteCollideSensorScript.QTETriggerCollider.enabled = false;
        // Wait until QTE finishes
        while (_rockyRhodes.QTESystemScript != null &&
               _rockyRhodes.QTESystemScript.EnableQuickTimeEvent)
        {
            Debug.Log("QTE State Active");
            yield return null;
        }

        // Resume normal AI AFTER QTE
        IsPerformingAbility = false;
        _rockyRhodes.IgnoreGroundCheck = false;
        _rockyRhodes.ToggleBehaviors(true); // Ensure his AI is turned back on if it was off
        Debug.Log("QTE ended, resuming normal AI.");
       
        CheckState(RockyRhodesStates.Idle);
    }
    // --------------------------------------- Main  abilities ------------------------------------//
    public IEnumerator AbilityChoose()
    {
        if (CurrentRockyState == RockyRhodesStates.QTEMode) yield break;
        Debug.Log("Regular State Active");
        yield return new WaitForSeconds(AbilityCooldown);

        if (CurrentRockyState == RockyRhodesStates.QTEMode) yield break;
        int randomIndex = Random.Range(0, _randomSelection.Count);
        //   CurrentRockyState = _randomSelection[randomIndex];
        CheckState(_randomSelection[randomIndex]);
        yield return null;
    }
    public IEnumerator BullRush()
    {
        if (CurrentRockyState == RockyRhodesStates.QTEMode) yield break;
        Debug.Log("Bull Rush State Active");
        if (PlayerTarget == null) yield break;

        IsPerformingAbility = true;
        _rockyRhodes.ToggleBehaviors(false);
        _rockyRhodes.IgnoreGroundCheck = true;
        qteCollideSensorScript.ResetOverlap();
        qteCollideSensorScript.QTETriggerCollider.enabled = true; // Enable the QTE trigger collider for the Bull Rush attack
        if(qteCollideSensorScript.QTETriggerCollider == true)Debug.Log("QTE Trigger Collider enabled for Bull Rush.");
        if ( _rockyRhodes.QTESystemScript.EnableQuickTimeEvent)
        {
            Debug.Log("Player successfully triggered QTE during Bull Rush! Transitioning to QTE mode.");
            IsPerformingAbility = false;
       qteCollideSensorScript.QTETriggerCollider.enabled = false;
            CheckState(RockyRhodesStates.QTEMode);
            yield break;
        }
        yield return new WaitForSeconds(5f); // charge-up delay
    qteCollideSensorScript.QTETriggerCollider.enabled = false; // Enable the QTE trigger collider for the Bull Rush attack
        Vector3 toTarget = PlayerTarget.position - transform.position;
        toTarget.y = 0f;
        toTarget.Normalize();
        if(toTarget != Vector3.zero)
        {
            _rockyRhodes.RockyRhodesMesh.rotation = Quaternion.LookRotation(toTarget);
        }


        float timer = 0f;
        float currentSpeed = isEnraged ? (bullRushSpeed * speedMultiplierLevel) : bullRushSpeed;
        while (timer < bullRushDuration)
        {
            if (!_rockyRhodes.isGrabbed && !_rockyRhodes.isPushed)
            {
                _rockyRhodes.rb.linearVelocity = toTarget * currentSpeed;
                _rockyRhodes.gameObject.tag = "DamagePlayer";
            }

            timer += Time.deltaTime;
            yield return null;
        }

        _rockyRhodes.rb.linearVelocity = Vector3.zero;
        // Consume the rage buff
        if (isEnraged)
        {
            isEnraged = false;
            if (auraPlaceholder != null) auraPlaceholder.SetActive(false);
            Debug.Log("Rage consumed");
        }

        _rockyRhodes.IgnoreGroundCheck = false;
        _rockyRhodes.ToggleBehaviors(true);
        IsPerformingAbility = false;

        CheckState(RockyRhodesStates.Idle);
    }
    public IEnumerator Haymaker()
    {
        _rockyAnimations.ChangeAnimation("RockyPunchChargeUp_demo");
        if (CurrentRockyState == RockyRhodesStates.QTEMode) yield break;
        Debug.Log("HAYMAKER State Active");
        
        if (PlayerTarget == null) yield break;

        IsPerformingAbility = true;
        _rockyRhodes.ToggleBehaviors(false);
        _rockyRhodes.IgnoreGroundCheck = true;

        yield return new WaitForSeconds(3f); // charge-up delay effect
       
        Vector3 toTarget = PlayerTarget.position - transform.position;
        toTarget.y = 0f;
        toTarget.Normalize();
        if (toTarget != Vector3.zero)
        {
            _rockyRhodes.RockyRhodesMesh.rotation = Quaternion.LookRotation(toTarget);
        }
        float timer = 0f;
        float currentSpeed = isEnraged ? (bullRushSpeed * speedMultiplierLevel) : bullRushSpeed;
        while (timer < bullRushDuration)
        {
            if (!_rockyRhodes.isGrabbed && !_rockyRhodes.isPushed)
            {
                _rockyAnimations.ChangeAnimation("HayMaker_demo");
                _rockyRhodes.rb.linearVelocity = toTarget * currentSpeed;
                _rockyRhodes.gameObject.tag = "DamagePlayer";
            }

            timer += Time.deltaTime;
            yield return null;
        }

        _rockyRhodes.rb.linearVelocity = Vector3.zero;
        if (isEnraged)
        {
            isEnraged = false;
            if (auraPlaceholder != null) auraPlaceholder.SetActive(false);
            Debug.Log("Rage consumed");
        }

        _rockyRhodes.IgnoreGroundCheck = false;
        _rockyRhodes.ToggleBehaviors(true);
        IsPerformingAbility = false;

        CheckState(RockyRhodesStates.Idle);
    }
    public IEnumerator ChestBump()
    {
        if (CurrentRockyState == RockyRhodesStates.QTEMode) yield break;
        Debug.Log("CHEST BUMP State Active");

        if (PlayerTarget == null) yield break;

        IsPerformingAbility = true;
        _rockyRhodes.ToggleBehaviors(false);
        _rockyRhodes.IgnoreGroundCheck = true;

        // No long charge-up like BullRush. Maybe a tiny windup for visual feedback.
        yield return new WaitForSeconds(0.2f); 

        Vector3 toTarget = PlayerTarget.position - transform.position;
        toTarget.y = 0f;
        toTarget.Normalize();
        
        // Force boss to face the dash direction
        if (toTarget != Vector3.zero)
        {
            _rockyRhodes.RockyRhodesMesh.rotation = Quaternion.LookRotation(toTarget);
        }

        // Use custom shorter duration/speed for the mini dash
        float chestBumpDuration = 0.3f; // Shorter than BullRush
        float chestBumpSpeed = 40f;     // Slightly slower/punchier
        float timer = 0f;

        float currentSpeed = isEnraged ? (chestBumpSpeed * speedMultiplierLevel) : chestBumpSpeed;
        while (timer < chestBumpDuration)
        {
            if (!_rockyRhodes.isGrabbed && !_rockyRhodes.isPushed)
            {
                // Optionally add animation change here if you have one!
                // _rockyAnimations.ChangeAnimation("ChestBump_demo");
                _rockyRhodes.rb.linearVelocity = toTarget * currentSpeed;
                _rockyRhodes.gameObject.tag = "DamagePlayer";
            }

            timer += Time.deltaTime;
            yield return null;
        }

        _rockyRhodes.rb.linearVelocity = Vector3.zero;
        if (isEnraged)
        {
            isEnraged = false;
            if (auraPlaceholder != null) auraPlaceholder.SetActive(false);
            Debug.Log("Rage consumed");
        }


        _rockyRhodes.IgnoreGroundCheck = false;
        _rockyRhodes.ToggleBehaviors(true);
        IsPerformingAbility = false;

        CheckState(RockyRhodesStates.Idle);
    }
    public IEnumerator HeelTaunt()
    {
        if (CurrentRockyState == RockyRhodesStates.QTEMode) yield break;
        Debug.Log("HEEL TAUNT State Active - Charging Rage...");

        IsPerformingAbility = true;
        _rockyRhodes.ToggleBehaviors(false);
        _rockyRhodes.IgnoreGroundCheck = true;
        yield return new WaitForSeconds(tauntChargeDuration);

        isEnraged = true;
        if (auraPlaceholder != null) auraPlaceholder.SetActive(true);
        Debug.Log("Rocky is ENRAGED! Next attack speed x" + speedMultiplierLevel);

        _rockyRhodes.IgnoreGroundCheck = false;
        _rockyRhodes.ToggleBehaviors(true);
        IsPerformingAbility = false;

        CheckState(RockyRhodesStates.Idle);
        yield return new WaitForSeconds(AbilityCooldown);
    }

    // --------------------------------------- Arena 1 abilities ------------------------------------//
    public IEnumerator RopeRush()
    {
        yield return new WaitForSeconds(AbilityCooldown);
    }
    // --------------------------------------- Arena 2 abilities ------------------------------------//
  /*  public IEnumerator CannonBall()
    {
        if (CurrentRockyState == RockyRhodesStates.QTEMode) yield break;
        Debug.Log("CANNONBALL State Active");
        if (PlayerTarget == null) yield break;

        IsPerformingAbility = true;
        _rockyRhodes.ToggleBehaviors(false);
        _rockyRhodes.IgnoreGroundCheck = true;

        Vector3 toTarget = PlayerTarget.position - transform.position;
        if (toTarget != Vector3.zero)
        {
            _rockyRhodes.RockyRhodesMesh.rotation = Quaternion.LookRotation(toTarget);
        }
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
    }*/
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
