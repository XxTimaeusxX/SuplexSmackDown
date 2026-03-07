using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class InGameMenuManager : MonoBehaviour
{
	[SerializeField] CharacterController playerCC;
	[SerializeField] PlayerMovement playerMovement;
	[SerializeField] PlayerDash playerDash;
	public PlayerInput playerInput;
	[SerializeField] GameObject _SuperSuplexUI;

	[SerializeField] string _MainMenuScene;
    [SerializeField] string _Stage1Scene;
    [SerializeField] string _Stage2Scene;
	
    [SerializeField] GameObject _PauseMenuContainer;
	[SerializeField] GameObject _HowToPlayPanel;
	[SerializeField] GameObject _SettingsPanel;
	[SerializeField] GameObject _WinMenuContainer;
	[SerializeField] GameObject _GameOverMenuContainer;
	[SerializeField] GameObject _CheatsMenu;
	
	[SerializeField] GameObject _PauseButtonContainer;
	[SerializeField] GameObject _DefaultPauseButton;
	[SerializeField] GameObject _DefaultHowToPlayButton;
	[SerializeField] GameObject _DefaultSettingsButton;
	[SerializeField] GameObject _DefaultWinButton;
	[SerializeField] GameObject _DefaultGameOverButton;
	[SerializeField] GameObject _DefaultCheatsButton;
	
	[SerializeField] GameObject _HealthUI;

	InputAction cheatsAction1;
	InputAction cheatsAction2;
	InputAction cheatsAction3;
	bool canInputCheats;

	//[SerializeField] GameObject _PausePoster;
	Vector3 pauseMaxScale = new Vector3(1.5f, 1.5f, 1f);
	float pause_t = 0;

    bool isPaused = false;
	public bool canPause = true;
	
	[SerializeField] Animator pause_anim;
	
	public void Start(){
        cheatsAction1 = playerInput.actions.FindAction("RainbowSuplex");
        cheatsAction2 = playerInput.actions.FindAction("Dash");
		//check to make sure the system can find the inputs
		if (cheatsAction1 != null && cheatsAction2 != null)
			canInputCheats = true;
		else{
			canInputCheats = false;
			Debug.Log("inputs not found!");
		}
	}
	
	//listen for cheat code
	public void Update()
	{
		//if all cheat inputs are pressed together, activate the cheats menu
		if (_SettingsPanel.active == true && canInputCheats && cheatsAction1.IsPressed() && cheatsAction2.IsPressed())
        {
			CheatsMenuActivate();
        }
	}
	
	
	//pause and show pause menu
	public void Pause()
	{
		if(canPause){
			//if already paused, act as ResumeButtonClicked
			if (isPaused){
				ResumeButtonClicked();
			}
			//pausing: play a sound, show cursor, set timeScale to 0, hide super suplex UI, and show pause menu
			else{
				playerCC.enabled = false; //prevent player input in the menu
				playerMovement.enabled = false;
				playerDash.enabled = false;
				
				AudioManager.PlaySuplexSlam();
				Cursor.lockState = CursorLockMode.Confined;
				Cursor.visible = true;
				Time.timeScale = 0.0f;
				isPaused = true;
				_PauseMenuContainer.SetActive(true);
				_PauseButtonContainer.SetActive(true);
				_SuperSuplexUI.SetActive(false);
				if (pause_anim != null){
					pause_anim.SetTrigger("justPaused");
				}
				else 
					Debug.Log("no pause anim!");

				// Set default selected button for navigation
				EventSystem.current.SetSelectedGameObject(_DefaultPauseButton);
			}
		}
	}

	//lock and hide cursor, set timeScale to 1, show super suplex UI, and hide pause menu
	public void ResumeButtonClicked()
	{
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		Time.timeScale = 1.0f;
		if(_PauseMenuContainer) _PauseMenuContainer.SetActive(false);
		if(_SettingsPanel) _SettingsPanel.SetActive(false);
		if(_HowToPlayPanel) _HowToPlayPanel.SetActive(false);
		if(_WinMenuContainer) _WinMenuContainer.SetActive(false);
		if(_GameOverMenuContainer) _GameOverMenuContainer.SetActive(false);
		if(_CheatsMenu) _CheatsMenu.SetActive(false);
		isPaused = false;
		_SuperSuplexUI.SetActive(true);
		pause_anim.SetBool("isPaused", false);
		
		playerCC.enabled = true; //allow the player to move again
		playerMovement.enabled = true;
		playerDash.enabled = true;
	}
	
	//when hovering over a button, set it to selected
	public void ButtonHover(GameObject curButton){
		EventSystem.current.SetSelectedGameObject(curButton);
		//Debug.Log("hovered " + curButton.name);
	}
	
	public void HowToPlayButtonClicked()
	{
		_PauseButtonContainer.SetActive(false);
		_HowToPlayPanel.SetActive(true);
		EventSystem.current.SetSelectedGameObject(_DefaultHowToPlayButton);
	}
	
	public void HowToPlayBackButtonClicked()
	{
		_PauseButtonContainer.SetActive(true);
		_HowToPlayPanel.SetActive(false);
		EventSystem.current.SetSelectedGameObject(_DefaultPauseButton);
	}
	
	public void SettingsButtonClicked()
	{
		_PauseButtonContainer.SetActive(false);
		_SettingsPanel.SetActive(true);
		EventSystem.current.SetSelectedGameObject(_DefaultSettingsButton);
	}
	
	public void SettingsBackButtonClicked()
	{
		_PauseButtonContainer.SetActive(true);
		_SettingsPanel.SetActive(false);
		EventSystem.current.SetSelectedGameObject(_DefaultPauseButton);
	}
	
	//acts as ResumeButtonClicked and restart level
	public void RestartButtonClicked()
	{
		ResumeButtonClicked();
		PlaySceneMusic();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); //reload current scene
	}
	
	//unpause and return to main menu
	public void QuitButtonClicked()
	{
		isPaused = false;
		Time.timeScale = 1.0f;
		SceneManager.LoadScene(_MainMenuScene);
	}
	
	//unpause and go to stage 2
	public void Stage2ButtonClicked()
	{
		isPaused = false;
		Time.timeScale = 1.0f;
		PlaySceneMusic();
        SceneManager.LoadScene(_Stage2Scene);
	}
	
	public void CheatsMenuActivate()
	{
		_SettingsPanel.SetActive(false);
		_CheatsMenu.SetActive(true);
		EventSystem.current.SetSelectedGameObject(_DefaultCheatsButton);
	}
	
	//hide cheats menu and select settings
	public void CheatsBackButtonClicked()
	{
		_CheatsMenu.SetActive(false);
		_SettingsPanel.SetActive(true);
		EventSystem.current.SetSelectedGameObject(_DefaultSettingsButton);
	}
	
	//show cursor, pause, and show game over menu
	public void GameOver()
	{
		canPause = false;
		Cursor.lockState = CursorLockMode.Confined;
		Cursor.visible = true;
		Time.timeScale = 0.0f;
		_GameOverMenuContainer.SetActive(true);
		AudioManager.StopMusic();
		AudioManager.PlayDefeat();

        // Set default selected button for navigation
        EventSystem.current.SetSelectedGameObject(_DefaultGameOverButton);
	}
	
	//show cursor, pause, and show win screen
	public void WinScreen()
	{
		canPause = false;
		Cursor.lockState = CursorLockMode.Confined;
		Cursor.visible = true;
		Time.timeScale = 0.0f;
		_WinMenuContainer.SetActive(true);
		AudioManager.StopMusic();
		AudioManager.PLayVictory();
        // Set default selected button for navigation
        EventSystem.current.SetSelectedGameObject(_DefaultWinButton);
	}
	
	//hides all UI - used in loading screen
	public void HideAllUI()
	{
		if(_PauseMenuContainer) _PauseMenuContainer.SetActive(false);
		if(_WinMenuContainer) _WinMenuContainer.SetActive(false);
		if(_GameOverMenuContainer) _GameOverMenuContainer.SetActive(false);
		if(_HealthUI) _HealthUI.SetActive(false);
	}
    //plays the appropriate music based on the current scene
    void PlaySceneMusic()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        if (currentSceneName == _MainMenuScene)
        {
            AudioManager.PlayMainMenuBGM();
        }
		else if (currentSceneName == _Stage1Scene)
		{
				AudioManager.PlayConstructionBGM();
        }
        else if (currentSceneName == _Stage2Scene)
		{
			AudioManager.PlayConstructionBGM();
		}     
    }
}
