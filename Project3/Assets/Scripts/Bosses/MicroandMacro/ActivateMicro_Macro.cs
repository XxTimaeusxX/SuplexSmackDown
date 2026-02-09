using UnityEngine;

public class ActivateMicro_Macro : MonoBehaviour
{
    public GameObject BossColliderTrigger;
    public GameObject bossFight;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("BossTrigger"))
        {
            if (BossColliderTrigger != null)
            {
                bossFight.SetActive(true);
                Destroy(BossColliderTrigger);
            }
        }
    }
}
