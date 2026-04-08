using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class RockyActionManager : MonoBehaviour
{
    [Header("References")]
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public NavMeshAgent agent;
    public GameObject player;

    [Header("Scripts")]
    RockyArena1Attacks arena1Attack;
    RockyArena2Attacks arena2Attack;
    RockyArena3Attacks arena3Attack;
    RockyRhodes rocky;

    [Header("Arena 1")]

    [Header("Arena 2")]
    public Transform[] tiles;
    public float jumpForce;
    public float jumpTime;
    public float jumpDelay;
    public float slamForce;
    public GameObject shockwave;
    public bool canChooseRandom;
    public bool cannonball;
    public Transform chosenPoint;
    public float heightOffset;

    [Header("Arena 3")]

    [Header("Flags")]
    public bool canPerformAction;
    public bool canAttack;
    public bool grabbed;
    public bool arena1;
    public bool arena2;
    public bool arena3;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        arena1Attack = GetComponent<RockyArena1Attacks>();
        arena2Attack = GetComponent<RockyArena2Attacks>();
        arena3Attack = GetComponent<RockyArena3Attacks>();
        rocky = GetComponent<RockyRhodes>();
    }

    private void Update()
    {
        arena2Attack.StartCannonball();
        if (canAttack)
        {
            if (arena1)
            {

            }

            if (arena2)
            {
                if (canPerformAction)
                {
                    cannonball = true;
                }
            }

            if (arena3)
            {

            }
        }

        if (arena1 || arena3)
        {
            rocky.enabled = true;
        }
        if (arena2)
        {
            rocky.enabled = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Arena2"))
        {
            canChooseRandom = true;
            if (canPerformAction && !grabbed)
            {
                canPerformAction = false;
                arena2Attack.Shockwave();
                rb.linearVelocity = Vector3.zero;
                gameObject.tag = "Stunned Rocky";
                if (arena2 || arena1)
                {
                    StartCoroutine(arena2Attack.RepeatCannonball(jumpDelay));
                }
            }
        }
    }
}
