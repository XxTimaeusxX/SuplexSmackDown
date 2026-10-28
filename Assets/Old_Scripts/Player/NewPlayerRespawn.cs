using System.Linq;
using UnityEngine;

public class NewPlayerRespawn : MonoBehaviour
{
    PlayerHealth health;
    public static bool respawnForBossFight;

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

    private void FixedUpdate()
    {
        Collider[] hitColliders = Physics.OverlapBox(transform.position, transform.localScale / 2);

        foreach (var hitCollider in hitColliders)
        {
            if (objectThatActivatesRespawn.Contains(hitCollider.gameObject))
            {
                if (!respawnForBossFight)
                {
                    transform.position = respawnPoint;
                    health.TakeDamage();
                }
                else if (respawnForBossFight)
                {
                    transform.position = bossRespawnPoint;
                    health.TakeDamage();
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (objectThatChangesRespawnPoint.Contains(other.gameObject))
        {
            respawnPoint = transform.position;
        }
        if (objectsThatChangeRespawnPointForBossBattle.Contains(other.gameObject))
        {
            bossRespawnPoint = transform.position;
            respawnForBossFight = true;
        }
    }
}
