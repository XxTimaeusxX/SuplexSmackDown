using UnityEngine;

public class RockyVoiceManager : MonoBehaviour
{
    public void PlayBullrushCharge()
    {
        int rand = Random.Range(0, 2);
        switch (rand)
        {
            case 0: AudioManager.PlaySFX(AudioManager.Instance.BullrushChargePhrase1, 1f); break;
            case 1: AudioManager.PlaySFX(AudioManager.Instance.BullrushChargePhrase2, 1f); break;
        }
    }

    public void PlayBullrushGo()
    {
        int rand = Random.Range(0, 2);
        switch (rand)
        {
            case 0: AudioManager.PlaySFX(AudioManager.Instance.BullrushGoPhrase, 1f); break;
            case 1: AudioManager.PlaySFX(AudioManager.Instance.BullrushGoPhrase2, 1f); break;
        }
    }

    public void PlayHaymaker()
    {
        int rand = Random.Range(0, 3);
        switch (rand)
        {
            case 0: AudioManager.PlaySFX(AudioManager.Instance.HaymakerPhrase, 1f); break;
            case 1: AudioManager.PlaySFX(AudioManager.Instance.HaymakerPhrase2, 1f); break;
            case 2: AudioManager.PlaySFX(AudioManager.Instance.HaymakerPhrase3, 1f); break;
        }
    }

    public void PlayHaymakerGo()
    {
        int rand = Random.Range(0, 3);
        switch (rand)
        {
            case 0: AudioManager.PlaySFX(AudioManager.Instance.HaymakerPhraseGo1, 1f); break;
            case 1: AudioManager.PlaySFX(AudioManager.Instance.HaymakerPhraseGo2, 1f); break;
            case 2: AudioManager.PlaySFX(AudioManager.Instance.HaymakerPhraseGo3, 1f); break;
        }
    }

    public void PlayChestBump()
    {
        int rand = Random.Range(0, 2);
        switch (rand)
        {
            case 0: AudioManager.PlaySFX(AudioManager.Instance.ChestBumpPhrase1, 1f); break;
            case 1: AudioManager.PlaySFX(AudioManager.Instance.ChestBumpPhrase2, 1f); break;
        }
    }

    public void PlayHeelTaunt()
    {
        int rand = Random.Range(0, 3);
        switch (rand)
        {
            case 0: AudioManager.PlaySFX(AudioManager.Instance.HeelTauntPhrase1, 1f); break;
            case 1: AudioManager.PlaySFX(AudioManager.Instance.HeelTauntPhrase2, 1f); break;
            case 2: AudioManager.PlaySFX(AudioManager.Instance.HeelTauntPhrase3, 1f); break;
        }
    }

    public void PlayQTEFail()
    {
        int rand = Random.Range(0, 4);
        switch (rand)
        {
            case 0: AudioManager.PlaySFX(AudioManager.Instance.QTEfailPhrase1, 1f); break;
            case 1: AudioManager.PlaySFX(AudioManager.Instance.QTEfailPhrase2, 1f); break;
            case 2: AudioManager.PlaySFX(AudioManager.Instance.QTEfailPhrase3, 1f); break;
            case 3: AudioManager.PlaySFX(AudioManager.Instance.QTEfailPhrase4, 1f); break;
        }
    }

    public void PlayCannonball()
    {
        int rand = Random.Range(0, 2);
        switch (rand)
        {
            case 0: AudioManager.PlaySFX(AudioManager.Instance.CannonballPhrase1, 1f); break;
            case 1: AudioManager.PlaySFX(AudioManager.Instance.CannonballPhrase2, 1f); break;
        }
    }

    public void PlayHurt()
    {
        AudioManager.PlaySFX(AudioManager.Instance.RockyRhodesHurt, 1f);
    }
}
