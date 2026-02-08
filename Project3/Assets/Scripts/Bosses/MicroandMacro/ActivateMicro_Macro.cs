using UnityEngine;

public class ActivateMicro_Macro : MonoBehaviour
{
    public GameObject microMacroBoss;
    public GameObject microMacroHealth;
    public GameObject BossColliderTrigger;
    public GameObject drones;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("BossTrigger"))
        {
            if (BossColliderTrigger != null)
            {
                microMacroBoss.SetActive(true);
                microMacroHealth.SetActive(true);
                drones.SetActive(true);
                Destroy(BossColliderTrigger);
            }
        }
    }
}
