using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class MainMenuManager : MonoBehaviour
{
	[SerializeField] int _GameplaySceneInt;
    [SerializeField] GameObject _DefaultPlayButton;
    [SerializeField] GameObject _MainMenuButtonContainer;
	private Button MainMenuButtons;
    [SerializeField] GameObject _SettingsPanel;
	SettingsManager sm;
    [SerializeField] GameObject _SettingsBackButton;
    [SerializeField] GameObject _ControlsPanel;
    [SerializeField] GameObject _ControlsBackButton;
    [SerializeField] GameObject _CreditsPanel;
    [SerializeField] GameObject _CreditsBackButton;
    [SerializeField] LoadingScreenManager _loadingScreenManager;
    [SerializeField] Volume SkyVolume;
   
    public void Start(){
		Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
		EventSystem.current.SetSelectedGameObject(_DefaultPlayButton);
		AudioManager.PlayMainMenuBGM();// Play menu music (clip assigned on AudioManager)
		sm = _SettingsPanel.GetComponent<SettingsManager>();
		sm.GammaInit(SkyVolume);
    }
	
	//when hovering over a button, set it to selected
	public void ButtonHover(GameObject curButton){
		EventSystem.current.SetSelectedGameObject(curButton);
		//Debug.Log("hovered " + curButton.name);
	}
	
	public void StartButtonClicked(){
		Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
	//	AudioManager.PlayConstructionBGM();// Play construction music (clip assigned on AudioManager)
        _loadingScreenManager.StartLoadingScene(_GameplaySceneInt);
	}
	
	public void HowToPlayButtonClicked(){
		if(_ControlsPanel) _ControlsPanel.SetActive(true);
		if(_MainMenuButtonContainer) _MainMenuButtonContainer.SetActive(false);
		EventSystem.current.SetSelectedGameObject(_ControlsBackButton);
	}
	
	public void SettingsButtonClicked(){
		if(_SettingsPanel) _SettingsPanel.SetActive(true);
		if(_MainMenuButtonContainer) _MainMenuButtonContainer.SetActive(false);
		EventSystem.current.SetSelectedGameObject(_SettingsBackButton);
	}
	
	public void CreditsButtonClicked(){
		if(_CreditsPanel) _CreditsPanel.SetActive(true);
		if(_MainMenuButtonContainer) _MainMenuButtonContainer.SetActive(false);
		EventSystem.current.SetSelectedGameObject(_CreditsBackButton);
	}
	
	public void BackButtonToMainClicked(){
		if(_ControlsPanel) _ControlsPanel.SetActive(false);
		if(_SettingsPanel) _SettingsPanel.SetActive(false);
		if(_CreditsPanel) _CreditsPanel.SetActive(false);
		if(_MainMenuButtonContainer) _MainMenuButtonContainer.SetActive(true);
		EventSystem.current.SetSelectedGameObject(_DefaultPlayButton);
	}
	
	public void ExitButtonClicked(){
		Debug.Log("exit!");
#if UNITY_EDITOR
	UnityEditor.EditorApplication.ExitPlaymode();
#else
	Application.Quit();
#endif
	}
}
