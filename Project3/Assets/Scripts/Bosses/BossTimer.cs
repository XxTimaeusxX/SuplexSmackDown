using UnityEngine;

public class BossTimer : MonoBehaviour
{
    public GameObject microMacroBoss;
    public GameObject microMacroHealth;
 
    public float timer = 6f;

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            microMacroBoss.SetActive(true);
            microMacroHealth.SetActive(true);
            AudioManager.PlayBoss1BGM();
            enabled = false; // disable this script after activating the boss and health UI
        }
    }
}
