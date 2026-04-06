using UnityEngine;

public class Level2EnemyRespawn : MonoBehaviour
{
    private Vector3 respawnPoint;

    private void Start()
    {
        respawnPoint = transform.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Respawn"))
        {
            transform.position = respawnPoint;
        }
    }
}
