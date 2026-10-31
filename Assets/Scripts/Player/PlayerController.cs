using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Scripting;

public class PlayerController : MonoBehaviour
{
    private PlayerInput playerInput;
    public Camera playerCamera;

    private PlayerLocomotion locomotion;
    private Rigidbody rb;
    GroundChecker groundChecker;

    private InputAction moveAction;
    private InputAction jumpAction;

    public int movementSpeed = 5;
    public int jumpForce = 5;

    void Start()
    {
        try
        {
            rb = gameObject.GetComponent<Rigidbody>();
            playerInput = GetComponent<PlayerInput>();

            locomotion = new PlayerLocomotion();
            groundChecker = new GroundChecker();

            moveAction = playerInput.actions.FindAction("Move");
            jumpAction = playerInput.actions.FindAction("Jump");

        }
        catch (System.Exception)
        {
            Debug.LogError("PlayerController: Error initializing player input or locomotion.");
            throw;
        }

    }
    void Update()
    {
        locomotion.Move(rb, moveAction, movementSpeed, playerCamera);

        if (jumpAction.WasPressedThisFrame())
        {
            locomotion.Jump(rb, jumpForce, groundChecker);
        }
    }

}
