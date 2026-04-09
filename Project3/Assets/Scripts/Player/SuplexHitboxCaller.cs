using UnityEngine;

/// <summary>
/// Detects when an thisEnemy enters the suplex hitbox and triggers the suplex sequence.
/// This script should be attached to the hitbox GameObject (usually a child of the player).
/// </summary>

public class SuplexHitboxCaller : MonoBehaviour
{
    // Reference to the SuplexController component in the parent hierarchy
    private SuplexController suplexController;
    private SuplexConfig suplexConfig;

    public bool hitTarget = false;
    public bool hasGrabbed = false; // Prevents grabbing multiple objects at once


    private void Awake()
    {
        suplexController = GetComponentInParent<SuplexController>();
        suplexConfig = GetComponentInParent<SuplexConfig>();
    }

    private void Update()
    {
        if (suplexController.carriedObject == null)
        {
            hasGrabbed = false;
        }
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
        if (other.CompareTag("Enemy") || other.CompareTag("DontRespawn")
            || other.CompareTag("Macro") || other.CompareTag("Micro") || other.CompareTag("Drone") || 
            other.CompareTag("Solid") || other.CompareTag("GhostDancer") || other.CompareTag("Stunned Rocky")
            && playerSuplex != null)
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
            if (other.CompareTag("Stunned Rocky"))
            {
                other.gameObject.GetComponent<RockyRhodesManager>().grabbed = true;
            }
            if (other.CompareTag("Micro"))
            {
                AudioManager.PlayMicroGrabbed();
            }
            gameObject.SetActive(false); // Disable hitbox after a successful trigger to prevent multiple calls
            playerSuplex.StartSuplex(other); // Begin the suplex sequence on the enemy
        }
    }
    /// <summary>
    /// When hitbox is active from the dash ability it detects thisEnemy tag and starts the suplex sequence.
    /// </summary>

    //MARK: Trigger Detection
    private void OnTriggerEnter(Collider other)
    {
        // Stop immediately if we already grabbed something
        if (hasGrabbed)
            return;

        // Only react if the collider is tagged as "Enemy" and we have a OGPlayerSuplex reference
        if (other.CompareTag("canGrab") || other.CompareTag("Enemy"))
        {
            //Debug.Log("Object hit");
            if (other.CompareTag("Enemy"))
            {
                //Debug.Log("Enemy tag detected");
                hasGrabbed = true;

                EnemyBase thisEnemy = other.GetComponentInParent<EnemyBase>();      // Get the enemy script from the object we hit

                if (thisEnemy != null)
                {
                    // 2. Tell the thisEnemy to enter carried state
                    thisEnemy.EnterCarriedState(suplexConfig.carryPoint);

                    // 3. Tell the player suplex script which thisEnemy was grabbed
                    suplexController.StartSuplex(thisEnemy);
                }
                else
                    Debug.LogWarning("Hit object has tag but no OGEnemyBase in parent!");
            }
            else if (other.CompareTag("canGrab"))
            {
                Debug.Log("canGrab tag detected");

                hasGrabbed = true;

                MonoBehaviour[] components = other.GetComponentsInParent<MonoBehaviour>();
                MonoBehaviour carriableMono = null;

                foreach (var comp in components)
                {
                    if (comp is ICarriable)
                    {
                        carriableMono = comp;
                        break;
                    }
                }

                if (carriableMono == null)
                {
                    Debug.LogError($"Object '{other.name}' is tagged canGrab but has no ICarriable component!");
                    return;
                }
                carriableMono.GetComponent<ICarriable>().EnterCarriedState(suplexConfig.carryPoint);
                suplexController.StartSuplex(carriableMono);
            }
            hitTarget = true;
            gameObject.SetActive(false); // Disable hitbox after a successful trigger to prevent multiple calls
        }
    }
}