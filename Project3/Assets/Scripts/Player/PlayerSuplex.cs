using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using System.Collections.Generic;

[System.Serializable]
public class SuplexConfig
{
    public SuplexAbilities ability;
    public float liftDuration;
    public float LiftHeight;
    public float dropDuration;
    public float FowardDistance;
}
public enum SuplexAbilities
{
    None,
    Rocket,
    Rainbow,
    LongJump
}
public class PlayerSuplex : MonoBehaviour
{
    [Header("References")]
    public Transform heldEnemy;
    public PlayerInput playerInput;

    [Header("Suplex Configurations")]
    public List<SuplexConfig> suplexConfigs;

    private PlayerMovement playerMovement;
    private PlayerDash playerDash;
    private Transform grabbedEnemy;
    private CharacterController controller;

    private InputAction RocketSuplexAction;
    private InputAction RainbowSuplexAction;
    private InputAction LongJumpSuplexAction;
    private InputAction jumpAction;
  
   

    private bool isSuplexing = false;
    private SuplexAbilities currentSuplex = SuplexAbilities.None;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (playerDash == null)
            playerDash = GetComponentInParent<PlayerDash>();
        playerMovement = GetComponentInParent<PlayerMovement>();
        jumpAction = playerInput.actions.FindAction("Jump");
        RocketSuplexAction = playerInput.actions.FindAction("RocketSuplex");
        RainbowSuplexAction = playerInput.actions.FindAction("RainbowSuplex");
        LongJumpSuplexAction = playerInput.actions.FindAction("LongjumpSuplex");

    }
    public void StartSuplex(Collider enemy)
    {
        Debug.Log($"StartSuplex called. StackTrace: {System.Environment.StackTrace}");
        if (isSuplexing) return;
        isSuplexing = true;
        Debug.Log("Suplexed an enemy!");
        HoldEnemy(enemy);
        StartCoroutine(WaitForSuplexInput());
    }
    void HoldEnemy(Collider enemy)
    {
       
        grabbedEnemy = enemy.transform;
        grabbedEnemy.position = playerDash.transform.position + Vector3.up * 1.5f;
        grabbedEnemy.SetParent(heldEnemy);
        grabbedEnemy.localPosition = Vector3.zero;
        // Optionally disable enemy AI here
        var rb = enemy.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true; // Prevent physics while held
        }
    }
    // Helper method to release the enemy
    void ReleaseEnemy(bool slam, SuplexConfig config = null)
    {
        if (grabbedEnemy != null)
        {
            var rb = grabbedEnemy.GetComponent<Rigidbody>();
            if (rb != null)
            {
                grabbedEnemy.SetParent(null);
                rb.isKinematic = false;
                if (slam)
                {
                    rb.linearVelocity = Vector3.down * (config.LiftHeight / config.dropDuration);
                }
                // else: let gravity handle the fall naturally
            }
            grabbedEnemy = null;
        }
    }
    IEnumerator WaitForSuplexInput()
    {
        currentSuplex = SuplexAbilities.None;
        while (currentSuplex == SuplexAbilities.None)
        {
            if (RocketSuplexAction != null && RocketSuplexAction.WasPressedThisFrame())
                currentSuplex = SuplexAbilities.Rocket;
            else if (RainbowSuplexAction != null && RainbowSuplexAction.WasPressedThisFrame())
                currentSuplex = SuplexAbilities.Rainbow;
            else if (LongJumpSuplexAction != null && LongJumpSuplexAction.WasPressedThisFrame())
                currentSuplex = SuplexAbilities.LongJump;
            
            yield return null;
        }
        Debug.Log($"Performing suplex: {currentSuplex}");
        ReleaseEnemy(false); // Unhold before performing
        PerformSuplex(currentSuplex);
    }
    void PerformSuplex(SuplexAbilities type)
    {
        var config = suplexConfigs.Find(cfg => cfg.ability == type);
        if (config != null)
            StartCoroutine(SuplexRoutine(config));
        else
            isSuplexing = false;
    }
    IEnumerator SuplexRoutine(SuplexConfig config)
    {
        Debug.Log($"SuplexRoutine started at frame {Time.frameCount}");
        Vector3 start = playerDash.transform.position;
        Vector3 end = start + playerDash.transform.forward * config.FowardDistance;
        float arcHeight = config.LiftHeight;
        float upDuration = config.liftDuration;
        float downDuration = config.dropDuration;
        float t = 0;
        bool jumpedOff = false;

        // Move up (or arc up)
        while (t < upDuration)
        {
            t += Time.deltaTime;
            float normalized = Mathf.Clamp01(t / upDuration);

            Vector3 pos;
          
            if (config.FowardDistance > 0f)
            {
                // Arc: interpolate forward, add vertical arc
                pos = Vector3.Lerp(start, end, normalized);
                pos.y += Mathf.Sin(Mathf.PI * normalized) * arcHeight;
            }
            else
            {
                // Vertical: just up
                pos = Vector3.Lerp(start, start + Vector3.up * arcHeight, normalized);
            }

            playerDash.transform.position = pos;
          /* if (grabbedEnemy != null)
                grabbedEnemy.position = pos;*/

            if (!jumpedOff && jumpAction.WasPressedThisFrame())
            {
                playerMovement.ForceJump();
                jumpedOff = true;
                ReleaseEnemy(false, config);
                break;
            }
            yield return null;
        }

        if (!jumpedOff)
            yield return new WaitForSeconds(0.1f);

        // Move down (or arc down)
        t = 0;
        while (!jumpedOff && t < downDuration)
        {
            t += Time.deltaTime;
            float normalized = Mathf.Clamp01(t / downDuration);

            Vector3 pos;
            if (config.FowardDistance > 0f)
            {
                // Arc: interpolate forward, add vertical arc (descending)
                pos = Vector3.Lerp(start + Vector3.up * arcHeight, end, normalized);
                pos.y += Mathf.Sin(Mathf.PI * (1 - normalized)) * arcHeight;
            }
            else
            {
                // Vertical: just down
                pos = Vector3.Lerp(start + Vector3.up * arcHeight, start, normalized);
            }

            playerDash.transform.position = pos;
             /* if (grabbedEnemy != null)
                grabbedEnemy.position = pos;*/

            if (!jumpedOff && jumpAction.WasPressedThisFrame())
            {
                playerMovement.ForceJump();
                jumpedOff = true;
                ReleaseEnemy(false, config);
                break;
            }
            yield return null;
        }

        if (!jumpedOff)
            ReleaseEnemy(true, config);

        isSuplexing = false;
    }
}
