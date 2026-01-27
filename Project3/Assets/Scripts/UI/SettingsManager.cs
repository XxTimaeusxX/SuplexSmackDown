using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class SettingsManager : MonoBehaviour
{
	[SerializeField] Slider Master_Slider;
	[SerializeField] Slider SFX_Slider;
	[SerializeField] Slider BGM_Slider;
	AudioManager am;

	//settings for changing brightness via post-processing
	[SerializeField] Slider Gamma_Slider;
	[SerializeField] Volume SkyVolume; //should be set to the scene's Sky and Fog Global Volume
	[SerializeField] float ExposureMultiplier = 5f; //use this (plus the slider settings themselves) to fine tune the gamma slider
	ColorAdjustments ca;

	void Start(){
		if(!SkyVolume.profile.TryGet<ColorAdjustments>(out var color_adjust)){
			color_adjust = SkyVolume.profile.Add<ColorAdjustments>(false);
		}
		ca = color_adjust;
		
		am = GameObject.Find("AudioManager").GetComponent<AudioManager>();
		SFX_Slider.value = am.sfxVolume;
		BGM_Slider.value = am.musicVolume;
		Master_Slider.value = am.masterVolume;
	}

	public void OnGammaSliderChange()
	{
		//the gamma slider is set to 0.5 by default
		//set the gamma range from -5 to 5
		//0.5 = 0, 0 = -5, 1 = 5
		//(slider-0.5)*5
		ca.postExposure.value = 2*ExposureMultiplier*(Gamma_Slider.value-0.5f);
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
