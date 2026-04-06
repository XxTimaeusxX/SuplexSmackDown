using UnityEngine;

public class NewEnemyRespawn : MonoBehaviour
{
    private Vector3 respawnPoint;
    public GameObject objectThatActivatesRespawn;

    private void Start()
    {
        respawnPoint = transform.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == objectThatActivatesRespawn)
        {
            transform.position = respawnPoint;
        }
    }
}
