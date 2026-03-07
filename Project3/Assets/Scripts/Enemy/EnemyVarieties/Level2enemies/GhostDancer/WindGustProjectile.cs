using System.Collections;
using UnityEngine;

public class WindGustProjectile : MonoBehaviour
{
    [Header("Knockback Settings")]
    public float KnockbackForce = 10f;
    public float KnockbackDuration = 0.5f;
    public float UpwardForce = 2f;

    [Header("Projectile Settings")]
    public float Lifetime = 5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, Lifetime);
    }

     private void OnTriggerEnter(Collider other)
     {
         if (other.CompareTag("Player"))
         {
            if (!other.CompareTag("Player"))
                return;

            PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                // Push away from the projectile
                Vector3 dir = (other.transform.position - transform.position).normalized;
                dir.y = 0f; // horizontal only

                Vector3 newVelocity = dir * KnockbackForce + Vector3.up * UpwardForce;

                // Just like MariGold: set velocity once, no coroutine / timing
                playerMovement.velocity = newVelocity;
            }

            Destroy(gameObject);
        }
     }
    private IEnumerator ApplyKnockbackOverTime(CharacterController controller, Vector3 knockVelocity)
    {
        float elapsed = 0f;

        while (elapsed < KnockbackDuration && controller != null)
        {
            float t = elapsed / KnockbackDuration;

            // Ease-out: strong at start, weaker towards the end
            float strength = 1f - t*2f; // linear
            // or use something smoother: float strength = 1f - t * t;

            Vector3 frameMove = knockVelocity * strength * Time.fixedTime;
            controller.Move(frameMove);
            

            elapsed += Time.deltaTime;
            yield return null;
        }
    }
}
