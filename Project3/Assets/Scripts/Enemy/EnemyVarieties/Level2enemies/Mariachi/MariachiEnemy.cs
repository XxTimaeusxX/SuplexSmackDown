
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

    [Header("Projectile")]
    public Transform SpawnPoint;
    public float speed;
    public float size;
    [Header("Mariachi Settings")]
    public bool IsKillable = true;
    public float DefaultWalkSpeed;
    public float DefaultRunMoveSpeed;
    public float DefaultAttackCooldown;

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
        if (isPushed && pushCooldown < 0 && IsKillable)
        {
            Debug.Log("Mariachi is pushed and can be");
            gameObject.SetActive(false);
            return;
        }
    }
    protected override void CustomAttack()
    {
      //  Debug.Log("Mariachi attack!");
      AudioManager.PlayGuitarNote();
        Shootmusic();
    }
    public void Shootmusic()
    {
       //  Debug.Log("Mariachi shoots music!");
         Transform spawnPoint = SpawnPoint != null ? SpawnPoint : transform;
      
        GameObject projectile = Instantiate(MusicalProjectilePrefab, spawnPoint.position, spawnPoint.rotation);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        rb.linearVelocity = spawnPoint.forward * speed;
        var randomscale = Random.Range(size *1f, size * 7f);
        projectile.transform.localScale = Vector3.one * randomscale;

    }
}

