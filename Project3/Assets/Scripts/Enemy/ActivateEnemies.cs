using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ActivateEnemies : MonoBehaviour
{
    public GameObject enemyHealth;
    public GameObject targetGameObject;
    public GameObject window;
    public GameObject areaOneEnemies;
    public GameObject wave1;
    public GameObject wave2;
    public GameObject wave3;
    public bool start = false;
    private float spawnTimer;
    public float maxSpawnTimer;

    private void Start()
    {
        spawnTimer = maxSpawnTimer;
    }

    private void Update()
    {
        if (start)
        {
            spawnTimer -= Time.deltaTime;
            wave1.SetActive(true);
        }
        if (spawnTimer <= 10)
        {
            wave2.SetActive(true);
        }
        if (spawnTimer <= 0)
        {
            start = false;
            spawnTimer = maxSpawnTimer;
            wave3.SetActive(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Collider"))
        {
            if (targetGameObject != null)
            {
                start = true;
                enemyHealth.SetActive(true);
                window.SetActive(true);
                areaOneEnemies.SetActive(false);
                AudioManager.PlayShoalPhrase1();
                Destroy(targetGameObject);
            }
        }
    }
}
