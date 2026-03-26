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

    public bool IsPerformingAbility = false;
    private float _abilityTimer = 0f;
    public Transform PlayerTarget;


    private RockyRhodes _rockyRhodes;
    private Coroutine _currentStateCoroutine;

    public RockyRhodesStates CurrentRockyState { get; private set; }

    private readonly List<RockyRhodesStates> _randomSelection = new List<RockyRhodesStates>
    {
        RockyRhodesStates.BoulderEruption,
        RockyRhodesStates.BullRock,
    };

    private void Awake()
    {
        _rockyRhodes = GetComponent<RockyRhodes>();

    }

    public void InterruptAbility()
    {
        StopAllCoroutines();
        _currentStateCoroutine = null;
        Debug.Log("Ability interrupted by grab/push. Stopping ability and waiting for release.");
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
                _currentStateCoroutine = StartCoroutine(Regular());
                break;
            case RockyRhodesStates.BoulderEruption:
                _currentStateCoroutine = StartCoroutine(BoulderEruption());
                break;
            case RockyRhodesStates.BullRock:
                _currentStateCoroutine = StartCoroutine(BullRock());
                break;
        }
    }

    public IEnumerator Regular()
    {
        Debug.Log("Regular State Active");
        yield return new WaitForSeconds(AbilityCooldown);
        int randomIndex = Random.Range(0, _randomSelection.Count);
        CurrentRockyState = _randomSelection[randomIndex];
        CheckState(CurrentRockyState);
        yield return null;
    }

    public IEnumerator BoulderEruption()
    {
        IsPerformingAbility = true;
        _rockyRhodes.ToggleBehaviors(false);
        _rockyRhodes.IgnoreGroundCheck = true;

        Vector3 toTarget = (PlayerTarget.position - transform.position);
        toTarget.y = 0f;
        toTarget.Normalize();
        _rockyRhodes.rb.AddForce((Vector3.up * jumpForce * 1f) + (Vector3.forward + toTarget * 20f), ForceMode.Impulse);

        yield return new WaitForSeconds(0.5f);

        _rockyRhodes.IgnoreGroundCheck = false;
        while (!_rockyRhodes.IsEnemyGrounded() && IsPerformingAbility)
        {
            if (!_rockyRhodes.isGrabbed && !_rockyRhodes.isPushed)
            {
                Debug.Log("Boulder Eruption -in motion.");
                _rockyRhodes.RockyRhodesMesh.Rotate(Vector3.forward, 1000f * Time.deltaTime, Space.World);
            }
            yield return null;
        }

        _rockyRhodes.RockyRhodesMesh.localRotation = _rockyRhodes.originalMeshRotation;
        while (_rockyRhodes.isGrabbed && _rockyRhodes.isPushed)
        {
            yield return null;
        }
        yield return new WaitForSeconds(AbilityCooldown);

        Debug.Log("TOGGLING ---------------- BEHAVIORS-------------.");
        _rockyRhodes.ToggleBehaviors(true);
        IsPerformingAbility = false;
        CheckState(RockyRhodesStates.Regular);
    }

    public IEnumerator BullRock()
    {
        IsPerformingAbility = true;
        _rockyRhodes.ToggleBehaviors(false);
        _rockyRhodes.IgnoreGroundCheck = true;
        _rockyRhodes.rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);

        yield return new WaitForSeconds(0.5f);

        _rockyRhodes.IgnoreGroundCheck = false;

        while (!_rockyRhodes.IsEnemyGrounded() && IsPerformingAbility)
        {
            if (!_rockyRhodes.isGrabbed && !_rockyRhodes.isPushed)
            {
                Debug.Log("Bull rock -in motion.");
                _rockyRhodes.RockyRhodesMesh.Rotate(Vector3.up * 1400f * Time.deltaTime, Space.World);
            }
            yield return null;
        }

        _rockyRhodes.RockyRhodesMesh.localRotation = _rockyRhodes.originalMeshRotation;
        while (_rockyRhodes.isGrabbed && _rockyRhodes.isPushed)
        {
            yield return null;
        }
        yield return new WaitForSeconds(AbilityCooldown);

        Debug.Log("TOGGLING ---------------- BEHAVIORS-------------.");
        _rockyRhodes.ToggleBehaviors(true);
        IsPerformingAbility = false;
        CheckState(RockyRhodesStates.Regular);
    }
}
