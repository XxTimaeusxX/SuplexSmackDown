using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class MainMenuManager : MonoBehaviour
{
	[SerializeField] int _TutorialSceneInt;
	[SerializeField] int _Level1SceneInt;
    [SerializeField] GameObject _MainMenuCanvas;
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
    [SerializeField] GameObject _TutorialPanel;
    [SerializeField] GameObject _TutorialYesButton;
    [SerializeField] GameObject _CutscenePanel;
    [SerializeField] GameObject _CutsceneContinueButton;
    [SerializeField] LoadingScreenManager _loadingScreenManager;
    [SerializeField] Volume SkyVolume;
	
	//variables for gamma updates
	Image[] menuImg;
	Color[] menuImgColor;
	Text[] menuText;
	Color[] menuTextColor;
	float H,S,V;
   
    public void Start(){
		Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
		EventSystem.current.SetSelectedGameObject(_DefaultPlayButton);
		AudioManager.PlayMainMenuBGM();// Play menu music (clip assigned on AudioManager)
		sm = _SettingsPanel.GetComponent<SettingsManager>();
		sm.GammaInit(SkyVolume);
		
		ColorInit();
    }
	
	//when hovering over a button, set it to selected
	public void ButtonHover(GameObject curButton){
		EventSystem.current.SetSelectedGameObject(curButton);
		//Debug.Log("hovered " + curButton.name);
	}
	
	public void StartButtonClicked(){
		if(_TutorialPanel) _TutorialPanel.SetActive(true);
		if(_MainMenuButtonContainer) _MainMenuButtonContainer.SetActive(false);
		EventSystem.current.SetSelectedGameObject(_TutorialYesButton);
	}
	
	public void StartGameWithTutorial(){
		Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
	//	AudioManager.PlayConstructionBGM();// Play construction music (clip assigned on AudioManager)
		StarTracker.ResetStars();
        _loadingScreenManager.StartLoadingScene(_TutorialSceneInt);
	}
	
	public void StartGameNoTutorial(){        
		//hide tutorial panel and main menu panel and show cutscene menu panel
		if(_CutscenePanel) _CutscenePanel.SetActive(true);
		if(_MainMenuButtonContainer) _MainMenuButtonContainer.SetActive(false);
		EventSystem.current.SetSelectedGameObject(_CutsceneContinueButton);
	}
	
	public void CutsceneContinueButtonClicked(){
		Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
	//	AudioManager.PlayConstructionBGM();// Play construction music (clip assigned on AudioManager)
		StarTracker.ResetStars();
		_loadingScreenManager.StartLoadingScene(_Level1SceneInt);
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
		if(_TutorialPanel) _TutorialPanel.SetActive(false);
		if(_MainMenuButtonContainer) _MainMenuButtonContainer.SetActive(true);
		EventSystem.current.SetSelectedGameObject(_DefaultPlayButton);
	}
	
	//play menu sounds, called in On Click () and Event Trigger (Select) in the inspector
	public void PlaySelectSound(){
		AudioManager.PlayMenuNavigateSelect();
	}
	
	public void PlayBackSound(){
		AudioManager.PlayMenuNavigateBack();
	}
	
	public void PlayNavigateSound(){
		AudioManager.PlayMenuNavigate();
	}
	
	//gamma updates
	void ColorInit(){
		if(_MainMenuCanvas){
			menuImg = _MainMenuCanvas.GetComponentsInChildren<Image>(true);
			menuText = _MainMenuCanvas.GetComponentsInChildren<Text>(true);
			
			menuImgColor = new Color[menuImg.Length];
			menuTextColor = new Color[menuText.Length];
			
			int index = 0;
			float newV;
			foreach (Image img in menuImg){
				menuImgColor[index] = img.color;
				Color.RGBToHSV(menuImgColor[index], out H, out S, out V);
				newV = GammaCalc(V, PlayerPrefs.GetFloat("gamma"));
				img.color = Color.HSVToRGB(H,S,newV);
				index++;
			}
			index = 0;
			foreach (Text newText in menuText){
				menuTextColor[index] = newText.color;
				Color.RGBToHSV(menuTextColor[index], out H, out S, out V);
				newV = GammaCalc(V, PlayerPrefs.GetFloat("gamma"));
				newText.color = Color.HSVToRGB(H,S,newV);
				index++;
			}
		}
	}
	
	//change value depending on slider
	float GammaCalc(float V, float newGamma){
		if(newGamma <= 0.05f)
			return V*(0.5f); //prevents everything from becoming too dark to see
		else if(newGamma <= 0.5f)
			return V*(newGamma+0.5f);
		else
			return 2*V*(newGamma);
	}
	
	public void GammaUIUpdate(float newGamma){
		if(_MainMenuCanvas){
			int index = 0;
			float newV;
			foreach (Image img in menuImg){
				Color.RGBToHSV(menuImgColor[index], out H, out S, out V);
				newV = GammaCalc(V, newGamma);
				img.color = Color.HSVToRGB(H,S,newV);
				index++;
			}
			index = 0;
			foreach (Text newText in menuText){
				Color.RGBToHSV(menuTextColor[index], out H, out S, out V);
				newV = GammaCalc(V, newGamma);
				newText.color = Color.HSVToRGB(H,S,newV);
				index++;
			}
		}
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
