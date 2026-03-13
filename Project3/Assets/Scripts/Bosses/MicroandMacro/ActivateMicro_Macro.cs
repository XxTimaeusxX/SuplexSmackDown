using UnityEngine;

public class ActivateMicro_Macro : MonoBehaviour
{
    public GameObject BossColliderTrigger;
    public GameObject BossColliderTrigger2;
    public GameObject bossFight;
    public GameObject areaTwoEnemies;
    public GameObject bossArena;
    public float enemySpawnTimer;
    public bool enemySpawn = false;
    public ActivateRespawn respawn;
    [SerializeField] InGameMenuManager menu;
    [SerializeField] Cinema_final cinema;

    private void Update()
    {
        if (enemySpawn)
        {
            enemySpawnTimer -= Time.deltaTime;
        }
        if (enemySpawnTimer <= 0)
        {
            enemySpawn = false;
            enemySpawnTimer = 1;
            bossFight.SetActive(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("BossTrigger"))
        {
            respawn.fallTime = 10f;
            if (BossColliderTrigger != null)
            {
                enemySpawn = true;
                bossArena.SetActive(true);
                areaTwoEnemies.SetActive(false);
                Destroy(BossColliderTrigger);
                cinema.introPlayed = true;
                cinema.isPhase1Intro = true;
                
            }
        }
        if (other.CompareTag("BossTrigger"))
        {
            respawn.fallTime = 10f;
            if (BossColliderTrigger2 != null)
            {
                cinema.introPlayed = true;
            }
        }
    }
}
