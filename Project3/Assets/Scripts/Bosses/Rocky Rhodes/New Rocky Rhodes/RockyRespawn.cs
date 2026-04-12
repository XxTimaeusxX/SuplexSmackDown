using UnityEngine;

public class RockyRespawn : MonoBehaviour
{
    RockyRhodesManager manager;
    RhockyHealth health;
    RockyRhodes rocky;
    RhockyAbilities abilities;

    public static bool arena2;
    public static bool arena3;

    public Transform arena1Spawn;
    public Transform arena2Spawn;
    public Transform arena3Spawn;

    public GameObject arena1Floor;
    public GameObject arena2Floor;

    private void Awake()
    {
        manager = GetComponent<RockyRhodesManager>();
        health = GetComponent<RhockyHealth>();
        rocky = GetComponent<RockyRhodes>();
        abilities = GetComponent<RhockyAbilities>();
    }

    private void Start()
    {
        if (arena2)
        {
            manager.arena1 = false;
            manager.arena2 = true;
            transform.position = arena2Spawn.position;
            health._currentPhase = 2;
            health.Applyhealth(5);
            manager.agent.enabled = false;
            rocky.enabled = false;
            abilities.enabled = false;
            manager.rb.isKinematic = false;
            arena1Floor.SetActive(false);
        }
        else if (arena3)
        {
            manager.arena1 = false;
            manager.arena3 = true;
            manager.agent.Warp(arena3Spawn.position);
            health._currentPhase = 3;
            health.Applyhealth(6);
            arena1Floor.SetActive(false);
            arena2Floor.SetActive(false);
        }
        else
        {
            transform.position = arena1Spawn.position;
        }
    }

    private void Update()
    {
        if (manager.arena2)
        {
            arena2 = true;
        }
        if (manager.arena3)
        {
            arena2 = false;
            arena3 = true;
        }
    }
}
