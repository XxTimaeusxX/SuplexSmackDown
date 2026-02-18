using UnityEngine.UI;
using UnityEngine;

public class ActivateEnemies : MonoBehaviour
{
    public GameObject hoardEnemies;
    public GameObject enemyHealth;
    public GameObject targetGameObject;
    public GameObject window;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Collider"))
        {
            if (targetGameObject != null)
            {
                hoardEnemies.SetActive(true);
                enemyHealth.SetActive(true);
                window.SetActive(true);
                AudioManager.PlayShoalPhrase1();
                Destroy(targetGameObject);
            }
        }
    }
}
