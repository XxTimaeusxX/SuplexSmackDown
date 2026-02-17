using System.Collections;
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
    //TODO: Add aiming assist for throwing, maybe a trajectory prediction or something similar
    public void Throw()
    {
        if (readyToThrow && carryingObject)
        {
            var carriable = suplexController.carriedObject;
            var profile = carriable.CarryWeightProfile;
            var rb = carriable.Rigidbody;

            float forwardForce = profile.forwardThrowForce;
            float upwardForce = profile.upwardThrowForce;

            Vector3 throwDirection = movementController.transform.forward * forwardForce + transform.up * upwardForce;

            rb.AddForce(throwDirection, ForceMode.Impulse);
            suplexController.ReleaseEnemy(throwDirection);

            //Debug.Log("Throwing object");
            //Debug.Log($"Applied forward throw force: {forwardForce}\nApplied upward throw force: {upwardForce}");
        }
    }
}
