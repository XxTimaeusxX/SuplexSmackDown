using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MariachiBuff : MonoBehaviour
{
    [Header("Buff Settings")]
    public float MovementSpeedMultiplier = 1.5f;
    public float ProjectileSpeedMultiplier = 1.5f;
    public float ProjectileSize;
    public List<MariachiEnemy> mariachiEnemies = new List<MariachiEnemy>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ApplyBuff();
    }
    public void ApplyBuff()
    {
        
            // Only consider active enemies
            var activeMariachis = mariachiEnemies.Where(m => m != null && m.gameObject.activeInHierarchy).ToList();
            int activeEnemies = activeMariachis.Count;
            foreach (var mariachi in activeMariachis)
        {
            mariachi.patrolWalkSpeed = mariachi.DefaultWalkSpeed;
            mariachi.patrolRunSpeed = mariachi.DefaultRunMoveSpeed;
            if(activeEnemies == 3)
            {
                mariachi.patrolWalkSpeed *= 2f; 
                mariachi.patrolRunSpeed *= 2f;
                
            }
            else if (activeEnemies == 2)
            {
                Debug.Log("buff is 1.5x");
                mariachi.patrolRunSpeed *= 15f;
                mariachi.patrolWalkSpeed *= 15f;
            }
            else if (activeEnemies == 1)
            {
                mariachi.patrolRunSpeed *= .5f;
                mariachi.patrolWalkSpeed *= .5f;
                Debug.Log("buff is normal");

            }
            //  mariachi.speed *= ProjectileSpeedMultiplier;
            //  mariachi.size *= ProjectileSize;
        }
    }
}
