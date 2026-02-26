using UnityEngine;

public class MusicNoteProjectile : MonoBehaviour
{
    public float lifeTime = 5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter(Collision collision)
    {
 
        Destroy(gameObject); // Destroy the projectile on collision
    }
   /* private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Music note hit the player!");
            // Handle collision with player, e.g., apply damage
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(); // Example damage value
            }
        }
        
        // Destroy the projectile after collision
        Destroy(gameObject);
    }   */
}
