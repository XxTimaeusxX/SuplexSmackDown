using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
	[SerializeField] Slider Master_Slider;
	[SerializeField] Slider SFX_Slider;
	[SerializeField] Slider BGM_Slider;
	AudioManager am;

	void Start(){
		am = GameObject.Find("AudioManager").GetComponent<AudioManager>();
		SFX_Slider.value = am.sfxVolume;
		BGM_Slider.value = am.musicVolume;
		Master_Slider.value = am.masterVolume;
	}

	public void OnGammaSliderChange()
	{
		//TODO
	}
	
	public void OnMasterAudioSliderChange()
	{
        AudioManager.SetMasterVolume(Master_Slider.value);
	}

    public void OnSFXSliderChange()
    {
        AudioManager.SetSFXVolume(SFX_Slider.value);
		AudioManager.PlayJumping();
    }
	
    public void OnBGMSliderChange()
    {
        AudioManager.SetMusicVolume(BGM_Slider.value);
    }
}
