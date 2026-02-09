using UnityEngine;

public class ActivateMicro_Macro : MonoBehaviour
{
    public GameObject BossColliderTrigger;
    public GameObject BossColliderTrigger2;
    public GameObject bossFight;
    [SerializeField] InGameMenuManager menu;
    [SerializeField] Cinema_final cinema;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("BossTrigger"))
        {
            if (BossColliderTrigger != null)
            {
                bossFight.SetActive(true);
                Destroy(BossColliderTrigger);
                cinema.introPlayed = true;
            }
        }
        if (other.CompareTag("BossTrigger"))
        {
            if (BossColliderTrigger2 != null)
            {
                cinema.introPlayed = true;
            }
        }
    }
}
