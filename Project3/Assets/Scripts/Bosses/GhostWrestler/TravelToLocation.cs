using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TravelToLocation : MonoBehaviour
{
    [Header("Activate Flowers")]
    public List<GameObject> flowers;
    public int currentFlowerIndex;

    [Header("Settings")]
    public float travelSpeed;
    public float minDistanceThreshold;
    public int currentWaypointIndex;
    public float groundLevel;
    private float groundTimer;
    public float maxGroundTimer;

    public Collider bossCollider;
    public NavMeshAgent agent;
    public Level2BossManager boss;
    public Rigidbody rb;
    public AudioManager audioManager;
    NewEnemyRespawn respawn;

    [Header("Movement Locations")]
    public List<Transform> waypoints;

    [Header("Bools")]
    public bool moveToLocation;

    private void Awake()
    {
        audioManager = FindAnyObjectByType<AudioManager>();
        respawn = GetComponent<NewEnemyRespawn>();
    }

    void Start()
    {
        moveToLocation = false;
        groundTimer = maxGroundTimer;
    }

    void Update()
    {
        if (moveToLocation)
        {
            MoveLocation();
            rb.useGravity = false;
            bossCollider.isTrigger = true;
            agent.enabled = false;
            boss.enabled = false;
            groundTimer -= Time.deltaTime;
        }
        if (transform.position == waypoints[currentWaypointIndex].position)
        {
            Restart();
        }
    }

    private void MoveLocation()
    {
        float step = travelSpeed * Time.deltaTime;
        if (currentFlowerIndex == 0)
        {
            AudioManager.PlayHurt1();
        }
        if (currentFlowerIndex == 1)
        {
            AudioManager.PlayHurt2();
        }
        AudioManager.PlayMoveArena();
        transform.position = Vector3.MoveTowards(transform.position, waypoints[currentWaypointIndex].position, step);
        respawn.respawnPoint = waypoints[currentWaypointIndex].position;
        if (flowers != null)
        {
            flowers[currentFlowerIndex].SetActive(true);
        }
    }

    public void Restart()
    {
        Debug.Log("Restart");
        moveToLocation = false;
        rb.useGravity = true;
        bossCollider.isTrigger = false;
        agent.enabled = true;
        boss.enabled = true;
        currentWaypointIndex++;
        currentFlowerIndex++;
        groundTimer = maxGroundTimer;
    }
}
