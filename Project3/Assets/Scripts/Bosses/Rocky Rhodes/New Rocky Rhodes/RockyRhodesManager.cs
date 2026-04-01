using System.Collections;
using System.Collections.Generic;
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

    [Header("Flags")]
    public bool canPerformAction = true;
    public bool ropeRush;
    public bool cannonball;
    public bool enhancedRopeRush;

    private void Awake()
    {
        attacks = GetComponent<RockyRhodesAttacks>();
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
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

        OpenArenas();
    }

    private void OpenArenas()
    {
        if (healthSlider.value <= 0)
        {
            open = true;
            healthSlider.value = 6;
            StartCoroutine(ChangeArena(3f));
        }
    }

    private IEnumerator ChangeArena(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (arena1)
        {
            arena1 = false;
            arena2 = true;
        }
        if (arena2)
        {
            arena2 = false;
            arena3 = true;
        }
    }
}
