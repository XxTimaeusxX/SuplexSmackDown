using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class InGameMenuManager : MonoBehaviour
{
	[SerializeField] string _MainMenuScene;
	
	[SerializeField] GameObject _PauseMenuContainer;
	[SerializeField] GameObject _WinMenuContainer;
	[SerializeField] GameObject _GameOverMenuContainer;
	
	[SerializeField] GameObject _DefaultPauseButton;
	[SerializeField] GameObject _DefaultWinButton;
	[SerializeField] GameObject _DefaultGameOverButton;
	
	[SerializeField] GameObject _HealthUI;

	//[SerializeField] GameObject _PausePoster;
	Vector3 pauseMaxScale = new Vector3(1.5f, 1.5f, 1f);
	float pause_t = 0;

    bool isPaused = false;
	public bool canPause = true;
	
	
	//lock/hide cursor, unpause, and hide pause menu
	public void ResumeButtonClicked()
	{
		isPaused = false;
		Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
		Time.timeScale = 1.0f;
		if(_PauseMenuContainer) _PauseMenuContainer.SetActive(false);
		if(_WinMenuContainer) _WinMenuContainer.SetActive(false);
		if(_GameOverMenuContainer) _GameOverMenuContainer.SetActive(false);
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
		SceneManager.LoadScene(SceneManager.GetActiveScene().name); //reload current scene
	}
	
	//unpause and return to main menu
	public void QuitButtonClicked()
	{
		isPaused = false;
		Debug.Log("quit!");
		Time.timeScale = 1.0f;
		SceneManager.LoadScene(_MainMenuScene);
	}
	
	//pause and show pause menu
	public void Pause()
	{
		if(canPause){
			Debug.Log("canPause: " + canPause);
			Debug.Log("isPaused: " + isPaused);
			//unpausing: lock and hide cursor, set timeScale to 1, hide pause menu
			if (isPaused){
				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;
				Time.timeScale = 1.0f;
				_PauseMenuContainer.SetActive(false);
				isPaused = false;
			}
			//pausing: show cursor, set timeScale to 0, show pause menu
			else{
				Cursor.lockState = CursorLockMode.Confined;
				Cursor.visible = true;
				Time.timeScale = 0.0f;
				isPaused = true;
				_PauseMenuContainer.SetActive(true);
				_PauseMenuContainer.transform.localScale = pauseMaxScale;
				pause_t = 0f;
				StartCoroutine("PauseAnimation");

				// Set default selected button for navigation
				EventSystem.current.SetSelectedGameObject(_DefaultPauseButton);
				
			}
		}
	}
	
	IEnumerator PauseAnimation()
	{
		while (_PauseMenuContainer.transform.localScale.x > 1.0f){
			//Debug.Log("do something?");
			_PauseMenuContainer.transform.localScale = Vector3.Lerp(pauseMaxScale, Vector3.one, pause_t);
			pause_t += 0.1f;
			Debug.Log(pause_t);
			yield return null;
		}
		if(_PauseMenuContainer.transform.localScale.x <= 1.0f){
			_PauseMenuContainer.transform.localScale = Vector3.one;
			StopCoroutine("PauseAnimation");
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
	
}
