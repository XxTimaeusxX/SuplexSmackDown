using UnityEngine;

// Contributers: Istvan W.

// Last Modified: 1/15/2026 
// Modified By:  Istvan W.

public class GroundChecker : MonoBehaviour
{
    public Transform groundCheck;
    [SerializeField] private float checkDistance = 0.2f;
    [SerializeField] private LayerMask groundMask;

    public bool IsGrounded()
    {
        Debug.DrawRay(transform.position, Vector3.down * 2.0f, Color.red, checkDistance);
        return Physics.Raycast(groundCheck.position, Vector3.down, checkDistance, groundMask);
    }

}
