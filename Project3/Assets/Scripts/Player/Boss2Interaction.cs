using System.Threading;
using UnityEngine;

public class Boss2Interaction : MonoBehaviour
{
    [Header("References")]
    public PlayerMovement playerMovement;
    public GameObject bossShockwave;
    public PlayerHealth health;

    [Header("Strings")]
    public string boss;

    [Header("Settings")]
    public float slowMoveSpeed;
    public float maxSlowTimer;
    private float slowTimer;
    private float collideTimer;
    public float maxCollideTimer;

    [Header("Bools")]
    public bool slow;
    public bool collided;

    private void Start()
    {
        slowTimer = maxSlowTimer;
        collideTimer = maxCollideTimer;
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
        if (collided)
        {
            collideTimer -= Time.deltaTime;
        }
        if (collideTimer <= 0)
        {
            collided = false;
            collideTimer = maxCollideTimer;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(boss))
        {
            slow = true;
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        if (!collided)
        {
            if (other.CompareTag("BossShockwave"))
            {
                collided = true;
                health.TakeDamage();
                health.iFrames = true;
            }
        }
        
    }
}
