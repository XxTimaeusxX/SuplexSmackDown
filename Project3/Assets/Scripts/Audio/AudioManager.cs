using UnityEngine;

public sealed class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Volumes")]
    [Range(0f, 1f)] public float musicVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

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
        if (musicSource) musicSource.volume = musicVolume;
        if (sfxSource) sfxSource.volume = sfxVolume;
    }

    // Music control
    public static void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (!Instance || !clip) return;
        Instance.musicSource.clip = clip;
        Instance.musicSource.loop = loop;
        Instance.musicSource.volume = Instance.musicVolume;
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

    // Generic SFX (optional)
    public static void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (!Instance || !clip) return;
        Instance.sfxSource.PlayOneShot(clip, Mathf.Clamp01(volume) * Instance.sfxVolume);
    }

    // player SFX
    public static void PlayFootstep()
    {
        if (!Instance || !Instance.footstepClip) return;
        Instance.sfxSource.PlayOneShot(Instance.footstepClip, Instance.sfxVolume);
    }
    public static void PlayJumping()
    {
        if (!Instance || !Instance.jumpingClip) return;
        Instance.sfxSource.PlayOneShot(Instance.jumpingClip, Instance.sfxVolume);
    }
    // player suplex SFX
    public static void PlaySuplexStart()
    {
        if (!Instance || !Instance.LaunchSoundClip) return;
        Instance.sfxSource.PlayOneShot(Instance.LaunchSoundClip, Instance.sfxVolume);
    }

    public static void PlaySuplexSlam()
    {
        if (!Instance || !Instance.suplexSlamClip) return;
        Instance.sfxSource.PlayOneShot(Instance.suplexSlamClip, Instance.sfxVolume);
    }

    // PLayer health SFX
    public static void PlayHealth3()
    {
        if (!Instance || !Instance.health3Clip) return;
        Instance.sfxSource.PlayOneShot(Instance.health3Clip, Instance.sfxVolume);
    }
    public static void PlayHealth2()
    {
        if (!Instance || !Instance.health2Clip) return;
        Instance.sfxSource.PlayOneShot(Instance.health2Clip, Instance.sfxVolume);
    }
    public static void PlayHealth1()
    {
        if (!Instance || !Instance.health1Clip) return;
        Instance.sfxSource.PlayOneShot(Instance.health1Clip, Instance.sfxVolume);
    }
    public static void PlayGameOver()
    {
        if (!Instance || !Instance.GameOverClip) return;
        Instance.sfxSource.PlayOneShot(Instance.GameOverClip, Instance.sfxVolume);
    }

    // Enemy SFX
    public static void PlayEnemySlap()
    {
        if (!Instance || !Instance.enemySlapClip) return;
        Instance.sfxSource.PlayOneShot(Instance.enemySlapClip, Instance.sfxVolume);
    }
}           