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

    [Header("Assigned SFX (drag in Inspector)")]
    public AudioClip footstepClip;
    public AudioClip suplexStartClip;
    public AudioClip suplexSlamClip;
    public AudioClip enemySlapClip;

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

    // Named SFX helpers (no clip passing from callers)
    public static void PlayFootstep()
    {
        if (!Instance || !Instance.footstepClip) return;
        Instance.sfxSource.PlayOneShot(Instance.footstepClip, Instance.sfxVolume);
    }

    public static void PlaySuplexStart()
    {
        if (!Instance || !Instance.suplexStartClip) return;
        Instance.sfxSource.PlayOneShot(Instance.suplexStartClip, Instance.sfxVolume);
    }

    public static void PlaySuplexSlam()
    {
        if (!Instance || !Instance.suplexSlamClip) return;
        Instance.sfxSource.PlayOneShot(Instance.suplexSlamClip, Instance.sfxVolume);
    }

    public static void PlayEnemySlap()
    {
        if (!Instance || !Instance.enemySlapClip) return;
        Instance.sfxSource.PlayOneShot(Instance.enemySlapClip, Instance.sfxVolume);
    }
}           