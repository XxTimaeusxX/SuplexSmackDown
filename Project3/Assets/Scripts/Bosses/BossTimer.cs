using UnityEngine;

public class BossTimer : MonoBehaviour
{
    public GameObject microMacroBoss;
    public GameObject microMacroHealth;
    public float timer = 4f;

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            microMacroBoss.SetActive(true);
            microMacroHealth.SetActive(true);
        }
    }
}
