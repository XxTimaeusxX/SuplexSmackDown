using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.ProBuilder;
using UnityEngine.Rendering;

[DisallowMultipleComponent]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class GhostDancer : EnemyBase
{
    
    [Header("Projectile")]
    [SerializeField] private GameObject WindGustProjectilePrefab;
    public Transform SpawnPoint;
    public float Projectilespeed;
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

        DefaultWalkSpeed = patrolWalkSpeed;
        DefaultRunMoveSpeed = patrolRunSpeed;
        DefaultAttackCooldown = attackCooldown;

        Projectilesize = 1f;
        Projectilespeed = 10f;
        
    
    }

    // Update is called once per frame
    public override void Update()
    {
        if (isPushed && pushCooldown < 0 && IsKillable)
        {
            gameObject.SetActive(false);
        }
        base.Update();
       // Floating();
    }
    protected override void CustomAttack()
    {
        // Implement Ghost Dancer's specific attack behavior here
        AudioManager.PlayDancerAttack();
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
        windprojectile.transform.localScale = Vector3.one * Projectilesize;

    }
    public IEnumerator SpeedBuff()
    {

        Projectilesize = 2f;
        Projectilespeed = 20f;
        patrolWalkSpeed *= 20f;
        patrolRunSpeed *= 20f;
        attackCooldown *= .4f;
        float elapsedTime = 0f;
        float Duration = 7f;
        while (elapsedTime < Duration)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
         Projectilesize = 1f;
         Projectilespeed = 10f;
         patrolWalkSpeed = DefaultWalkSpeed;
         patrolRunSpeed = DefaultRunMoveSpeed;
        attackCooldown = DefaultAttackCooldown;
    }
    public void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);
        if (collision.gameObject.tag == "Shockwave")
        {
            AudioManager.PlaySFX(AudioManager.Instance.DancerHurt, 1f);
        }
    }
}
