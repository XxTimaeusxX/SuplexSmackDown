using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shockwave : MonoBehaviour
{
    public float pushForce;

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;
        if (body == null || body.isKinematic)
        {
            return;
        }
        if (hit.moveDirection.y < -0.3f)
        {
            return;
        }
        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
        body.AddForce(pushDir * pushForce, ForceMode.Impulse);
    }
}
