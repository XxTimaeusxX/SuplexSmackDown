using UnityEngine;
using UnityEngine.AI;

public class TravelToLocation : MonoBehaviour
{
    [Header("Settings")]
    public float travelSpeed;

    public Collider bossCollider;
    public NavMeshAgent agent;

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
            MoveLocation(waypoint.position);
            bossCollider.isTrigger = true;
            agent.enabled = false;
        }
    }

    private void MoveLocation(Vector3 targetPos)
    {
        Vector3 dir = targetPos - transform.position;
        transform.position = Vector3.MoveTowards(transform.position, targetPos, travelSpeed * Time.deltaTime);
    }
}
