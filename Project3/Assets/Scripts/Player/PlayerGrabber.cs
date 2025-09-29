using UnityEngine;
using UnityEngine.InputSystem;

// ~ Istvan W. 

// Script attaches to PlayerCharacter Object
// A HoldPosition gameobject must be added and identified in order to work

// TODO: Figure out controls for the game to enable a drop mechanic
public class PlayerGrabber : MonoBehaviour
{
    // Reference to PlayerInput (assigned automatically in Awake)
    public PlayerInput playerInput;

    public Transform holdPoint;       // Empty GameObject in front of player
    public float grabRange = 3f;      // How far the player can grab
    public LayerMask grabbableMask;   // Layer for grabbable objects
    public KeyCode dropKey = KeyCode.Q;

    private Rigidbody grabbedObject;
    private bool isGrabbed;

    InputAction attackAction;
    // InputAction dropAction;

    private void Awake()
    {
        //   playerMovement = GetComponent<PlayerMovement>();
        playerInput = GetComponent<PlayerInput>();
    }

    private void Start()
    {
        if (playerInput != null)
        {
            attackAction = playerInput.actions.FindAction("Attack");
            if (attackAction == null)
                Debug.LogError("PlayerGrabber: Could not find 'Attack' action in PlayerInput!");
        }
        else
        {
            Debug.LogError("PlayerGrabber: No PlayerInput component found!");
        }
    }

    void Update()
    {
        // Draw the ray in the Scene view for debugging
        Debug.DrawRay(transform.position, transform.forward * grabRange, Color.green);

        if (!isGrabbed && attackAction != null && attackAction.WasPressedThisFrame())
        {
            TryGrab();
            Debug.Log("Attempting grab!");
        }
        /*
        else if (isGrabbed && Input.GetKeyDown(dropKey))
        {
            Drop();
        }
        */
        if (isGrabbed && grabbedObject != null)
        {
            // Keep object at hold point
            grabbedObject.MovePosition(holdPoint.position);
        }
    }

    void TryGrab()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, grabRange, grabbableMask))
        {
            // Debug.Log("Raycast hit: " + hit.collider.name);

            // Only grab if tagged correctly
            if (hit.collider.CompareTag("canGrab"))
            {
                // Debug.Log("Hit object has 'canGrab' tag.");

                Rigidbody rb = hit.collider.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    grabbedObject = rb;
                    grabbedObject.useGravity = false;
                    grabbedObject.freezeRotation = true;
                    isGrabbed = true;

                    // Debug.Log("Successfully grabbed: " + grabbedObject.name);
                }
                else
                {
                    // Debug.LogWarning("Hit object has 'canGrab' tag but no Rigidbody!");
                }
            }
            else
            {
                // Debug.Log("Hit object does not have 'canGrab' tag.");
            }
        }
        else
        {
            // Debug.Log("Raycast did not hit any object within range.");
        }
    }

    void Drop()
    {
        if (grabbedObject != null)
        {
            grabbedObject.useGravity = true;
            grabbedObject.freezeRotation = false;
            // Debug.Log("Dropped object: " + grabbedObject.name);
            grabbedObject = null;
        }
        else
        {
            // Debug.Log("Drop called but no object was grabbed.");
        }

        isGrabbed = false;
    }
}
