using UnityEngine;

public class AnnouncerPhrases : MonoBehaviour
{
    [SerializeField] RhockyHealth _rhockyHealth;
    private int phase;
    private bool Phrase1Played, Phrase2Played, Phrase3Played, Phrase4Played, Phrase5Played, Phrase6Played = false;

        
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (_rhockyHealth == null) _rhockyHealth = GetComponent<RhockyHealth>();
    }

    // Update is called once per frame
    void Update()
    {
        HealthConditionsPhrases();
    }
    public void HealthConditionsPhrases()
    {
        float health = _rhockyHealth.HealthSlider.value;
        phase = (int)_rhockyHealth._currentPhase;
        if (phase == 1 && health <= 3f && !Phrase1Played) { AudioManager.PlayNarration(AudioManager.Instance.AnnouncerScene1Phrase1, 1f); Phrase1Played = true; }
        if (phase == 1 && health <= 1f && !Phrase2Played) { AudioManager.PlayNarration(AudioManager.Instance.AnnouncerScene1Phrase2, 1f); Phrase2Played = true; }
        if (phase == 2 && health <= 4f && !Phrase3Played) { AudioManager.PlayNarration(AudioManager.Instance.AnnouncerScene2Phrase1, 1f); Phrase3Played = true; }
        if (phase == 2 && health <= 1f && !Phrase4Played) { AudioManager.PlayNarration(AudioManager.Instance.AnnouncerScene2Phrase2, 1f); Phrase4Played = true; }
        if (phase == 3 && health <= 3f && !Phrase5Played) { AudioManager.PlayNarration(AudioManager.Instance.AnnouncerScene3Phrase1, 1f); Phrase5Played = true; }
        if (phase == 3 && health <= 2f && !Phrase6Played) { AudioManager.PlayNarration(AudioManager.Instance.AnnouncerScene3Phrase2, 1f); Phrase6Played = true; }
     }
 }

