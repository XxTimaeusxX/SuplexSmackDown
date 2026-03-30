using UnityEngine;
/// <summary>
/// Detects when an enemy enters the suplex hitbox and triggers the suplex sequence.
/// This script should be attached to the hitbox GameObject (usually a child of the player).
/// </summary>
public class SuplexHitboxCaller : MonoBehaviour
{
    // Reference to the PlayerSuplex component in the parent hierarchy
    private PlayerSuplex playerSuplex;

   
    private void Awake()
    {
        playerSuplex = GetComponentInParent<PlayerSuplex>();
    }

    /// <summary>
    /// When hitbox is active from the dash ability it detects enemy tag and starts the suplex sequence.
    /// </summary>

    private void OnTriggerEnter(Collider other)
    {
        // Only react if the collider is tagged as "Enemy" and we have a PlayerSuplex reference
        if (other.CompareTag("Enemy") || other.CompareTag("DontRespawn") || other.CompareTag("Macro") || other.CompareTag("Drone") || other.CompareTag("Solid") || other.CompareTag("GhostDancer") || other.CompareTag("Stunned Rocky") && playerSuplex != null)
        {
           //  Debug.Log("hitboxcollider called");
            if (other.CompareTag("Drone"))
            {
                other.gameObject.GetComponent<FlyingAI>().grabbed = true;
            }
            if (other.CompareTag("Solid"))
            {
                other.gameObject.GetComponent<Level2BossManager>().grabbed = true;
            }
            gameObject.SetActive(false); // Disable hitbox after a successful trigger to prevent multiple calls
            playerSuplex.StartSuplex(other); // Begin the suplex sequence on the enemy
        }
    }
}
