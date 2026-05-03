using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class SettingsManager : MonoBehaviour
{
	const float DEFAULT_AUDIO = 1f;
	const float DEFAULT_GAMMA = 0.5f;

	[SerializeField] Slider Master_Slider;
	[SerializeField] Slider SFX_Slider;
	[SerializeField] Slider BGM_Slider;
	AudioManager am;

	//settings for changing brightness via post-processing
	[SerializeField] Slider Gamma_Slider;
	[SerializeField] Volume SkyVolume; //should be set to the scene's Sky and Fog Global Volume
	[SerializeField] float ExposureMultiplier = 5f; //use this (plus the slider settings themselves) to fine tune the gamma slider
	ColorAdjustments ca;
	
	[SerializeField] MainMenuManager mainMenuManager;
	[SerializeField] InGameMenuManager inGameMenuManager;

	void Start(){
		if(!SkyVolume.profile.TryGet<ColorAdjustments>(out var color_adjust)){
			color_adjust = SkyVolume.profile.Add<ColorAdjustments>(false);
		}
		ca = color_adjust;
		
		if (AudioManager.Instance != null)
		{
            am = GameObject.Find("AudioManager").GetComponent<AudioManager>();
            SFX_Slider.value = am.sfxVolume;
            BGM_Slider.value = am.musicVolume;
            Master_Slider.value = am.masterVolume;
        }
		
		//set gamma to player prefs if it's been set already
		if(PlayerPrefs.GetFloat("gamma") != 0)
			Gamma_Slider.value = PlayerPrefs.GetFloat("gamma");
	}

	public void OnGammaSliderChange()
	{
		//the gamma slider is set to 0.5 by default
		ca.postExposure.value = 2*ExposureMultiplier*(Gamma_Slider.value-0.5f);
		PlayerPrefs.SetFloat("gamma", Gamma_Slider.value);
		
		//update the menus if applicable
		if(mainMenuManager) mainMenuManager.GammaUIUpdate(Gamma_Slider.value);
		if(inGameMenuManager) inGameMenuManager.GammaUIUpdate(Gamma_Slider.value);
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
	
	public void OnDefaultsButtonPressed()
	{
		SFX_Slider.value = DEFAULT_AUDIO;
		BGM_Slider.value = DEFAULT_AUDIO;
		Master_Slider.value = DEFAULT_AUDIO;
		Gamma_Slider.value = DEFAULT_GAMMA;
	}
	
	public void OnApplyButtonPressed()
	{
		PlayerPrefs.Save();
	}
	
	public void GammaInit(Volume vol)
	{
		if(!vol.profile.TryGet<ColorAdjustments>(out var color_adjust)){
			color_adjust = SkyVolume.profile.Add<ColorAdjustments>(false);
		}
		ca = color_adjust;
		ca.postExposure.value = 2*ExposureMultiplier*(PlayerPrefs.GetFloat("gamma")-0.5f);
	}
}
