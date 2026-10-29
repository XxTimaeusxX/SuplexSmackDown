using UnityEngine;

public class Level2Respawn : MonoBehaviour
{
    public static bool level2Respawn;
    public GameObject respawnCollider;
    public Transform respawnPoint;

    private void Awake()
    {
        if (level2Respawn)
        {
            gameObject.transform.position = respawnPoint.position;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == respawnCollider)
        {
            respawnCollider.SetActive(false);
            level2Respawn = true;
        }
    }
}
