using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class RockyRhodesManager : MonoBehaviour
{
    RockyRhodesAttacks attacks;
    [HideInInspector] public Rigidbody rb;
    public GameObject player;
    [HideInInspector] public NavMeshAgent agent;
    public Slider healthSlider;
    RhockyHealth health;
    RhockyAbilities abilities;

    [HideInInspector] public float moveSpeed;
    public float arena1MoveSpeed;
    public float arena3MoveSpeed;

    [Header("Rope Rush")]
    public int numberOfRopeRusheCharges;
    private int maxRopeRushCharges;
    public float ropeRushForce;
    public Transform[] ropeRushStartPoints;
    public float interactionDistance;

    [Header("Cannonball")]
    public Transform[] tiles;
    public float jumpForce;
    public float jumpTime;
    public float jumpDelay;
    public float slamForce;
    public GameObject shockwave;

    [Header("Enhanced Rope Rush")]
    public float enhancedRopeRushForce;
    public Transform[] enhancedRopeRushStartPoints;
    public int numberOfEnhancedRopeRusheCharges;
    private int maxEnhancedRopeRushCharges;

    [Header("Arenas")]
    public bool arena1;
    public bool arena2;
    public bool arena3;
    [HideInInspector] public bool open;
    public GameObject arena1Floor;

    [Header("Flags")]
    public bool canPerformAction = true;
    public bool ropeRush;
    public bool cannonball;
    public bool enhancedRopeRush;
    public bool grabbed;
    public bool navOff;

    private void Awake()
    {
        attacks = GetComponent<RockyRhodesAttacks>();
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        health = GetComponent<RhockyHealth>();
        abilities = GetComponent<RhockyAbilities>();
    }

    private void Start()
    {
        moveSpeed = arena1MoveSpeed;
        maxRopeRushCharges = numberOfRopeRusheCharges;
        maxEnhancedRopeRushCharges = numberOfEnhancedRopeRusheCharges;
        agent.speed = moveSpeed;
    }

    private void Update()
    {
        if (arena3)
        {
            moveSpeed = arena3MoveSpeed;
        }
        attacks.StartRopeRush();
        attacks.StartCannonball();
        attacks.StartEnhancedRopeRush();
        if (navOff)
        {
            agent.enabled = false;
        }
        if (!navOff)
        {
            agent.enabled = true;
        }
        OpenArenas();
    }

    private void OpenArenas()
    {
        if (healthSlider.value <= 0)
        {
            if (arena1)
            {
                agent.enabled = false;
                arena1Floor.SetActive(false);
                StartCoroutine(ChangeArena1(3f));
            }
            if (arena2)
            {
                open = true;
                StartCoroutine(ChangeArena2(3f));
            }
        }
    }

    private IEnumerator ChangeArena1(float delay)
    {
        yield return new WaitForSeconds(delay);
        arena1 = false;
        arena2 = true;
        Vector3 newPos = transform.position;
        newPos.y = 30.63f;
        newPos.z = 751.97f;
        transform.position = newPos;
    }

    private IEnumerator ChangeArena2(float delay)
    {
        yield return new WaitForSeconds(delay);
        arena2 = false;
        arena3 = true;
        Vector3 newPos = transform.position;
        newPos.y = 7.1f;
        transform.position = newPos;
    }

    private IEnumerator UnGrab(float delay)
    {
        yield return new WaitForSeconds(delay);
        canPerformAction = true;
        grabbed = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Shockwave"))
        {
            if (CompareTag("Stunned Rocky"))
            {
                if (arena1 || arena3)
                {
                    canPerformAction = true;
                    grabbed = false;
                }
                health.TakeDamage();
                gameObject.tag = "Rocky Rhodes";
                if (arena2)
                {
                    StartCoroutine(UnGrab(1));
                }

            }
        }
    }
}
