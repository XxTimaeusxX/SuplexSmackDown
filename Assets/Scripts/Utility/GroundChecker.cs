using UnityEngine;

public class GroundChecker
{
    public bool isGrounded;

    public bool CheckGrounded(Rigidbody rb)
    {
        // Ground check logic here
        Collider col = rb.gameObject.GetComponent<Collider>();
        Vector3 rayOrigin = col.bounds.center - new Vector3(0, col.bounds.extents.y, 0);
        //Debug.DrawRay(rayOrigin, Vector3.down * 0.1f, Color.red);

        if (Physics.Raycast(rayOrigin + Vector3.up * 0.1f, Vector3.down, out RaycastHit hitInfo, 0.2f))
        {
            isGrounded = true;
            Debug.Log($"{rb.gameObject.name} is grounded.");
        }
        else
        {
            isGrounded = false;
            Debug.Log($"{rb.gameObject.name} is not grounded.");
        }
        
        return isGrounded;
    }
}
