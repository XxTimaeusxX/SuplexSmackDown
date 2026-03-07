using UnityEngine;

public class EnemyRespawn : MonoBehaviour
{
    public GameObject enemyRespawn;
    public GameObject enemyRespawn2;
    public GameObject enemyRespawn3;
    public GameObject enemyRespawn4;
    public GameObject enemyRespawn5;
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
        if (enemyRespawn3 != null)
        {
            if (other.gameObject == enemyRespawn3 && this.gameObject.tag == "DontRespawn")
            {
                if (respawnPoint != null)
                {
                    transform.position = respawnPoint.position;
                }
            }
        }
        if (enemyRespawn4 != null)
        {
            if (other.gameObject == enemyRespawn4 && this.gameObject.tag == "DontRespawn")
            {
                if (respawnPoint != null)
                {
                    transform.position = respawnPoint.position;
                }
            }
        }
        if (enemyRespawn4 != null)
        {
            if (other.gameObject == enemyRespawn4 && this.gameObject.tag == "DontRespawn")
            {
                if (respawnPoint != null)
                {
                    transform.position = respawnPoint.position;
                }
            }
        }
    }
}
