using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TravelToLocation : MonoBehaviour
{
    [Header("Settings")]
    public float travelSpeed;
    public float minDistanceThreshold;
    private int currentWaypointIndex = 0;

    public Collider bossCollider;
    public NavMeshAgent agent;
    public Level2BossManager boss;
    public Rigidbody rb;

    [Header("Movement Locations")]
    public List<Transform> waypoints;

    [Header("Bools")]
    public bool moveToLocation;

    void Start()
    {
        moveToLocation = false;
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
        }
        if (transform.position == waypoints[currentWaypointIndex].position)
        {
            Restart();
        }
    }

    private void MoveLocation()
    {
        float step = travelSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, waypoints[currentWaypointIndex].position, step);
    }

    public void Restart()
    {
        Debug.Log("Restart");
        moveToLocation = false;
        rb.useGravity = true;
        bossCollider.isTrigger = false;
        agent.enabled = true;
        boss.enabled = true;
    }
}
