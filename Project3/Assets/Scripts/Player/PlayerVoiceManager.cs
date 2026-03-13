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
