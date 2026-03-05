using UnityEngine;
using UnityEngine.AI;

public class TravelToLocation : MonoBehaviour
{
    [Header("Settings")]
    public float travelSpeed;

    public Collider bossCollider;
    public NavMeshAgent agent;
    public Level2BossManager boss;
    public Rigidbody rb;

    [Header("Movement Locations")]
    public Transform waypoint;

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
            rb.useGravity = false;
            MoveLocation();
            bossCollider.isTrigger = true;
            agent.enabled = false;
            boss.enabled = false;
        }
    }

    private void MoveLocation()
    {
        float step = travelSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, waypoint.position, step);
    }
}
