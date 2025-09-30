using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDash : MonoBehaviour
{
    public CharacterController controller;
    InputAction dashAction;
    public PlayerInput PlayerInput;

    [Header("Dash Settings")]
    public float dashSpeed = 5f;
    public float dashDuration = 0.2f;
    private Vector3 dashDirection;
    private bool isDashing = false;
    private float dashTime;

    void Start()
    {
        dashAction = PlayerInput.actions.FindAction("Dash");
        
    }

    // Update is called once per frame
    void Update()
    {
        Dash();
    }
    void Dash()
    {
        if (dashAction.WasPressedThisFrame() && !isDashing)
        {
            dashDirection = transform.forward;
            isDashing = true;
            dashTime = dashDuration;
            Debug.Log("Dash initiated!");
        }
        if (isDashing)
        {
            if (dashTime > 0)
            {
                controller.Move(dashDirection * dashSpeed * Time.deltaTime);
                dashTime -= Time.deltaTime;
            }
            else
            {
                isDashing = false;
                Debug.Log("Dash ended!");
            }
        }
    }
}
