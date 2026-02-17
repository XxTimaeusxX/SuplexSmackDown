using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class InGameMenuManager : MonoBehaviour
{
	[SerializeField] string _MainMenuScene;
    [SerializeField] string _Stage1Scene;
    [SerializeField] string _Stage2Scene;
	
    [SerializeField] GameObject _PauseMenuContainer;
	[SerializeField] GameObject _ControlsPanel;
	[SerializeField] GameObject _SettingsPanel;
	[SerializeField] GameObject _WinMenuContainer;
	[SerializeField] GameObject _GameOverMenuContainer;
	[SerializeField] GameObject _CheatsMenu;
	
	[SerializeField] GameObject _PauseButtonContainer;
	[SerializeField] GameObject _DefaultPauseButton;
	[SerializeField] GameObject _DefaultControlsButton;
	[SerializeField] GameObject _DefaultSettingsButton;
	[SerializeField] GameObject _DefaultWinButton;
	[SerializeField] GameObject _DefaultGameOverButton;
	[SerializeField] GameObject _DefaultCheatsButton;
	
	[SerializeField] GameObject _HealthUI;
	[SerializeField] GameObject _SuperSuplexUI;

	[Header("Debug Menu")]
	[SerializeField] GameObject DEBUG_Player;
	CharacterController DEBUG_PlayerCC;
	[SerializeField] GameObject DEBUG_ShoalSpawnLocation;
	[SerializeField] GameObject DEBUG_BossSpawnLocation;
	
	public PlayerInput playerInput;
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
		DEBUG_PlayerCC = DEBUG_Player.GetComponent<CharacterController>();
        cheatsAction1 = playerInput.actions.FindAction("LongjumpSuplex");
        cheatsAction2 = playerInput.actions.FindAction("RainbowSuplex");
        cheatsAction3 = playerInput.actions.FindAction("Dash");
		//check to make sure the system can find the inputs
		if (cheatsAction1 != null && cheatsAction2 != null && cheatsAction3 != null)
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
		if (canInputCheats && cheatsAction2.IsPressed() && cheatsAction3.IsPressed())
        {
			Debug.Log("activated!");
			CheatsMenuActivate();
        }
	}
	
	//when hovering over a button, set it to selected
	public void ButtonHover(GameObject curButton){
		EventSystem.current.SetSelectedGameObject(curButton);
		Debug.Log("hovered " + curButton.name);
	}
	
	//lock/hide cursor, unpause, and hide pause menu
	public void ResumeButtonClicked()
	{
		isPaused = false;
		Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
		Time.timeScale = 1.0f;
		if(_PauseMenuContainer) _PauseMenuContainer.SetActive(false);
		if(_SettingsPanel) _SettingsPanel.SetActive(false);
		if(_ControlsPanel) _ControlsPanel.SetActive(false);
		if(_WinMenuContainer) _WinMenuContainer.SetActive(false);
		if(_GameOverMenuContainer) _GameOverMenuContainer.SetActive(false);
		_SuperSuplexUI.SetActive(true);
	}
	
	public void ControlsButtonClicked()
	{
		_PauseButtonContainer.SetActive(false);
		_ControlsPanel.SetActive(true);
		EventSystem.current.SetSelectedGameObject(_DefaultControlsButton);
	}
	
	public void ControlsBackButtonClicked()
	{
		_PauseButtonContainer.SetActive(true);
		_ControlsPanel.SetActive(false);
		EventSystem.current.SetSelectedGameObject(_DefaultPauseButton);
	}
	
	public void InGameSettingsButtonClicked()
	{
		_PauseButtonContainer.SetActive(false);
		_SettingsPanel.SetActive(true);
		EventSystem.current.SetSelectedGameObject(_DefaultSettingsButton);
	}
	
	public void InGameSettingsBackButtonClicked()
	{
		_PauseButtonContainer.SetActive(true);
		_SettingsPanel.SetActive(false);
		EventSystem.current.SetSelectedGameObject(_DefaultPauseButton);
	}
	
	//lock/hide cursor, unpause, and restart level
	public void RestartButtonClicked()
	{
		isPaused = false;
		Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
		Time.timeScale = 1.0f;
		_PauseMenuContainer.SetActive(false);
		_WinMenuContainer.SetActive(false);
		_GameOverMenuContainer.SetActive(false);
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
	
	//pause and show pause menu
	public void Pause()
	{
		if(canPause){
			//unpausing: lock and hide cursor, set timeScale to 1, show super suplex UI, and hide pause menu
			if (isPaused){
				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;
				Time.timeScale = 1.0f;
				_SettingsPanel.SetActive(false);	//allows unpausing while in the settings menu
				_ControlsPanel.SetActive(false);
				_PauseMenuContainer.SetActive(false);
				_CheatsMenu.SetActive(false);
				_SuperSuplexUI.SetActive(true);
				isPaused = false;
				pause_anim.SetBool("isPaused", false);
			}
			//pausing: play a sound, show cursor, set timeScale to 0, hide super suplex UI, and show pause menu
			else{
				AudioManager.PlaySuplexSlam();
				Cursor.lockState = CursorLockMode.Confined;
				Cursor.visible = true;
				Time.timeScale = 0.0f;
				isPaused = true;
				_PauseMenuContainer.SetActive(true);
				_PauseButtonContainer.SetActive(true);
				Debug.Log(_PauseMenuContainer.active);
				_SuperSuplexUI.SetActive(false);
				if (pause_anim != null){
					pause_anim.SetTrigger("justPaused");
					//pause_anim.Play("PauseMenuAnim");
					Debug.Log("-- play anim --");
				}
				else 
					Debug.Log("no anim!");
				//_PauseMenuContainer.transform.localScale = pauseMaxScale;
				//pause_t = 0f;
				//StartCoroutine("PauseAnimation");

				// Set default selected button for navigation
				EventSystem.current.SetSelectedGameObject(_DefaultPauseButton);
				
			}
		}
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

    //-------- DEBUG MENU OPTIONS --------//
    //transports the player, then functions as ResumeButtonClicked
    public void ToShoalButtonClicked()
	{
		StartCoroutine(DEBUG_Teleport(DEBUG_ShoalSpawnLocation.transform.position));
	}
	
	//transports the player, then functions as ResumeButtonClicked
	public void ToBossButtonClicked()
	{
		StartCoroutine(DEBUG_Teleport(DEBUG_BossSpawnLocation.transform.position));
	}
	
	IEnumerator DEBUG_Teleport(Vector3 newPosition){
		ResumeButtonClicked();
		DEBUG_PlayerCC.enabled = false;
		yield return new WaitForSeconds(.1f);
		DEBUG_Player.transform.position = newPosition;
		DEBUG_PlayerCC.enabled = true;
	}
	
}
