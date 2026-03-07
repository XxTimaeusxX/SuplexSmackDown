using UnityEngine;

public class CarryProxySensor : MonoBehaviour
{
    public bool isBlocked { get; private set; }

    private void OnCollisionEnter(Collision collision)
    {
        isBlocked = true;
        Debug.Log("Carry proxy blocked by: " + collision.gameObject.name);
    }
    private void OnCollisionExit(Collision collision)
    {
        isBlocked = false;
    }
}
