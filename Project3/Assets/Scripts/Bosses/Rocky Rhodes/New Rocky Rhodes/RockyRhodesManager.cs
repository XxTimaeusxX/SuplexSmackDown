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

    public float moveSpeed;

    [Header("Rope Rush")]
    public int numberOfRopeRusheCharges;
    private int maxRopeRushCharges;
    public float ropeRushForce;
    public Transform[] ropeRushStartPoints;
    public float interactionDistance;

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
    public bool enhancedRopeRush;

    private void Awake()
    {
        attacks = GetComponent<RockyRhodesAttacks>();
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        maxRopeRushCharges = numberOfRopeRusheCharges;
        maxEnhancedRopeRushCharges = numberOfEnhancedRopeRusheCharges;
        agent.speed = moveSpeed;
    }

    private void Update()
    {
        attacks.StartRopeRush();
        attacks.StartEnhancedRopeRush();

        if (numberOfRopeRusheCharges == 0 && attacks.collided)
        {
            Stunned();
        }
        if (numberOfEnhancedRopeRusheCharges == 0 && attacks.collided)
        {
            Stunned();
        }
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
    }

    public void Stunned()
    {
        gameObject.tag = "Stunned Rocky";
    }
}
