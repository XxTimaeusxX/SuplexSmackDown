using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameMenuManager : MonoBehaviour
{
	[SerializeField] string _MainMenuScene;
	[SerializeField] GameObject _PauseMenuContainer;
	
	public void ResumeButtonClicked(){
		Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
		Time.timeScale = 1.0f;
		_PauseMenuContainer.SetActive(false);
	}
	
	public void QuitButtonClicked(){
		Debug.Log("quit!");
		Time.timeScale = 1.0f;
		SceneManager.LoadScene(_MainMenuScene);
	}
	
	public void Pause(){
		Time.timeScale = 0.0f;
		_PauseMenuContainer.SetActive(true);
	}
}
