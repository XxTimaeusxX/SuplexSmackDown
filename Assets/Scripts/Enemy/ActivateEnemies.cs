using UnityEngine;

public class ActivateEnemies : MonoBehaviour
{
    public GameObject hoardEnemies;
    public GameObject enemyHealth;
    public GameObject targetGameObject;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Collider"))
        {
            if (targetGameObject != null)
            {
                hoardEnemies.SetActive(true);
                enemyHealth.SetActive(true);
                Destroy(targetGameObject);
            }
        }
    }
}
