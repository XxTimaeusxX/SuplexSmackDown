using Unity.Cinemachine;
using UnityEngine;

public class MusicNoteProjectile : MonoBehaviour
{
    public float lifeTime = 5f;
    PlayerHealth player;

    private void Awake()
    {
        player = FindAnyObjectByType<PlayerHealth>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player.TakeDamage();
            AudioManager.PlayGuitarNote();
            Debug.Log("Music note hit the player!");
            // Destroy the projectile after collision
            Destroy(gameObject);
        }


    }
}
