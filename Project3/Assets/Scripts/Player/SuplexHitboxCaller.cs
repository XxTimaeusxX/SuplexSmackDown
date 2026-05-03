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
        // 1. Check if the object is Rocky Rhodes AND if he is vulnerable
      /*  if (other.TryGetComponent<RockyRhodes>(out RockyRhodes rockyBoss))
        {
          
            if (other.gameObject.GetComponent<RockyRhodes>().isGrabbed = true)
            {
                Debug.Log("cangrabrocky-------------------------------------------------");
              //  rockyBoss.IsStunnedForGrab = false; // Turn off the stun so he can't be grabbed twice
                gameObject.SetActive(false); // Disable hitbox after a successful trigger to prevent multiple calls
                playerSuplex.StartSuplex(other); // Begin the suplex sequence on Rocky
            }
            // If Rocky is hit but NOT stunned, we just ignore the collision.
            return;
        }*/
        // Only react if the collider is tagged as "Enemy" and we have a PlayerSuplex reference
        if (other.gameObject.tag == "Enemy" || other.gameObject.tag == "DontRespawn"
            || other.gameObject.tag == "Macro" || other.gameObject.tag == "Micro" || other.gameObject.tag == "Drone" || 
            other.gameObject.tag == "Solid" || other.gameObject.tag == "GhostDancer" || other.gameObject.tag == "Stunned Rocky"
            && playerSuplex != null)
        {
           //  Debug.Log("hitboxcollider called");
            if (other.gameObject.tag == "Drone")
            {
                other.gameObject.GetComponent<FlyingAI>().grabbed = true;
            }
            if (other.gameObject.tag == "Solid")
            {
                other.gameObject.GetComponent<Level2BossManager>().grabbed = true;
            }
            if (other.gameObject.tag == "Stunned Rocky")
            {
                other.gameObject.GetComponent<RockyRhodesManager>().grabbed = true;
            }
            if (other.gameObject.tag == "Micro")
            {
                AudioManager.PlayMicroGrabbed();
            }
            gameObject.SetActive(false); // Disable hitbox after a successful trigger to prevent multiple calls
            playerSuplex.StartSuplex(other); // Begin the suplex sequence on the enemy
        }
    }
}
