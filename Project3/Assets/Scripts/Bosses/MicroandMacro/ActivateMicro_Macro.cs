using UnityEngine;

public class ActivateMicro_Macro : MonoBehaviour
{
    public GameObject BossColliderTrigger;
    public GameObject BossColliderTrigger2;
    public GameObject bossFight;
    public ActivateRespawn respawn;
    [SerializeField] InGameMenuManager menu;
    [SerializeField] Cinema_final cinema;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("BossTrigger"))
        {
            respawn.fallTime = 10f;
            if (BossColliderTrigger != null)
            {
                bossFight.SetActive(true);
                Destroy(BossColliderTrigger);
                cinema.introPlayed = true;
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
