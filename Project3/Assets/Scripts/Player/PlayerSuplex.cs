using UnityEngine;
using System.Collections;
public class PlayerSuplex : MonoBehaviour
{
   private PlayerDash playerDash;
    private Collider heldEnemy;
    private void Awake()
    {
        if (playerDash == null)
            playerDash = GetComponentInParent<PlayerDash>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Suplexed an enemy!");
            // Add suplex logic here (e.g., apply force, play animation, etc.)
            HoldEnemy(other);
            StartCoroutine(SuplexThrowCoroutine(other));
        }
    }
    void HoldEnemy(Collider enemy)
    {
        heldEnemy = enemy;
        enemy.transform.SetParent(playerDash.suplexHoldPoint);
        enemy.transform.localPosition = Vector3.zero;
        // Optionally disable enemy AI here
        var rb = enemy.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true; // Prevent physics while held
        }
    }

    IEnumerator SuplexThrowCoroutine(Collider enemy)
    {
        yield return new WaitForSeconds(0.3f); // Hold for a moment

        // Launch up
        var rb = enemy.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            enemy.transform.SetParent(null);
            rb.linearVelocity = Vector3.up * 10f; // Adjust force as needed
        }

        yield return new WaitForSeconds(0.4f); // Time in air

        // Slam down
        if (rb != null)
        {
            rb.linearVelocity = Vector3.down * 20f; // Adjust slam force as needed
        }

        // Optionally re-enable AI here after a short delay
        heldEnemy = null;
    }
}
