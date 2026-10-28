using UnityEngine;

public class Level3Announcer : MonoBehaviour
{
    public enum AnnouncerType
    {
        Stage2,
        Stage3
      
    }

    public AnnouncerType announcerType;
    private bool hasPlayed = false;

    void OnTriggerEnter(Collider other)
    {
        if (hasPlayed)
            return;

        if (other.CompareTag("Player"))
        {
            switch (announcerType)
            {
                case AnnouncerType.Stage2:
                    AudioManager.PlayAnnouncerTransferToStage2();
                    break;
                case AnnouncerType.Stage3:
                    AudioManager.PlayAnnouncerTransferToStage3();
                    break;
            }
            hasPlayed = true;
        }
    }
}
