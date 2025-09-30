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
            StartCoroutine(SuplexJumpAndSlam(other));
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

    IEnumerator SuplexJumpAndSlam(Collider enemy)
    {
        // Move up
        float upTime = 0.2f;
        float upHeight = 50f;
        Vector3 startPos = playerDash.transform.position;
        Vector3 apex = startPos + Vector3.up * upHeight;
        float t = 0;
        while (t < upTime)
        {
            t += Time.deltaTime;
            playerDash.transform.position = Vector3.Lerp(startPos, apex, t / upTime);
            yield return null;
        }

        // Pause at apex
        yield return new WaitForSeconds(0.1f);

        // Unparent and slam enemy
        var rb = enemy.GetComponent<Rigidbody>();
        if (rb != null)
        {
            enemy.transform.SetParent(null);
            rb.isKinematic = false;
            rb.linearVelocity = Vector3.down * 20f; // Slam force
        }
        // Move down
        float downTime = 0.2f;
        t = 0;
        Vector3 endPos = startPos; // or slightly below for effect
        while (t < downTime)
        {
            t += Time.deltaTime;
            playerDash.transform.position = Vector3.Lerp(apex, endPos, t / downTime);
            yield return null;
        }

      //  heldEnemy = null;
    }
}
