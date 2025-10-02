using UnityEngine;
public class SuplexHitboxCaller : MonoBehaviour
{
    private PlayerSuplex playerSuplex;

    private bool hasTriggered = false;
    private void Awake()
    {
        playerSuplex = GetComponentInParent<PlayerSuplex>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(hasTriggered) return; // Prevent multiple triggers
        if (other.CompareTag("Enemy") && playerSuplex != null)
        {
            hasTriggered = true;
            Debug.Log("hitboxcollider called");
            playerSuplex.StartSuplex(other);
            gameObject.SetActive(false); // Disable hitbox after calling
        }
    }
    // Optional: Reset if you want to allow retriggering after leaving
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
            hasTriggered = false;
    }
}
