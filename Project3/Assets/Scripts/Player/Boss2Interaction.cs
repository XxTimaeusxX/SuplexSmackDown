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
            objectRenderer.material = slowMaterial;
            animator.speed = slowMoveSpeed;
            playerMovement.enabled = false;
            slowTimer -= Time.deltaTime;
        }
        if (slowTimer <= 0)
        {
            slow = false;
            slowTimer = maxSlowTimer;
            playerMovement.enabled = true;
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
