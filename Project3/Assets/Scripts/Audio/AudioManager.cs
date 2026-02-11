using UnityEngine;
using System.Collections;

public sealed class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;
    public AudioSource narratorSource; // New: Dedicated source for narration/dialogue

    [Header("Volumes")]
    [Range(0f, 1f)] public float masterVolume = 1f;
    [Range(0f, 1f)] public float musicVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    [Header("Audio Ducking Settings")]
    [Range(0f, 1f)] public float duckVolume = 0f; // Volume to reduce to during narration
    public float duckSpeed = 2f; // How fast audio ducks/restores
    
    private bool isDucking = false;
    private float currentSfxMultiplier = 1f; // Track current SFX volume multiplier
    private float targetMusicVolume;
    private float targetSfxVolume;

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
    public AudioClip CohettePhraseOneclip;
    [Header("Suplex SFX")]
    public AudioClip LaunchSoundClip;
    public AudioClip suplexSlamClip;
    public AudioClip SuperSuplexSlam;

    [Header("Health Packs SFX")]
    public AudioClip smallHealthPackClip;
    [Header("Enemy SFX")]
    
    public AudioClip enemySlapClip;
    public AudioClip enemyDieclip;
    public AudioClip shoalPhraseClip;
    public AudioClip AngryShoalClip;

    [Header("Shoal Enemy SFX")]
    public AudioClip shoalFallingClip;
    public AudioClip shoalIdleclip;
    public AudioClip shoalDamageHitClip;
    public AudioClip ShoalPhrase1Clip;

    [Header("Drone enemy SFX")]
    public AudioClip DroneDetectionClip;
    public AudioClip DroneIdleClip;
    public AudioClip DroneGrabbedClip;
    public AudioClip DroneDieClip;

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
    public AudioClip MicroEncounterClip;// intro1 
    public AudioClip MicroTwoHealthClip;// intro2
    public AudioClip MicroOneHealthClip;

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
        if (!narratorSource)
        {
            var go = new GameObject("NarratorSource");
            go.transform.SetParent(transform, false);
            narratorSource = go.AddComponent<AudioSource>();
            narratorSource.playOnAwake = false;
            narratorSource.loop = false;
            narratorSource.spatialBlend = 0f; // 2D
            narratorSource.priority = 0; // Highest priority
        }

        targetMusicVolume = musicVolume;
        targetSfxVolume = sfxVolume;
        ApplyVolumes();
    }

    void Update()
    {
        // Smooth ducking transition
        if (isDucking)
        {
            musicSource.volume = Mathf.Lerp(musicSource.volume, duckVolume * masterVolume, Time.deltaTime * duckSpeed);
            currentSfxMultiplier = Mathf.Lerp(currentSfxMultiplier, duckVolume, Time.deltaTime * duckSpeed);
        }
        else
        {
            musicSource.volume = Mathf.Lerp(musicSource.volume, musicVolume * masterVolume, Time.deltaTime * duckSpeed);
            currentSfxMultiplier = Mathf.Lerp(currentSfxMultiplier, 1f, Time.deltaTime * duckSpeed);
        }
    }

    private void ApplyVolumes()
    {
        if (musicSource) musicSource.volume = musicVolume*masterVolume;
        if (sfxSource) sfxSource.volume = sfxVolume*masterVolume;
        if (narratorSource) narratorSource.volume = masterVolume; // Always full volume
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
        Instance.targetMusicVolume = Instance.musicVolume;
        Instance.ApplyVolumes();
    }

    public static void SetSFXVolume(float volume)
    {
        if (!Instance) return;
        Instance.sfxVolume = Mathf.Clamp01(volume);
        Instance.targetSfxVolume = Instance.sfxVolume;
        Instance.ApplyVolumes();
    }

    public static void SetMasterVolume(float volume)
    {
        if (!Instance) return;
        Instance.masterVolume = Mathf.Clamp01(volume);
        Instance.ApplyVolumes();
    }

    // Generic SFX - NOW RESPECTS DUCKING
    public static void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (!Instance || !clip) return;
        // Apply ducking multiplier to PlayOneShot volume parameter
        Instance.sfxSource.PlayOneShot(clip, Mathf.Clamp01(volume) * Instance.sfxVolume * Instance.currentSfxMultiplier);
    }

    // --- NEW: Narration/Priority Audio System ---
    /// <summary>
    /// Plays a priority audio clip (like narration or boss dialogue) and ducks other audio.
    /// Audio automatically restores after the clip finishes.
    /// </summary>
    public static void PlayNarration(AudioClip clip, float volume = 1f)
    {
        if (!Instance || !clip) return;
        Instance.StartCoroutine(Instance.PlayNarrationCoroutine(clip, volume));
    }

    private IEnumerator PlayNarrationCoroutine(AudioClip clip, float volume)
    {
        isDucking = true;
        
        // INSTANT mute music and SFX - no lerp delay
        musicSource.volume = duckVolume * masterVolume;
        currentSfxMultiplier = duckVolume;
        
        narratorSource.PlayOneShot(clip, volume);
        
        // Wait for clip to finish
        yield return new WaitForSeconds(clip.length);
        
        isDucking = false;
    }

    /// <summary>
    /// Manually start ducking audio (useful for dialogue sequences)
    /// </summary>
    public static void StartDucking()
    {
        if (!Instance) return;
        Instance.isDucking = true;
    }

    /// <summary>
    /// Manually stop ducking audio
    /// </summary>
    public static void StopDucking()
    {
        if (!Instance) return;
        Instance.isDucking = false;
    }

    // BGM
    public static void PlayMainMenuBGM() => PlayMusic(Instance?.mainMenuBGM);
    public static void PlayConstructionBGM() => PlayMusic(Instance?.constructionBGM);
    public static void PlayBoss1BGM() => PlayMusic(Instance?.boss1BGM);
    public static void PLayVictory() => PlayMusic(Instance?.VictoryBGM,false);
    public static void PlayDefeat() => PlayMusic(Instance?.DefeatBGM,false);

    // player SFX
    public static void PlayFootstep() => PlaySFX(Instance?.footstepClip, 1f);
    public static void PlayJumping() => PlaySFX(Instance?.jumpingClip, 1f);

    // player suplex SFX
    public static void PlaySuplexStart() => PlaySFX(Instance?.LaunchSoundClip, 1f);

    public static void PlaySuplexSlam() => PlaySFX(Instance?.suplexSlamClip, 1f);

    // PLayer health SFX
    public static void PlayCohettePhraseOne() => PlayNarration(Instance?.CohettePhraseOneclip, 1f); // Uses narration system for important player lines
    public static void PlayHealth3() => PlaySFX(Instance?.health3Clip, 1f);

    public static void PlayHealth2() => PlaySFX(Instance?.health2Clip, 1f);
   
    public static void PlayHealth1() => PlaySFX(Instance?.health1Clip, 1f);

    public static void PlayGameOver() => PlaySFX(Instance?.GameOverClip, 1f);


    // Enemy SFX
    public static void PlayEnemySlap() => PlaySFX(Instance?.enemySlapClip, 1f);


   // ----Shoal Enemy SFX----
    public static void PlayEnemyDie() => PlaySFX(Instance?.enemyDieclip, 1f);

    public static void PlayShoalFalling() => PlaySFX(Instance?.shoalFallingClip, 1f);
    public static void PlayShoalIdle() => PlaySFX(Instance?.shoalIdleclip, 1f);
    public static void PlayShoalDamageHit() => PlaySFX(Instance?.shoalDamageHitClip, 1f);
    public static void PlayShoalPhrase1() => PlayNarration(Instance?.ShoalPhrase1Clip, 1f); // Uses narration system
    // ----Construction Enemy SFX----
    public static void PlayConstructionIdle() => PlaySFX(Instance?.ConstructionIdleClip, 1f);
    public static void PlayConstructionSeenOne() => PlaySFX(Instance?.ConstructionSeenOneClip, 1f);
    public static void PlayConstructionSeentwo() => PlaySFX(Instance?.ConstructionSeentwoClip, 1f);
    public static void PlayConstructionFalling() => PlaySFX(Instance?.ConstructionFallingClip, 1f);
    public static void PlayConstructionDamageHitOne() => PlaySFX(Instance?.ConstructionDamageHitOneClip, 1f);
    public static void PlayConstructionDamageHitTwo() => PlaySFX(Instance?.ConstructionDamageHitTwoClip, 1f);
    // ----Macro Enemy SFX----
    public static void PlayMacroIdle() => PlaySFX(Instance?.MacroIdleClip, 1f);
    public static void PlayMacroRetreatOne() => PlaySFX(Instance?.MacroRetreatOneClip, 1f);
    public static void PlayMacroRetreatTwo() => PlaySFX(Instance?.MacroRetreatTwoClip, 1f);
    public static void PlayMacroDamageHitOne() => PlaySFX(Instance?.MacroDamageHitOneClip, 1f);
    public static void PlayMacroDamageHitTwo() => PlaySFX(Instance?.MacroDamageHitTwoClip, 1f);
    // ----Micro Enemy SFX----
    public static void PlayMicroEncounterOne() => PlayNarration(Instance?.MicroEncounterClip, 1f); // Uses narration system for spawn line
    public static void PlayMicroTwoHealth() => PlayNarration(Instance?.MicroTwoHealthClip, 1f); // Uses narration system for spawn line
    public static void PlayMicroOneHealth() => PlayNarration(Instance?.MicroOneHealthClip, 1f); // Uses narration system for spawn line
    public static void PlayMicroChaseOne() => PlaySFX(Instance?.MicroChaseOneClip, 1f);
    public static void PlayMicroPrepareAttack() => PlaySFX(Instance?.MicroPrepareAttackClip, 1f);
    public static void PlayMicroAttack() => PlaySFX(Instance?.MicroAttackClip, 1f);
    public static void PlayMicroEncounter() => PlaySFX(Instance?.MicroDamageHitOneClip, 1f);
    public static void PlayMicroDamageHitTwo() => PlaySFX(Instance?.MicroDamageHitTwoClip, 1f);


    public static void PlayMicroDie() => PlaySFX(Instance?.MicroDieClip, 1f);


}