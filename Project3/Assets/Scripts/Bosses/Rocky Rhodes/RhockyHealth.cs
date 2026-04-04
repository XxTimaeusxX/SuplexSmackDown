using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RockyRhodes))]
public class RhockyHealth : MonoBehaviour
{
    [Header("Phase Health")]
    [SerializeField] private float phase1Health = 4f;
    [SerializeField] private float phase2Health = 5f;
    [SerializeField] private float phase3Health = 6f;

    [Header("UI")]
    public Slider HealthSlider;

    private RockyRhodes _rockyRhodes;
    private QTESystem _qteSystem;
    private RhockyAbilities _rhockyAbilities; // Added reference
    private float _lastHealthValue;
    private bool _healthhasDecreased;
    private float _currentPhase = 1f;

    private void Awake()
    {
        _rockyRhodes = GetComponent<RockyRhodes>();
        _rhockyAbilities = GetComponent<RhockyAbilities>();
        _qteSystem = _rockyRhodes.QTESystemScript;
    }

    private void Start()
    {
        if (HealthSlider == null) return;

        Applyhealth(phase1Health);
        CurrentPhaseMode();
        CheckHealthState();
        _healthhasDecreased = false;
    }

    private void Update()
    {
        if (HealthSlider == null) return;

        if (HealthSlider.value != _lastHealthValue)
        {
            _lastHealthValue = HealthSlider.value;
            CheckHealthState();
        }
    }
    public void TakeDamage()
    {
        if (HealthSlider == null) return;
        HealthSlider.value = Mathf.Max(HealthSlider.value - 1f, 0f);
    }
    public void CheckHealthState()
    {
        // When health drops to 1, we either transition phases or trigger the desperation move
        if (_lastHealthValue == 1f && !_healthhasDecreased)
        {
            _healthhasDecreased = true; // Lock so we only trigger once per health drop to 1

            if (_currentPhase < 3)
            {
                // Heal and jump to next phase
                float nextPhaseHealth = _currentPhase == 1f ? phase2Health : phase3Health;
                StartCoroutine(HealAndJump(nextPhaseHealth));
            }
            else if (_currentPhase == 3)
            {
                // Trigger the flurry when Phase 3 reaches exactly 1 health!
                Debug.Log("Health is 1 in Phase 3! Initiating Unstoppable Flurry!");

                // Lock him into ONLY doing the flurry until he dies
                _rhockyAbilities._randomSelection.Clear();
                _rhockyAbilities._randomSelection.Add(RockyRhodesStates.DesperationFlurry);

                // Start the first one immediately
                _rhockyAbilities.CheckState(RockyRhodesStates.DesperationFlurry);
            }
        }
        else if (_lastHealthValue <= 0f && _currentPhase >= 3)
        {
            _rockyRhodes.Dead();
        }
    }

    private void CurrentPhaseMode()
    {
        if (_qteSystem == null) return;

        switch (_currentPhase)
        {
            case 1:
                Debug.Log("regular mode");
                _qteSystem.SetDifficulty(20, 15f, 500f);
                _qteSystem.TimerRate = 1f;
                break;
            case 2:
                Debug.Log("Medium mode");
                _qteSystem.SetDifficulty(20, 10f, 1500f);
                _qteSystem.TimerRate = 1f;
                break;
            case 3:
                Debug.Log("Intense mode");
                _qteSystem.SetDifficulty(20, 9f, 2000f);
                _qteSystem.TimerRate = 1f;
                // Removed the initial CheckState from here, moving it to CheckHealthState
                break;
        }
    }

    private IEnumerator HealAndJump(float healthAmount)
    {
        yield return StartCoroutine(_rockyRhodes.JumpToPlatform());
        _currentPhase++;
        Applyhealth(healthAmount);
        CurrentPhaseMode();
        _healthhasDecreased = false; // Reset the flag so Phase 2 and 3 can trigger their 1HP events
        CheckHealthState();
    }

    private void Applyhealth(float value)
    {
        HealthSlider.maxValue = Mathf.Max(HealthSlider.maxValue, value);
        HealthSlider.value = value;
        _lastHealthValue = HealthSlider.value;
    }
}
