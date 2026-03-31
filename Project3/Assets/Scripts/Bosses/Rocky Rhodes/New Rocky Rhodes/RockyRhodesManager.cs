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
    public int numberOfCharges;
    [HideInInspector] public int chargesRemaining;
    public float rushForce;
    public float chargeDuration;
    public Transform[] ropeRushStartPoints;
    public float interactionDistance;

    [Header("Arenas")]
    public bool open;
    public bool arena1, arena2, arena3;

    [Header("Flags")]
    public bool canPerformAction = true;
    public bool ropeRush;

    private void Awake()
    {
        attacks = GetComponent<RockyRhodesAttacks>();
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        agent.speed = moveSpeed;
    }

    private void Update()
    {
        attacks.StartRopeRush();
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
}
