using UnityEngine;

public class Level3Announcer : MonoBehaviour
{
    public GameObject Trigger1;
    public GameObject Trigger2;

    private bool hasPlayedIntercom1 = false;
    private bool hasPlayedIntercom2 = false;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == Trigger1 && !hasPlayedIntercom1)
        {
            AudioManager.PlayAnnouncerTransferToStage2();
            hasPlayedIntercom1 = true;
        }
        if (collision.gameObject == Trigger2 && !hasPlayedIntercom2)
        {
            AudioManager.PlayAnnouncerTransferToStage3();
            hasPlayedIntercom2 = true;
        }
    }
}
