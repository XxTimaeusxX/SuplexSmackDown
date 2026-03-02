using System.Threading;
using UnityEngine;

public class Boss2Interaction : MonoBehaviour
{
    [Header("References")]
    public PlayerMovement playerMovement;

    [Header("Strings")]
    public string boss;

    [Header("Settings")]
    public float slowMoveSpeed;
    public float maxSlowTimer;
    private float slowTimer;

    [Header("Bools")]
    public bool slow;

    private void Start()
    {
        slowTimer = maxSlowTimer;
    }

    private void Update()
    {
        if (slow)
        {
            playerMovement.moveSpeed = slowMoveSpeed;
            slowTimer -= Time.deltaTime;
        }
        if (slowTimer <= 0)
        {
            slow = false;
            slowTimer = maxSlowTimer;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(boss))
        {
            slow = true;
        }
    }
}
