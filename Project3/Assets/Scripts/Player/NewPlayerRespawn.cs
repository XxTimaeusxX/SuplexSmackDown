using System.Linq;
using UnityEngine;

public class NewPlayerRespawn : MonoBehaviour
{
    PlayerHealth health;
    private static bool respawnForBossFight;

    private Vector3 respawnPoint;
    private static Vector3 bossRespawnPoint;
    public GameObject[] objectsThatChangeRespawnPointForBossBattle;
    public GameObject[] objectThatActivatesRespawn;
    public GameObject[] objectThatChangesRespawnPoint;

    private void Awake()
    {
        health = GetComponent<PlayerHealth>();
    }

    private void Start()
    {
        if (respawnForBossFight)
        {
            transform.position = bossRespawnPoint;
        }
        respawnPoint = transform.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (objectThatActivatesRespawn.Contains(collision.gameObject))
        {
            transform.position = respawnPoint;
            health.TakeDamage();
        }

        if (objectThatChangesRespawnPoint.Contains(collision.gameObject))
        {
            respawnPoint = transform.position;
        }

        if (objectsThatChangeRespawnPointForBossBattle.Contains(collision.gameObject))
        {
            bossRespawnPoint = transform.position;
            respawnForBossFight = true;
        }
    }
}
