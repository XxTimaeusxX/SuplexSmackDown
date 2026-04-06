
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[DisallowMultipleComponent]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class MariachiEnemy : EnemyBase
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private GameObject MusicalProjectilePrefab;
    [SerializeField] private Transform leader;
    [Header("Projectile")]
    public Transform SpawnPoint;
    public float Projectilespeed = 4f;
    public float Projectilesize;
    [Header("Mariachi Settings")]
    public bool IsKillable = true;
    public float DefaultWalkSpeed;
    public float DefaultRunMoveSpeed;
    public float DefaultAttackCooldown;
    private bool playerDetected = false;


    // Update is called once per frame
    public void Start()
    {
        base.Start();
        DefaultWalkSpeed = agent.speed;
        DefaultRunMoveSpeed = agent.speed;
        DefaultAttackCooldown = attackCooldown;
    }
    public override void Update()
      {
          base.Update();
        if (leader != null && agent.enabled && agent.isOnNavMesh)
        {
            agent.destination = leader.transform.position;
            agent.speed = patrolRunSpeed;
        }
        if (isPushed && pushCooldown < 0 && IsKillable)
        {
            Debug.Log("Mariachi is pushed and can be");
            gameObject.SetActive(false);
            return;
        }
        DetectPlayer();
    }
    public void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);
        if (collision.gameObject.CompareTag("Shockwave"))
        {
            AudioManager.PlayMariachiHurt();
        }
    }
    private void DetectPlayer()
    {
        if (m_Distance < chaseRange && !playerDetected)
        {
            playerDetected = true;
            AudioManager.PlayMariachiDetection();
        }
        else
        {
            playerDetected = false;
        }
    }

    protected override void CustomAttack()
    {
      //  Debug.Log("Mariachi attack!");
      
        Shootmusic();
    }
    public void Shootmusic()
    {
        //  Debug.Log("Mariachi shoots music!");
       // AudioManager.PlayGuitarNote();
        Transform spawnPoint = SpawnPoint != null ? SpawnPoint : transform;
      
        GameObject projectile = Instantiate(MusicalProjectilePrefab, spawnPoint.position, spawnPoint.rotation);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        rb.linearVelocity = spawnPoint.forward * Projectilespeed;
      //  var randomscale = Random.Range(size *1f, size * 7f);
      //  projectile.transform.localScale = Vector3.one * randomscale;
       projectile.transform.localScale = Vector3.one * Projectilesize;

    }
   
}

