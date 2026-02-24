using UnityEngine;
using UnityEngine.UI;

public class Level2Boss : MonoBehaviour
{
    [Header("Settings")]
    public float moveSpeed;
    public float travelSpeed;

    [Header("Health")]
    public Slider bossHealth;
    public GameObject bossHealthScreen;

    [Header("Movement Locations")]
    public Transform waypoint1;
    public Transform waypoint2;

    [Header("Bools")]
    public bool move1;

    void Start()
    {
        move1 = false;
    }

    void Update()
    {
        if (move1)
        {
            MoveLocation(waypoint1.position);
        }
    }

    private void MoveLocation(Vector3 targetPos)
    {
        Vector3 dir = targetPos - transform.position;
        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
    }
}
