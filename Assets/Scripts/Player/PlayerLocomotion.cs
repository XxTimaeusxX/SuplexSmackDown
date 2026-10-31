using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerLocomotion
{

    Vector2 direction;


    public void Move(Rigidbody rb, InputAction moveAction, int speed, Camera camera)
    {
        direction = moveAction.ReadValue<Vector2>();

        if (direction.magnitude > 0.1f)
        {
            Vector3 moveDirection = Quaternion.Euler(0f, camera.transform.eulerAngles.y, 0f) * new Vector3(direction.x, 0, direction.y);
            //Debug.DrawRay(rb.transform.position, moveDirection * 5f, Color.red);

            Vector3 velocity = moveDirection * speed;
            rb.linearVelocity = new Vector3(velocity.x, rb.linearVelocity.y, velocity.z);
        }
        else
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        }
    }

    public void Jump(Rigidbody rb, int jumpForce, GroundChecker groundChecker)
    {
        Debug.Log("Jump triggered.");

        if (groundChecker != null && groundChecker.CheckGrounded(rb))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
}
