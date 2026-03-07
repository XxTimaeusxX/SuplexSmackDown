using UnityEngine;
using UnityEngine.AI;

[DisallowMultipleComponent]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class GhostDancer : EnemyBase
{
    
    [Header("Projectile")]
    [SerializeField] private GameObject WindGustProjectilePrefab;
    public Transform SpawnPoint;
    public float Projectilespeed = 4f;
    public float Projectilesize;
    [Header("Ghost Dancer Settings")]
    public bool IsKillable = true;
    public float DefaultWalkSpeed;
    public float DefaultRunMoveSpeed;
    public float DefaultAttackCooldown;
    Vector3 _StartPos;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Start()
    {
        base.Start();
        _StartPos = transform.position;
        DefaultWalkSpeed = agent.speed;
        DefaultRunMoveSpeed = agent.speed;
        DefaultAttackCooldown = attackCooldown;
        agent.stoppingDistance = meleeRange - 1f;
    
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
       // Floating();
    }
    protected override void CustomAttack()
    {
        // Implement Ghost Dancer's specific attack behavior here
        ShootWindGust();

    }
    public void Floating()
    {
        // Implement floating behavior here (e.g., using a sine wave for vertical movement)
        float floatHeight = _StartPos.y + Mathf.Sin(Time.time* 5f) * 1f; // Adjust amplitude as needed
        transform.position = new Vector3(transform.position.x,  floatHeight, transform.position.z);

    }
    public void ShootWindGust()
    {


        Debug.Log("Ghost Dancer shoots wind gust!");
        Transform spawnPoint = SpawnPoint != null ? SpawnPoint : transform;
        GameObject windprojectile = Instantiate(WindGustProjectilePrefab, spawnPoint.position, spawnPoint.rotation);
        Rigidbody rb = windprojectile.GetComponent<Rigidbody>();
        rb.linearVelocity = spawnPoint.forward * Projectilespeed;

    }
}
