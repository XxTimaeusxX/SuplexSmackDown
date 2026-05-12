using UnityEngine;

public class PlayerSFXManager : MonoBehaviour
{
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayDashSoundFX()
    {
        audioSource.PlayOneShot(WorldSFXManager.instance.dashSFX);
    }
}
