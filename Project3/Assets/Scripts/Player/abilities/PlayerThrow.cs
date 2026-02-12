using UnityEngine;

public class PlayerThrow : MonoBehaviour
{

    [Header("References")]
    SuplexController suplexController;
    MovementController movementController;

    [Header("Throw Settings")]
    Rigidbody thrownObject;

    [Header("Boolean States")]
    public bool readyToThrow = false;
    public bool carryingObject = false;

    private void Awake()
    {
        suplexController = GetComponent<SuplexController>();
        movementController = GetComponent<MovementController>();

    }
    public void Throw()
    {
        if (readyToThrow && carryingObject)
        {
            thrownObject = suplexController.carriedObject.GetComponent<Rigidbody>();

            suplexController.ReleaseEnemy();

            //thrownObject.isKinematic = false; // Allow physics to affect the thrown object
            // TOOO: Add additional logic here to correct the throw force, direction, etc.
            // Make sure to use carry weight profiles if applicable
            thrownObject.AddForce(transform.forward * 100f, ForceMode.Impulse); // Adjust the force as needed

            Debug.Log("Throwing object: " + thrownObject.name);
        }
    }
}
