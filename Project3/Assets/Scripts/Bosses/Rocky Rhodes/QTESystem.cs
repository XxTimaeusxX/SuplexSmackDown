using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class QTESystem : MonoBehaviour
{
    [Header("Button Mash QTE settings")]
    public bool EnableQuickTimeEvent;
    public int NumberOfButtonClicksRequired = 25;
    public float CountDownTimer = 10f;
    public int CurrentButtonClicks = 0;
    private float _initialCountDownTimer;
    private Coroutine _currentQTECoroutine;

    [Header("UI references")]
    public PlayerInput playerInput; 
    private InputAction buttonMashAction;
    public TextMeshProUGUI   timerText;
    public Slider ButtonMashbarSlider;

    [Header("CinemaMachine references")]
   [SerializeField] Cinema_final CinemaComponent;

    [Header("Other References")]
   [SerializeField] PlayerHealth PlayerHealth;
    void Awake()
    {
        if (playerInput == null) playerInput = GetComponent<PlayerInput>();
        if (playerInput != null) buttonMashAction = playerInput.actions.FindAction("Jump");
        if(CinemaComponent == null) CinemaComponent = GetComponent<Cinema_final>();
        if(PlayerHealth == null) PlayerHealth = GetComponent<PlayerHealth>();
        _initialCountDownTimer = CountDownTimer; // Store the initial timer value
        DisableButtonMashUI(); 
    }

    private void UpdateTimerUI()
    {
        if (timerText == null) return;
        // show seconds with one decimal place
        timerText.text = "Timer:" + CountDownTimer.ToString("F0");
    }
    private void UpdateButtonMashBarfill()
    {
       ButtonMashbarSlider.value = (float)CurrentButtonClicks / NumberOfButtonClicksRequired;
    }
    public void StartQTE()
    {
        if (_currentQTECoroutine != null) return; // already running
        CurrentButtonClicks = 0;
        CountDownTimer = _initialCountDownTimer; // Reset timer to initial value
        UpdateTimerUI();
        UpdateButtonMashBarfill();
        EnableButtonMashUI();
        CinemaComponent.Boss3Intro = true;
        _currentQTECoroutine = StartCoroutine(ButtonMashQTE());
        EnableQuickTimeEvent = true; // Prevent multiple triggers
    }

    public void StopQTE()
    {
        if (_currentQTECoroutine != null)
        {
            StopCoroutine(_currentQTECoroutine);
            _currentQTECoroutine = null;
        }
        DisableButtonMashUI();
        CinemaComponent.Boss3Intro = false;
        CurrentButtonClicks = 0;
        CountDownTimer = _initialCountDownTimer; // Reset timer to initial value
        timerText.text = "";
         ButtonMashbarSlider.value = 0f;
        EnableQuickTimeEvent = false;


    }
    private void EnableButtonMashUI()
    {
        timerText.gameObject.SetActive(true);
        ButtonMashbarSlider.gameObject.SetActive(true);
    }
    private void DisableButtonMashUI()
    {
        timerText.gameObject.SetActive(false);
        ButtonMashbarSlider.gameObject.SetActive(false);
    }
    private IEnumerator ButtonMashQTE()
    {
        UpdateTimerUI();
        while (CountDownTimer > 0)
        {
            if (buttonMashAction.WasPressedThisFrame()) // Example button, replace with actual input
            {
                CurrentButtonClicks++;
                UpdateButtonMashBarfill();
                if (CurrentButtonClicks >= NumberOfButtonClicksRequired)
                {
                    // QTE success logic here
                    Debug.Log("QTE Success!");
                    timerText.text = "success";
                    // RhockyRhodes.TakeDamage();
                    CinemaComponent.EndRockyPanIn();
                    _currentQTECoroutine = null; // Clear the coroutine reference
                    EnableQuickTimeEvent = false; // Prevent further triggers
                    DisableButtonMashUI();
                    yield break; // Exit the coroutine
                }
            }
            CountDownTimer -= Time.deltaTime;
            UpdateButtonMashBarfill();
            UpdateTimerUI();
            yield return null; // Wait for the next frame
        }
        // QTE failure logic here
        Debug.Log("QTE Failed!");
        timerText.text = "failed";
        PlayerHealth.TakeDamage();
        CinemaComponent.EndRockyPanIn();
        EnableQuickTimeEvent = false; // Prevent further triggers
        _currentQTECoroutine = null;
        DisableButtonMashUI();
    }
}
