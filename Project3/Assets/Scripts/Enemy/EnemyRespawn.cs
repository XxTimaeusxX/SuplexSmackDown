using UnityEngine;

public class EnemyRespawn : MonoBehaviour
{
    public GameObject enemyRespawn;
    public Transform respawnPoint;
    public bool outside;
    public float maxTimer;
    [SerializeField] private float timer;

    private void Start()
    {
        outside = false;
        timer = maxTimer;
        respawnPoint.position = transform.position;
    }

    private void Update()
    {
        if (outside)
        {
            timer -= Time.deltaTime;
        }
        if (timer <= 0)
        {
            if (respawnPoint != null)
            {
                transform.position = respawnPoint.position;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == enemyRespawn)
        {
            outside = false;
            timer = maxTimer;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == enemyRespawn)
        {
            if (respawnPoint != null)
            {
                outside = true;
            }
        }
    }
}
