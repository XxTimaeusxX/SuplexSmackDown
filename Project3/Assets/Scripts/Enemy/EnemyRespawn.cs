using UnityEngine;

public class EnemyRespawn : MonoBehaviour
{
    public GameObject enemyRespawn;
    public Transform respawnPoint;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == enemyRespawn)
        {
            if (respawnPoint != null)
            {
                transform.position = respawnPoint.position;
            }
        }
    }
}
