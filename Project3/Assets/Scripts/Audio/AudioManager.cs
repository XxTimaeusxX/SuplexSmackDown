using UnityEngine;

public sealed class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Volumes")]
    [Range(0f, 1f)] public float masterVolume = 1f;
    [Range(0f, 1f)] public float musicVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    [Header("BGM Clips")]
    public AudioClip mainMenuBGM;
    public AudioClip constructionBGM;
    public AudioClip boss1BGM;
    public AudioClip VictoryBGM;
    public AudioClip DefeatBGM;

    [Header("Player SFX")]
    public AudioClip footstepClip;
    public AudioClip jumpingClip;
    public AudioClip health3Clip;
    public AudioClip health2Clip;
    public AudioClip health1Clip;
    public AudioClip GameOverClip;
    public AudioClip GrabClip;
    [Header("Suplex SFX")]
    public AudioClip LaunchSoundClip;
    public AudioClip suplexSlamClip;
    public AudioClip SuperSuplexSlam;

    [Header("Health Packs SFX")]
    public AudioClip smallHealthPackClip;
    [Header("Enemy SFX")]
    
    public AudioClip enemySlapClip;
    public AudioClip enemyDieclip;

    [Header("Shoal Enemy SFX")]
    public AudioClip shoalFallingClip;
    public AudioClip shoalIdleclip;
    public AudioClip shoalDamageHitClip;

    [Header("Construction Enemy SFX")]
    public AudioClip ConstructionIdleClip;
    public AudioClip ConstructionSeenOneClip;
    public AudioClip ConstructionSeentwoClip;
    public AudioClip ConstructionFallingClip;
    public AudioClip ConstructionDamageHitOneClip;
    public AudioClip ConstructionDamageHitTwoClip;

    [Header("Macro Enemy SFX")]
    public AudioClip MacroIdleClip;
    public AudioClip MacroRetreatOneClip;
    public AudioClip MacroRetreatTwoClip;
    public AudioClip MacroDamageHitOneClip;
    public AudioClip MacroDamageHitTwoClip;

    [Header("Micro Enemy SFX")]
    public AudioClip MicroChaseOneClip; // chase state sounds when micro is chasing player 
    public AudioClip MicroPrepareAttackClip; // tells macro hes ready to throw him
    public AudioClip MicroAttackClip; // attack sound when micro is thrown at player
    public AudioClip MicroDamageHitOneClip; 
    public AudioClip MicroDamageHitTwoClip; 
    public AudioClip MicroDieClip; // when micro dies sound

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Ensure sources exist if not assigned
        if (!musicSource)
        {
            var go = new GameObject("MusicSource");
            go.transform.SetParent(transform, false);
            musicSource = go.AddComponent<AudioSource>();
            musicSource.playOnAwake = false;
            musicSource.loop = true;
            musicSource.spatialBlend = 0f; // 2D
        }
        if (!sfxSource)
        {
            var go = new GameObject("SFXSource");
            go.transform.SetParent(transform, false);
            sfxSource = go.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
            sfxSource.loop = false;
            sfxSource.spatialBlend = 0f; // 2D
        }

        ApplyVolumes();
    }

    private void ApplyVolumes()
    {
        if (musicSource) musicSource.volume = musicVolume*masterVolume;
        if (sfxSource) sfxSource.volume = sfxVolume*masterVolume;
    }

    // Music control
    public static void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (!Instance || !clip) return;
        Instance.musicSource.clip = clip;
        Instance.musicSource.loop = loop;
        Instance.musicSource.volume = Instance.musicVolume*Instance.masterVolume;
        Instance.musicSource.Play();
    }

    public static void StopMusic()
    {
        if (!Instance) return;
        Instance.musicSource.Stop();
    }

    public static void SetMusicVolume(float volume)
    {
        if (!Instance) return;
        Instance.musicVolume = Mathf.Clamp01(volume);
        Instance.ApplyVolumes();
    }

    public static void SetSFXVolume(float volume)
    {
        if (!Instance) return;
        Instance.sfxVolume = Mathf.Clamp01(volume);
        Instance.ApplyVolumes();
    }

    public static void SetMasterVolume(float volume)
    {
        if (!Instance) return;
        Instance.masterVolume = Mathf.Clamp01(volume);
        Instance.ApplyVolumes();
    }

    // Generic SFX (optional)
    public static void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (!Instance || !clip) return;
        Instance.sfxSource.PlayOneShot(clip, Mathf.Clamp01(volume) * Instance.sfxVolume);
    }

    // BGM
    public static void PlayMainMenuBGM() => PlayMusic(Instance?.mainMenuBGM);
    public static void PlayConstructionBGM() => PlayMusic(Instance?.constructionBGM);
    public static void PlayBoss1BGM() => PlayMusic(Instance?.boss1BGM);
    public static void PLayVictory() => PlayMusic(Instance?.VictoryBGM,false);
    public static void PlayDefeat() => PlayMusic(Instance?.DefeatBGM,false);

    // player SFX
    public static void PlayFootstep() => Instance?.sfxSource?.PlayOneShot(Instance?.footstepClip, Instance.sfxVolume);
    public static void PlayJumping() => Instance?.sfxSource?.PlayOneShot(Instance?.jumpingClip, Instance.sfxVolume);

    // player suplex SFX
    public static void PlaySuplexStart() => Instance?.sfxSource?.PlayOneShot(Instance?.LaunchSoundClip, Instance.sfxVolume);

    public static void PlaySuplexSlam() => Instance?.sfxSource?.PlayOneShot(Instance?.suplexSlamClip, Instance.sfxVolume);

    // PLayer health SFX
    public static void PlayHealth3() => Instance?.sfxSource?.PlayOneShot(Instance?.health3Clip, Instance.sfxVolume);

    public static void PlayHealth2() => Instance?.sfxSource?.PlayOneShot(Instance?.health2Clip, Instance.sfxVolume);
   
    public static void PlayHealth1() => Instance?.sfxSource?.PlayOneShot(Instance?.health1Clip, Instance.sfxVolume);

    public static void PlayGameOver() => Instance?.sfxSource?.PlayOneShot(Instance?.GameOverClip, Instance.sfxVolume);


    // Enemy SFX
    public static void PlayEnemySlap() => Instance?.sfxSource?.PlayOneShot(Instance?.enemySlapClip, Instance.sfxVolume);


   // ----Shoal Enemy SFX----
    public static void PlayEnemyDie() => Instance?.sfxSource?.PlayOneShot(Instance?.enemyDieclip, Instance.sfxVolume);

    public static void PlayShoalFalling() => Instance?.sfxSource?.PlayOneShot(Instance?.shoalFallingClip, Instance.sfxVolume);
    public static void PlayShoalIdle() => Instance?.sfxSource?.PlayOneShot(Instance?.shoalIdleclip, Instance.sfxVolume);
    public static void PlayShoalDamageHit() => Instance?.sfxSource?.PlayOneShot(Instance?.shoalDamageHitClip, Instance.sfxVolume);
    // ----Construction Enemy SFX----
    public static void PlayConstructionIdle() => Instance?.sfxSource?.PlayOneShot(Instance?.ConstructionIdleClip, Instance.sfxVolume);
    public static void PlayConstructionSeenOne() => Instance?.sfxSource?.PlayOneShot(Instance?.ConstructionSeenOneClip, Instance.sfxVolume);
    public static void PlayConstructionSeentwo() => Instance?.sfxSource?.PlayOneShot(Instance?.ConstructionSeentwoClip, Instance.sfxVolume);
    public static void PlayConstructionFalling() => Instance?.sfxSource?.PlayOneShot(Instance?.ConstructionFallingClip, Instance.sfxVolume);
    public static void PlayConstructionDamageHitOne() => Instance?.sfxSource?.PlayOneShot(Instance?.ConstructionDamageHitOneClip, Instance.sfxVolume);
    public static void PlayConstructionDamageHitTwo() => Instance?.sfxSource?.PlayOneShot(Instance?.ConstructionDamageHitTwoClip, Instance.sfxVolume);
    // ----Macro Enemy SFX----
    public static void PlayMacroIdle() => Instance?.sfxSource?.PlayOneShot(Instance?.MacroIdleClip, Instance.sfxVolume);
    public static void PlayMacroRetreatOne() => Instance?.sfxSource?.PlayOneShot(Instance?.MacroRetreatOneClip, Instance.sfxVolume);
    public static void PlayMacroRetreatTwo() => Instance?.sfxSource?.PlayOneShot(Instance?.MacroRetreatTwoClip, Instance.sfxVolume);
    public static void PlayMacroDamageHitOne() => Instance?.sfxSource?.PlayOneShot(Instance?.MacroDamageHitOneClip, Instance.sfxVolume);
    public static void PlayMacroDamageHitTwo() => Instance?.sfxSource?.PlayOneShot(Instance?.MacroDamageHitTwoClip, Instance.sfxVolume);
    // ----Micro Enemy SFX----
    public static void PlayMicroChaseOne() => Instance?.sfxSource?.PlayOneShot(Instance?.MicroChaseOneClip, Instance.sfxVolume);
    public static void PlayMicroPrepareAttack() => Instance?.sfxSource?.PlayOneShot(Instance?.MicroPrepareAttackClip, Instance.sfxVolume);
    public static void PlayMicroAttack() => Instance?.sfxSource?.PlayOneShot(Instance?.MicroAttackClip, Instance.sfxVolume);
    public static void PlayMicroDamageHitOne() => Instance?.sfxSource?.PlayOneShot(Instance?.MicroDamageHitOneClip, Instance.sfxVolume);
    public static void PlayMicroDamageHitTwo() => Instance?.sfxSource?.PlayOneShot(Instance?.MicroDamageHitTwoClip, Instance.sfxVolume);
    public static void PlayMicroDie() => Instance?.sfxSource?.PlayOneShot(Instance?.MicroDieClip, Instance.sfxVolume);


}