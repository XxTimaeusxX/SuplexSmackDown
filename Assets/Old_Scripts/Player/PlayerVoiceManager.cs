using UnityEngine;

public class PlayerVoiceManager : MonoBehaviour
{
      [Header("Voice Line Settings")]
    private bool hasPlayedLevelStartLine = false;

    private void Start()
    {
        // Play level start voice line
        PlayLevelStartLine();
    }

    /// <summary>
    /// Play the player's voice line when the level starts
    /// </summary>
    private void PlayLevelStartLine()
    {
        if (!hasPlayedLevelStartLine)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.PlayNarration(AudioManager.Instance.CohettePhraseOneclip, 1f);
            }
            hasPlayedLevelStartLine = true;
        }
    }

    public void PlaySuplexVoicePhrase(SuplexAbilities suplexType)
    {
        int RandomSuperValue = Random.Range(1, 4); // Assuming you have 3 variations for each suplex type
        int RandomRainbowAndSuperValue = Random.Range(1, 3); // Assuming you have 2 variations for Rainbow and Super suplex types
        switch (suplexType)
        {
            case SuplexAbilities.Long:
                switch(RandomSuperValue)
                {
                    case 1: AudioManager.PlaySFX(AudioManager.Instance.CohetteLongSuplex, 1f);  break;                      
                    case 2:AudioManager.PlaySFX(AudioManager.Instance.CohetteLongSuplex2, 1f); break;
                    case 3: AudioManager.PlaySFX(AudioManager.Instance.CohetteLongSuplex3, 1f); break;               
                }
                break;
            case SuplexAbilities.Rainbow:
                switch(RandomRainbowAndSuperValue)
                {
                    case 1: AudioManager.PlaySFX(AudioManager.Instance.CohetteRainbowSuplex, 1f); break;
                    case 2: AudioManager.PlaySFX(AudioManager.Instance.CohetteRainbowSuplex2, 1f); break;
                }
                break;
            case SuplexAbilities.Super:
                switch(RandomRainbowAndSuperValue)
                {
                    case 1: AudioManager.PlaySFX(AudioManager.Instance.CohetteSuperSuplex, 1f); break;
                    case 2: AudioManager.PlaySFX(AudioManager.Instance.CohetteSuperSuplex2, 1f); break;
                }
                break;
        }   
    }
    public void PlayDamageVoiceLine()
    {
        int randomValue = Random.Range(1, 5); // Assuming you have 4 variations for damage voice lines
        switch(randomValue)
        {
            case 1: AudioManager.PlaySFX(AudioManager.Instance.CohetteHurtClip, 1f); break;
            case 2: AudioManager.PlaySFX(AudioManager.Instance.CohetteHurtClip2, 1f); break;
            case 3: AudioManager.PlaySFX(AudioManager.Instance.CohetteHurtClip3, 1f); break;
            case 4: AudioManager.PlaySFX(AudioManager.Instance.CohetteHurtClip4, 1f); break;
        }
    }

    public void PlayerDashPhrase()
    {
        int randomValue = Random.Range(1, 4); // Assuming you have 3 variations for dash voice lines
        switch(randomValue)
        {
         case 1: AudioManager.PlaySFX(AudioManager.Instance.DashClip, 1f); break;
         case 2: AudioManager.PlaySFX(AudioManager.Instance.Dash2Clip, 1f); break;
         case 3: AudioManager.PlaySFX(AudioManager.Instance.Dash3Clip, 1f); break;
        }
    }
    public void PlayerFallPhrase()
    {
        int randomvalue = Random.Range(1, 4);
        switch (randomvalue)
        {
            case 1: AudioManager.PlaySFX(AudioManager.Instance.FallClip, 1f); break;
            case 2: AudioManager.PlaySFX(AudioManager.Instance.FallClip2, 1f); break;
            case 3: AudioManager.PlaySFX(AudioManager.Instance.FallClip3, 1f); break;
        }
    }
    /// <summary>
    /// Call this when player encounters a specific boss
    /// </summary>
/*    public void OnEncounterBoss(string bossName)
    {
        switch (bossName)
        {
            case "MicroBoss":
                AudioManager.PlayNarration(AudioManager.Instance.PhraseOneclip, 1f);
                break;

            case "RockyRhodes":
                AudioManager.PlayNarration(AudioManager.Instance.PhraseOneclip, 1f);
                break;
        }
    }*/

    /// <summary>
    /// Call this when player defeats a boss
    /// </summary>
  /*  public void OnBossDefeated(string bossName)
    {
        AudioManager.PlayNarration(AudioManager.Instance.PhraseOneclip, 1f);
    }*/

    /// <summary>
    /// Call this for any custom trigger/event
    /// </summary>
  /*  public void PlayCustomLine(AudioClip clip)
    {
        if (clip != null)
        {
            AudioManager.PlayNarration(clip, 1f);
        }
    }*/
}
