using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Level1EnemyRespawn : MonoBehaviour
{
    [HideInInspector] public Vector3 respawnPoint;
    public GameObject[] objectThatActivatesRespawn;
    NavMeshAgent agent;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        respawnPoint = transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (objectThatActivatesRespawn.Contains(other.gameObject))
        {
            transform.position = respawnPoint;
            agent.enabled = true;
        }
    }
}
