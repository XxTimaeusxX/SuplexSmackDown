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

            Debug.Log("Throwing object: " + thrownObject.name);
        }
    }
}
