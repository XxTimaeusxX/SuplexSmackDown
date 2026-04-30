using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;

public class QTESystem : MonoBehaviour
{

    [Header("Button Mash QTE settings")]
    public bool EnableQuickTimeEvent;
    public int NumberOfButtonClicksRequired = 25;
    public float CountDownTimer = 10f;
    public int CurrentButtonClicks = 0;
    public float TimerRate;
    private float _initialCountDownTimer;
    private Coroutine _currentQTECoroutine;

    [Header("Timing Slider QTE settings")]
    public Image BackgroundSlider;
    public Transform pointA; // Reference to the starting point
    public Transform pointB; // Reference to the ending point
    public RectTransform safeZone; // Reference to the safe zone RectTransform
    public float moveSpeed = 100f; // Speed of the pointer movement

    private float direction = 1f; // 1 for moving towards B, -1 for moving towards A
    public RectTransform pointerTransform;
    private Vector3 targetPosition;
    [SerializeField]private Transform clashPoint;


    [Header("UI references")]
    public PlayerInput playerInput; 
    private InputAction buttonMashAction;
    public TextMeshProUGUI   timerText;
    public Slider ButtonMashbarSlider;

    [Header("CinemaMachine references")]
   [SerializeField] Cinema_final CinemaComponent;

    [Header("Other References")]
   [SerializeField] PlayerHealth PlayerHealth;
    [SerializeField] RhockyHealth RhockyHealth;
    [SerializeField] RockyRhodes RockyRhodesScript;
    [SerializeField] RhockyAbilities RockyRhodesAbilities;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private RockyAnimations rockyAnimations;

    [Header("Player Ability References")]
    [SerializeField] PlayerSuplex playerSuplex;
    [SerializeField] PlayerDash playerDash;

    [Header("QTE Input Actions")]
    public List<string> qteInputActions = new List<string>
    {
        "Jump",
        "RainbowSuplex",
        "LongjumpSuplex",
        "Dash",
    };
    private List<InputAction> _qteAction = new List<InputAction>();
    private InputAction _currentQTEAction;
    void Awake()
    {
        if(playerMovement == null) playerMovement = GetComponent<PlayerMovement>();
         if(rockyAnimations == null) rockyAnimations = GetComponent<RockyAnimations>();
         if(RockyRhodesAbilities == null) RockyRhodesAbilities = GetComponent<RhockyAbilities>();
        if (pointerTransform == null) pointerTransform = GetComponent<RectTransform>();
        targetPosition = pointB.position;
        if (playerInput == null) playerInput = GetComponent<PlayerInput>();
        if (playerInput != null) buttonMashAction = playerInput.actions.FindAction("Jump");
        if(CinemaComponent == null) CinemaComponent = GetComponent<Cinema_final>();
        if(PlayerHealth == null) PlayerHealth = GetComponent<PlayerHealth>();
     
        if (RhockyHealth== null) RhockyHealth = GetComponent<RhockyHealth>();
        _initialCountDownTimer = CountDownTimer; // Store the initial timer value
        foreach(string actionName in qteInputActions)
        {
            InputAction action = playerInput.actions.FindAction(actionName);
            if (action != null)
            {
               _qteAction.Add(action);
            }
            else
            {
                Debug.LogWarning($"Input Action '{actionName}' not found in PlayerInput.");
            }
        }

        DisableAllQTE(); 
    }

    private void UpdateTimerUI()
    {
        if (timerText == null) return;
        // show seconds with one decimal place
        timerText.text = "Press"  + GetActionKeyName(_currentQTEAction) + "\n Timer:" + CountDownTimer.ToString("F0");
    }
    private void UpdateButtonMashBarfill()
    {
       ButtonMashbarSlider.value = (float)CurrentButtonClicks / NumberOfButtonClicksRequired;
    }
    private string GetActionKeyName(InputAction action)
    {
        if (action == null) return "???";
        // Try to get the display string for the first binding
        if (action.bindings.Count > 0)
        {
            return action.GetBindingDisplayString(0);
        }
        return action.name;
    }
    private void DisablePlayerAbilities()
    {
        if (playerSuplex != null) playerSuplex.enabled = false;
        if (playerDash != null) playerDash.enabled = false;
    }

    private void EnablePlayerAbilities()
    {
        if (playerSuplex != null) playerSuplex.enabled = true;
        if (playerDash != null) playerDash.enabled = true;
    }
    private IEnumerator ResumeSuplexAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (playerSuplex != null)
        {
            // Find and re-enable the suplex hitbox GameObject
        /*    var hitboxCaller = playerSuplex.GetComponentInChildren<SuplexHitboxCaller>(true);
            if (hitboxCaller != null)
                hitboxCaller.gameObject.SetActive(true);*/

            // Optionally, you can also ensure Rocky's collider is enabled here if needed:
            var rockyCollider = RockyRhodesScript.GetComponent<Collider>();
            if (rockyCollider != null)
                rockyCollider.enabled = true;
            playerSuplex.StartSuplex(rockyCollider);
        }
    }
    public void SetDifficulty(int buttonClicks, float timer, float sliderMovSpeed)
    {
        NumberOfButtonClicksRequired = buttonClicks;
        _initialCountDownTimer = timer;
        moveSpeed = sliderMovSpeed;
    }
    public void StartQTE()
    {
        if (_currentQTECoroutine != null) return; // already running
        CurrentButtonClicks = 0;
        CountDownTimer = _initialCountDownTimer; // Reset timer to initial value
         // Pick a random action for this QTE round                                        
        if (_qteAction.Count > 0)
        {
            _currentQTEAction = _qteAction[Random.Range(0, _qteAction.Count)];
        }
        DisablePlayerAbilities();//disable player inputs and abilities
        // stop rhocky rhode jumping  and transition camera pan in shot
        StartCoroutine(CinemaComponent.StartBoss3PanIn());

        _currentQTECoroutine = StartCoroutine(QTEClashSequence());
        EnableQuickTimeEvent = true; // Prevent multiple triggers
    }
    private  IEnumerator QTEClashSequence()
    {
        yield return StartCoroutine(MoveToClashPoints(playerMovement.transform, RockyRhodesScript.transform, .2f));
        StartClashAnimations();
        yield return new WaitForSeconds(.2f);
        yield return StartCoroutine(QTERandomizer());
    }
    private IEnumerator MoveToClashPoints(Transform playerTransform, Transform rockyTransform, float duration, float offset = 2f)
    {
        Vector3 playerStart = playerTransform.position;
        Vector3 rockyStart = rockyTransform.position;

        // Calculate the direction from player to Rocky
        Vector3 direction = (rockyStart - playerStart).normalized;

        // Find the midpoint
        Vector3 midpoint = (playerStart + rockyStart) / 2f;

        // Offset each character from the midpoint along the direction vector
        Vector3 playerTarget = midpoint - direction * offset;
        Vector3 rockyTarget = midpoint + direction * offset;

        playerTarget.y += 1f;// Keep original Y position
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            playerTransform.position = Vector3.Lerp(playerStart, playerTarget, t);
            rockyTransform.position = Vector3.Lerp(rockyStart, rockyTarget, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        playerTransform.position = playerTarget;
        rockyTransform.position = rockyTarget;
    }
    private void StartClashAnimations()
    {
        playerMovement.ChangeAnimtion("CoheteClash");
        rockyAnimations.ChangeAnimation("RockyClash");
    }
    private void EndClash()
    {
        playerMovement.ChangeAnimtion("IDLE");
        rockyAnimations.ChangeAnimation("RockyIdle");
    }
    public void StopQTE()
    {
        StopAllCoroutines();
        _currentQTECoroutine = null;
        DisableAllQTE();
        EnablePlayerAbilities();
        CinemaComponent.EndRockyPanIn();
        CurrentButtonClicks = 0;
        CountDownTimer = _initialCountDownTimer; // Reset timer to initial value
        timerText.text = "";
         ButtonMashbarSlider.value = 0f;
        EnableQuickTimeEvent = false;
        Debug.Log("QTE Stopped and cleaned up.");

    }
    private IEnumerator QTERandomizer()
    {
        int randomIndex = Random.Range(0, 2);
        switch(randomIndex)
            {
            case 0:
                yield return StartCoroutine(ButtonMashQTE());
                break;
            case 1:
                yield return StartCoroutine(TimingSliderQTE());
                break;
        }
    }

    private void EnableButtonMashUI()
    {
        timerText.gameObject.SetActive(true);
        ButtonMashbarSlider.gameObject.SetActive(true);
    }
    private void EnableTimingSliderUI()
    {
        BackgroundSlider.gameObject.SetActive(true);
        timerText.gameObject.SetActive(true);
        safeZone.gameObject.SetActive(true);
        pointerTransform.gameObject.SetActive(true);
    }
    private void DisableAllQTE()
    {
        BackgroundSlider.gameObject.SetActive(false);
        timerText.gameObject.SetActive(false);
        ButtonMashbarSlider.gameObject.SetActive(false);
        safeZone.gameObject.SetActive(false);
        pointerTransform.gameObject.SetActive(false);

    }

    private IEnumerator FailQTEAnimation()
    {
        RockyRhodesAbilities.CurrentRockyState = RockyRhodesStates.QTEFail;
        Vector3 KnockBackDir = (playerMovement.transform.position - RockyRhodesScript.transform.position).normalized;
        KnockBackDir.y = 0f;
        float knockBackForce = 15f;
        float knockUpForce = 15f;
        playerMovement.velocity= (KnockBackDir * knockBackForce) + (Vector3.up * knockUpForce);
        yield return new WaitForSeconds(1f);
        RockyRhodesAbilities.CurrentRockyState = RockyRhodesStates.Idle;
    }
    private IEnumerator ButtonMashQTE()
    {
        EnableButtonMashUI();
        UpdateTimerUI();
        UpdateButtonMashBarfill();
        while (CountDownTimer > 0)
        {
            if (_currentQTEAction.WasPressedThisFrame()) // Example button, replace with actual input
            {
                CurrentButtonClicks++;
                UpdateButtonMashBarfill();
                if (CurrentButtonClicks >= NumberOfButtonClicksRequired)
                {
                    // QTE success logic here
                 //   Debug.Log("QTE Success!");
                    timerText.text = "success";
                   
                    CinemaComponent.EndRockyPanIn();
                    RockyRhodesScript.gameObject.tag = "Enemy";
                    // --- Grab logic integration ---
                    EndClash();
                    RockyRhodesAbilities.StartCoroutine(RockyRhodesAbilities.DizzyForSeconds(4f));
                    StopQTE(); // Clean up UI and re-enable abilities
                    StartCoroutine(ResumeSuplexAfterDelay(1f)); // Resume suplex after a short delay to allow for any success animations
                    yield break; // Exit the coroutine
                }
            }
            CountDownTimer -= Time.deltaTime * TimerRate;
            UpdateButtonMashBarfill();
            UpdateTimerUI();
            yield return null; // Wait for the next frame
        }
        // QTE failure logic here
    //    Debug.Log("QTE Failed!");
        timerText.text = "failed";
        PlayerHealth.TakeDamage();
        CinemaComponent.EndRockyPanIn();
        yield return StartCoroutine(FailQTEAnimation());
        EndClash();
        StopQTE(); // Clean up UI and re-enable abilities
    }

    private IEnumerator TimingSliderQTE()
    {
        EnableTimingSliderUI();
        CountDownTimer = _initialCountDownTimer;
        pointerTransform.position = pointA.position; // reset pointer to start
        targetPosition = pointB.position;
        direction = 1f;
        UpdateTimerUI();

        while (CountDownTimer > 0)
        {
            // Move the pointer towards the target position
            pointerTransform.position = Vector3.MoveTowards(pointerTransform.position, targetPosition, moveSpeed * Time.deltaTime);

            // Change direction if the pointer reaches one of the points
            if (Vector3.Distance(pointerTransform.position, pointA.position) < 0.1f)
            {
                targetPosition = pointB.position;
                direction = 1f;
            }
            else if (Vector3.Distance(pointerTransform.position, pointB.position) < 0.1f)
            {
                targetPosition = pointA.position;
                direction = -1f;
            }

            // Check for input
            if (_currentQTEAction.WasPressedThisFrame())
            {
                if (RectTransformUtility.RectangleContainsScreenPoint(safeZone, pointerTransform.position, null))
                {
                  //  Debug.Log("Success!");
                    timerText.text = "success";
                    RockyRhodesScript.gameObject.tag = "Enemy";
                    // --- Grab logic integration ---
                    EndClash();
                    RockyRhodesAbilities.StartCoroutine(RockyRhodesAbilities.DizzyForSeconds(4f));
                    StopQTE();
                  StartCoroutine(ResumeSuplexAfterDelay(1f)); // Resume suplex after a short delay to allow for any success animations    
                    yield break;
                }
                else
                {
                //    Debug.Log("Failed!");
                    timerText.text = "failed";
                    PlayerHealth.TakeDamage();
                yield return StartCoroutine(FailQTEAnimation());
                    EndClash();
                    StopQTE();
                    yield break;
                }
            }

            CountDownTimer -= Time.deltaTime * TimerRate;
            UpdateTimerUI();
            yield return null;
        }

        // Ran out of time
      //  Debug.Log("Timing Slider QTE Failed! Time ran out.");
        timerText.text = "failed";
        PlayerHealth.TakeDamage();
        yield return StartCoroutine(FailQTEAnimation());
        EndClash();
        StopQTE();
    }

}
