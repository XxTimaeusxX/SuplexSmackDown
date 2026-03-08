using UnityEngine;

public class GhostDancer : EnemyBase
{
    [SerializeField] private GameObject WindGustProjectilePrefab;
    [Header("Projectile")]
    public Transform SpawnPoint;
    public float Projectilespeed = 4f;
    public float Projectilesize;
    [Header("Ghost Dancer Settings")]
    public bool IsKillable = true;
    public float DefaultWalkSpeed;
    public float DefaultRunMoveSpeed;
    public float DefaultAttackCooldown;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    protected override void CustomAttack()
    {
        // Implement Ghost Dancer's specific attack behavior here
        ShootWindGust();

    }

    public void ShootWindGust()
    {
               // Implement the logic to shoot a wind gust projectile here
        Debug.Log("Ghost Dancer shoots a wind gust!");
    }
}
