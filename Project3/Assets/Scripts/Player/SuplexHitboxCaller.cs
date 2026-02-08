using UnityEngine;

// Last Edited: 1/19/2026 by Istvan W.

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


    private void Awake()
    {
        suplexController = GetComponentInParent<SuplexController>();
        suplexConfig = GetComponentInParent<SuplexConfig>();
    }

    /// <summary>
    /// When hitbox is active from the dash ability it detects thisEnemy tag and starts the suplex sequence.
    /// </summary>

    //MARK: Trigger Detection
    private void OnTriggerEnter(Collider other)
    {
        // Only react if the collider is tagged as "Enemy" and we have a OGPlayerSuplex reference
        if (other.CompareTag("canGrab") || other.CompareTag("Enemy"))
        {
            //Debug.Log("Object hit");
            if (other.CompareTag("Enemy"))
            {
                //Debug.Log("Enemy tag detected");

                EnemyBase thisEnemy = other.GetComponentInParent<EnemyBase>();      // Get the enemy script from the object we hit

                if (thisEnemy != null)
                {
                    // 2. Tell the thisEnemy to enter carried state
                    thisEnemy.EnterCarriedState(suplexConfig.carryPoint);

                    // 3. Tell the player suplex script which thisEnemy was grabbed
                    suplexController.StartSuplex(thisEnemy);
                }
                else
                    Debug.LogWarning("Hit object has tag but no EnemyBase in parent!");
            }
            else if (other.CompareTag("canGrab"))
            {
                Debug.Log("canGrab tag detected");
            }

            


            hitTarget = true;

            gameObject.SetActive(false); // Disable hitbox after a successful trigger to prevent multiple calls
        }
    }
}
