using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
	[SerializeField] int _GameplaySceneInt;
    [SerializeField] GameObject _DefaultPlayButton;
    [SerializeField] GameObject _MainMenuButtonContainer;
    [SerializeField] GameObject _SettingsPanel;
    [SerializeField] GameObject _SettingsBackButton;
    [SerializeField] GameObject _CreditsPanel;
    [SerializeField] GameObject _CreditsBackButton;
    [SerializeField] LoadingScreenManager _loadingScreenManager;
   
    public void Start(){
		Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
		EventSystem.current.SetSelectedGameObject(_DefaultPlayButton);
		AudioManager.PlayMainMenuBGM();// Play menu music (clip assigned on AudioManager)
    }
	
	public void StartButtonClicked(){
		Debug.Log("start!");
		Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
		AudioManager.PlayConstructionBGM();// Play construction music (clip assigned on AudioManager)
        _loadingScreenManager.StartLoadingScene(_GameplaySceneInt);
	}
	
	public void CreditsButtonClicked(){
		if(_CreditsPanel) _CreditsPanel.SetActive(true);
		if(_MainMenuButtonContainer) _MainMenuButtonContainer.SetActive(false);
		EventSystem.current.SetSelectedGameObject(_CreditsBackButton);
	}
	
	public void SettingsButtonClicked(){
		if(_SettingsPanel) _SettingsPanel.SetActive(true);
		if(_MainMenuButtonContainer) _MainMenuButtonContainer.SetActive(false);
		EventSystem.current.SetSelectedGameObject(_SettingsBackButton);
	}
	
	public void BackButtonToMainClicked(){
		if(_CreditsPanel) _CreditsPanel.SetActive(false);
		if(_SettingsPanel) _SettingsPanel.SetActive(false);
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
