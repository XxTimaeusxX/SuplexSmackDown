using UnityEngine;

// Contributers: Istvan W.

// Last Modified: 1/15/2026 
// Modified By:  Istvan W.

public class GroundChecker : MonoBehaviour
{
    public Transform groundCheck;
    [SerializeField] private float checkDistance = 0.2f;
    [SerializeField] private LayerMask groundMask , enviromentMask;

    public bool IsGrounded()
    {
        int combinedMask = groundMask | enviromentMask;

        Debug.DrawRay(groundCheck.position, Vector3.down * checkDistance, Color.red);
        return Physics.Raycast(groundCheck.position, Vector3.down, checkDistance, combinedMask);
    }

}
