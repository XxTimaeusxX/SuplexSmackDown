using UnityEngine;

public class SuplexTest : MonoBehaviour
{
    [Header("References")]
    private MovementConfig movementConfig;
    private MovementController movementController;
    private CharacterController controller;

    [Header("State Flags")]
    private bool isLooping = false;
    private bool isPlayingOneShot = false;

    public float t;
    private AnimationCurve curve0;
    private Vector3 loopStartPosition;
    private float lastT = 0f;

    private void Awake()
    {
        movementController = GetComponent<MovementController>();
        movementConfig = GetComponent<MovementConfig>();
        controller = GetComponent<CharacterController>();

        // Curve being tested
        curve0 = movementConfig.rainbowSuplexHeight;
    }

    private void Update()
    {
        if (isLooping)
        {
            Loop();
        }

        if (isPlayingOneShot)
        {
            OneShot();

            // Stop automatically when finished
            float duration = curve0.keys[curve0.length - 1].time;
            if (t >= duration)
            {
                isPlayingOneShot = false;
                movementController.overrideVerticalMotion = false;
            }
        }
    }

    // Loop or OneShot called inside MovementController based on test type
    public void Loop()
    {
        float duration = curve0.keys[curve0.length - 1].time;

        t = Time.time % duration;

        // Detect wrap-around: last frame t was near the end, now it's near the start
        if (t < lastT)
        {
            controller.enabled = false;               // CharacterController hates teleporting
            transform.position = loopStartPosition;   // Snap back
            controller.enabled = true;
        }

        lastT = t;

        Test();

    }
    public void OneShot()
    {
        t += Time.deltaTime;
        float duration = curve0.keys[curve0.length - 1].time;
        t = Mathf.Min(t, duration);
        Test();
    }
    void Test()
    {
        float velocityY = curve0.Evaluate(t);
        controller.Move(new Vector3(0, velocityY, 0) * Time.deltaTime);
    }

    // Handlers to start tests
    public void PlayOneShot()
    {
        t = 0f;
        isPlayingOneShot = true;
        isLooping = false;

        // Take control of vertical motion
        movementController.overrideVerticalMotion = true;

    }

    public void StartLoop()
    {
        isLooping = !isLooping;
        Debug.Log(isLooping);
        isPlayingOneShot = false;

        t = 0f;
        lastT = 0f;

        loopStartPosition = transform.position;   // store starting point
        movementController.overrideVerticalMotion = true;
    }

}
