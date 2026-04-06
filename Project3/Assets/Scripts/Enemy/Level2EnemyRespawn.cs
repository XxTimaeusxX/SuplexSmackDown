using UnityEngine;

public class Level2EnemyRespawn : MonoBehaviour
{
    public Transform repsawnPoint;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Respawn"))
        {
            transform.position = collision.transform.position;
        }
    }
}
