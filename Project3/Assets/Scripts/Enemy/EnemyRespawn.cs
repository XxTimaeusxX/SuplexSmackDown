using UnityEngine;

public class EnemyRespawn : MonoBehaviour
{
    public GameObject enemyRespawn;
    public GameObject enemyRespawn2;
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
        if (enemyRespawn2 != null)
        {
            if (other.gameObject == enemyRespawn2 && this.gameObject.tag == "DontRespawn")
            {
                if (respawnPoint != null)
                {
                    transform.position = respawnPoint.position;
                }
            }
        }
    }
}
