using UnityEngine;

public class HealthPickUp : MonoBehaviour
{
    [SerializeField] int points = 1;
    [SerializeField] int healthAmount = 1;



    void Update()
    {
        transform.Rotate(0f, 360f * Time.deltaTime, 0f);
    }
    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        var playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth == null) return;

        // If player already full, don't pick up
        if (playerHealth.currentHealth >= playerHealth.maxHealth)
        {
            Debug.Log("Player health full — pickup ignored.");
            return;
        }

        // Apply healing (pickup is responsible for healing)
        int newHP = Mathf.Min(playerHealth.currentHealth + healthAmount, playerHealth.maxHealth);
        playerHealth.UpdateHealth(newHP);

        // Award score via ScoreManager
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.AddScore(points);

        Destroy(gameObject);
    }
}
