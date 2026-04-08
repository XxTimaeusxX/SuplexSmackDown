using System.Linq;
using UnityEngine;

public class NewEnemyRespawn : MonoBehaviour
{
    private Vector3 respawnPoint;
    public GameObject[] objectThatActivatesRespawn;

    private void Start()
    {
        respawnPoint = transform.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (objectThatActivatesRespawn.Contains(collision.gameObject))
        {
            transform.position = respawnPoint;
        }
    }
}
