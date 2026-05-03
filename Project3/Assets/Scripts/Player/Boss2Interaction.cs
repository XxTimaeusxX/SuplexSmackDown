using System.Collections;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class Boss2Interaction : MonoBehaviour
{
    [Header("References")]
    public PlayerMovement playerMovement;
    public GameObject bossShockwave;
    public PlayerHealth health;
    public Material playerMaterial;
    public Material slowMaterial;
    public Renderer objectRenderer;
    public Animator animator;
    public Level2BossManager level2BossManager;
    public PlayerDash playerDash;

    [Header("Strings")]
    public string boss;

    [Header("Settings")]
    public float slowMoveSpeed;
    public float maxSlowTimer;
    private float slowTimer;
    private float collideTimer;
    public float maxCollideTimer;
    public float normalAniamtionSpeed;
    public float slowAnimationSpeed;

    [Header("Bools")]
    public bool slow;
    public bool collided;

    private void Start()
    {
        animator = GetComponent<Animator>();
        slowTimer = maxSlowTimer;
        collideTimer = maxCollideTimer;
    }

    private void Update()
    {
        if (slow)
        {
            level2BossManager.slow = true;
            objectRenderer.material = slowMaterial;
            animator.speed = slowMoveSpeed;
            playerMovement.enabled = false;
            playerDash.enabled = false;
            slowTimer -= Time.deltaTime;
        }
        if (slowTimer <= 0)
        {
            slow = false;
            level2BossManager.slow = false;
            slowTimer = maxSlowTimer;
            playerMovement.enabled = true;
            playerDash.enabled = true;
            animator.speed = normalAniamtionSpeed;
            objectRenderer.material = playerMaterial;
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
        if (other.gameObject.tag == boss)
        {
            slow = true;
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        if (!collided)
        {
            if (other.gameObject.tag == "BossShockwave")
            {
                collided = true;
                health.TakeDamage();
                health.iFrames = true;
            }
        }
        
    }
}
